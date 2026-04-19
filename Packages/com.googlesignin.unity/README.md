# Google Sign-In for Unity (UPM)

Native Google Sign-In for **Android** and **iOS** with a small C# API (`GoogleSignInManager`).

## Install

### Embedded in this repository

The package lives at `Packages/com.googlesignin.unity` and is picked up automatically by Unity.

### Another project (git URL)

Add **External Dependency Manager** and this package to your project’s **`Packages/manifest.json`** (EDM cannot be declared as a Git dependency *inside* this package — Unity requires semver there — so you add EDM at the **root** of your manifest):

```json
"dependencies": {
  "com.google.external-dependency-manager": "https://github.com/googlesamples/unity-jar-resolver.git?path=upm",
  "com.googlesignin.unity": "https://github.com/Priyanshu-Dash/GoogleSDK-Unity.git?path=Packages/com.googlesignin.unity"
}
```

Or copy the `Packages/com.googlesignin.unity` folder into your project’s `Packages` directory (you still need EDM in the manifest for Android/iOS resolution).

## One-time setup

1. **External Dependency Manager (EDM)**  
   Ensure **`com.google.external-dependency-manager`** is in your **`Packages/manifest.json`** (see install above). After import, use **Assets → External Dependency Manager → Android Resolver → Force Resolve** (and the iOS resolver if prompted) so Gradle / CocoaPods pick up Google libraries.

2. **Android**  
   - Add `google-services.json` from the [Firebase Console](https://console.firebase.google.com/) (or Google Cloud) under `Assets/`.  
   - Ensure your app’s **SHA-1** fingerprints are registered for the OAuth client used by your game.

3. **iOS**  
   - Add `GoogleService-Info.plist` anywhere under `Assets/` (the build post-processor searches the project, copies it into the Xcode build, and adds the `REVERSED_CLIENT_ID` URL scheme when possible).  
   - Run **Force Resolve** / `pod install` as required by EDM after building or opening the generated Xcode project.

4. **Sample scene**  
   Samples live under **`Samples~`** so they are **not** compiled as part of the package (this avoids duplicate **`GoogleSignIn.Samples`** assemblies if you also import the sample into `Assets/Samples/`). Use **Window → Package Manager →** this package **→ Samples → Import** to copy **Basic Sample** into your project, then open the scene from **`Assets/Samples/…`**. To browse the raw files without importing, use your OS file explorer under `Packages/com.googlesignin.unity/Samples~/BasicSample/`.

## Usage

```csharp
using GoogleSignIn;

GoogleSignInManager.Instance.Initialize(
    serverClientId: "YOUR_WEB_CLIENT_ID.apps.googleusercontent.com", // optional, for server auth code
    webClientId: "YOUR_WEB_CLIENT_ID.apps.googleusercontent.com"     // optional, Android ID token
);

GoogleSignInManager.Instance.SignIn(result =>
{
    if (result.Success)
    {
        Debug.Log(result.User.Email);
        Debug.Log(result.User.IdToken);
    }
});
```

- **Server client ID** (OAuth client of type *Web application*): enables `ServerAuthCode` for your backend.  
- **Web client ID** (same type): passed to Android as `requestIdToken` so `IdToken` is populated on device.

## Android activity

The included `GoogleSignInUnityActivity` forwards `onActivityResult` to the plugin. If your project uses a **custom** main activity, forward sign-in results to `GoogleSignInHelper` using the same pattern as in that class.

## Support matrix

| Platform        | Editor | Device |
|----------------|--------|--------|
| Android        | Stub   | Native |
| iOS            | Stub   | Native |
| Other / Editor | Stub   | Stub   |
