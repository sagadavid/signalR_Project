using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("Please specify the URL of SignalR Hub");

var url = Console.ReadLine();

var hubConnection = new HubConnectionBuilder()
                         .WithUrl(url)
                         .Build();

hubConnection.On<string>("ReceiveMessage",
    message => Console.WriteLine($"SignalR Hub Message: {message}"));

try
{
    await hubConnection.StartAsync();
    var running = true;

    while (running)
    {
        var message = string.Empty;

        Console.WriteLine("Please specify the action:");
        Console.WriteLine("0 - broadcast to all");
        Console.WriteLine("1 - send message to other clients");
        Console.WriteLine("exit - Exit the program");

        var action = Console.ReadLine();

        Console.WriteLine("Please specify the message:");
        message = Console.ReadLine();

        switch (action)
        {
            case "0":
                await hubConnection.SendAsync("BroadcastMessage", message);
                break;
            case "1":
                await hubConnection.SendAsync("SendToOthers", message);
                break;
            case "exit":
                running=false;
                break;
            default:
                Console.WriteLine("choose 0 or 1 or exit");
                break;
        }

        
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
    return;
}

//url when asked : https://localhost:7180/learninghub