using Firebase.Messaging;
using Android.Gms.Tasks;
using Task = System.Threading.Tasks.Task;
using TaskCompletionSource = System.Threading.Tasks.TaskCompletionSource<string>;

namespace FCMtest.Services
{
    public class NotificationService : INotificationService
    {
        public async Task InitializeAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine($"FCM Token initialized: {token}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing notifications: {ex.Message}");
            }
        }

        public async Task<string> GetTokenAsync()
        {
            var tcs = new TaskCompletionSource();

            var task = FirebaseMessaging.Instance.GetToken();
            task.AddOnCompleteListener(new OnCompleteListener(t =>
            {
                if (t.IsSuccessful)
                {
                    tcs.SetResult(t.Result.ToString());
                }
                else
                {
                    tcs.SetException(t.Exception);
                }
            }));

            return await tcs.Task;
        }
    }

    public class OnCompleteListener : Java.Lang.Object, IOnCompleteListener
    {
        private readonly Action<Android.Gms.Tasks.Task> _onComplete;

        public OnCompleteListener(Action<Android.Gms.Tasks.Task> onComplete)
        {
            _onComplete = onComplete;
        }

        public void OnComplete(Android.Gms.Tasks.Task task)
        {
            _onComplete(task);
        }
    }
}
