using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using System.Collections.Generic;

namespace Hospital.Api.Extensions;

public static class SerilogExtensions
{
    public static void AddSerilogBootstrap()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();
    }

    public static IHostBuilder ConfigureSerilog(this IHostBuilder host)
    {
        return host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration)
                         .Enrich.FromLogContext();

            var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                var columnWriters = new Dictionary<string, ColumnWriterBase>
                {
                    { "timestamp", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
                    { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                    { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                    { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                    { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) }
                };

                configuration.WriteTo.PostgreSQL(
                    connectionString: connectionString,
                    tableName: "SystemLogs",
                    columnOptions: columnWriters,
                    needAutoCreateTable: true
                );
            }
        });
    }
}
