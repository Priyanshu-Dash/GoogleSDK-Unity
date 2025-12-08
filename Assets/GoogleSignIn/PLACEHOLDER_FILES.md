# Required Configuration Files

You need to add these files manually after setting up Google Cloud Console:

## Android

### `Assets/Plugins/Android/google-services.json`
- Download from Firebase Console
- Place in: `Assets/Plugins/Android/google-services.json`
- Contains your Android OAuth client configuration

## iOS

### `Assets/Plugins/iOS/GoogleService-Info.plist`
- Download from Firebase Console
- Place in: `Assets/Plugins/iOS/GoogleService-Info.plist`
- Contains your iOS OAuth client configuration

## Gradle Configuration

### `Assets/Plugins/Android/mainTemplate.gradle`
- Add Google Sign-In dependency
- See `mainTemplate.gradle.example` for reference

## CocoaPods Configuration (iOS)

### `Assets/Plugins/iOS/Podfile`
- For CocoaPods installation
- See `Podfile.example` for reference
- Run `pod install` after building Xcode project

---

**Note:** These files are not included in the SDK and must be obtained from:
- Google Cloud Console (for OAuth clients)
- Firebase Console (for google-services.json and GoogleService-Info.plist)

