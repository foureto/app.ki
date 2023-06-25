namespace App.Ki.Commons.Models.Paging;

public class SortDescriptor
{
    public SortDescriptor(string field, EnumSortDirection order = EnumSortDirection.Asc)
    {
        Field = field;
        Order = order;
    }

    public string Field { get; set; }

    public EnumSortDirection Order { get; set; }

    public override string ToString()
    {
        var direction = string.Empty;

        if (Order == EnumSortDirection.Desc) direction = $" {Order.ToString()}";

        return $"{Field}{direction}";
    }
}