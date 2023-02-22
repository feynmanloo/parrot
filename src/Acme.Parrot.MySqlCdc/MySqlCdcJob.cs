using Acme.Parrot.Core;
using Microsoft.Extensions.Logging;
using MySqlCdc;
using MySqlCdc.Events;

namespace Acme.Parrot.MySqlCdc;

public class MySqlCdcJob: ILeaderMajorJob
{
    private readonly MySqlCdcClientFactory _mySqlCdcClientFactory;
    private readonly IBinlogEventHandler _binlogEventHandler;
    private readonly ILogger<MySqlCdcJob> _logger;

    public MySqlCdcJob(MySqlCdcClientFactory mySqlCdcClientFactory, ILogger<MySqlCdcJob> logger, IBinlogEventHandler binlogEventHandler)
    {
        _mySqlCdcClientFactory = mySqlCdcClientFactory;
        _logger = logger;
        _binlogEventHandler = binlogEventHandler;
    }

    public async ValueTask ExecuteAsync(BinlogState arg, Action<BinlogState> updateBinlogStateAction, CancellationToken cancellationToken)
    {
        var bc = _mySqlCdcClientFactory.Create();
        await foreach (var binlogEvent in bc.Replicate(cancellationToken))
        {
            await BinlogEventHandlerAsync(binlogEvent, cancellationToken);

            arg.FileName = bc.State.Filename;
            arg.Position = bc.State.Position;
            updateBinlogStateAction.Invoke(arg);
        }
    }
    
    private async ValueTask BinlogEventHandlerAsync(IBinlogEvent binlogEvent, CancellationToken linkSourceToken)
    {
        switch (binlogEvent)
        {
            case TableMapEvent tableMap:
                await _binlogEventHandler.HandleTableMapEventAsync(tableMap);
                break;
            case WriteRowsEvent writeRows:
                await _binlogEventHandler.HandleWriteRowsEventAsync(writeRows);
                break;
            case UpdateRowsEvent updateRows:
                await _binlogEventHandler.HandleUpdateRowsEventAsync(updateRows);
                break;
            case DeleteRowsEvent deleteRows:
                await _binlogEventHandler.HandleDeleteRowsEventAsync(deleteRows);
                break;
            default:
                _logger.LogDebug($"unknow type: {binlogEvent.GetType().FullName}");
                break;
        }
    }
}