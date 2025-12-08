# External Dependency Manager (EDM4U) Setup

## What is EDM4U?

External Dependency Manager for Unity (EDM4U) automatically manages CocoaPods dependencies for iOS builds. This means you **don't need to manually run `pod install`** anymore!

## What Was Added

✅ **External Dependency Manager** added to `Packages/manifest.json`
✅ **Dependencies.xml** created at `Assets/GoogleSignIn/Plugins/iOS/Dependencies.xml`
✅ This automatically configures Google Sign-In SDK via CocoaPods

## How It Works

1. **Unity builds iOS project** → EDM4U automatically:
   - Generates Podfile
   - Runs `pod install`
   - Creates `.xcworkspace` file
   - Links all dependencies

2. **You just open the workspace** - everything is ready!

## Setup Steps

### Step 1: Wait for Package Import
After adding EDM4U to `manifest.json`, Unity will automatically:
- Download the package
- Import it into your project
- This may take a minute or two

### Step 2: Verify EDM4U is Installed
1. In Unity, go to **Window** → **External Dependency Manager** → **iOS Resolver** → **Settings**
2. You should see EDM4U options

### Step 3: Build iOS Project
1. **File** → **Build Settings**
2. Select **iOS**
3. Click **Build**
4. EDM4U will automatically:
   - Generate Podfile
   - Run `pod install`
   - Create workspace file

### Step 4: Open Workspace
After build completes:
- Open `Unity-iPhone.xcworkspace` (not `.xcodeproj`)
- Everything should be ready!

## Benefits

✅ **No manual `pod install`** - EDM4U handles it automatically
✅ **Automatic workspace creation** - `.xcworkspace` is created for you
✅ **Consistent builds** - Dependencies are always up to date
✅ **Team-friendly** - Everyone gets the same setup automatically

## Dependencies.xml

The `Dependencies.xml` file tells EDM4U which CocoaPods to install:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<dependencies>
  <iosPods>
    <iosPod name="GoogleSignIn" version="~> 7.0" />
  </iosPods>
</dependencies>
```

## Troubleshooting

### EDM4U Not Installing
- Check Unity Console for errors
- Try: **Assets** → **External Dependency Manager** → **iOS Resolver** → **Settings** → **Reset**
- Restart Unity

### Pod Install Still Required
- EDM4U should handle this automatically
- If not, check: **Window** → **External Dependency Manager** → **iOS Resolver** → **Settings**
- Ensure "Podfile Generation" is enabled

### Workspace Not Created
- EDM4U should create it automatically
- Check build logs for EDM4U messages
- Verify `Dependencies.xml` is in the correct location

## Manual Fallback

If EDM4U doesn't work, you can still use the manual Podfile method:
- The `Assets/Plugins/iOS/Podfile` will be used as a fallback
- You'll need to run `pod install` manually

## Next Steps

1. **Wait for Unity to import EDM4U** (check Package Manager)
2. **Build iOS project** - EDM4U will handle CocoaPods automatically
3. **Open `.xcworkspace`** - Everything should be ready!

No more manual `pod install` needed! 🎉

