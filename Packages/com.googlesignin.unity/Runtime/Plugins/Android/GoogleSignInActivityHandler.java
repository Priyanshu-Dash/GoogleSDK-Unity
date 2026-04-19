package com.googlesignin;

import android.app.Activity;
import android.content.Intent;

/**
 * Optional bridge for custom Unity activities: call {@link #initialize} then forward
 * {@link #handleActivityResult} from {@code onActivityResult}. If you use {@link GoogleSignInUnityActivity},
 * you do not need this class.
 */
public class GoogleSignInActivityHandler {
    public static void initialize(Activity activity, String serverClientId) {
        initialize(activity, serverClientId, null);
    }

    public static void initialize(Activity activity, String serverClientId, String webClientId) {
        GoogleSignInHelper.getInstance(activity, serverClientId, webClientId);
    }

    public static void handleActivityResult(int requestCode, int resultCode, Intent data) {
        GoogleSignInHelper helper = GoogleSignInHelper.peekInstance();
        if (helper != null) {
            helper.handleActivityResult(requestCode, resultCode, data);
        }
    }
}
