using Acme.Parrot.Cluster;
using MySqlCdc;

namespace Acme.Parrot.Server.ConsoleHost;

public class ParrotOptions
{
    public ClusterOptions ClusterOptions { get; set; }
    public ReplicaOptions ReplicaOptions { get; set; }
}