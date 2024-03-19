using Microsoft.AspNetCore.SignalR;
using signalR_Project.Interfaces;

namespace signalR_Project.Hubs
{
    public class LearningHub : Hub<ILearningHubClient>
    {
        public async Task BroadcastMessage (string message)
        {
            await Clients.All.ReceiveMessage ("message via signalr server : "+message);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
