using System.Collections;
using System.Linq.Expressions;

namespace App.Ki.Clickhouse;

public class ChQueryable<T> : IQueryable<T> where T : class
{
    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Type ElementType => typeof(T);
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }
}
