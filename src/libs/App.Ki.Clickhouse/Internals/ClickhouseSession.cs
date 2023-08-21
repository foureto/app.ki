using System.Runtime.CompilerServices;
using App.Ki.Clickhouse.Extensions;
using App.Ki.Clickhouse.Helpers;
using App.Ki.Clickhouse.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octonica.ClickHouseClient;

namespace App.Ki.Clickhouse.Internals;

public class ClickhouseSession : IDisposable, IAsyncDisposable
{
    private readonly ClickHouseConnection _connection;
    private readonly IOptions<ClickhouseSettings> _dbSettings;
    private readonly ILogger<ClickhouseSession> _logger;
    private readonly Type[] _scalars = { typeof(string), typeof(DateTime), typeof(DateTimeOffset) };

    public string Database => _dbSettings.Value.Database;
    public ClickhouseSettings Settings => _dbSettings.Value;

    public ClickhouseSession(
        ClickHouseConnection connection,
        IOptions<ClickhouseSettings> dbSettings,
        ILogger<ClickhouseSession> logger)
    {
        _connection = connection;
        _dbSettings = dbSettings;
        _logger = logger;
    }

    internal async IAsyncEnumerable<T> Get<T>(
        string query,
        IDictionary<string, object> statements = null,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        _logger.LogInformation("Will execute command with {Query}", query);
        await using var command = GetCommand(query, statements);
        var props = _scalars.Contains(typeof(T))
            ? null
            : typeof(T).GetProperties()
                .Select((e, i) => new { Propery = e, DbName = e.Name, Index = i })
                .ToArray();

        await using var reader = await command.ExecuteReaderAsync(token);
        while (!token.IsCancellationRequested && await reader.ReadAsync(token))
        {
            T newObj = typeof(T).IsValueType || typeof(T) == typeof(string)
                ? default
                : (T)Activator.CreateInstance(typeof(T));

            if (props is { Length: > 0 })
                foreach (var propertyRef in props)
                {
                    var index = reader.GetOrdinal(propertyRef.DbName);
                    var type = reader.GetFieldType(index);
                    var data = reader.GetValue(index);
                    newObj.SetProperty(propertyRef.Propery, data, type);
                }
            else
                newObj = (T)reader.GetValue(0).ConvertFromDbValue(reader.GetFieldType(0));

            yield return newObj;
        }
    }

    internal async ValueTask BulkAdd(Type type, List<object> data, CancellationToken token)
    {
        await using var writer = await _connection.CreateColumnWriterAsync(
            $"insert into {CachedObjectInvertor.GetTableName(type)} values", token);
        var rows = CachedObjectInvertor.GetColumnsWithValues(data);
        await writer.WriteTableAsync(rows, data.Count, token);

        await writer.EndWriteAsync(token);
    }

    public async Task<int> Run(
        string query,
        IDictionary<string, object> statements = null,
        CancellationToken token = default)
    {
        await using var command = GetCommand(query, statements);
        return await command.ExecuteNonQueryAsync(token);
    }

    private ClickHouseCommand GetCommand(string query, IDictionary<string, object> statements = null)
    {
        var command = _connection.CreateCommand(query);
        if (statements is null)
            return command;

        foreach (var (key, value) in statements)
            command.Parameters.AddWithValue(key, value);

        return command;
    }

    private static async IAsyncEnumerable<T> ExecuteCommand<T>(
        ClickHouseCommand command,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where T : class, new()
    {
        var props = typeof(T)
            .GetProperties()
            .Select((e, i) => new { Propery = e, DbName = e.Name, Index = i })
            .ToArray();

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (!cancellationToken.IsCancellationRequested && await reader.ReadAsync(cancellationToken))
        {
            var newObj = new T();

            if (props.Length > 0)
                foreach (var propertyRef in props)
                {
                    var index = reader.GetOrdinal(propertyRef.DbName);
                    var type = reader.GetFieldType(index);
                    var data = reader.GetValue(index);
                    newObj.SetProperty(propertyRef.Propery, DBNull.Value == data ? default : data, type);
                }
            else
                newObj = (T)reader.GetValue(0).ConvertFromDbValue(reader.GetFieldType(0));

            yield return newObj;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}