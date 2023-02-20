namespace Acme.Parrot.Cluster;

public class ClusterOptions
{
    public int Port { get; set; }
    public bool ColdStart { get; set; } = false;
}