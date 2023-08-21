namespace App.Ki.Clickhouse.Filters;

public enum Direction
{
    Asc,
    Desc
}

public class OrderBy
{
    public string FieldName { get; set; }
    public Direction Direction { get; set; }
}