using Microsoft.AspNetCore.SignalR;

namespace OBS2Browser;

public class WSHub : Hub
{
    private static object _oLocker = new object();
    private static ManualResetEvent _evReady = new(true);
    private static string? _answer = null;

    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"WSHub:OnConnectedAsync -> {Context.ConnectionId}");
        return Task.CompletedTask;
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"WSHub:OnDisconnectedAsync -> {Context.ConnectionId}");
        return Task.CompletedTask;
    }

 #region // Warning: A healthier method may be found!

    public void SendSdpMessage(string sdp)
    {
        _answer = sdp;
        _evReady.Set();
    }

    public static void ResetEvent()
    {
        _answer = null;
        _evReady.Reset();
    }

    public static Task<string?> GetAnswerAsync()
    {
        return Task.Run(() => {
            _evReady.WaitOne(TimeSpan.FromSeconds(5));
            return _answer;
        });
    }
    
#endregion

}