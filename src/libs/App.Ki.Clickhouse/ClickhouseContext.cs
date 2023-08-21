using System.Runtime.CompilerServices;
using App.Ki.Clickhouse.Helpers;
using App.Ki.Clickhouse.Internals;

namespace App.Ki.Clickhouse;

public class ClickhouseContext
{
    private readonly ClickhouseConnectionFactory _factory;

    public ClickhouseContext(ClickhouseConnectionFactory factory)
    {
        _factory = factory;
    }

    public void Add<T>(T entity) where T : class
        => QueuedBuffer.Push(entity);

    public void AddMany<T>(IEnumerable<T> entities) where T : class
    {
        foreach (var entity in entities)
            QueuedBuffer.Push(entity);
    }

    public async IAsyncEnumerable<T> Get<T>(
        string query,
        IDictionary<string, object> statements = null,
        [EnumeratorCancellation] CancellationToken token = default)
    {
        await using var session = await _factory.GetConnection();
        await foreach (var item in session.Get<T>(query, statements, token))
            yield return item;
    }

    public IQueryable<T> Query<T>()
        => Enumerable.Empty<T>().AsQueryable();
}