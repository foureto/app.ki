using System.Linq.Expressions;
using System.Reflection;
using App.Ki.Clickhouse.Attributes;

namespace App.Ki.Clickhouse.Helpers;

internal static class CachedObjectInvertor
{
    private static readonly Dictionary<Type, Dictionary<string, Func<object, object>>> TypesCache = new();
    private static readonly Dictionary<Type, string> Inserts = new();
    private static readonly Dictionary<Type, string> Tables = new();

    internal static string GetTableName<T>() where T : class
        => GetTableName(typeof(T));

    internal static string GetTableName(Type type)
    {
        if (Tables.TryGetValue(type, out var name))
            return name;

        var tableName = (type.GetCustomAttributes(typeof(ChTableAttribute), false)
            .FirstOrDefault() as ChTableAttribute)?.Name ?? type.Name;

        Tables.TryAdd(type, tableName);
        return tableName;
    }

    internal static string GetInsertQuery(Type type)
    {
        if (Inserts.TryGetValue(type, out var insertQuery))
            return insertQuery;

        var tableName = GetTableName(type);

        var fields = string.Join(", ", type.GetProperties().Select(e => e.Name));
        var query = $"insert into {tableName} ({fields}) values ";
        Inserts.TryAdd(type, query);

        return query;
    }

    internal static Dictionary<string, object> GetColumnsWithValues<T>(List<T> data)
    {
        var type = data.FirstOrDefault()?.GetType();
        if (type is null)
            return new Dictionary<string, object>();

        if (!TypesCache.ContainsKey(type))
        {
            var query = type.GetProperties().Select(p =>
            {
                var attribute = p.GetCustomAttribute(typeof(ChFieldAttribute)) as ChFieldAttribute;
                return new {attribute?.Sort, Prop = p};
            }).ToList();

            if (query.Any(e => e.Sort != null))
                query = query.OrderByDescending(e => e.Sort.HasValue).ThenBy(e => e.Sort).ToList();
            
            TypesCache.TryAdd(type, new Dictionary<string, Func<object, object>>());
            foreach (var propertyInfo in query.Select(e => e.Prop))
            {
                var property = propertyInfo.Name;
                var parameter = Expression.Parameter(typeof(object));
                var cast = Expression.Convert(parameter, type);
                var propertyGetter = Expression.Property(cast, property);

                var callExpr = propertyInfo.PropertyType switch
                {
                    {IsEnum: true} =>
                        Expression.Call(
                            propertyGetter, typeof(Enum).GetMethods().FirstOrDefault(e => e.Name == "ToString")!, null),
                    _ => null
                };
                
                if (callExpr != null)
                {
                    TypesCache[type].Add(
                        property,
                        Expression.Lambda<Func<object, object>>(callExpr, parameter).Compile());
                    continue;
                }
                
                var expression = propertyInfo.PropertyType switch
                {
                    { } dt when dt == typeof(DateTime) =>
                        Expression.Convert(Expression.Convert(
                            propertyGetter, typeof(DateTimeOffset)), typeof(object)),
                    {} bul when bul == typeof(bool) =>
                        Expression.Convert(Expression.Convert(
                            propertyGetter, typeof(byte)), typeof(object)),
                    {IsEnum: true} =>
                        Expression.Convert(
                            Expression.Convert(propertyGetter, typeof(sbyte)), typeof(object)),

                    _ => Expression.Convert(propertyGetter, typeof(object))
                };

                TypesCache[type].Add(
                    property,
                    Expression.Lambda<Func<object, object>>(expression, parameter).Compile());
            }
        }

        var properties = TypesCache[type].Keys;
        var result = properties.ToDictionary(e => e, _ => new List<object>());

        foreach (var item in data)
        foreach (var prop in properties)
            result[prop].Add(TypesCache[type][prop](item));

        return result.ToDictionary(e => e.Key, e => e.Value as object);
    }
}