using System;
using UnityEngine;

namespace GoogleSignIn
{
    /// <summary>
    /// Main manager for Google Sign-In functionality
    /// </summary>
    public class GoogleSignInManager : MonoBehaviour
    {
        private static GoogleSignInManager _instance;
        private static readonly object _lock = new object();

        // Callbacks
        private Action<GoogleSignInResult> _signInCallback;
        private Action<GoogleSignInResult> _signOutCallback;
        private bool _signInInProgress;

        public static GoogleSignInManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            GameObject go = new GameObject("GoogleSignInManager");
                            _instance = go.AddComponent<GoogleSignInManager>();
                            DontDestroyOnLoad(go);
                        }
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Initialize Google Sign-In.
        /// </summary>
        /// <param name="serverClientId">OAuth web client ID used for server-side auth codes (optional).</param>
        /// <param name="webClientId">OAuth web client ID used for Android ID tokens via requestIdToken (optional).</param>
        public void Initialize(string serverClientId = null, string webClientId = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.Initialize(serverClientId, webClientId);
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.Initialize(serverClientId);
#else
            Debug.LogWarning("Google Sign-In is only available on Android and iOS devices");
#endif
        }

        /// <summary>
        /// Sign in with Google - opens native popup
        /// </summary>
        /// <param name="callback">Callback when sign-in completes</param>
        public void SignIn(Action<GoogleSignInResult> callback = null)
        {
            if (_signInInProgress)
            {
                Debug.LogWarning("[GoogleSignIn] Sign-in already in progress; ignoring duplicate request.");
                callback?.Invoke(GoogleSignInResult.CreateError(
                    "Sign-in already in progress. Wait for the current flow to finish.",
                    -99));
                return;
            }

            _signInInProgress = true;
            _signInCallback = callback;

#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.SignIn(OnSignInResult);
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.SignIn(OnSignInResult);
#else
            // Editor/Standalone fallback
            Debug.LogWarning("Google Sign-In is only available on Android and iOS devices");
            OnSignInResult(GoogleSignInResult.CreateError("Not supported on this platform"));
#endif
        }

        /// <summary>
        /// Sign out from Google
        /// </summary>
        /// <param name="callback">Callback when sign-out completes</param>
        public void SignOut(Action<GoogleSignInResult> callback = null)
        {
            _signOutCallback = callback;

#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.SignOut(OnSignOutResult);
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.SignOut(OnSignOutResult);
#else
            Debug.LogWarning("Google Sign-In is only available on Android and iOS devices");
            OnSignOutResult(GoogleSignInResult.CreateError("Not supported on this platform"));
#endif
        }

        /// <summary>
        /// Get the current signed-in user
        /// </summary>
        /// <param name="callback">Callback with current user or null if not signed in</param>
        public void GetCurrentUser(Action<GoogleSignInUser> callback)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.GetCurrentUser(callback);
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.GetCurrentUser(callback);
#else
            callback?.Invoke(null);
#endif
        }

        /// <summary>
        /// Check if user is currently signed in
        /// </summary>
        /// <returns>True if signed in</returns>
        public bool IsSignedIn()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return GoogleSignInAndroid.IsSignedIn();
#elif UNITY_IOS && !UNITY_EDITOR
            return GoogleSignInIOS.IsSignedIn();
#else
            return false;
#endif
        }

        private void OnSignInResult(GoogleSignInResult result)
        {
            _signInInProgress = false;
            _signInCallback?.Invoke(result);
            _signInCallback = null;
        }

        private void OnSignOutResult(GoogleSignInResult result)
        {
            _signOutCallback?.Invoke(result);
            _signOutCallback = null;
        }

        // Called from native Android/iOS code via UnitySendMessage
        public void OnSignInSuccess(string userJson)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.OnSignInSuccess(userJson);
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.OnSignInSuccess(userJson);
#endif
        }

        // Called from native Android/iOS code via UnitySendMessage
        public void OnSignInError(string errorData)
        {
            // Format: "errorMessage|errorCode"
            string[] parts = errorData.Split('|');
            string errorMessage = parts.Length > 0 ? parts[0] : errorData;
            int errorCode = parts.Length > 1 ? int.Parse(parts[1]) : 0;

#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.OnSignInError(errorMessage, errorCode);
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.OnSignInError(errorMessage, errorCode);
#endif
        }

        // Called from native Android/iOS code via UnitySendMessage
        public void OnSignOutSuccess(string empty)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.OnSignOutSuccess();
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.OnSignOutSuccess();
#endif
        }

        // Called from native Android/iOS code via UnitySendMessage
        public void OnSignOutError(string errorMessage)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.OnSignOutError(errorMessage);
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.OnSignOutError(errorMessage);
#endif
        }

        // Called from native Android/iOS code via UnitySendMessage
        public void OnGetUserSuccess(string userJson)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.OnGetUserSuccess(userJson);
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.OnGetUserSuccess(userJson);
#endif
        }

        // Called from native Android/iOS code via UnitySendMessage
        public void OnGetUserError(string empty)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            GoogleSignInAndroid.OnGetUserError();
#elif UNITY_IOS && !UNITY_EDITOR
            GoogleSignInIOS.OnGetUserError();
#endif
        }
    }
}

