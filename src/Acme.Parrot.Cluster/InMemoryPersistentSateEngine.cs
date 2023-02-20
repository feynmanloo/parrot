using System.Text.Json;
using Acme.Parrot.Core;
using DotNext;
using DotNext.Net.Cluster.Consensus.Raft;

namespace Acme.Parrot.Cluster;

public class InMemoryPersistentSateEngine : MemoryBasedStateMachine, ISupplier<BinlogState>
{
    private BinlogState? content = null;
    
    public InMemoryPersistentSateEngine(string path, AppEventSource source)
        : base(path, 50, CreateOptions(source))
    {
    }
    
    public InMemoryPersistentSateEngine(DirectoryInfo path, int recordsPerPartition, Options? configuration = null) : base(path, recordsPerPartition, configuration)
    {
    }

    public InMemoryPersistentSateEngine(string path, int recordsPerPartition, Options? configuration = null) : base(path, recordsPerPartition, configuration)
    {
    }

    private async ValueTask UpdateValue(LogEntry entry)
    {
        var value = (BinlogState)(await entry.DeserializeFromJsonAsync())!;
        Console.WriteLine($"Accepting value {value.FileName}-{value.Position}");
    }

    protected override ValueTask ApplyAsync(LogEntry entry)
        => entry.Length == 0L ? new ValueTask() : UpdateValue(entry);

    protected override SnapshotBuilder CreateSnapshotBuilder(in SnapshotBuilderContext context)
    {
        Console.WriteLine("Building snapshot");
        return new SimpleSnapshotBuilder(context);
    }

    public BinlogState Invoke()
    {
        return content ?? new BinlogState();
    }
    
    private static Options CreateOptions(AppEventSource source)
    {
        var result = new Options
        {
            InitialPartitionSize = 50 * 8,
            WriteCounter = new("WAL.Writes", source),
            ReadCounter = new("WAL.Reads", source),
            CommitCounter = new("WAL.Commits", source),
            CompactionCounter = new("WAL.Compaction", source),
            LockContentionCounter = new("WAL.LockContention", source),
            LockDurationCounter = new("WAL.LockDuration", source),
        };

        result.WriteCounter.DisplayUnits =
            result.ReadCounter.DisplayUnits =
                result.CommitCounter.DisplayUnits =
                    result.CompactionCounter.DisplayUnits = "entries";

        result.LockDurationCounter.DisplayUnits = "milliseconds";
        result.LockDurationCounter.DisplayName = "WAL Lock Duration";

        result.LockContentionCounter.DisplayName = "Lock Contention";

        result.WriteCounter.DisplayName = "Number of written entries";
        result.ReadCounter.DisplayName = "Number of retrieved entries";
        result.CommitCounter.DisplayName = "Number of committed entries";
        result.CompactionCounter.DisplayName = "Number of squashed entries";

        return result;
    }
    private sealed class SimpleSnapshotBuilder : IncrementalSnapshotBuilder
    {
        private BinlogState? _value;

        public SimpleSnapshotBuilder(in SnapshotBuilderContext context)
            : base(context)
        {
        }

        protected override async ValueTask ApplyAsync(LogEntry entry)
        {
            // value = await entry.ToTypeAsync<long, LogEntry>().ConfigureAwait(false);
            _value = (BinlogState)(await entry.DeserializeFromJsonAsync())!;
        }

        public override ValueTask WriteToAsync<TWriter>(TWriter writer, CancellationToken token)
        {
            using var memory = new MemoryStream();
            JsonSerializer.Serialize(memory, _value);
            return writer.WriteAsync(memory.GetBuffer(), null, token);
            // return writer.WriteAsync(value, token);
        }
    }
}