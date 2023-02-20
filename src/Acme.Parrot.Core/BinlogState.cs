namespace Acme.Parrot.Core;

public class BinlogState
{
    public string FileName { get; set; }
    public long Position { get; set; }
}