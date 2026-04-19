# GoogleSDK-Unity

Unity **6** sample project and embedded UPM package for **native Google Sign-In** on **Android** and **iOS** (profile, email, photo URL, ID token, optional server auth code).

**Repository:** [https://github.com/Priyanshu-Dash/GoogleSDK-Unity](https://github.com/Priyanshu-Dash/GoogleSDK-Unity)

**Unity version:** `6000.4.x` (project pinned to `6000.4.3f1`)

---

## What’s in this repo

| Path | Purpose |
|------|--------|
| `Packages/com.googlesignin.unity` | UPM package **`com.googlesignin.unity`** — runtime (`GoogleSignInManager`), Android Java, iOS native bridge, Editor post-process (iOS plist / URL scheme). |
| `Packages/com.googlesignin.unity/Samples/BasicSample` | Sample scene + UI script demonstrating sign-in, sign-out, and “current user”. |
| `Assets/Plugins/Android` | Custom Gradle / manifest merges required for this Unity + GameActivity + Play Services setup (see below). |

---

## Quick start (clone and run)

1. **Clone**

   ```bash
   git clone https://github.com/Priyanshu-Dash/GoogleSDK-Unity.git
   cd GoogleSDK-Unity
   ```

2. Open the project in **Unity Hub** with editor **6000.4.x** (match `ProjectSettings/ProjectVersion.txt` if possible).

3. **Google Cloud / Firebase**

   - Create OAuth clients (Android + iOS + Web as needed).
   - **Android:** add **`google-services.json`** under `Assets/` and register your app’s **SHA-1 / SHA-256** for the signing key you use for builds.
   - **iOS:** add **`GoogleService-Info.plist`** under `Assets/` (the package post-processor can copy it into the Xcode build and help with URL schemes).

4. **External Dependency Manager (EDM)**

   After packages resolve: **Assets → External Dependency Manager → Android Resolver → Force Resolve** (and follow any iOS prompts). This pulls **Play Services** / CocoaPods versions declared by the package.

5. **Sample**

   Open **`Packages/com.googlesignin.unity/Samples/BasicSample/SampleScene.unity`**, assign UI references on **`GoogleSignInExample`** if needed, set optional **`webClientId`** / **`serverClientId`** for ID token / server auth code, then build to a device.

---

## Use only the package in another project

Add to your **`Packages/manifest.json`**:

```json
"dependencies": {
  "com.googlesignin.unity": "https://github.com/Priyanshu-Dash/GoogleSDK-Unity.git?path=Packages/com.googlesignin.unity"
}
```

Then complete **EDM Force Resolve**, platform config (`google-services.json` / `GoogleService-Info.plist`, SHA fingerprints), and the **Android** notes in the next section (launcher activity + Gradle alignment).

---

## Android notes (important)

This repo is tuned for **Unity 6 + GameActivity** and Google’s **Sign-In hub** flow.

- **`GoogleSignInUnityActivity`** forwards **`onActivityResult`** to the plugin. The merged **`Assets/Plugins/Android/AndroidManifest.xml`** sets it as the launcher activity so sign-in callbacks reach C# and the UI can update after account picker.
- **`mainTemplate.gradle`** includes **`play-services-auth`** and a **`games-activity`** version compatible with Unity’s native GameActivity sources (see comments in that file).
- **`gradleTemplate.properties`** + Player Settings **Custom Gradle Properties Template** support EDM **Jetifier** / AndroidX flags.

If you copy the package alone into another project, compare your **`Assets/Plugins/Android`** files with this repo when Android build or **sign-in UI stuck on “Signing in…”** issues appear.

---

## iOS notes

- Add **`GoogleService-Info.plist`** under **`Assets/`**.
- The Editor post-processor can merge **`REVERSED_CLIENT_ID`** into **`Info.plist`** and copy the plist into the Xcode project when possible.
- Run **EDM** / **`pod install`** as needed for the **GoogleSignIn** pod declared by the package.

---

## Troubleshooting

| Symptom | Things to check |
|--------|------------------|
| **Android: UI stays on “Signing in…”** after picking an account | Launcher activity must forward activity results (this repo uses **`GoogleSignInUnityActivity`** via manifest merge). |
| **Android: `package ... does not exist` (GMS)** | **`play-services-auth`** must be on the Gradle classpath (`mainTemplate.gradle` / EDM resolve). |
| **Android: CMake / `GameActivity_getLocaleLanguage` errors** | **`androidx.games:games-activity`** must be a **4.x** release matching Unity’s GameActivity C++ (see `mainTemplate.gradle`). |
| **EDM: Jetifier / gradleTemplate.properties** | Enable **Custom Gradle Properties Template** and keep **`**ADDITIONAL_PROPERTIES**`** in `gradleTemplate.properties` for EDM injection. |
| **Build: `IOException: Directory not empty` (MoveFinalPackage)** | Close tools locking the project; delete **`.utmp`**, **`Library/Bee/Android`**, and related Android artifacts; rebuild. |
| **iOS: first sign-in cancels (-5)** | Avoid overlapping sign-in calls; do not tap Sign In repeatedly before the sheet returns. |

---

## Security

- Do **not** commit real **`google-services.json`**, **`GoogleService-Info.plist`**, keystores, or passwords. Use **.gitignore** and GitHub **Secrets** for CI.
- Treat **ID tokens** like secrets in logs and support tickets.

---

## Further reading

- Package-specific setup: **`Packages/com.googlesignin.unity/README.md`**
- [Google Sign-In for Android](https://developers.google.com/identity/sign-in/android/start)
- [Google Sign-In for iOS](https://developers.google.com/identity/sign-in/ios/start)

---

## Author

**Priyanshu Dash** — [GoogleSDK-Unity](https://github.com/Priyanshu-Dash/GoogleSDK-Unity)

Contributions and issues are welcome via GitHub.
