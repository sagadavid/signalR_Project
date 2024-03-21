using BlazorClient.Pages;
using Microsoft.AspNetCore.SignalR;
using signalR_Project.Interfaces;
using System.Runtime.CompilerServices;

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
            await Groups.AddToGroupAsync(Context.ConnectionId, "HubUsers");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "HubUsers");
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

        public async Task SendToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).ReceiveMessage(GetConnIdSinMessage(message));
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);//context has already connectionId
            await Clients.Caller.ReceiveMessage($"current user added to group: {groupName}");
            await Clients.Others.ReceiveMessage($"user: {Context.ConnectionId}, added to group:{groupName}");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.ReceiveMessage($"current user removed from group: {groupName}");
            await Clients.Others.ReceiveMessage($"user: {Context.ConnectionId}, removed from group:{groupName}");
        }

        public async Task BroadcastStream(IAsyncEnumerable<string> stream)
        {
            await foreach (var item in stream)
            {
                await Clients.Caller.ReceiveMessage($"Server received {item}");
            }
        }



    }
}
