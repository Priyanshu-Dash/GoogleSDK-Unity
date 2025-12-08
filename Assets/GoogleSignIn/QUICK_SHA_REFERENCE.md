# SHA Fingerprints - Quick Reference

## Your SHA-1 Fingerprint (Copy this to Google Cloud Console)

```
72:38:37:A2:65:61:64:E3:9D:52:A8:64:AD:21:7D:8E:EB:A2:6F:51
```

## SHA-256 Fingerprint (Optional)

```
15:26:37:20:C3:1B:73:59:FD:D9:0F:68:82:3B:D1:D4:1E:7E:AC:F9:78:1B:3C:56:84:72:D8:23:DF:CF:11:C2
```

## Keystore Information

- **File**: `user.keystore`
- **Alias**: `dev`
- **Password**: `123456`
- **Valid Until**: Nov 27, 2075

## Quick Steps to Add to Google Cloud Console

1. **Copy the SHA-1 fingerprint** above
2. Go to [Google Cloud Console](https://console.cloud.google.com/)
3. **APIs & Services** > **Credentials**
4. Click **Create Credentials** > **OAuth client ID**
5. Select **Android**
6. Enter your **Package Name** (from Unity: `Edit > Project Settings > Player > Android`)
7. **Paste the SHA-1 fingerprint**
8. Click **Create**

## Regenerate Fingerprints (if needed)

If you need to regenerate these fingerprints later, run:

```powershell
keytool -list -v -keystore "user.keystore" -alias dev -storepass 123456 -keypass 123456
```

Or for just SHA-1:

```powershell
keytool -list -v -keystore "user.keystore" -alias dev -storepass 123456 -keypass 123456 | findstr SHA1
```

