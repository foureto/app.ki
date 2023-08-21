namespace App.Ki.Clickhouse.Internals;

public record PropertyFieldMetadata(
    string Name,
    string Type,
    bool Nullable,
    int? Sort = null);