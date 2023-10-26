using System.Reflection;
using App.Ki.Clickhouse.Attributes;
using App.Ki.Clickhouse.Internals;

namespace App.Ki.Clickhouse;

public interface IMigration
{
    ValueTask<bool> Up(ClickhouseSession session, IServiceProvider provider);
    ValueTask<bool> Down(ClickhouseSession session, IServiceProvider provider);
}

public abstract class ChMigration : IMigration
{
    private const string DefaultEngine = "MergeTree";

    public abstract ValueTask<bool> Up(ClickhouseSession session, IServiceProvider provider);
    public abstract ValueTask<bool> Down(ClickhouseSession session, IServiceProvider provider);

    protected static string GetTableCtor<T>(ClickhouseSession session) where T : class
    {
        var table = GetTableMetadata<T>();
        var fields = GetFieldsWithSettings<T>();
        var (cluster, engine) = GetClusterAndEngine<T>(session, table.Name, table.Engine, table.Sign);

        var result = @$"
create table if not exists `{table.Name}` {cluster}
(
    {string.Join(",\n\t", fields.Select(f => $"`{f.Name}` {(f.Nullable ? "Nullable(" : string.Empty)}{f.Type}{(f.Nullable ? ")" : string.Empty)}"))}
)
ENGINE = {engine}
{(string.IsNullOrWhiteSpace(table.PartitionField) ? string.Empty : $"PARTITION BY to{table.PartitionFormat}({table.PartitionField})")} 
ORDER BY (
    {string.Join(",\n\t", fields.Where(e => e.Sort is > 0).Select(e => e.Name))}
)";
        if (!string.IsNullOrWhiteSpace(table.Ttl))
            result += $"\nTTL {table.Ttl}";
        
        return result;
    }

    private static (string cluster, string engine) GetClusterAndEngine<T>(
        ClickhouseSession session,
        string tableName,
        string defaultEngine = DefaultEngine,
        string sign = null) where T : class
    {
        var clustered = !string.IsNullOrWhiteSpace(session.Settings.Cluster);
        var cluster = clustered ? $"on cluster '{session.Settings.Cluster}'" : string.Empty;

        var engine = clustered
            ? $"Replicated{defaultEngine}('/clickhouse/tables/{{shard}}/{session.Database}/{tableName}', '{{replica}}' {(string.IsNullOrWhiteSpace(sign) ? string.Empty : $", {sign}")})"
            : $"{defaultEngine}({sign ?? string.Empty})";

        return (cluster, engine);
    }

    public static TableMetadata GetTableMetadata<T>() where T : class
    {
        var type = typeof(T);
        var tableAttribute = type.GetCustomAttribute<ChTableAttribute>();
        return new TableMetadata(
            tableAttribute?.Name ?? type.Name,
            tableAttribute?.Engine ?? DefaultEngine,
            tableAttribute?.PartitionField,
            tableAttribute?.PartitionFormat ?? ChTableAttribute.DefaultFormat,
            tableAttribute?.Ttl,
            tableAttribute?.Sign);
    }

    private static IReadOnlyList<PropertyFieldMetadata> GetFieldsWithSettings<T>() where T : class
    {
        return typeof(T).GetProperties()
            .Select(e =>
            {
                var attribute = e.GetCustomAttribute(typeof(ChFieldAttribute)) as ChFieldAttribute;
                return new PropertyFieldMetadata(
                    attribute?.Name ?? e.Name,
                    attribute?.Type ?? GetPropertyType(e.PropertyType),
                    attribute?.Nullable ?? false,
                    attribute?.Sort);
            })
            .OrderByDescending(e => e.Sort.HasValue).ThenBy(e => e.Sort)
            .ToList();
    }

    private static string GetPropertyType(Type propertyType)
        => propertyType switch
        {
            not null when propertyType == typeof(bool) => "UInt8",
            not null when propertyType == typeof(sbyte) => "Int8",
            not null when propertyType == typeof(byte) => "Int16",
            not null when propertyType == typeof(int) => "Int32",
            not null when propertyType == typeof(int?) => "Nullable(Int32)",
            not null when propertyType == typeof(uint) => "UInt32",
            not null when propertyType == typeof(long) => "Int64",
            not null when propertyType == typeof(ulong) => "UInt64",
            not null when propertyType == typeof(float) => "Float32",
            not null when propertyType == typeof(double) => "Float64",
            not null when propertyType == typeof(decimal) => "Decimal(28,18)",
            not null when propertyType == typeof(string) => "String",
            not null when propertyType == typeof(DateTime) => "DateTime64(3)",
            not null when propertyType == typeof(DateTimeOffset) => "DateTime64(3)",
            not null when propertyType == typeof(Guid) => "UUID",
            _ => throw new ArgumentOutOfRangeException(nameof(propertyType)),
        };
}