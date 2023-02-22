// See https://aka.ms/new-console-template for more information

using System.Net;
using Acme.Parrot.Cluster;
using Acme.Parrot.MySqlCdc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlCdc;
using MySqlCdc.Constants;

Console.WriteLine("Hello, World!");
var address = args[0];
var port = int.Parse(args[1]);
// Console.WriteLine($"{new IPEndPoint(IPAddress.Parse(address), port)}");
await Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddCommandLine(args);
        builder.AddEnvironmentVariables();
        builder.AddJsonFile("appsettings.json", optional: true);
    })
    .ConfigureServices(services =>
    {
        services.UseRaftClusterServer(new ClusterOptions()
        {
            Host = address,
            Port = port
        });
        // services.UseMySqlCdcClient(new ReplicaOptions()
        // {
        //     Hostname = "rm-7xva38l164hgvn540io.mysql.rds.aliyuncs.com",
        //     Port = 3306,
        //     Username = "nrop_user",
        //     Password = "7L_daih7ttAo7uWRvrkf",
        //     Database = "acme_cap",
        //     SslMode = SslMode.Disabled,
        //     HeartbeatInterval = TimeSpan.FromSeconds(30),
        //     Blocking = true
        // });
    })
    .ConfigureLogging(builder => builder
        .AddEventSourceLogger()
        .AddDebug()
        .AddConsole())
    .RunConsoleAsync();
Console.WriteLine("Bye, World!");
