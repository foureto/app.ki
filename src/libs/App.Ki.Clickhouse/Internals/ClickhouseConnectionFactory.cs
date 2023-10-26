using App.Ki.Clickhouse.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octonica.ClickHouseClient;

namespace App.Ki.Clickhouse.Internals;

public class ClickhouseConnectionFactory
{
    private readonly ClickHouseConnectionStringBuilder _connectionBuilder;
    private readonly IOptions<ClickhouseSettings> _options;
    private readonly ILoggerFactory _loggerFactory;
    private int _createDbAttempts;

    public ClickhouseConnectionFactory(
        IOptions<ClickhouseSettings> options,
        ILoggerFactory loggerFactory)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _loggerFactory = loggerFactory;
        _connectionBuilder = new ClickHouseConnectionStringBuilder
        {
            Host = _options.Value.Host,
            Port = _options.Value.Port,
            Database = _options.Value.Database,
            User = _options.Value.User,
            Password = _options.Value.Password,
            CommandTimeout = _options.Value.CommandTimeout
        };
    }

    public async ValueTask<ClickhouseSession> GetConnection()
    {
        if (_createDbAttempts == 0 && Interlocked.Increment(ref _createDbAttempts) == 1)
            await TryCreatedDatabase();

        var connection = new ClickHouseConnection(_connectionBuilder);

        await connection.OpenAsync();

        return new ClickhouseSession(
            connection, _options, _loggerFactory.CreateLogger<ClickhouseSession>());
    }

    private async Task TryCreatedDatabase()
    {
        var connectionBuilder = new ClickHouseConnectionStringBuilder
        {
            Host = _options.Value.Host,
            Port = _options.Value.Port,
            User = _options.Value.User,
            Password = _options.Value.Password,
            CommandTimeout = _options.Value.CommandTimeout
        };

        var connection = new ClickHouseConnection(connectionBuilder);
        await connection.OpenAsync();

        await using var session = new ClickhouseSession(
            connection, _options, _loggerFactory.CreateLogger<ClickhouseSession>());
        await session.Run($"CREATE DATABASE IF NOT EXISTS {_options.Value.Database};");
    }
}