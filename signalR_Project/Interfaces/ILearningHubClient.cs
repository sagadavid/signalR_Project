namespace signalR_Project.Interfaces
{
    public interface ILearningHubClient
    {
        Task ReceiveMessage (string message);
    }
}
