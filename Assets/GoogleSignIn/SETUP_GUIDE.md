# Google Sign-In SDK Setup Guide

## Quick Start

### Step 1: Google Cloud Console Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable **Google Sign-In API**
4. Go to **APIs & Services > Credentials**

### Step 2: Android Configuration

#### 2.1 Get SHA-1 Fingerprint

**Windows:**
```powershell
# Debug keystore (default Unity)
keytool -list -v -keystore "%USERPROFILE%\.android\debug.keystore" -alias androiddebugkey -storepass android -keypass android

# Release keystore
keytool -list -v -keystore "path\to\your\keystore" -alias "your-alias"
```

**Mac/Linux:**
```bash
# Debug keystore
keytool -list -v -keystore ~/.android/debug.keystore -alias androiddebugkey -storepass android -keypass android
```

Copy the **SHA-1** value (looks like: `AA:BB:CC:DD:...`)

#### 2.2 Create Android OAuth Client

1. In Google Cloud Console, click **Create Credentials > OAuth client ID**
2. Select **Android** as application type
3. Enter your package name (from Unity: `Edit > Project Settings > Player > Android > Package Name`)
4. Paste your SHA-1 fingerprint
5. Click **Create**
6. **Save the Client ID** - you'll need it for `google-services.json`

#### 2.3 Download google-services.json

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Create a project or link to your Google Cloud project
3. Add Android app with your package name
4. Download `google-services.json`
5. Place it in: `Assets/Plugins/Android/google-services.json`

#### 2.4 Configure Gradle

Create or edit `Assets/Plugins/Android/mainTemplate.gradle`:

```gradle
dependencies {
    implementation 'com.google.android.gms:play-services-auth:20.7.0'
}
```

**OR** if you're using `gradleTemplate.properties`, enable:
```
android.useAndroidX=true
android.enableJetifier=true
```

### Step 3: iOS Configuration

#### 3.1 Create iOS OAuth Client

1. In Google Cloud Console, click **Create Credentials > OAuth client ID**
2. Select **iOS** as application type
3. Enter your Bundle ID (from Unity: `Edit > Project Settings > Player > iOS > Bundle Identifier`)
4. Click **Create**
5. **Save the Client ID**

#### 3.2 Download GoogleService-Info.plist

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Add iOS app with your Bundle ID
3. Download `GoogleService-Info.plist`
4. Place it in: `Assets/Plugins/iOS/GoogleService-Info.plist`

#### 3.3 Add Google Sign-In SDK

**Option A: Using CocoaPods (Recommended)**

1. Create `Assets/Plugins/iOS/Podfile`:
```ruby
platform :ios, '11.0'
target 'UnityFramework' do
  pod 'GoogleSignIn', '~> 7.0'
end
```

2. After building Xcode project, run in terminal:
```bash
cd <path-to-xcode-project>
pod install
```

**Option B: Manual Framework**

1. Download [Google Sign-In SDK](https://developers.google.com/identity/sign-in/ios/sdk)
2. Extract and add `GoogleSignIn.framework` to `Assets/Plugins/iOS/`
3. In Xcode, link the framework manually

#### 3.4 Configure URL Scheme

After building to Xcode:

1. Open `Info.plist` in Xcode
2. Find `REVERSED_CLIENT_ID` in `GoogleService-Info.plist`
3. Add URL Scheme in `Info.plist`:
   - Right-click `Info.plist` > Open As > Source Code
   - Add before `</dict>`:
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

### Step 4: Test the SDK

1. Create a test scene
2. Add `GoogleSignInExample` script to a GameObject
3. Build and run on a **physical device** (Sign-In doesn't work in Editor)
4. Click Sign In button - native popup should appear!

## Troubleshooting

### Android

**"DEVELOPER_ERROR"**
- Verify SHA-1 fingerprint matches in Google Cloud Console
- Make sure package name is correct

**"Sign in failed"**
- Check `google-services.json` is in correct location
- Verify Google Play Services is installed on device
- Check internet connection

**No popup appears**
- Ensure device has Google Play Services
- Check AndroidManifest.xml has INTERNET permission

### iOS

**"GoogleService-Info.plist not found"**
- Verify file is in `Assets/Plugins/iOS/`
- Check file is included in build (should be automatic)

**URL scheme not working**
- Verify reversed client ID is correct in Info.plist
- Rebuild Xcode project after adding URL scheme

**Build errors**
- Run `pod install` if using CocoaPods
- Check framework is properly linked in Xcode

## Server-Side Authentication (Optional)

If you need to authenticate with your backend:

1. Create **Web application** OAuth client in Google Cloud Console
2. Use the Client ID as `serverClientId` when initializing:
```csharp
GoogleSignInManager.Instance.Initialize("YOUR_SERVER_CLIENT_ID");
```
3. After sign-in, use `result.User.ServerAuthCode` to authenticate with your backend

## Notes

- SDK only works on **physical Android/iOS devices**, not in Unity Editor
- Test on real devices for best results
- User can cancel sign-in, which returns an error
- Sign-in state persists across app restarts

