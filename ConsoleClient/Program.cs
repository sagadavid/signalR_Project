﻿using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;


Console.WriteLine("Please specify the URL of SignalR Hub");

var url = Console.ReadLine();

var hubConnection = new HubConnectionBuilder()
                         .WithUrl(url,
                            HttpTransportType.WebSockets,
                            options => {
                                options.AccessTokenProvider = null;
                                options.HttpMessageHandlerFactory = null;
                                options.Headers["CustomData"] = "value";
                                options.SkipNegotiation = true;
                                options.ApplicationMaxBufferSize = 1_000_000;
                                options.ClientCertificates = new System.Security.Cryptography.X509Certificates.X509CertificateCollection();
                                options.CloseTimeout = TimeSpan.FromSeconds(5);
                                options.Cookies = new System.Net.CookieContainer();
                                options.DefaultTransferFormat = TransferFormat.Text;
                                options.Credentials = null;
                                options.Proxy = null;
                                options.UseDefaultCredentials = true;
                                options.TransportMaxBufferSize = 1_000_000;
                                options.WebSocketConfiguration = null;
                                options.WebSocketFactory = null;
                            })
                         .ConfigureLogging(logging => {
                             logging.SetMinimumLevel(LogLevel.Information);
                             logging.AddConsole();
                         })
                         .Build();

hubConnection.HandshakeTimeout = TimeSpan.FromSeconds(15);
hubConnection.ServerTimeout = TimeSpan.FromSeconds(30);
hubConnection.KeepAliveInterval = TimeSpan.FromSeconds(10);

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
        Console.WriteLine("7 - trigger a server stream");
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
                if (message?.Contains(';') ?? false)
                {
                    var channel = Channel.CreateBounded<string>(10);
                    await hubConnection.SendAsync("BroadcastStream", channel.Reader);

                    foreach (var item in message.Split(';'))
                    {
                        await channel.Writer.WriteAsync(item);
                    }

                    channel.Writer.Complete();
                }
                else
                {
                    hubConnection.SendAsync("BroadcastMessage", message).Wait();
                }
                break;
            case "1":
                await hubConnection.SendAsync("SendToOthers", message);
                break;
            case "2":
                await hubConnection.SendAsync("SendToCaller", message);
                break;
            case "3":
                Console.WriteLine("please spesify the client's connection id:");
                var connectionId = Console.ReadLine();
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
            case "7":
                Console.WriteLine("Please specify the number of jobs to execute.");
                var numberOfJobs = int.Parse(Console.ReadLine() ?? "0");
                var cancellationTokenSource = new CancellationTokenSource();
                var stream = hubConnection.StreamAsync<string>(
                    "TriggerStream", numberOfJobs, cancellationTokenSource.Token);

                await foreach (var reply in stream)
                {
                    Console.WriteLine(reply);
                }
                break;
            case "exit":
                running = false;
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