using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NotificationText : MonoBehaviour, INotificationSender
{
    public float NotificationTime 
    { 
        get => _NotificationTime; 
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            _NotificationTime = value;
            UpdateDelay();
        }
    }
    public bool ShowingNotification => _ShowingNotification;
    [SerializeField] private float _NotificationTime;
    private int _MilisecondsDelay;
    private TMP_Text _Text;
    private bool _ShowingNotification = false;
    private int _Delay = 10;
    private CancellationTokenSource _Source;

    private void Start()
    {
        _Text = GetComponent<TMP_Text>();
        UpdateDelay();
    }

    public async void ShowNotification(string text, CancellationToken token = default) => 
        await ShowNotificationAwaitable(text, token);

    public async Task ShowNotificationAwaitable(string text, CancellationToken token = default)
    {
        if (_Source != null && _ShowingNotification)
            _Source.Cancel();

        _Source = new CancellationTokenSource();
        Task task = ShowNotificationInternal(text, _Source.Token);

        while (!task.IsCompleted)
        {
            await Task.Delay(_Delay);

            if (token.IsCancellationRequested)
            {
                _Source.Cancel();
            }
        }
    }

    private async Task ShowNotificationInternal(string text, CancellationToken token = default)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));

        _Text.text = text;
        _ShowingNotification = true;

        try
        {
            await Task.Delay(_MilisecondsDelay, token);
        }
        catch 
        { 
            
        }

        _Text.text = string.Empty;
        _ShowingNotification = false;
    }

    private void UpdateDelay() => _MilisecondsDelay = Mathf.FloorToInt(_NotificationTime * 1000);
}
