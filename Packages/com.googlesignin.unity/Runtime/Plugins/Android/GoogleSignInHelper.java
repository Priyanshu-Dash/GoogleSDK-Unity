package com.googlesignin;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;
import com.google.android.gms.auth.api.signin.GoogleSignIn;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;
import com.google.android.gms.common.api.ApiException;
import com.google.android.gms.tasks.Task;
import com.unity3d.player.UnityPlayer;
import org.json.JSONObject;

public class GoogleSignInHelper {
    private static final String TAG = "GoogleSignInHelper";
    private static GoogleSignInHelper instance;
    private Activity activity;
    private GoogleSignInClient googleSignInClient;
    private static final int RC_SIGN_IN = 9001;
    private String serverClientId;
    private String webClientId;

    private GoogleSignInHelper(Activity activity, String serverClientId, String webClientId) {
        this.activity = activity;
        this.serverClientId = serverClientId;
        this.webClientId = webClientId;
        initializeGoogleSignIn();
    }

    private static boolean isEmpty(String s) {
        return s == null || s.isEmpty();
    }

    private static boolean idEquals(String a, String b) {
        if (isEmpty(a) && isEmpty(b)) {
            return true;
        }
        if (isEmpty(a) || isEmpty(b)) {
            return false;
        }
        return a.equals(b);
    }

    public static GoogleSignInHelper getInstance(Activity activity, String serverClientId, String webClientId) {
        if (instance == null) {
            instance = new GoogleSignInHelper(activity, serverClientId, webClientId);
            return instance;
        }
        instance.activity = activity;
        if (!idEquals(instance.serverClientId, serverClientId) || !idEquals(instance.webClientId, webClientId)) {
            instance = new GoogleSignInHelper(activity, serverClientId, webClientId);
        }
        return instance;
    }

    public static GoogleSignInHelper peekInstance() {
        return instance;
    }

    private void initializeGoogleSignIn() {
        GoogleSignInOptions.Builder builder = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
                .requestEmail()
                .requestProfile();

        if (!isEmpty(webClientId)) {
            builder.requestIdToken(webClientId);
        }
        if (!isEmpty(serverClientId)) {
            builder.requestServerAuthCode(serverClientId);
        }

        GoogleSignInOptions gso = builder.build();
        googleSignInClient = GoogleSignIn.getClient(activity, gso);
    }

    public void signIn() {
        try {
            Intent signInIntent = googleSignInClient.getSignInIntent();
            activity.startActivityForResult(signInIntent, RC_SIGN_IN);
        } catch (Exception e) {
            Log.e(TAG, "Error starting sign in: " + e.getMessage());
            UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnSignInError", "Failed to start sign in: " + e.getMessage() + "|0");
        }
    }

    public void signOut() {
        googleSignInClient.signOut()
                .addOnCompleteListener(activity, task -> {
                    if (task.isSuccessful()) {
                        UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnSignOutSuccess", "");
                    } else {
                        String error = task.getException() != null ? task.getException().getMessage() : "Unknown error";
                        UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnSignOutError", error);
                    }
                });
    }

    public void getCurrentUser() {
        GoogleSignInAccount account = GoogleSignIn.getLastSignedInAccount(activity);
        if (account != null) {
            String userJson = accountToJson(account);
            UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnGetUserSuccess", userJson);
        } else {
            UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnGetUserError", "");
        }
    }

    public boolean isSignedIn() {
        GoogleSignInAccount account = GoogleSignIn.getLastSignedInAccount(activity);
        return account != null;
    }

    public void handleActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == RC_SIGN_IN) {
            Task<GoogleSignInAccount> task = GoogleSignIn.getSignedInAccountFromIntent(data);
            handleSignInResult(task);
        }
    }

    private void handleSignInResult(Task<GoogleSignInAccount> completedTask) {
        try {
            GoogleSignInAccount account = completedTask.getResult(ApiException.class);
            if (account != null) {
                String userJson = accountToJson(account);
                UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnSignInSuccess", userJson);
            } else {
                UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnSignInError", "Sign in failed: Account is null|0");
            }
        } catch (ApiException e) {
            Log.e(TAG, "signInResult:failed code=" + e.getStatusCode());
            String errorMessage = "Sign in failed: " + e.getMessage();
            UnityPlayer.UnitySendMessage("GoogleSignInManager", "OnSignInError", errorMessage + "|" + e.getStatusCode());
        }
    }

    private String accountToJson(GoogleSignInAccount account) {
        try {
            JSONObject json = new JSONObject();
            json.put("Id", account.getId() != null ? account.getId() : "");
            json.put("Email", account.getEmail() != null ? account.getEmail() : "");
            json.put("DisplayName", account.getDisplayName() != null ? account.getDisplayName() : "");
            json.put("PhotoUrl", account.getPhotoUrl() != null ? account.getPhotoUrl().toString() : "");
            json.put("IdToken", account.getIdToken() != null ? account.getIdToken() : "");
            json.put("ServerAuthCode", account.getServerAuthCode() != null ? account.getServerAuthCode() : "");
            json.put("AccessToken", "");
            return json.toString();
        } catch (Exception e) {
            Log.e(TAG, "Error converting account to JSON: " + e.getMessage());
            return "{}";
        }
    }
}
