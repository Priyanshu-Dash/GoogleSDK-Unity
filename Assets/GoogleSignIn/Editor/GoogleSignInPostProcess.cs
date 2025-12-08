#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace GoogleSignIn.Editor
{
    /// <summary>
    /// Post-process build script for iOS Google Sign-In setup
    /// Automatically configures Info.plist with URL schemes from GoogleService-Info.plist
    /// </summary>
    public class GoogleSignInPostProcess
    {
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            Debug.Log("Google Sign-In: Starting iOS post-process build...");

            // Note: If using External Dependency Manager (EDM4U), it will automatically handle
            // CocoaPods dependencies via Dependencies.xml. The Podfile copying below is a fallback
            // for projects not using EDM4U.
            
            // Copy Podfile to Xcode project directory (fallback if EDM4U is not used)
            string podfileSource = Path.Combine(Application.dataPath, "Plugins", "iOS", "Podfile");
            string podfileDest = Path.Combine(pathToBuiltProject, "Podfile");
            
            // Only copy if EDM4U hasn't already created a Podfile
            if (File.Exists(podfileSource) && !File.Exists(podfileDest))
            {
                File.Copy(podfileSource, podfileDest, true);
                Debug.Log($"Google Sign-In: Copied Podfile to {podfileDest}");
            }
            else if (File.Exists(podfileDest))
            {
                Debug.Log("Google Sign-In: Podfile already exists (likely created by External Dependency Manager)");
            }

            // Get the path to Info.plist
            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");
            if (!File.Exists(plistPath))
            {
                Debug.LogWarning("Google Sign-In: Info.plist not found at " + plistPath);
                return;
            }

            // Read Info.plist
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Get the root dictionary
            PlistElementDict rootDict = plist.root;

            // Check if GoogleService-Info.plist exists in the project
            string googleServiceInfoPath = Path.Combine(Application.dataPath, "GoogleService-Info.plist");
            if (!File.Exists(googleServiceInfoPath))
            {
                Debug.LogWarning("Google Sign-In: GoogleService-Info.plist not found in Assets folder. " +
                    "Please download it from Firebase Console and add it to your Assets folder. " +
                    "URL scheme configuration will be skipped.");
            }
            else
            {
                // Read GoogleService-Info.plist to get REVERSED_CLIENT_ID
                PlistDocument googleServiceInfo = new PlistDocument();
                googleServiceInfo.ReadFromFile(googleServiceInfoPath);
                PlistElementDict googleServiceDict = googleServiceInfo.root;

                string reversedClientId = googleServiceDict["REVERSED_CLIENT_ID"]?.AsString();
                if (string.IsNullOrEmpty(reversedClientId))
                {
                    Debug.LogWarning("Google Sign-In: REVERSED_CLIENT_ID not found in GoogleService-Info.plist");
                }
                else
                {
                    Debug.Log($"Google Sign-In: Found REVERSED_CLIENT_ID: {reversedClientId}");

                    // Get or create CFBundleURLTypes array
                    PlistElementArray urlTypes;
                    if (rootDict.values.ContainsKey("CFBundleURLTypes"))
                    {
                        urlTypes = rootDict["CFBundleURLTypes"].AsArray();
                    }
                    else
                    {
                        urlTypes = rootDict.CreateArray("CFBundleURLTypes");
                    }

                    // Check if URL scheme already exists
                    bool urlSchemeExists = false;
                    foreach (PlistElement element in urlTypes.values)
                    {
                        PlistElementDict urlTypeDict = element.AsDict();
                        if (urlTypeDict.values.ContainsKey("CFBundleURLSchemes"))
                        {
                            PlistElementArray schemes = urlTypeDict["CFBundleURLSchemes"].AsArray();
                            foreach (PlistElement scheme in schemes.values)
                            {
                                if (scheme.AsString() == reversedClientId)
                                {
                                    urlSchemeExists = true;
                                    break;
                                }
                            }
                        }
                        if (urlSchemeExists) break;
                    }

                    // Add URL scheme if it doesn't exist
                    if (!urlSchemeExists)
                    {
                        PlistElementDict urlTypeDict = urlTypes.AddDict();
                        PlistElementArray schemes = urlTypeDict.CreateArray("CFBundleURLSchemes");
                        schemes.AddString(reversedClientId);
                        Debug.Log($"Google Sign-In: Added URL scheme '{reversedClientId}' to Info.plist");
                    }
                    else
                    {
                        Debug.Log("Google Sign-In: URL scheme already exists in Info.plist");
                    }
                }
            }

            // Write the modified Info.plist
            plist.WriteToFile(plistPath);
            Debug.Log("Google Sign-In: iOS post-process build completed successfully");
        }
    }
}
#endif

