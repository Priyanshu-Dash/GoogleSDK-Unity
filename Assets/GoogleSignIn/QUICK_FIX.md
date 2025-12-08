# Quick Fix for iOS Build Error

## The Problem
You're getting: `'GoogleSignIn/GoogleSignIn.h' file not found`

This happens because:
1. CocoaPods dependencies are not installed
2. You need to run `pod install` in your Xcode project directory

## Immediate Solution

### Step 1: Navigate to Your Xcode Project
Open Terminal and go to your exported Xcode project:
```bash
cd "/Users/priyanshudash/Desktop/GoogleSDk Unity"
```

### Step 2: Install CocoaPods Dependencies
```bash
pod install
```

**Note:** If you get "Podfile not found", the Podfile should now be in `Assets/Plugins/iOS/Podfile` in your Unity project. Rebuild the iOS project from Unity first, then run `pod install`.

### Step 3: Open the Workspace (NOT the Project)
After `pod install` completes:
- **DO NOT** open `Unity-iPhone.xcodeproj`
- **DO** open `Unity-iPhone.xcworkspace` (double-click it in Finder)

### Step 4: Build Again
In Xcode, select your target and build. The error should be resolved.

## What Was Fixed

✅ Created `Assets/Plugins/iOS/Podfile` - Unity will copy this to your Xcode project
✅ Created PostProcessBuild script - Automatically configures Info.plist URL schemes
✅ Created setup documentation - See `Assets/GoogleSignIn/Plugins/iOS/IOS_SETUP.md`

## Still Need to Do

1. **Get GoogleService-Info.plist** from Firebase Console
   - Download it and place in `Assets/` folder (same level as `google-services.json`)
   - This is required for the app to work

2. **Rebuild iOS project from Unity** (if you haven't already)
   - This will copy the Podfile to the Xcode project

3. **Run `pod install`** in the Xcode project directory

4. **Open `.xcworkspace`** instead of `.xcodeproj`

## Why This Happens

Unity doesn't automatically run CocoaPods. You must:
1. Build the iOS project from Unity (which copies the Podfile)
2. Manually run `pod install` in Terminal
3. Open the `.xcworkspace` file (which includes CocoaPods dependencies)

This is a one-time setup (or whenever you update dependencies).


