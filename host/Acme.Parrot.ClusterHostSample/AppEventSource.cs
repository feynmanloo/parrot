using System.Diagnostics.Tracing;

namespace Acme.Parrot.Server;

public class AppEventSource : EventSource
{
    public AppEventSource()
        : base("RaftNode.Events")
    {
    }
}