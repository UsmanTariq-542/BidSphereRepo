using Microsoft.AspNetCore.SignalR;

namespace BidSphereProject.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task BroadcastNotification(string Message, DateTime Timestamp)
        {
            await Clients.All.SendAsync("ReceiveNotification", Message, Timestamp);
        }
    }
}