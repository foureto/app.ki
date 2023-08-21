using System.Text;

namespace App.Ki.Clickhouse.Filters;

public class FilterBuilder
{
    public int Count => _filters.Count + _groupBy.Count + _limitBy.Count + _orderBy.Count;

    private readonly List<Filter> _filters = new();
    private readonly List<GroupBy> _groupBy = new();
    private readonly List<LimitBy> _limitBy = new();
    private readonly List<OrderBy> _orderBy = new();

    public FilterBuilder Add(
        string fieldName,
        string strOperator,
        object value,
        LogicOperator prevOperator = LogicOperator.NONE,
        LogicOperator nextOperator = LogicOperator.NONE)
    {
        var filter = new Filter
        {
            FieldName = fieldName,
            ParameterValue = value,
            PrevOperator = prevOperator,
            NextOperator = nextOperator,
            Operator = strOperator
        };

        _filters.Add(filter);

        return this;
    }

    public FilterBuilder AddBracket(RoundBracketType type)
    {
        switch (type)
        {
            case RoundBracketType.Left:
                Add("(", "", "");
                break;
            case RoundBracketType.Right:
                Add("", "", ")");
                break;
        }

        return this;
    }

    public FilterBuilder Remove(string fieldName)
    {
        _filters.RemoveAll(f => f.FieldName.Equals(fieldName));
        return this;
    }

    public void Clear()
    {
        _filters.Clear();
        _orderBy.Clear();
        _groupBy.Clear();
    }

    public string ApplyFilters(string query)
    {
        var sb = new StringBuilder();
        sb.Append(query);

        if (_filters.Count > 0)
        {
            sb.Append(" WHERE");

            _filters.ForEach(f => { sb.Append(f.ToCriteria()); });
        }

        if (_groupBy.Count > 0)
        {
            sb.Append(" GROUP BY ");
            sb.Append(string.Join(", ", _groupBy.Select(g => g.FieldName).ToArray()));
        }

        if (_orderBy.Count > 0)
        {
            sb.Append(" ORDER BY ");
            sb.Append(string.Join(", ", _orderBy.Select(o => $"{o.FieldName} {o.Direction}").ToArray()));
        }

        if (_limitBy.Count <= 0)
            return sb.ToString();

        foreach (var limitBy in _limitBy)
            sb.Append(" LIMIT ")
                .Append(limitBy.Count)
                .Append(" BY ")
                .Append(limitBy.FieldName);

        return sb.ToString();
    }

    public FilterBuilder AddOrderBy(string fieldName, Direction direction = Direction.Asc)
    {
        _orderBy.Add(new OrderBy {FieldName = fieldName, Direction = direction});
        return this;
    }

    public FilterBuilder AddGroupBy(string fieldName)
    {
        _groupBy.Add(new GroupBy {FieldName = fieldName});
        return this;
    }

    public List<OrderBy> GetOrdersBy()
    {
        return _orderBy;
    }

    public FilterBuilder AddLimitBy(string fieldName, int count)
    {
        _limitBy.Add(new LimitBy {FieldName = fieldName, Count = count});
        return this;
    }
}