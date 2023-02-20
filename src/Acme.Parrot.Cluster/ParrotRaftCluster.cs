using System.Diagnostics;
using DotNext.Net.Cluster;
using DotNext.Net.Cluster.Consensus.Raft;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Acme.Parrot.Cluster;

public class ParrotRaftCluster: RaftCluster, IHostedService
{
    public ParrotRaftCluster(NodeConfiguration configuration, PersistentState persistentState) : base(configuration)
    {
        AuditTrail = persistentState;
    }
    
    public new async Task StartAsync(CancellationToken cancellationToken)
    {
        this.LeaderChanged += LeaderChangedEvent;
        await base.StartAsync(cancellationToken);
        Logger.LogDebug("cluster start");
    }
    
    public new async Task StopAsync(CancellationToken cancellationToken)
    {
        this.LeaderChanged -= LeaderChangedEvent;
        await base.StopAsync(cancellationToken);
        Logger.LogDebug("cluster stop");
    }
    
    private void LeaderChangedEvent(ICluster cluster, IClusterMember? leader)
    {
        Debug.Assert(cluster is IRaftCluster);
        var term = ((IRaftCluster)cluster).Term;
        var timeout = ((IRaftCluster)cluster).ElectionTimeout;
        Logger.LogDebug(leader is null
            ? "Consensus cannot be reached"
            : $"New cluster leader is elected. Leader address is {leader.EndPoint}");
        Logger.LogDebug($"Term of local cluster member is {term}. Election timeout {timeout}");
    }
}