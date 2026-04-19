#import <Foundation/Foundation.h>
#import <GoogleSignIn/GoogleSignIn.h>
#import "UnityInterface.h"

extern "C" {
    // Forward declarations
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
}

static NSString* serverClientId = nil;
static BOOL isInitialized = NO;

// Helper function to safely send message to Unity on main thread
void SafeUnitySendMessage(const char* obj, const char* method, const char* msg) {
    if ([NSThread isMainThread]) {
        UnitySendMessage(obj, method, msg);
    } else {
        // Copy the message string since it might be deallocated before dispatch executes
        char* msgCopy = (char*)malloc(strlen(msg) + 1);
        strcpy(msgCopy, msg);
        dispatch_async(dispatch_get_main_queue(), ^{
            UnitySendMessage(obj, method, msgCopy);
            free(msgCopy);
        });
    }
}

// Helper function to safely send message with NSString (handles memory properly)
void SafeUnitySendMessageWithString(const char* obj, const char* method, NSString* msgString) {
    const char* msg = [msgString UTF8String];
    if ([NSThread isMainThread]) {
        UnitySendMessage(obj, method, msg);
    } else {
        // Copy the message string since NSString might be deallocated before dispatch executes
        char* msgCopy = (char*)malloc(strlen(msg) + 1);
        strcpy(msgCopy, msg);
        dispatch_async(dispatch_get_main_queue(), ^{
            UnitySendMessage(obj, method, msgCopy);
            free(msgCopy);
        });
    }
}

// Forward declaration
NSString* userToJsonWithServerAuthCode(GIDGoogleUser* user, NSString* serverAuthCode);

// Helper function to convert GoogleSignInUser to JSON
NSString* userToJson(GIDGoogleUser* user) {
    return userToJsonWithServerAuthCode(user, nil);
}

