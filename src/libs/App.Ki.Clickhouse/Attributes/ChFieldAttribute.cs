namespace App.Ki.Clickhouse.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ChFieldAttribute : Attribute
{
    public bool Nullable { get; set; }
    public int Sort { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
}