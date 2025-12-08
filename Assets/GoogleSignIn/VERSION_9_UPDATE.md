# Updated to Google Sign-In SDK 9.0.0

## ✅ What Was Updated

1. **Dependencies.xml** (both locations):
   - Updated from `~> 7.0` to `~> 9.0`
   - Location 1: `Assets/Plugins/iOS/Dependencies.xml`
   - Location 2: `Assets/GoogleSignIn/Plugins/iOS/Dependencies.xml`

2. **Podfile**:
   - Updated from `~> 7.1` to `~> 9.0`
   - Location: `Assets/Plugins/iOS/Podfile`

3. **Native Code** (`GoogleSignInIOS.mm`):
   - Already updated to use SDK 7.0+ APIs which are compatible with 9.0:
     - ✅ Uses `imageURLWithDimension:` for profile images
     - ✅ Uses `serverAuthCode` from `GIDSignInResult`
     - ✅ Uses synchronous `signOut` method

## 🚀 Next Steps

1. **Rebuild iOS Project:**
   - In Unity: **File** → **Build Settings** → **iOS** → **Build**
   - EDM4U will automatically install version 9.0.0

2. **Or Manually Update:**
   ```bash
   cd /path/to/your/Xcode/project
   pod update GoogleSignIn
   ```

3. **Verify Installation:**
   - Check Unity Console for EDM4U messages
   - Verify in Xcode: `Pods/GoogleSignIn/GoogleSignIn.podspec`

## 📋 Version Information

- **Previous Version:** 7.0.x
- **New Version:** 9.0.0 (July 2025)
- **Version Constraint:** `~> 9.0` (will get latest 9.x version)

## ⚠️ Important Notes

1. **API Compatibility:**
   - The current code should be compatible with SDK 9.0.0
   - If you encounter any API errors, they may need additional updates

2. **Breaking Changes:**
   - Version 9.0.0 is a major version update
   - Check Google's release notes for any breaking changes
   - Test thoroughly after updating

3. **Deployment Target:**
   - Podfile is set to iOS 12.0 minimum
   - Version 9.0.0 may require iOS 13.0+ (check release notes)

## 🔍 Verification

After building, verify the version:

```bash
cd /path/to/your/Xcode/project
cat Pods/GoogleSignIn/GoogleSignIn.podspec | grep "s.version"
```

Should show: `s.version = "9.0.0"`

## 📚 Resources

- [Google Sign-In iOS Documentation](https://developers.google.com/identity/sign-in/ios)
- [Google Sign-In iOS GitHub](https://github.com/google/GoogleSignIn-iOS)
- [CocoaPods GoogleSignIn](https://cocoapods.org/pods/GoogleSignIn)

## 🐛 Troubleshooting

If you encounter issues after updating:

1. **Clean Build:**
   - Delete `Pods/` directory
   - Delete `Podfile.lock`
   - Run `pod install` again

2. **Check API Compatibility:**
   - Review Google's migration guide for version 9.0
   - Update native code if needed

3. **Rollback if Needed:**
   - Change version back to `~> 7.0` in Dependencies.xml and Podfile
   - Rebuild project

