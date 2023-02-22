using MySqlCdc.Events;

namespace Acme.Parrot.MySqlCdc;

public interface IBinlogEventHandler
{
    ValueTask HandleTableMapEventAsync(TableMapEvent tableMap);
    ValueTask HandleWriteRowsEventAsync(WriteRowsEvent writeRows);
    ValueTask HandleUpdateRowsEventAsync(UpdateRowsEvent updateRows);
    ValueTask HandleDeleteRowsEventAsync(DeleteRowsEvent deleteRows);
}