// Helper function to convert GoogleSignInUser to JSON with serverAuthCode
NSString* userToJsonWithServerAuthCode(GIDGoogleUser* user, NSString* serverAuthCode) {
    NSMutableDictionary* dict = [NSMutableDictionary dictionary];
    
    if (user.userID) {
        dict[@"Id"] = user.userID;
    } else {
        dict[@"Id"] = @"";
    }
    
    if (user.profile.email) {
        dict[@"Email"] = user.profile.email;
    } else {
        dict[@"Email"] = @"";
    }
    
    if (user.profile.name) {
        dict[@"DisplayName"] = user.profile.name;
    } else {
        dict[@"DisplayName"] = @"";
    }
    
    // GoogleSignIn 9.0: imageURL is now a method that takes dimension
    NSURL* imageURL = [user.profile imageURLWithDimension:100];
    if (imageURL) {
        dict[@"PhotoUrl"] = [imageURL absoluteString];
    } else {
        dict[@"PhotoUrl"] = @"";
    }
    
    if (user.idToken.tokenString) {
        dict[@"IdToken"] = user.idToken.tokenString;
    } else {
        dict[@"IdToken"] = @"";
    }
    
    // GoogleSignIn 9.0: serverAuthCode is now on GIDSignInResult
    if (serverAuthCode) {
        dict[@"ServerAuthCode"] = serverAuthCode;
    } else {
        dict[@"ServerAuthCode"] = @"";
    }
    
    if (user.accessToken.tokenString) {
        dict[@"AccessToken"] = user.accessToken.tokenString;
    } else {
        dict[@"AccessToken"] = @"";
    }
    
    NSError* error;
    NSData* jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
    if (error) {
        return @"{}";
    }
    
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

extern "C" {
    void _GoogleSignIn_Initialize(const char* serverClientIdStr) {
        // Ensure initialization happens on main thread
        if (![NSThread isMainThread]) {
            dispatch_async(dispatch_get_main_queue(), ^{
                _GoogleSignIn_Initialize(serverClientIdStr);
            });
            return;
        }
        
        if (serverClientIdStr && strlen(serverClientIdStr) > 0) {
            serverClientId = [NSString stringWithUTF8String:serverClientIdStr];
        }
        
        // Get the client ID from GoogleService-Info.plist
        NSString* path = [[NSBundle mainBundle] pathForResource:@"GoogleService-Info" ofType:@"plist"];
        if (!path) {
            NSLog(@"[GoogleSignIn] ERROR: GoogleService-Info.plist not found in bundle");
            SafeUnitySendMessage("GoogleSignInManager", "OnSignInError", "GoogleService-Info.plist not found in app bundle|0");
            return;
        }
        
        NSDictionary* plist = [NSDictionary dictionaryWithContentsOfFile:path];
        if (!plist) {
            NSLog(@"[GoogleSignIn] ERROR: Failed to read GoogleService-Info.plist");
            SafeUnitySendMessage("GoogleSignInManager", "OnSignInError", "Failed to read GoogleService-Info.plist|0");
            return;
        }
        
        NSString* clientId = [plist objectForKey:@"CLIENT_ID"];
        if (!clientId || [clientId length] == 0) {
            NSLog(@"[GoogleSignIn] ERROR: CLIENT_ID not found in GoogleService-Info.plist");
            SafeUnitySendMessage("GoogleSignInManager", "OnSignInError", "CLIENT_ID missing from GoogleService-Info.plist|0");
            return;
        }
        
        GIDConfiguration* config = nil;
        if (serverClientId && [serverClientId length] > 0) {
            config = [[GIDConfiguration alloc] initWithClientID:clientId serverClientID:serverClientId];
            NSLog(@"[GoogleSignIn] Initializing with clientID and serverClientID");
        } else {
            config = [[GIDConfiguration alloc] initWithClientID:clientId];
            NSLog(@"[GoogleSignIn] Initializing with clientID only");
        }
        
        if (!config) {
            NSLog(@"[GoogleSignIn] ERROR: Failed to create GIDConfiguration");
            SafeUnitySendMessage("GoogleSignInManager", "OnSignInError", "Failed to create GIDConfiguration|0");
            return;
        }
        
        [GIDSignIn sharedInstance].configuration = config;
        isInitialized = YES;
        
        // Verify configuration was set
        GIDConfiguration* verifyConfig = [GIDSignIn sharedInstance].configuration;
        if (verifyConfig && verifyConfig.clientID) {
            NSLog(@"[GoogleSignIn] Native initialization complete. ClientID: %@", verifyConfig.clientID);
        } else {
            NSLog(@"[GoogleSignIn] WARNING: Configuration set but verification failed!");
        }
    }
    
    void _GoogleSignIn_SignIn() {
        // Check if configuration is set (more reliable than static flag)
        GIDSignIn* signIn = [GIDSignIn sharedInstance];
        GIDConfiguration* config = signIn.configuration;
        
        NSLog(@"[GoogleSignIn] SignIn called. Config exists: %d, ClientID: %@, isInitialized flag: %d", 
              (config != nil), 
              (config ? config.clientID : @"nil"), 
              isInitialized);
        
        if (!config || !config.clientID) {
            NSLog(@"[GoogleSignIn] ERROR: Not initialized! Configuration is nil or missing clientID. Attempting to re-initialize...");
            
            // Try to re-initialize automatically
            NSString* path = [[NSBundle mainBundle] pathForResource:@"GoogleService-Info" ofType:@"plist"];
            if (path) {
                NSDictionary* plist = [NSDictionary dictionaryWithContentsOfFile:path];
                NSString* clientId = [plist objectForKey:@"CLIENT_ID"];
                if (clientId) {
                    GIDConfiguration* autoConfig = [[GIDConfiguration alloc] initWithClientID:clientId];
                    signIn.configuration = autoConfig;
                    NSLog(@"[GoogleSignIn] Auto-reinitialized with clientID: %@", clientId);
                } else {
                    SafeUnitySendMessage("GoogleSignInManager", "OnSignInError", "Google Sign-In not initialized. Call Initialize() first|0");
                    return;
                }
            } else {
                SafeUnitySendMessage("GoogleSignInManager", "OnSignInError", "Google Sign-In not initialized. Call Initialize() first|0");
                return;
            }
        }
        
        UIViewController* rootViewController = UnityGetGLViewController();
        if (!rootViewController) {
            SafeUnitySendMessage("GoogleSignInManager", "OnSignInError", "Root view controller is nil|0");
            return;
        }
        
        [[GIDSignIn sharedInstance] signInWithPresentingViewController:rootViewController
                                                              completion:^(GIDSignInResult* result, NSError* error) {
            // Completion handler may be called on background thread, so dispatch to main thread
            dispatch_async(dispatch_get_main_queue(), ^{
                if (error) {
                    NSString* errorMsg = [NSString stringWithFormat:@"Sign in failed: %@", error.localizedDescription];
                    int errorCode = (int)error.code;
                    NSString* message = [NSString stringWithFormat:@"%@|%d", errorMsg, errorCode];
                    SafeUnitySendMessageWithString("GoogleSignInManager", "OnSignInError", message);
                } else if (result && result.user) {
                    // GoogleSignIn 9.0: serverAuthCode is now on GIDSignInResult
                    NSString* userJson = userToJsonWithServerAuthCode(result.user, result.serverAuthCode);
                    SafeUnitySendMessageWithString("GoogleSignInManager", "OnSignInSuccess", userJson);
                } else {
                    SafeUnitySendMessage("GoogleSignInManager", "OnSignInError", "Sign in failed: No user data|0");
                }
            });
        }];
    }
    
    void _GoogleSignIn_SignOut() {
        // GoogleSignIn 9.0: signOutWithCompletion is replaced with signOut() (synchronous)
        [[GIDSignIn sharedInstance] signOut];
        SafeUnitySendMessage("GoogleSignInManager", "OnSignOutSuccess", "");
    }
    
    void _GoogleSignIn_GetCurrentUser() {
        GIDGoogleUser* user = [[GIDSignIn sharedInstance] currentUser];
        if (user) {
            NSString* userJson = userToJson(user);
            SafeUnitySendMessageWithString("GoogleSignInManager", "OnGetUserSuccess", userJson);
        } else {
            SafeUnitySendMessage("GoogleSignInManager", "OnGetUserError", "");
        }
    }
    
    bool _GoogleSignIn_IsSignedIn() {
        GIDGoogleUser* user = [[GIDSignIn sharedInstance] currentUser];
        return user != nil;
    }
}

