# Installing External Dependency Manager (EDM4U)

## Method 1: Via Package Manager (Recommended - Already Added)

I've already added EDM4U to your `Packages/manifest.json`. Unity should automatically download it.

**To verify:**
1. Open Unity
2. Go to **Window** → **Package Manager**
3. Look for "External Dependency Manager" in the list
4. If it's not there, Unity may need to refresh packages

## Method 2: Via .unitypackage (Alternative)

If the Package Manager method doesn't work:

1. **Download EDM4U:**
   - Visit: https://github.com/googlesamples/unity-jar-resolver/releases
   - Download the latest `.unitypackage` file

2. **Import into Unity:**
   - In Unity: **Assets** → **Import Package** → **Custom Package...**
   - Select the downloaded `.unitypackage`
   - Click **Import**

3. **Verify Installation:**
   - Go to **Assets** → **External Dependency Manager** → **iOS Resolver** → **Settings**
   - You should see EDM4U options

## Method 3: Via Git URL (Current Setup)

The current setup uses a Git URL in `manifest.json`:
```json
"com.google.external-dependency-manager": "https://github.com/googlesamples/unity-jar-resolver.git"
```

This should work automatically. If Unity doesn't download it:
1. Close Unity
2. Delete `Library/PackageCache` folder (Unity will regenerate it)
3. Reopen Unity
4. Unity will download EDM4U automatically

## After Installation

Once EDM4U is installed:

1. **Verify it's working:**
   - **Window** → **External Dependency Manager** → **iOS Resolver** → **Settings**
   - Ensure "Podfile Generation" is enabled

2. **Build iOS project:**
   - EDM4U will automatically:
     - Generate Podfile from `Dependencies.xml`
     - Run `pod install`
     - Create `.xcworkspace`

3. **No manual steps needed!**

## Troubleshooting

### Package Not Downloading
- Check Unity Console for errors
- Try: **Assets** → **Reimport All**
- Restart Unity

### EDM4U Menu Not Appearing
- The package might not be fully imported
- Check Package Manager window
- Try Method 2 (.unitypackage) instead

### Still Need Manual pod install
- Check EDM4U settings: **Assets** → **External Dependency Manager** → **iOS Resolver** → **Settings**
- Ensure "Podfile Generation" is enabled
- Try: **iOS Resolver** → **Resolve** (manual resolve)

## Current Status

✅ `Dependencies.xml` created - tells EDM4U which pods to install
✅ `manifest.json` updated - includes EDM4U package
✅ PostProcessBuild script updated - works with EDM4U

**Next:** Wait for Unity to import EDM4U, then build iOS project!

