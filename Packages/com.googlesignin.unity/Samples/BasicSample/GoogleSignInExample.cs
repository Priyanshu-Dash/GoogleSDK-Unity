using System.Collections;
using GoogleSignIn;
using UnityEngine;
using UnityEngine.Networking;
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
        public Image profileImage;

        [Header("Configuration")]
        [Tooltip("Optional: OAuth web client ID for server auth codes (backend login).")]
        public string serverClientId = "";
        [Tooltip("Optional: OAuth web client ID used on Android for ID tokens (often the same value as server client ID).")]
        public string webClientId = "";

        private void Start()
        {
            // Initialize the SDK
            GoogleSignInManager.Instance.Initialize(
                string.IsNullOrEmpty(serverClientId) ? null : serverClientId,
                string.IsNullOrEmpty(webClientId) ? null : webClientId);

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
                    
                    // Load and display profile picture
                    if (!string.IsNullOrEmpty(result.User.PhotoUrl))
                    {
                        StartCoroutine(LoadProfilePicture(result.User.PhotoUrl));
                    }
                    
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
                    
                    // Clear profile picture on sign out
                    if (profileImage != null)
                    {
                        profileImage.sprite = null;
                    }
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
                    
                    // Load and display profile picture
                    if (!string.IsNullOrEmpty(user.PhotoUrl))
                    {
                        StartCoroutine(LoadProfilePicture(user.PhotoUrl));
                    }
                }
                else
                {
                    UpdateStatus("No user is currently signed in");
                    Debug.Log("No user signed in");
                    // Clear profile picture if no user
                    if (profileImage != null)
                    {
                        profileImage.sprite = null;
                    }
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

        private IEnumerator LoadProfilePicture(string photoUrl)
        {
            if (profileImage == null)
            {
                Debug.LogWarning("Profile Image UI component is not assigned. Please assign it in the inspector.");
                yield break;
            }

            if (string.IsNullOrEmpty(photoUrl))
            {
                Debug.LogWarning("Photo URL is empty.");
                yield break;
            }

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(photoUrl))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    if (texture != null)
                    {
                        // Create a sprite from the texture
                        Sprite sprite = Sprite.Create(
                            texture,
                            new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f)
                        );
                        
                        profileImage.sprite = sprite;
                        Debug.Log("Profile picture loaded successfully");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to load profile picture: {www.error}");
                }
            }
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

