namespace FCMtest.Services
{
    public interface INotificationService
    {
        Task InitializeAsync();
        Task<string> GetTokenAsync();
    }
}
