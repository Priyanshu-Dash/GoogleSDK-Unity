using GoogleSignIn;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GoogleSignIn.Examples
{
    /// <summary>
    /// Example script showing how to use Google Sign-In SDK
    /// Attach this to a GameObject with UI buttons
    /// </summary>
    public class GoogleSignInExample : MonoBehaviour
    {
        [Header("UI References (Optional)")]
        public Button signInButton;
        public Button signOutButton;
        public Button checkUserButton;
        public TextMeshProUGUI statusText;

        [Header("Configuration")]
        [Tooltip("Optional: Server Client ID for backend authentication")]
        public string serverClientId = "";

        private void Start()
        {
            // Initialize the SDK
            GoogleSignInManager.Instance.Initialize(string.IsNullOrEmpty(serverClientId) ? null : serverClientId);

            // Setup button listeners if provided
            if (signInButton != null)
            {
                signInButton.onClick.AddListener(OnSignInClicked);
            }

            if (signOutButton != null)
            {
                signOutButton.onClick.AddListener(OnSignOutClicked);
            }

            if (checkUserButton != null)
            {
                checkUserButton.onClick.AddListener(OnCheckUserClicked);
            }

            UpdateStatus("Ready. Click Sign In to start.");
        }

        public void OnSignInClicked()
        {
            UpdateStatus("Signing in...");
            
            // Sign in - this will open the native Google Sign-In popup
            GoogleSignInManager.Instance.SignIn((result) =>
            {
                if (result.Success)
                {
                    UpdateStatus($"Signed in successfully!\nName: {result.User.DisplayName}\nEmail: {result.User.Email}");
                    Debug.Log($"User ID: {result.User.Id}");
                    Debug.Log($"ID Token: {result.User.IdToken}");
                    
                    if (!string.IsNullOrEmpty(result.User.ServerAuthCode))
                    {
                        Debug.Log($"Server Auth Code: {result.User.ServerAuthCode}");
                        // Send this to your backend server for authentication
                    }
                }
                else
                {
                    UpdateStatus($"Sign in failed: {result.ErrorMessage} (Code: {result.ErrorCode})");
                    Debug.LogError($"Sign in error: {result.ErrorMessage}");
                }
            });
        }

        public void OnSignOutClicked()
        {
            UpdateStatus("Signing out...");
            
            GoogleSignInManager.Instance.SignOut((result) =>
            {
                if (result.Success)
                {
                    UpdateStatus("Signed out successfully!");
                    Debug.Log("User signed out");
                }
                else
                {
                    UpdateStatus($"Sign out failed: {result.ErrorMessage}");
                    Debug.LogError($"Sign out error: {result.ErrorMessage}");
                }
            });
        }

        public void OnCheckUserClicked()
        {
            UpdateStatus("Checking current user...");
            
            GoogleSignInManager.Instance.GetCurrentUser((user) =>
            {
                if (user != null)
                {
                    UpdateStatus($"Current user:\nName: {user.DisplayName}\nEmail: {user.Email}");
                    Debug.Log($"User is signed in: {user.DisplayName}");
                }
                else
                {
                    UpdateStatus("No user is currently signed in");
                    Debug.Log("No user signed in");
                }
            });
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log($"[GoogleSignIn] {message}");
        }

        private void OnDestroy()
        {
            // Clean up button listeners
            if (signInButton != null)
            {
                signInButton.onClick.RemoveListener(OnSignInClicked);
            }

            if (signOutButton != null)
            {
                signOutButton.onClick.RemoveListener(OnSignOutClicked);
            }

            if (checkUserButton != null)
            {
                checkUserButton.onClick.RemoveListener(OnCheckUserClicked);
            }
        }
    }
}

