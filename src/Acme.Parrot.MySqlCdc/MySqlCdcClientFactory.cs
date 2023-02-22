using Acme.Parrot.Core;
using Microsoft.Extensions.Options;
using MySqlCdc;

namespace Acme.Parrot.MySqlCdc;

public class MySqlCdcClientFactory
{
    private readonly ReplicaOptions _options;

    public MySqlCdcClientFactory(IOptions<ReplicaOptions> options)
    {
        _options = options.Value;
    }

    public BinlogClient Create(BinlogState? state = null)
    {
        return new BinlogClient(options =>
        {
            options.Hostname = _options.Hostname;
            options.Port = _options.Port;
            options.Username = _options.Username;
            options.Password = _options.Password;
            options.Database = _options.Database;
            options.SslMode = _options.SslMode;
            options.HeartbeatInterval = _options.HeartbeatInterval;
            options.Blocking = _options.Blocking;
            if (state != null && !string.IsNullOrEmpty(state.FileName) && state.Position > 0)
            {
                options.Binlog = BinlogOptions.FromPosition(state.FileName, state.Position);
            }
            else
            {
                options.Binlog = BinlogOptions.FromEnd();
            }
        });
    }
}