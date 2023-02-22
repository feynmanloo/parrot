using Acme.Parrot.Core;
using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Acme.Parrot.Cluster;

public class ParrotClusterBackgroundWorker: BackgroundService
{
    private readonly ILeaderMajorJob _majorJob;
    private readonly IRaftCluster _cluster;
    private readonly ISupplier<BinlogState> _supplier;
    private readonly PersistentState _persistentState;
    private readonly ILogger<ParrotClusterBackgroundWorker> _logger;

    public ParrotClusterBackgroundWorker(
        IRaftCluster cluster, 
        PersistentState persistentState, 
        ILogger<ParrotClusterBackgroundWorker> logger, 
        ILeaderMajorJob majorJob,
        ISupplier<BinlogState> supplier)
    {
        _cluster = cluster;
        _persistentState = persistentState;
        _logger = logger;
        _majorJob = majorJob;
        _supplier = supplier;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            var leadershipToken = _cluster.LeadershipToken;
            if (!leadershipToken.IsCancellationRequested)
            {
                var source = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, leadershipToken);
                try
                {
                    var lastBinlogState = _supplier.Invoke();
                    await _majorJob.ExecuteAsync(lastBinlogState, async currentBinlogState =>
                    {
                        try
                        {
                            if (!_cluster.LeadershipToken.IsCancellationRequested)
                            {
                                var entry = _persistentState.CreateJsonLogEntry(currentBinlogState);
                                _logger.LogInformation($"发布消息");
                                await _cluster.ReplicateAsync(entry, source.Token);
                            }
                        }
                        catch (OperationCanceledException operationCanceledException)
                        {
                    
                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Unexpected error {0}", e);
                        }
                    }, source.Token);
                }
                catch (OperationCanceledException operationCanceledException)
                {
                    
                }
                catch (Exception e)
                {
                    _logger.LogError("Unexpected error {0}", e);
                }
            }
        }
    }
}