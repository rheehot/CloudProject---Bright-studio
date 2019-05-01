using System;
using Fleck;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

public class SyncSocketListener
{

    public static IConfiguration Settings;

    public static int Main(String[] args)
    {
        Settings = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"SubscriptionKey", "85e67eabe2994a55a8cec7d8c5cb09fd"},
                    {"FaceEndpoint", "https://westcentralus.api.cognitive.microsoft.com/face/v1.0"},
                    {"WSInputPort", "12217"},
                    {"IsWsSecurity", "false"},
                    {"ImageFolder", @"D:\Images"},
                    {"ImageFormat", ".jpg"},
                    {"DbConnectionString", "Server=localhost;Database=ThesisDb;Trusted_Connection=True;"},
                    {"WSOutputPort", "12218"}
                })
                .Build();

        //var mainDataProcessing = new MainDataProcessing();

        var wsType = bool.Parse(Settings["IsWsSecurity"]) ? "wss" : "ws";
        var webSocketApplier = new WebSocketServer($"{wsType}://0.0.0.0:{Settings["WSInputPort"]}");
        var webSocketSender  = new WebSocketServer($"{wsType}://0.0.0.0:{Settings["WSOutputPort"]}");

        webSocketApplier.Start(socket =>
        {
            socket.OnOpen = () =>
                Console.WriteLine($"A:New connection from {socket.ConnectionInfo.ClientIpAddress}");

            socket.OnClose = () =>
                Console.WriteLine($"A:Connection lost with {socket.ConnectionInfo.ClientIpAddress}");

            socket.OnError = e =>
                Console.WriteLine($"A:Error in connection with {socket.ConnectionInfo.ClientIpAddress}: {e.Message}");

            socket.OnMessage = msg =>
            {
                var message = msg; //From json or somehow?
                Console.WriteLine($"New message!: {msg}");
                if (message == null) return;

            };
        });

        webSocketSender.Start(socket =>
        {
            socket.OnOpen = () =>
                Console.WriteLine($"S:New connection from {socket.ConnectionInfo.ClientIpAddress}");

            socket.OnClose = () =>
                Console.WriteLine($"S:Connection lost with {socket.ConnectionInfo.ClientIpAddress}");

            socket.OnError = e =>
                   Console.WriteLine($"S:Error in connection with {socket.ConnectionInfo.ClientIpAddress}");

            socket.OnMessage = msg =>
            {
                var message = msg; //From json or somehow?
                if (message == null) return;
                socket.Send(msg);
            };
        });

        bool on = true;

        while (on)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.C) break;
        }

        return 0;
    }

}