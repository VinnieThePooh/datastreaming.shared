﻿using System.Net.Sockets;
using DataStreaming.Protocols.Factories;
using DataStreaming.Services.RTT;
using DataStreaming.Settings;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var hostSettings = configuration.GetSection("").Get<HostSettings>();
var protoFactory = (IRttMeteringProtocolFactory)RttMeteringProtocolFactory.Create();
var server = new RttEchoServer(hostSettings, protoFactory);

try
{
    _ = await server.Start();
}
catch (SocketException e)
{
    //todo: log here
    Console.WriteLine(e);
    await server.DisposeAsync();
    Environment.Exit(e.ErrorCode);
}
catch (Exception e)
{
    //todo: log here
    Console.WriteLine($"Unexpected exception: {e}");
    Environment.Exit(e.HResult);
}
finally
{
    await server.DisposeAsync();
}
