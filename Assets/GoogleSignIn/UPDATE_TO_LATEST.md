# Updating to Latest Google Sign-In SDK

## Current Configuration

✅ **Updated to use latest 7.1.x version**
- `Dependencies.xml`: `~> 7.1`
- `Podfile`: `~> 7.1`

This will automatically get the latest 7.1.x version available on CocoaPods.

## How to Get the Absolute Latest Version

### Option 1: Remove Version Constraint (Not Recommended)

If you want to always get the absolute latest version (including major version updates):

**In `Dependencies.xml`:**
```xml
<iosPod name="GoogleSignIn" />
```

**In `Podfile`:**
```ruby
pod 'GoogleSignIn'
```

⚠️ **Warning:** This may break your code if Google releases a major version with API changes.

### Option 2: Check Latest Version Manually

1. **Check CocoaPods for latest version:**
   ```bash
   pod search GoogleSignIn
   ```

2. **Update to specific version:**
   ```xml
   <!-- In Dependencies.xml -->
   <iosPod name="GoogleSignIn" version="7.1.0" />
   ```
   
   ```ruby
   # In Podfile
   pod 'GoogleSignIn', '7.1.0'
   ```

### Option 3: Use Latest Major Version (When Available)

When Google releases version 8.0 or higher:

**In `Dependencies.xml`:**
```xml
<iosPod name="GoogleSignIn" version="~> 8.0" />
```

**In `Podfile`:**
```ruby
pod 'GoogleSignIn', '~> 8.0'
```

⚠️ **Important:** Major version updates may have breaking API changes. You'll need to update the native code (`GoogleSignInIOS.mm`) accordingly.

## Current Code Compatibility

The current implementation (`GoogleSignInIOS.mm`) is compatible with:
- ✅ Google Sign-In SDK 7.0+
- ✅ Uses `imageURLWithDimension:` for profile images
- ✅ Uses `serverAuthCode` from `GIDSignInResult`
- ✅ Uses synchronous `signOut` method

## After Updating Version

1. **Rebuild iOS project** from Unity
2. **Run `pod install`** (or let EDM4U do it automatically)
3. **Check for API changes** in Google's release notes
4. **Update native code** if needed

## Checking Current Installed Version

After building, check the installed version:

```bash
cd /path/to/your/Xcode/project
cat Pods/GoogleSignIn/GoogleSignIn.podspec | grep "s.version"
```

Or in Xcode:
- Open `Pods` project
- Select `GoogleSignIn` target
- Check version in target settings

## Version Constraint Syntax

- `~> 7.1` = Latest 7.1.x (7.1.0, 7.1.1, 7.1.2, etc.)
- `~> 7.0` = Latest 7.x (7.0.0, 7.1.0, 7.2.0, etc., but not 8.0.0)
- `7.1.0` = Exact version 7.1.0 only
- No version = Latest available (any version)

## Recommended Approach

✅ **Current setup (Recommended):**
- Using `~> 7.1` ensures you get the latest patch/bugfix updates
- Maintains compatibility with your code
- Won't break if Google releases 8.0

This is the safest approach for production use.

