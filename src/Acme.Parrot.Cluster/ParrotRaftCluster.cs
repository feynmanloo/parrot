using System.Diagnostics;
using DotNext.Net.Cluster;
using DotNext.Net.Cluster.Consensus.Raft;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Acme.Parrot.Cluster;

public class ParrotRaftCluster: RaftCluster, IHostedService
{
    private new ILogger Logger { get; set; }
    public ParrotRaftCluster(NodeConfiguration configuration, PersistentState persistentState) : base(configuration)
    {
        AuditTrail = persistentState;
        Logger = configuration.LoggerFactory.CreateLogger<ParrotRaftCluster>();
    }
    
    public new async Task StartAsync(CancellationToken cancellationToken)
    {
        this.LeaderChanged += LeaderChangedEvent;
        await base.StartAsync(cancellationToken);
        Logger.LogInformation("cluster start");
    }
    
    public new async Task StopAsync(CancellationToken cancellationToken)
    {
        this.LeaderChanged -= LeaderChangedEvent;
        await base.StopAsync(cancellationToken);
        Logger.LogInformation("cluster stop");
    }
    
    private void LeaderChangedEvent(ICluster cluster, IClusterMember? leader)
    {
        Debug.Assert(cluster is IRaftCluster);
        var term = ((IRaftCluster)cluster).Term;
        var timeout = ((IRaftCluster)cluster).ElectionTimeout;
        Logger.LogInformation(leader is null
            ? "Consensus cannot be reached"
            : $"New cluster leader is elected. Leader address is {leader.EndPoint}");
        Logger.LogInformation($"Term of local cluster member is {term}. Election timeout {timeout}");
    }
}