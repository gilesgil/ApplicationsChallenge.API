using ApplicationsChallenge.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace ApplicationsChallenge.API.Hubs
{
    public class ApplicationHub : Hub
    {
        // Will be called from the background service when an application status is updated
        public async Task NotifyStatusChange(Application application)
        {
            await Clients.All.SendAsync("ReceiveStatusUpdate", application);
        }
    }
}
