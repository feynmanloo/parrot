using Acme.Parrot.Core;

namespace Acme.Parrot.Cluster;

public class EmptyLeaderMajorJob: ILeaderMajorJob
{
    public async ValueTask ExecuteAsync(BinlogState lastState, Action<BinlogState> updateBinlogStateAction, CancellationToken cancellationToken)
    {
        lastState.FileName = $"{DateTime.Now:yyyyMMddHHmmss}";
        lastState.Position ++;
        updateBinlogStateAction.Invoke(lastState);
    }
}