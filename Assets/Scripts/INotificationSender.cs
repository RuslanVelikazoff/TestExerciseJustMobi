using System.Threading;
using System.Threading.Tasks;

public interface INotificationSender
{
    public Task ShowNotificationAwaitable(string text, CancellationToken token = default);
    public void ShowNotification(string text, CancellationToken token = default);
    public float NotificationTime { get; set; }
    public bool ShowingNotification { get; }
}
