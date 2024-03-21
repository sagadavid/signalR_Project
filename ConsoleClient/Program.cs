﻿using Microsoft.AspNetCore.SignalR.Client;

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
        var groupName = string.Empty;

        Console.WriteLine("Please specify the action:");
        Console.WriteLine("0 - broadcast to all");
        Console.WriteLine("1 - send message to other clients");
        Console.WriteLine("2 - send message to self");
        Console.WriteLine("3 - send message to one client");
        Console.WriteLine("4 - send to a group");
        Console.WriteLine("5 - add user to a group");
        Console.WriteLine("6 - remove user from a group");
        Console.WriteLine("exit - Exit the program");

        var action = Console.ReadLine();

        if (action != "5" && action != "6")
        {
            Console.WriteLine("Please specify the message:");
            message = Console.ReadLine();
        }

        if (action == "4" || action == "5" || action == "6")
        {
            Console.WriteLine("Please specify the group name:");
            groupName = Console.ReadLine();
        }


        switch (action)
        {
            case "0":
                await hubConnection.SendAsync("BroadcastMessage", message);
                break;
            case "1":
                await hubConnection.SendAsync("SendToOthers", message);
                break;
            case "2":
                await hubConnection.SendAsync("SendToCaller", message);
                break;
            case "3":
                Console.WriteLine("please spesify the client's connection id:");
                var connectionId= Console.ReadLine();
                await hubConnection.SendAsync("SendToOneClient", connectionId, message);
                break;
            case "4":
                hubConnection.SendAsync("SendToGroup", groupName, message).Wait();
                break;
            case "5":
                hubConnection.SendAsync("AddToGroup", groupName).Wait();
                break;
            case "6":
                hubConnection.SendAsync("RemoveFromGroup", groupName).Wait();
                break;
            case "exit":
                running=false;
                break;
            default:
                Console.WriteLine("choose 0, 1, 2, 3 or exit");
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