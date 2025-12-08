# Google Sign-In SDK for Unity

A native Google Sign-In SDK for Unity that works on both Android and iOS platforms. This SDK opens the native Google Sign-In popup on mobile devices.

## Features

- ✅ Native Google Sign-In popup on Android and iOS
- ✅ Sign in with Google account
- ✅ Sign out functionality
- ✅ Get current user information
- ✅ Server-side authentication support (optional)
- ✅ Simple Unity C# API

## Setup Instructions

### Prerequisites

1. **Google Cloud Console Setup**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select an existing one
   - Enable Google Sign-In API
   - Create OAuth 2.0 credentials for both Android and iOS

### Android Setup

1. **Get your SHA-1 fingerprint:**
   ```bash
   # For debug keystore (default Unity keystore)
   keytool -list -v -keystore "%USERPROFILE%\.android\debug.keystore" -alias androiddebugkey -storepass android -keypass android
   
   # For release keystore
   keytool -list -v -keystore "path/to/your/keystore" -alias "your-alias"
   ```

2. **Configure OAuth Client in Google Cloud Console:**
   - Application type: Android
   - Package name: Your Unity package name (e.g., `com.yourcompany.yourapp`)
   - SHA-1 certificate fingerprint: From step 1

3. **Add Google Services to your Android project:**
   - Download `google-services.json` from Firebase Console
   - Place it in `Assets/Plugins/Android/google-services.json`

4. **Add Gradle Dependencies:**
   Create or edit `Assets/Plugins/Android/mainTemplate.gradle`:
   ```gradle
   dependencies {
       implementation 'com.google.android.gms:play-services-auth:20.7.0'
   }
   ```

### iOS Setup

1. **Configure OAuth Client in Google Cloud Console:**
   - Application type: iOS
   - Bundle ID: Your Unity bundle identifier (e.g., `com.yourcompany.yourapp`)

2. **Add Google Services to your iOS project:**
   - Download `GoogleService-Info.plist` from Firebase Console
   - Place it in `Assets/Plugins/iOS/GoogleService-Info.plist`

3. **Add Google Sign-In SDK:**
   - Option A: Using CocoaPods (Recommended)
     - Create `Assets/Plugins/iOS/Podfile`:
     ```ruby
     platform :ios, '11.0'
     target 'UnityFramework' do
       pod 'GoogleSignIn', '~> 7.0'
     end
     ```
   - Option B: Manual Framework
     - Download Google Sign-In SDK from [Google](https://developers.google.com/identity/sign-in/ios/sdk)
     - Add `GoogleSignIn.framework` to `Assets/Plugins/iOS/`

4. **Configure URL Scheme:**
   - In Unity, go to `Edit > Project Settings > Player > iOS`
   - Add your reversed client ID as a URL scheme
   - Or edit `Info.plist` after build to add:
   ```xml
   <key>CFBundleURLTypes</key>
   <array>
       <dict>
           <key>CFBundleURLSchemes</key>
           <array>
               <string>YOUR_REVERSED_CLIENT_ID</string>
           </array>
       </dict>
   </array>
   ```

## Usage

### Basic Usage

```csharp
using GoogleSignIn;
using UnityEngine;

public class SignInExample : MonoBehaviour
{
    void Start()
    {
        // Initialize (optional - can pass server client ID for backend auth)
        GoogleSignInManager.Instance.Initialize();
    }

    public void OnSignInButtonClick()
    {
        // Sign in - opens native popup
        GoogleSignInManager.Instance.SignIn((result) =>
        {
            if (result.Success)
            {
                Debug.Log($"Signed in as: {result.User.DisplayName}");
                Debug.Log($"Email: {result.User.Email}");
                Debug.Log($"ID Token: {result.User.IdToken}");
            }
            else
            {
                Debug.LogError($"Sign in failed: {result.ErrorMessage}");
            }
        });
    }

    public void OnSignOutButtonClick()
    {
        GoogleSignInManager.Instance.SignOut((result) =>
        {
            if (result.Success)
            {
                Debug.Log("Signed out successfully");
            }
        });
    }

    public void CheckCurrentUser()
    {
        GoogleSignInManager.Instance.GetCurrentUser((user) =>
        {
            if (user != null)
            {
                Debug.Log($"Current user: {user.DisplayName}");
            }
            else
            {
                Debug.Log("No user signed in");
            }
        });
    }
}
```

### With Server Authentication

If you need to authenticate with your backend server:

```csharp
// Initialize with server client ID
GoogleSignInManager.Instance.Initialize("YOUR_SERVER_CLIENT_ID");

// After sign in, use result.User.ServerAuthCode to authenticate with your backend
GoogleSignInManager.Instance.SignIn((result) =>
{
    if (result.Success)
    {
        string serverAuthCode = result.User.ServerAuthCode;
        // Send serverAuthCode to your backend server
    }
});
```

## API Reference

### GoogleSignInManager

- `Instance` - Singleton instance
- `Initialize(string serverClientId = null)` - Initialize the SDK
- `SignIn(Action<GoogleSignInResult> callback)` - Sign in with Google
- `SignOut(Action<GoogleSignInResult> callback)` - Sign out
- `GetCurrentUser(Action<GoogleSignInUser> callback)` - Get current signed-in user
- `IsSignedIn()` - Check if user is signed in

### GoogleSignInUser

- `Id` - User ID
- `Email` - User email
- `DisplayName` - User display name
- `PhotoUrl` - User profile photo URL
- `IdToken` - ID token for client-side verification
- `ServerAuthCode` - Server auth code (if server client ID provided)
- `AccessToken` - Access token (iOS only)

### GoogleSignInResult

- `Success` - Whether the operation succeeded
- `User` - User data (if successful)
- `ErrorMessage` - Error message (if failed)
- `ErrorCode` - Error code (if failed)

## Troubleshooting

### Android Issues

- **"Sign in failed"**: Check that `google-services.json` is in the correct location
- **"DEVELOPER_ERROR"**: Verify SHA-1 fingerprint matches in Google Cloud Console
- **No popup appears**: Ensure Google Play Services is installed on the device

### iOS Issues

- **"GoogleService-Info.plist not found"**: Ensure the file is in `Assets/Plugins/iOS/`
- **URL scheme not working**: Verify reversed client ID is added to Info.plist
- **Build errors**: Ensure Google Sign-In SDK is properly linked (CocoaPods or manual)

## Notes

- This SDK only works on **Android and iOS devices**, not in the Unity Editor
- Test on physical devices for best results
- The native popup will appear automatically when `SignIn()` is called
- User can cancel the sign-in flow, which will return an error result

## License

This SDK is provided as-is for use in your Unity projects.

