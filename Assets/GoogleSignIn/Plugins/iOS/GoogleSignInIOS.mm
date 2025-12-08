#import <Foundation/Foundation.h>
#import <GoogleSignIn/GoogleSignIn.h>
#import "UnityInterface.h"

extern "C" {
    // Forward declarations
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
}

static NSString* serverClientId = nil;

// Forward declaration
NSString* userToJsonWithServerAuthCode(GIDGoogleUser* user, NSString* serverAuthCode);

// Helper function to convert GoogleSignInUser to JSON
NSString* userToJson(GIDGoogleUser* user) {
    return userToJsonWithServerAuthCode(user, @"");
}

// Helper function to convert GoogleSignInUser to JSON with server auth code
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
    
    // Get profile image URL - API changed in SDK 7.0
    NSURL* imageURL = nil;
    if (user.profile.hasImage) {
        // Use imageURLWithDimension: method for SDK 7.0+
        imageURL = [user.profile imageURLWithDimension:128];
    }
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
    
    // Use provided server auth code (from sign-in result)
    dict[@"ServerAuthCode"] = serverAuthCode ? serverAuthCode : @"";
    
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
        if (serverClientIdStr && strlen(serverClientIdStr) > 0) {
            serverClientId = [NSString stringWithUTF8String:serverClientIdStr];
        }
        
        // Get the client ID from GoogleService-Info.plist
        NSString* path = [[NSBundle mainBundle] pathForResource:@"GoogleService-Info" ofType:@"plist"];
        NSDictionary* plist = [NSDictionary dictionaryWithContentsOfFile:path];
        NSString* clientId = [plist objectForKey:@"CLIENT_ID"];
        
        if (!clientId) {
            UnitySendMessage("GoogleSignInManager", "OnSignInError", "GoogleService-Info.plist not found or CLIENT_ID missing|0");
            return;
        }
        
        GIDConfiguration* config = [[GIDConfiguration alloc] initWithClientID:clientId];
        if (serverClientId) {
            config = [[GIDConfiguration alloc] initWithClientID:clientId serverClientID:serverClientId];
        }
        
        [GIDSignIn sharedInstance].configuration = config;
    }
    
    void _GoogleSignIn_SignIn() {
        UIViewController* rootViewController = UnityGetGLViewController();
        
        [[GIDSignIn sharedInstance] signInWithPresentingViewController:rootViewController
                                                              completion:^(GIDSignInResult* result, NSError* error) {
            if (error) {
                NSString* errorMsg = [NSString stringWithFormat:@"Sign in failed: %@", error.localizedDescription];
                int errorCode = (int)error.code;
                NSString* message = [NSString stringWithFormat:@"%@|%d", errorMsg, errorCode];
                UnitySendMessage("GoogleSignInManager", "OnSignInError", [message UTF8String]);
            } else if (result && result.user) {
                // Get server auth code from result if available
                NSString* serverAuthCode = @"";
                if (result.serverAuthCode) {
                    serverAuthCode = result.serverAuthCode;
                }
                
                // Create user JSON with server auth code
                NSString* userJson = userToJsonWithServerAuthCode(result.user, serverAuthCode);
                UnitySendMessage("GoogleSignInManager", "OnSignInSuccess", [userJson UTF8String]);
            } else {
                UnitySendMessage("GoogleSignInManager", "OnSignInError", "Sign in failed: No user data|0");
            }
        }];
    }
    
    void _GoogleSignIn_SignOut() {
        // API changed in SDK 7.0+ - signOut doesn't have completion handler
        // Sign out is synchronous, so we call it directly
        [[GIDSignIn sharedInstance] signOut];
        
        // Sign out is synchronous, so we can send success immediately
        UnitySendMessage("GoogleSignInManager", "OnSignOutSuccess", "");
    }
    
    void _GoogleSignIn_GetCurrentUser() {
        GIDGoogleUser* user = [[GIDSignIn sharedInstance] currentUser];
        if (user) {
            NSString* userJson = userToJson(user);
            UnitySendMessage("GoogleSignInManager", "OnGetUserSuccess", [userJson UTF8String]);
        } else {
            UnitySendMessage("GoogleSignInManager", "OnGetUserError", "");
        }
    }
    
    bool _GoogleSignIn_IsSignedIn() {
        GIDGoogleUser* user = [[GIDSignIn sharedInstance] currentUser];
        return user != nil;
    }
}

