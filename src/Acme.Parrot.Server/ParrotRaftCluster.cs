using DotNext.Net.Cluster.Consensus.Raft;
using Microsoft.Extensions.Hosting;

namespace Acme.Parrot.Server;

public class ParrotRaftCluster: RaftCluster, IHostedService
{
    private readonly RaftCluster _cluster;
    
    public ParrotRaftCluster(NodeConfiguration configuration) : base(configuration)
    {
        this._cluster = new RaftCluster(configuration);
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _cluster.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cluster.StopAsync(cancellationToken);
    }
}