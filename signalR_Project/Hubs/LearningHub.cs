using BlazorClient.Pages;
using Microsoft.AspNetCore.SignalR;
using signalR_Project.Interfaces;

namespace signalR_Project.Hubs
{
    public class LearningHub : Hub<ILearningHubClient>
    {
        public async Task BroadcastMessage (string message)
        {
            await Clients.All.ReceiveMessage(GetConnIdSinMessage(message));
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendToOthers(string message)
        {
            await Clients.Others.ReceiveMessage(GetConnIdSinMessage(message));
        }

        public async Task SendToCaller(string message)
        {
            await Clients.Caller.ReceiveMessage(GetConnIdSinMessage(message));
        }

        private string GetConnIdSinMessage(string message)
        {
            return $"Client connection id: {Context.ConnectionId}.Message:{message}";
        }

        public async Task SendToOneClient(string connectionId, string message)
        {
            //select client per id and then message per id 
            await Clients.Client(connectionId).ReceiveMessage(GetConnIdSinMessage(message));
        }


    }
}
