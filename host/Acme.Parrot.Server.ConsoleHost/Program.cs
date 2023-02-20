// See https://aka.ms/new-console-template for more information

using Acme.Parrot.Cluster;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");
var address = args[0];
var port = int.Parse(args[1]);
await Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.UseRaftClusterServer(address, port);
    })
    .ConfigureLogging(builder => builder
        .AddEventSourceLogger()
        .AddDebug()
        .AddConsole())
    .RunConsoleAsync();
Console.WriteLine("Bye, World!");
