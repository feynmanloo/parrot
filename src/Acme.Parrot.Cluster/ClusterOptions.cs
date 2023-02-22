namespace Acme.Parrot.Cluster;

public class ClusterOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public bool ColdStart { get; set; } = false;
}