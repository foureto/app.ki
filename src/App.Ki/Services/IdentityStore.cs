using System.Text.Json;

namespace App.Ki.Services;

public interface IIdentityStore
{
    Task Store<T>(T identity, string key);
    Task<T> Get<T>(string key);
}

internal class IdentityStore : IIdentityStore
{
    private readonly Dictionary<string, string> _store = new();

    public Task Store<T>(T identity, string key)
    {
        var id = identity ?? throw new ArgumentNullException(nameof(identity));
        if (!_store.TryAdd(key ?? throw new ArgumentNullException(nameof(key)), JsonSerializer.Serialize(id)))
            throw new Exception();

        return Task.CompletedTask;
    }

    public Task<T> Get<T>(string key)
    {
        if (_store.TryGetValue(key, out var id))
            return Task.FromResult(JsonSerializer.Deserialize<T>(id));

        throw new Exception();
    }
}