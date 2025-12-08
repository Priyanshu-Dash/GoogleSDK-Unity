# iOS Google Sign-In Setup Guide

This guide will help you set up Google Sign-In for iOS in your Unity project.

## Prerequisites

1. **CocoaPods** installed on your Mac
   - Install via: `sudo gem install cocoapods`
   - Verify: `pod --version`

2. **Firebase Console** access
   - You need a Firebase project with iOS app configured

## Step 1: Download GoogleService-Info.plist

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Select your project
3. Click the iOS app (or add one if it doesn't exist)
4. Download `GoogleService-Info.plist`
5. **Copy the file to your Unity project's `Assets/` folder** (same level as `google-services.json`)

**Important:** The file must be named exactly `GoogleService-Info.plist` and placed in the `Assets/` folder root.

## Step 2: Build iOS Project from Unity

1. In Unity, go to **File > Build Settings**
2. Select **iOS** platform
3. Click **Build** (or **Build and Run**)
4. Choose a folder to export the Xcode project

## Step 3: Install CocoaPods Dependencies

1. Open Terminal
2. Navigate to your exported Xcode project directory:
   ```bash
   cd /path/to/your/Unity-iPhone
   ```
3. Run CocoaPods install:
   ```bash
   pod install
   ```
4. Wait for the installation to complete

## Step 4: Open the Workspace (Not the Project)

**Important:** After running `pod install`, you must open the `.xcworkspace` file, NOT the `.xcodeproj` file.

1. In Finder, navigate to your exported project folder
2. Open `Unity-iPhone.xcworkspace` (double-click it)
3. This will open Xcode with all CocoaPods dependencies properly linked

## Step 5: Configure URL Schemes (Automatic)

The PostProcessBuild script should automatically configure the URL scheme in Info.plist. However, if you need to verify or manually configure:

1. In Xcode, select your project in the navigator
2. Select the **Unity-iPhone** target
3. Go to the **Info** tab
4. Expand **URL Types**
5. Verify that a URL scheme matching your `REVERSED_CLIENT_ID` from `GoogleService-Info.plist` exists

The `REVERSED_CLIENT_ID` can be found in your `GoogleService-Info.plist` file.

## Step 6: Build and Run

1. Select your target device or simulator
2. Click the **Play** button in Xcode
3. The app should build and run successfully

## Troubleshooting

### Error: 'GoogleSignIn/GoogleSignIn.h' file not found

**Solution:** This means CocoaPods dependencies are not installed.
- Make sure you ran `pod install` in the Xcode project directory
- Make sure you opened `.xcworkspace` and not `.xcodeproj`

### Error: GoogleService-Info.plist not found

**Solution:** 
- Download `GoogleService-Info.plist` from Firebase Console
- Place it in `Assets/` folder (root of Assets, not in a subfolder)
- Rebuild the iOS project from Unity

### Error: URL scheme not configured

**Solution:**
- The PostProcessBuild script should handle this automatically
- If not, manually add the `REVERSED_CLIENT_ID` as a URL scheme in Info.plist
- The `REVERSED_CLIENT_ID` value is in your `GoogleService-Info.plist` file

### Podfile not found

**Solution:**
- The Podfile should be automatically copied from `Assets/Plugins/iOS/Podfile` during Unity build
- If it's missing, manually copy it to the Xcode project directory before running `pod install`

## Additional Notes

- The minimum iOS version is set to 11.0 in the Podfile
- Google Sign-In SDK version is set to ~> 7.0 (latest 7.x version)
- The PostProcessBuild script runs automatically after each iOS build
- You only need to run `pod install` once, or when you update dependencies

## Verification

To verify everything is set up correctly:

1. Build and run the app on a device or simulator
2. Call `GoogleSignInManager.Instance.Initialize()` in your code
3. Call `GoogleSignInManager.Instance.SignIn()` 
4. You should see the Google Sign-In popup

If you encounter any issues, check the Xcode console for detailed error messages.


