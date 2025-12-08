#import <Foundation/Foundation.h>
#import <GoogleSignIn/GoogleSignIn.h>
#import "UnityInterface.h"

extern "C" {
    // Forward declarations
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
}

static NSString* serverClientId = nil;

// Helper function to convert GoogleSignInUser to JSON
NSString* userToJson(GIDGoogleUser* user) {
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
    
    if (user.profile.imageURL) {
        dict[@"PhotoUrl"] = [user.profile.imageURL absoluteString];
    } else {
        dict[@"PhotoUrl"] = @"";
    }
    
    if (user.idToken.tokenString) {
        dict[@"IdToken"] = user.idToken.tokenString;
    } else {
        dict[@"IdToken"] = @"";
    }
    
    if (user.serverAuthCode) {
        dict[@"ServerAuthCode"] = user.serverAuthCode;
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
                NSString* userJson = userToJson(result.user);
                UnitySendMessage("GoogleSignInManager", "OnSignInSuccess", [userJson UTF8String]);
            } else {
                UnitySendMessage("GoogleSignInManager", "OnSignInError", "Sign in failed: No user data|0");
            }
        }];
    }
    
    void _GoogleSignIn_SignOut() {
        [[GIDSignIn sharedInstance] signOutWithCompletion:^(NSError* error) {
            if (error) {
                NSString* errorMsg = [NSString stringWithFormat:@"Sign out failed: %@", error.localizedDescription];
                UnitySendMessage("GoogleSignInManager", "OnSignOutError", [errorMsg UTF8String]);
            } else {
                UnitySendMessage("GoogleSignInManager", "OnSignOutSuccess", "");
            }
        }];
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

