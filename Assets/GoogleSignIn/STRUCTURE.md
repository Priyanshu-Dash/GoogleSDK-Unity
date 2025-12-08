# Google Sign-In SDK Structure

## Project Structure

```
Assets/GoogleSignIn/
├── Scripts/
│   ├── GoogleSignInManager.cs          # Main Unity API (Singleton)
│   ├── GoogleSignInUser.cs             # User data model
│   ├── GoogleSignInResult.cs           # Result data model
│   └── Platform/
│       ├── GoogleSignInAndroid.cs       # Android platform bridge
│       └── GoogleSignInIOS.cs          # iOS platform bridge
│
├── Plugins/
│   ├── Android/
│   │   ├── GoogleSignInHelper.java     # Android native implementation
│   │   ├── UnityGoogleSignInActivity.java  # Optional activity extension
│   │   ├── GoogleSignInActivityHandler.java # Activity result handler
│   │   ├── AndroidManifest.xml         # Android permissions
│   │   └── mainTemplate.gradle.example # Gradle dependency example
│   │
│   └── iOS/
│       ├── GoogleSignInIOS.mm           # iOS native implementation (Objective-C++)
│       └── Podfile.example             # CocoaPods example
│
├── Examples/
│   └── GoogleSignInExample.cs          # Usage example script
│
└── Documentation/
    ├── README.md                        # Main documentation
    ├── SETUP_GUIDE.md                   # Detailed setup instructions
    ├── PLACEHOLDER_FILES.md            # Required config files
    └── STRUCTURE.md                     # This file

```

## How It Works

### Unity C# Layer
- `GoogleSignInManager`: Singleton that provides the main API
- Handles callbacks and manages state
- Platform-agnostic interface

### Android Native Layer
- `GoogleSignInHelper.java`: Java class using Google Sign-In Android SDK
- Opens native Google Sign-In popup via `GoogleSignInClient`
- Returns user data to Unity via `UnitySendMessage`
- Handles activity results for sign-in flow

### iOS Native Layer
- `GoogleSignInIOS.mm`: Objective-C++ bridge using Google Sign-In iOS SDK
- Opens native Google Sign-In popup via `GIDSignIn`
- Returns user data to Unity via P/Invoke callbacks

### Communication Flow

```
Unity C# → Platform Bridge → Native SDK → Google Services
                ↓
         User Interaction (Native Popup)
                ↓
Native SDK → Platform Bridge → Unity C# Callback
```

## Key Features

✅ **Native Popups**: Uses platform-native Google Sign-In UI
✅ **Cross-Platform**: Single API for both Android and iOS
✅ **Server Auth**: Optional server-side authentication support
✅ **Persistent State**: Sign-in state persists across app restarts
✅ **Error Handling**: Comprehensive error reporting
✅ **Simple API**: Easy to use Unity C# interface

## Platform Requirements

- **Android**: 
  - Minimum API Level 21 (Android 5.0)
  - Google Play Services required
  - Internet permission

- **iOS**:
  - Minimum iOS 11.0
  - Google Sign-In framework
  - URL scheme configuration

## Dependencies

### Android
- `com.google.android.gms:play-services-auth:20.7.0`

### iOS
- `GoogleSignIn` framework (~> 7.0)

## Usage Flow

1. **Initialize**: `GoogleSignInManager.Instance.Initialize()`
2. **Sign In**: `GoogleSignInManager.Instance.SignIn(callback)` → Opens native popup
3. **Handle Result**: Callback receives `GoogleSignInResult` with user data
4. **Sign Out**: `GoogleSignInManager.Instance.SignOut(callback)`
5. **Check Status**: `GoogleSignInManager.Instance.IsSignedIn()`

## Next Steps

1. Follow `SETUP_GUIDE.md` for configuration
2. Add required config files (see `PLACEHOLDER_FILES.md`)
3. Test with `GoogleSignInExample.cs` script
4. Integrate into your game/app

