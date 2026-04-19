using System;

namespace GoogleSignIn
{
    /// <summary>
    /// Represents a Google user account
    /// </summary>
    [Serializable]
    public class GoogleSignInUser
    {
        public string Id;
        public string Email;
        public string DisplayName;
        public string PhotoUrl;
        public string IdToken;
        public string ServerAuthCode;
        public string AccessToken;

        public GoogleSignInUser()
        {
        }

        public GoogleSignInUser(string id, string email, string displayName, string photoUrl, string idToken, string serverAuthCode, string accessToken)
        {
            Id = id;
            Email = email;
            DisplayName = displayName;
            PhotoUrl = photoUrl;
            IdToken = idToken;
            ServerAuthCode = serverAuthCode;
            AccessToken = accessToken;
        }
    }
}

