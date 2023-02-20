using Acme.Parrot.Core;

namespace Acme.Parrot.Cluster;

public interface ILeaderMajorJob
{
    ValueTask<BinlogState> ExecuteAsync(BinlogState arg, CancellationToken cancellationToken);
}