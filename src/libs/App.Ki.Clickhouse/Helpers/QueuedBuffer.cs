using System.Collections.Concurrent;

namespace App.Ki.Clickhouse.Helpers;

internal static class QueuedBuffer
{
    private static readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> Buffers = new();

    public static IEnumerable<Type> Types => Buffers.Keys;

    public static void Push<T>(T item) where T : class
    {
        var type = item.GetType();
        Buffers.TryAdd(type, new ConcurrentQueue<object>());
        Buffers[type].Enqueue(item);
    }

    public static IEnumerable<T> Pop<T>(int count = 1000, int min = 100) where T : class
        => Pop(typeof(T), count, min).Cast<T>();

    public static IEnumerable<object> Pop(Type type, int count = 1000, int min = 100)
    {
        if (type == null || !Buffers.ContainsKey(type) || Buffers[type].IsEmpty || Buffers[type].Count < min)
            yield break;

        var cnt = Math.Max(count, Buffers[type].Count);
        for (var i = 0; i < cnt; i++)
        {
            if (!Buffers[type].TryDequeue(out object item))
                yield break;
            yield return item;
        }
    }
}