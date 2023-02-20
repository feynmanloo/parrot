using System.Diagnostics.Tracing;

namespace Acme.Parrot.Cluster;

public class AppEventSource : EventSource
{
    public AppEventSource()
        : base("RaftNode.Events")
    {
    }
}