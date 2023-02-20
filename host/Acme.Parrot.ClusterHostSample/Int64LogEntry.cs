using DotNext.IO;
using DotNext.IO.Log;
using DotNext.Net.Cluster.Consensus.Raft;

namespace Acme.Parrot.Server;

public class Int64LogEntry : BinaryTransferObject<long>, IRaftLogEntry
{
    internal Int64LogEntry()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }

    bool ILogEntry.IsSnapshot => false;

    public long Term { get; set; }

    public DateTimeOffset Timestamp { get; }
}