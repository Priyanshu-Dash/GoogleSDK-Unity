package com.googlesignin;

import android.content.Intent;
import com.unity3d.player.UnityPlayerGameActivity;

/**
 * Unity Activity extension to handle Google Sign-In activity results
 * Unity will automatically use this if it's in the Plugins/Android folder
 * Make sure to set this as your main activity in AndroidManifest or use it as a base
 */
public class GoogleSignInUnityActivity extends UnityPlayerGameActivity {
    private GoogleSignInHelper googleSignInHelper;

    @Override
    protected void onStart() {
        super.onStart();
        // Initialize helper when activity starts
        googleSignInHelper = GoogleSignInHelper.getInstance(this, null);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        // Handle Google Sign-In result
        if (googleSignInHelper != null) {
            googleSignInHelper.handleActivityResult(requestCode, resultCode, data);
        }
    }
}

