using BidSphereProject.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace SignalR_hw09.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class NotificationController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        // Constructor injection
        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult SendNotification()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(string Message, DateTime Timestamp)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", Message, Timestamp);
            return Ok();
        }

    }
}
