#if UNITY_IOS && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GoogleSignIn
{
    /// <summary>
    /// iOS platform implementation for Google Sign-In
    /// </summary>
    public static class GoogleSignInIOS
    {
        private static Action<GoogleSignInResult> _signInCallback;
        private static Action<GoogleSignInResult> _signOutCallback;
        private static Action<GoogleSignInUser> _getUserCallback;

        // Native function declarations
        [DllImport("__Internal")]
        private static extern void _GoogleSignIn_Initialize(string serverClientId);

        [DllImport("__Internal")]
        private static extern void _GoogleSignIn_SignIn();

        [DllImport("__Internal")]
        private static extern void _GoogleSignIn_SignOut();

        [DllImport("__Internal")]
        private static extern void _GoogleSignIn_GetCurrentUser();

        [DllImport("__Internal")]
        private static extern bool _GoogleSignIn_IsSignedIn();

        public static void Initialize(string serverClientId = null)
        {
            try
            {
                _GoogleSignIn_Initialize(serverClientId ?? "");
                Debug.Log("Google Sign-In iOS initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize Google Sign-In iOS: {e.Message}");
            }
        }

        public static void SignIn(Action<GoogleSignInResult> callback)
        {
            _signInCallback = callback;

            try
            {
                _GoogleSignIn_SignIn();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to sign in: {e.Message}");
                callback?.Invoke(GoogleSignInResult.CreateError(e.Message));
            }
        }

        public static void SignOut(Action<GoogleSignInResult> callback)
        {
            _signOutCallback = callback;

            try
            {
                _GoogleSignIn_SignOut();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to sign out: {e.Message}");
                callback?.Invoke(GoogleSignInResult.CreateError(e.Message));
            }
        }

        public static void GetCurrentUser(Action<GoogleSignInUser> callback)
        {
            _getUserCallback = callback;

            try
            {
                _GoogleSignIn_GetCurrentUser();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get current user: {e.Message}");
                callback?.Invoke(null);
            }
        }

        public static bool IsSignedIn()
        {
            try
            {
                return _GoogleSignIn_IsSignedIn();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to check sign-in status: {e.Message}");
                return false;
            }
        }

        // Called from native iOS side
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

        // Called from native iOS side
        public static void OnSignInError(string errorMessage, int errorCode)
        {
            _signInCallback?.Invoke(GoogleSignInResult.CreateError(errorMessage, errorCode));
            _signInCallback = null;
        }

        // Called from native iOS side
        public static void OnSignOutSuccess()
        {
            _signOutCallback?.Invoke(GoogleSignInResult.CreateSuccess(null));
            _signOutCallback = null;
        }

        // Called from native iOS side
        public static void OnSignOutError(string errorMessage)
        {
            _signOutCallback?.Invoke(GoogleSignInResult.CreateError(errorMessage));
            _signOutCallback = null;
        }

        // Called from native iOS side
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

        // Called from native iOS side
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
    public static class GoogleSignInIOS
    {
        public static void Initialize(string serverClientId = null) { }
        public static void SignIn(System.Action<GoogleSignInResult> callback) { }
        public static void SignOut(System.Action<GoogleSignInResult> callback) { }
        public static void GetCurrentUser(System.Action<GoogleSignInUser> callback) { }
        public static bool IsSignedIn() => false;
    }
}
#endif

