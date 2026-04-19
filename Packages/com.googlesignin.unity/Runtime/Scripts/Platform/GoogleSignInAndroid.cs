#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using UnityEngine;

namespace GoogleSignIn
{
    /// <summary>
    /// Android platform implementation for Google Sign-In
    /// </summary>
    public static class GoogleSignInAndroid
    {
        private static AndroidJavaObject _googleSignInHelper;
        private static Action<GoogleSignInResult> _signInCallback;
        private static Action<GoogleSignInResult> _signOutCallback;
        private static Action<GoogleSignInUser> _getUserCallback;

        public static void Initialize(string serverClientId = null, string webClientId = null)
        {
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass helperClass = new AndroidJavaClass("com.googlesignin.GoogleSignInHelper");
                
                _googleSignInHelper = helperClass.CallStatic<AndroidJavaObject>(
                    "getInstance",
                    currentActivity,
                    serverClientId ?? "",
                    webClientId ?? "");
                
                Debug.Log("Google Sign-In Android initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize Google Sign-In Android: {e.Message}");
            }
        }

        public static void SignIn(Action<GoogleSignInResult> callback)
        {
            if (_googleSignInHelper == null)
            {
                Initialize();
            }

            _signInCallback = callback;

            try
            {
                _googleSignInHelper.Call("signIn");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to sign in: {e.Message}");
                callback?.Invoke(GoogleSignInResult.CreateError(e.Message));
            }
        }

        public static void SignOut(Action<GoogleSignInResult> callback)
        {
            if (_googleSignInHelper == null)
            {
                callback?.Invoke(GoogleSignInResult.CreateError("Not initialized"));
                return;
            }

            _signOutCallback = callback;

            try
            {
                _googleSignInHelper.Call("signOut");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to sign out: {e.Message}");
                callback?.Invoke(GoogleSignInResult.CreateError(e.Message));
            }
        }

        public static void GetCurrentUser(Action<GoogleSignInUser> callback)
        {
            if (_googleSignInHelper == null)
            {
                callback?.Invoke(null);
                return;
            }

            _getUserCallback = callback;

            try
            {
                _googleSignInHelper.Call("getCurrentUser");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get current user: {e.Message}");
                callback?.Invoke(null);
            }
        }

        public static bool IsSignedIn()
        {
            if (_googleSignInHelper == null)
            {
                return false;
            }

            try
            {
                return _googleSignInHelper.Call<bool>("isSignedIn");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to check sign-in status: {e.Message}");
                return false;
            }
        }

        // Called from Java side
        public static void OnSignInSuccess(string userJson)
        {
            try
            {
                GoogleSignInUser user = JsonUtility.FromJson<GoogleSignInUser>(userJson);
                _signInCallback?.Invoke(GoogleSignInResult.CreateSuccess(user));
                _signInCallback = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse user data: {e.Message}");
                _signInCallback?.Invoke(GoogleSignInResult.CreateError($"Failed to parse user data: {e.Message}"));
                _signInCallback = null;
            }
        }

        // Called from Java side
        public static void OnSignInError(string errorMessage, int errorCode)
        {
            _signInCallback?.Invoke(GoogleSignInResult.CreateError(errorMessage, errorCode));
            _signInCallback = null;
        }

        // Called from Java side
        public static void OnSignOutSuccess()
        {
            _signOutCallback?.Invoke(GoogleSignInResult.CreateSuccess(null));
            _signOutCallback = null;
        }

        // Called from Java side
        public static void OnSignOutError(string errorMessage)
        {
            _signOutCallback?.Invoke(GoogleSignInResult.CreateError(errorMessage));
            _signOutCallback = null;
        }

        // Called from Java side
        public static void OnGetUserSuccess(string userJson)
        {
            try
            {
                GoogleSignInUser user = JsonUtility.FromJson<GoogleSignInUser>(userJson);
                _getUserCallback?.Invoke(user);
                _getUserCallback = null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse user data: {e.Message}");
                _getUserCallback?.Invoke(null);
                _getUserCallback = null;
            }
        }

        // Called from Java side
        public static void OnGetUserError()
        {
            _getUserCallback?.Invoke(null);
            _getUserCallback = null;
        }
    }
}
#else
namespace GoogleSignIn
{
    public static class GoogleSignInAndroid
    {
        public static void Initialize(string serverClientId = null, string webClientId = null) { }
        public static void SignIn(System.Action<GoogleSignInResult> callback) { }
        public static void SignOut(System.Action<GoogleSignInResult> callback) { }
        public static void GetCurrentUser(System.Action<GoogleSignInUser> callback) { }
        public static bool IsSignedIn() => false;
    }
}
#endif

