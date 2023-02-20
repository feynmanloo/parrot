using System.Diagnostics;
using DotNext.Net.Cluster;
using DotNext.Net.Cluster.Consensus.Raft;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Acme.Parrot.Server;

public class ParrotRaftCluster: RaftCluster, IHostedService
{
    private readonly ILogger<ParrotRaftCluster> _logger;

    public ParrotRaftCluster(NodeConfiguration configuration, ILogger<ParrotRaftCluster> logger, PersistentState persistentState) : base(configuration)
    {
        _logger = logger;
        this.AuditTrail = persistentState;
    }
    
    public new async Task StartAsync(CancellationToken cancellationToken)
    {
        this.LeaderChanged += LeaderChangedEvent;
        await base.StartAsync(cancellationToken);
        _logger.LogDebug("cluster start");
    }
    
    public new async Task StopAsync(CancellationToken cancellationToken)
    {
        this.LeaderChanged -= LeaderChangedEvent;
        await base.StopAsync(cancellationToken);
        _logger.LogDebug("cluster stop");
    }
    
    private void LeaderChangedEvent(ICluster cluster, IClusterMember? leader)
    {
        Debug.Assert(cluster is IRaftCluster);
        var term = ((IRaftCluster)cluster).Term;
        var timeout = ((IRaftCluster)cluster).ElectionTimeout;
        _logger.LogDebug(leader is null
            ? "Consensus cannot be reached"
            : $"New cluster leader is elected. Leader address is {leader.EndPoint}");
        _logger.LogDebug($"Term of local cluster member is {term}. Election timeout {timeout}");
    }
}