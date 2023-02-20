using Acme.Parrot.Core;

namespace Acme.Parrot.Cluster;

public class EmptyLeaderMajorJob: ILeaderMajorJob
{
    public ValueTask<BinlogState> ExecuteAsync(BinlogState arg, CancellationToken cancellationToken)
    {
        arg.FileName = $"{DateTime.Now:yyyyMMddHHmmss}";
        arg.Position = DateTime.Now.Ticks;
        return ValueTask.FromResult(arg);
    }
}