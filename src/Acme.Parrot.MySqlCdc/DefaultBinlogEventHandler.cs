using MySqlCdc.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Acme.Parrot.MySqlCdc;

public class DefaultBinlogEventHandler: IBinlogEventHandler
{
    public async ValueTask HandleTableMapEventAsync(TableMapEvent tableMap)
    {
        // Console.WriteLine($"Processing {tableMap.DatabaseName}.{tableMap.TableName}");
        // await PrintEventAsync(tableMap);
        await Task.CompletedTask;
    }

    public async ValueTask HandleWriteRowsEventAsync(WriteRowsEvent writeRows)
    {
        Console.WriteLine($"{writeRows.Rows.Count} rows were written");
        await PrintEventAsync(writeRows);

        foreach (var row in writeRows.Rows)
        {
            // Do something
        }
    }

    public async ValueTask HandleUpdateRowsEventAsync(UpdateRowsEvent updatedRows)
    {
        Console.WriteLine($"{updatedRows.Rows.Count} rows were updated");
        await PrintEventAsync(updatedRows);

        foreach (var row in updatedRows.Rows)
        {
            var rowBeforeUpdate = row.BeforeUpdate;
            var rowAfterUpdate = row.AfterUpdate;
            // Do something
        }
    }

    public async ValueTask HandleDeleteRowsEventAsync(DeleteRowsEvent deleteRows)
    {
        Console.WriteLine($"{deleteRows.Rows.Count} rows were deleted");
        await PrintEventAsync(deleteRows);

        foreach (var row in deleteRows.Rows)
        {
            // Do something
        }
    }
    
    private static async Task PrintEventAsync(IBinlogEvent binlogEvent)
    {
        var json = JsonConvert.SerializeObject(binlogEvent, Formatting.Indented,
            new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            });
        await Console.Out.WriteLineAsync(json);
    }
}