# EDM4U Not Generating Workspace - Troubleshooting Guide

## Issue: Workspace Not Being Generated

If EDM4U is not automatically generating the workspace, follow these steps:

## ✅ Step 1: Verify Dependencies.xml Location

EDM4U looks for `Dependencies.xml` in these locations (in order):
1. `Assets/Plugins/iOS/Dependencies.xml` ✅ **Created here**
2. `Assets/GoogleSignIn/Plugins/iOS/Dependencies.xml` ✅ **Also exists here**

Both files should exist and contain:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<dependencies>
  <iosPods>
    <iosPod name="GoogleSignIn" version="~> 7.0" />
  </iosPods>
</dependencies>
```

## ✅ Step 2: Check EDM4U Settings

1. In Unity, go to: **Assets** → **External Dependency Manager** → **iOS Resolver** → **Settings**

2. Verify these settings are enabled:
   - ✅ **Podfile Generation** - Must be enabled
   - ✅ **CocoaPods Integration** - Must be enabled
   - ✅ **Pod Tool Path** - Should point to CocoaPods (usually `/usr/local/bin/pod` or `/opt/homebrew/bin/pod`)

3. If settings are disabled, enable them and click **Apply**

## ✅ Step 3: Manually Trigger Resolution

Sometimes EDM4U needs to be manually triggered:

1. **Assets** → **External Dependency Manager** → **iOS Resolver** → **Resolve**
   - This manually triggers dependency resolution
   - You should see messages in Unity Console

2. Check Unity Console for:
   - "iOS Resolver: Generating Podfile..."
   - "iOS Resolver: Running pod install..."
   - Any error messages

## ✅ Step 4: Check Build Logs

When building iOS project, watch Unity Console for EDM4U messages:

**Expected messages:**
- `iOS Resolver: Processing Dependencies.xml...`
- `iOS Resolver: Generating Podfile...`
- `iOS Resolver: Running pod install...`
- `iOS Resolver: Pod installation complete`

**If you don't see these messages:**
- EDM4U might not be running
- Check for errors in Console

## ✅ Step 5: Verify CocoaPods Installation

EDM4U needs CocoaPods to be installed:

1. Open Terminal
2. Run: `pod --version`
3. If not installed: `sudo gem install cocoapods`

If CocoaPods is not in the standard location:
1. Find CocoaPods: `which pod`
2. In Unity: **Assets** → **External Dependency Manager** → **iOS Resolver** → **Settings**
3. Set **Pod Tool Path** to the path from step 1

## ✅ Step 6: Force EDM4U to Run

If EDM4U still doesn't run automatically:

1. **Assets** → **External Dependency Manager** → **iOS Resolver** → **Settings**
2. Click **Reset** (this resets EDM4U settings)
3. Re-enable **Podfile Generation**
4. Click **Apply**
5. Try building again

## ✅ Step 7: Check Post-Process Build Order

EDM4U runs during the build process. Make sure:

1. EDM4U PostProcessBuild runs before your custom PostProcessBuild
2. Check build order in Unity Console
3. EDM4U should run first, then your script

## ✅ Step 8: Manual Workaround

If EDM4U still doesn't work, you can manually trigger it:

1. Build iOS project from Unity
2. After build completes, in Unity:
   - **Assets** → **External Dependency Manager** → **iOS Resolver** → **Resolve**
3. This will generate Podfile and run pod install
4. Then manually create workspace (see below)

## 🔧 Manual Workspace Creation (Fallback)

If EDM4U doesn't create the workspace automatically:

1. **Build iOS project** from Unity
2. **Navigate to Xcode project directory**
3. **Run manually:**
   ```bash
   cd "/path/to/your/Xcode/project"
   pod install
   ```
4. This will create the workspace

## 📋 Checklist

Before building, verify:
- [ ] `Dependencies.xml` exists at `Assets/Plugins/iOS/Dependencies.xml`
- [ ] EDM4U menu appears: **Assets** → **External Dependency Manager**
- [ ] iOS Resolver Settings: **Podfile Generation** is enabled
- [ ] CocoaPods is installed: `pod --version` works
- [ ] Pod Tool Path is set correctly in EDM4U settings

After building, check:
- [ ] Unity Console shows EDM4U messages
- [ ] `Podfile` exists in Xcode project directory
- [ ] `Pods/` directory exists
- [ ] `Unity-iPhone.xcworkspace` exists

## 🚨 Common Issues

### Issue: EDM4U Menu Not Appearing
**Solution:**
- EDM4U might not be fully imported
- Check Package Manager window
- Restart Unity

### Issue: No EDM4U Messages in Console
**Solution:**
- EDM4U might not be running
- Manually trigger: **Assets** → **External Dependency Manager** → **iOS Resolver** → **Resolve**

### Issue: Podfile Generated But No Workspace
**Solution:**
- EDM4U generated Podfile but didn't run `pod install`
- Manually run: `pod install` in Xcode project directory

### Issue: CocoaPods Not Found
**Solution:**
- Install CocoaPods: `sudo gem install cocoapods`
- Or set Pod Tool Path in EDM4U settings

## 📝 Next Steps

1. **Verify all checklist items above**
2. **Try manual resolve:** **Assets** → **External Dependency Manager** → **iOS Resolver** → **Resolve**
3. **Build iOS project** and watch Unity Console
4. **If still not working**, use manual `pod install` as fallback

## 💡 Pro Tip

You can also check EDM4U logs:
- **Assets** → **External Dependency Manager** → **iOS Resolver** → **Settings**
- Look for "Log Level" - set to "Verbose" for more details

