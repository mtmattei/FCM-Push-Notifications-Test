using Android.App;
using Android.Content;
using Firebase.Messaging;
using AndroidX.Core.App;
using Android.OS;
using Android.Media;
using Android.Graphics;

namespace FCMtest.Platforms.Android
{
    [Service(Exported = false)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            var notificationBody = message.GetNotification()?.Body;
            var notificationTitle = message.GetNotification()?.Title;
            var imageUrl = message.GetNotification()?.ImageUrl?.ToString();
            var dataPayload = message.Data;
            SendNotification(notificationTitle, notificationBody, imageUrl, dataPayload);
        }

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            System.Diagnostics.Debug.WriteLine($"FCM Token: {token}");
        }

        private async void SendNotification(string title, string body, string imageUrl, IDictionary<string, string> data)
        {
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            var channelId = "DefaultChannel";

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    channelId,
                    "Default",
                    NotificationImportance.Max)
                {
                    Description = "Default notifications channel"
                };
                channel.SetShowBadge(true);
                channel.EnableLights(true);
                channel.EnableVibration(true);
                channel.SetBypassDnd(true);
                channel.LockscreenVisibility = NotificationVisibility.Public;
                notificationManager.CreateNotificationChannel(channel);
            }

            var intent = PackageManager.GetLaunchIntentForPackage(PackageName);
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent,
                PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

            var defaultSoundUri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var notificationBuilder = new NotificationCompat.Builder(this, channelId)
                .SetSmallIcon(global::Android.Resource.Drawable.IcDialogAlert)
                .SetContentTitle(title)
                .SetContentText(body)
                .SetAutoCancel(true)
                .SetSound(defaultSoundUri)
                .SetPriority(NotificationCompat.PriorityMax)
                .SetDefaults(NotificationCompat.DefaultAll)
                .SetVisibility(NotificationCompat.VisibilityPublic)
                .SetContentIntent(pendingIntent);

            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    using var client = new HttpClient();
                    var bitmap = await client.GetByteArrayAsync(imageUrl);
                    var image = BitmapFactory.DecodeByteArray(bitmap, 0, bitmap.Length);
                    notificationBuilder.SetLargeIcon(image)
                        .SetStyle(new NotificationCompat.BigPictureStyle()
                            .BigPicture(image)
                            .BigLargeIcon((Bitmap)null));

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }

            var notification = notificationBuilder.Build();
            notificationManager.Notify(new Random().Next(), notification);
        }
    }
}
