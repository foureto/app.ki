using App.Ki.Clickhouse.Attributes;

namespace App.Ki.Clickhouse.Internals;

public record TableMetadata(
    string Name,
    string Engine,
    string PartitionField,
    string PartitionFormat = ChTableAttribute.DefaultFormat,
    string Ttl = null,
    string Sign = null);