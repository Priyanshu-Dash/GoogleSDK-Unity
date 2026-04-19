package com.googlesignin;

import android.content.Intent;
import com.unity3d.player.UnityPlayerGameActivity;

/**
 * Forwards Google Sign-In activity results to {@link GoogleSignInHelper}.
 * The helper is created from C# via {@link GoogleSignInHelper#getInstance}; do not initialize here.
 */
public class GoogleSignInUnityActivity extends UnityPlayerGameActivity {
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        GoogleSignInHelper helper = GoogleSignInHelper.peekInstance();
        if (helper != null) {
            helper.handleActivityResult(requestCode, resultCode, data);
        }
    }
}
