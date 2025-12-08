package com.googlesignin;

import android.app.Activity;
import android.content.Intent;

/**
 * Helper class to handle activity results for Google Sign-In
 * Call handleActivityResult from your main Unity activity's onActivityResult
 */
public class GoogleSignInActivityHandler {
    private static GoogleSignInHelper helper;

    public static void initialize(Activity activity, String serverClientId) {
        helper = GoogleSignInHelper.getInstance(activity, serverClientId);
    }

    public static void handleActivityResult(int requestCode, int resultCode, Intent data) {
        if (helper != null) {
            helper.handleActivityResult(requestCode, resultCode, data);
        }
    }
}

