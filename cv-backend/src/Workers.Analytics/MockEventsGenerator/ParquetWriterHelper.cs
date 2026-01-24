using System.Threading.Tasks;
using Parquet;
using Parquet.Data;
using Parquet.Schema;
using Portfolio.Application.Analytics;

namespace Workers.Analytics.MockEventsGenerator;

public static class ParquetWriterHelper
{
    private static readonly DataField<DateTime> TimestampField = new("Timestamp");
    private static readonly DataField<string> EventTypeField = new("EventType");
    private static readonly DataField<string> RouteField = new("Route");
    private static readonly DataField<string?> MethodField = new("Method", true);
    private static readonly DataField<int> StatusCodeField = new("StatusCode");
    private static readonly DataField<long> DurationMsField = new("DurationMs");
    private static readonly DataField<Guid?> SessionIdField = new("SessionId");
    private static readonly DataField<Guid?> AnonymousIdField = new("AnonymousId");

    public static async Task<ParquetWriter> CreateWriter(Stream stream)
    {
        var schema = new ParquetSchema(
            TimestampField,
            EventTypeField,
            RouteField,
            MethodField,
            StatusCodeField,
            DurationMsField,
            SessionIdField,
            AnonymousIdField
        );

        var writer = await ParquetWriter.CreateAsync(schema, stream);
        return writer;
    }

    public static async Task WriteRowGroup(this ParquetWriter writer, List<ActivityEvent> batch)
    {
        using var rg = writer.CreateRowGroup();

        await rg.WriteColumnAsync(new DataColumn(TimestampField, batch.Select(x => x.Timestamp.UtcDateTime).ToArray()));
        await rg.WriteColumnAsync(new DataColumn(EventTypeField, batch.Select(x => x.EventType).ToArray()));
        await rg.WriteColumnAsync(new DataColumn(RouteField, batch.Select(x => x.Route).ToArray()));
        await rg.WriteColumnAsync(new DataColumn(MethodField, batch.Select(x => x.Method).ToArray()));
        await rg.WriteColumnAsync(new DataColumn(StatusCodeField, batch.Select(x => x.StatusCode).ToArray()));
        await rg.WriteColumnAsync(new DataColumn(DurationMsField, batch.Select(x => x.DurationMs).ToArray()));
        await rg.WriteColumnAsync(new DataColumn(SessionIdField, batch.Select(x => x.SessionId).ToArray()));
        await rg.WriteColumnAsync(new DataColumn(AnonymousIdField, batch.Select(x => x.AnonymousId).ToArray()));
    }
}
