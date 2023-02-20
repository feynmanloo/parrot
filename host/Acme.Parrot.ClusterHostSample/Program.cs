// See https://aka.ms/new-console-template for more information

using Acme.Parrot.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");
await Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.UseRaftClusterServer(int.TryParse(args[0], out var port) ? port : 8080);
        services.AddHostedService<DataModifier>();
    })
    .ConfigureLogging(builder => builder.AddConsole())
    .RunConsoleAsync();
Console.WriteLine("Bye, World!");