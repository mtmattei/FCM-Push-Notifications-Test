using Android.App;
using Android.OS;
using Android.Content.PM;
using Firebase;

namespace FCMtest.Platforms.Android
{
    [Activity(MainLauncher = true)]
    public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Initialize Firebase
            FirebaseApp.InitializeApp(this);

            // Request notification permissions for Android 13+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                RequestNotificationPermission();
            }
        }

        private void RequestNotificationPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                this.RequestPermissions(
                    new[] { global::Android.Manifest.Permission.PostNotifications },
                    1001);
            }
        }
    }
}
