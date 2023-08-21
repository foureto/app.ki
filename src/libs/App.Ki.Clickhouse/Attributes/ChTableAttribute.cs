namespace App.Ki.Clickhouse.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ChTableAttribute : Attribute
{
    public const string DefaultFormat = "YYYYMM";

    public string Name { get; }
    public string Engine { get; set; }
    public string PartitionField { get; set; }
    public string PartitionFormat { get; set; } = DefaultFormat;
    public string Sign { get; set; }
    public string Ttl { get; set; }

    public ChTableAttribute(string name)
    {
        Name = name;
    }
}