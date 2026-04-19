using System;

namespace GoogleSignIn
{
    /// <summary>
    /// Result of a Google Sign-In operation
    /// </summary>
    [Serializable]
    public class GoogleSignInResult
    {
        public bool Success;
        public GoogleSignInUser User;
        public string ErrorMessage;
        public int ErrorCode;

        public GoogleSignInResult(bool success, GoogleSignInUser user = null, string errorMessage = null, int errorCode = 0)
        {
            Success = success;
            User = user;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public static GoogleSignInResult CreateSuccess(GoogleSignInUser user)
        {
            return new GoogleSignInResult(true, user);
        }

        public static GoogleSignInResult CreateError(string errorMessage, int errorCode = 0)
        {
            return new GoogleSignInResult(false, null, errorMessage, errorCode);
        }
    }
}

