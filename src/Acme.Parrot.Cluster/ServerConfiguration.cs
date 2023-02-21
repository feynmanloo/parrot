using System.Net;
using Acme.Parrot.Core;
using DotNext;
using DotNext.Net.Cluster;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Net.Cluster.Consensus.Raft.Membership;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Acme.Parrot.Cluster;

public static class ServerConfiguration
{
    public static IServiceCollection UseRaftClusterServer(this IServiceCollection services, string host, int port)
    {
        if (!IPAddress.TryParse(host, out var ipAddress))
        {
            ipAddress = Dns.GetHostAddresses(host).FirstOrDefault();
            if (ipAddress == null)
            {
                throw new ArgumentNullException($"{host}解析IpAddress异常");
            }
        }
        var configuration = new RaftCluster.TcpConfiguration(new IPEndPoint(ipAddress, port))
        {
            RequestTimeout = TimeSpan.FromMilliseconds(140),
            LowerElectionTimeout = 150,
            UpperElectionTimeout = 300,
            TransmissionBlockSize = 4096,
            ColdStart = true,
        };
        AddMembersToCluster(configuration.UseInMemoryConfigurationStorage());
        services.AddSingleton<InMemoryPersistentSateEngine>(new InMemoryPersistentSateEngine($"{DateTime.Now:yyyyMMddHHmmss}", new AppEventSource()));
        services.AddSingleton<ISupplier<BinlogState>>(sp => sp.GetRequiredService<InMemoryPersistentSateEngine>());
        services.AddSingleton<PersistentState>(sp => sp.GetRequiredService<InMemoryPersistentSateEngine>());
        services.AddSingleton<RaftCluster.NodeConfiguration>(sp =>
        {
            configuration.LoggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return configuration;
        });
        services.AddSingleton<ParrotRaftCluster>();
        services.AddSingleton<IRaftCluster>(sp => sp.GetRequiredService<ParrotRaftCluster>());
        services.AddHostedService(sp => sp.GetRequiredService<ParrotRaftCluster>());

        services.AddSingleton<ILeaderMajorJob, EmptyLeaderMajorJob>();
        services.AddHostedService<ParrotClusterBackgroundWorker>();
        
        return services;
    }

    private static void AddMembersToCluster(InMemoryClusterConfigurationStorage<EndPoint> storage)
    {
        var builder = storage.CreateActiveConfigurationBuilder();

        var address = new IPEndPoint(IPAddress.Loopback, 8080);
        builder.Add(ClusterMemberId.FromEndPoint(address), address);

        address = new(IPAddress.Loopback, 8081);
        builder.Add(ClusterMemberId.FromEndPoint(address), address);

        address = new(IPAddress.Loopback, 8082);
        builder.Add(ClusterMemberId.FromEndPoint(address), address);

        builder.Build();
    }
}