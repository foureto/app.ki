using App.Ki.Clickhouse.Internals;

namespace App.Ki.Clickhouse;

public class ClickhouseContext
{
    private readonly ClickhouseConnectionFactory _factory;

    public ClickhouseContext(ClickhouseConnectionFactory factory)
    {
        _factory = factory;
        InitCollections();
    }
    
    private void InitCollections()
    {
        var collections = GetType().GetProperties()
            .Where(p => p.PropertyType.IsGenericType &&
                        p.PropertyType.GetGenericTypeDefinition() == typeof(ChSet<>))
            .ToList();

        collections.ForEach(p => p.SetValue(this, Activator.CreateInstance(p.PropertyType, _factory.GetConnection().Result)));
    }

}