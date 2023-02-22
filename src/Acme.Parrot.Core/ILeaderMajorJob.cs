namespace Acme.Parrot.Core;

public interface ILeaderMajorJob
{
    ValueTask ExecuteAsync(BinlogState lastState, Action<BinlogState> updateBinlogStateAction, CancellationToken cancellationToken);
}