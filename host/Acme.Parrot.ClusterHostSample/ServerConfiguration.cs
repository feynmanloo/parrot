using System.Net;
using DotNext;
using DotNext.Net.Cluster;
using DotNext.Net.Cluster.Consensus.Raft;
using DotNext.Net.Cluster.Consensus.Raft.Membership;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.Parrot.Server;

public static class ServerConfiguration
{
    public static IServiceCollection UseRaftClusterServer(this IServiceCollection services, int port)
    {
        var configuration = new RaftCluster.UdpConfiguration(new IPEndPoint(IPAddress.Loopback, port))
        {
            LowerElectionTimeout = 150,
            UpperElectionTimeout = 300,
            DatagramSize = 1024,
            ColdStart = false,
        };
        AddMembersToCluster(configuration.UseInMemoryConfigurationStorage());
        services.AddSingleton<SimplePersistentState>(new SimplePersistentState($"{DateTime.Now:yyyyMMddHHmmss}", new AppEventSource()));
        services.AddSingleton<ISupplier<long>>(sp => sp.GetRequiredService<SimplePersistentState>());
        services.AddSingleton<PersistentState>(sp => sp.GetRequiredService<SimplePersistentState>());
        services.AddSingleton<RaftCluster.NodeConfiguration>(configuration);
        services.AddSingleton<ParrotRaftCluster>();
        services.AddSingleton<IRaftCluster>(sp => sp.GetRequiredService<ParrotRaftCluster>());
        services.AddHostedService(sp => sp.GetRequiredService<ParrotRaftCluster>());
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