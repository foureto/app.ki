namespace App.Ki.Commons.Models.Paging;

public class PageContext : IPageContext
{
    public PageContext(
        int pageIndex,
        int pageSize,
        IEnumerable<SortDescriptor> listSort = null)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        ListSort = listSort ?? Array.Empty<SortDescriptor>();
    }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public IEnumerable<SortDescriptor> ListSort { get; set; }

    public bool IsValid()
    {
        return PageIndex > 0 && PageSize > 0 && ListSort != null;
    }
}

public class PageContext<T> : IPageContext<T>
    where T : class, new()
{
    public PageContext(
        int pageIndex,
        int pageSize,
        IEnumerable<SortDescriptor> listSort = null,
        T filter = null)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Filter = filter ?? new T();
        ListSort = listSort ?? Array.Empty<SortDescriptor>();
    }

    public int PageIndex { get; set; }

    public int PageSize { get; set; }


    public T Filter { get; set; }

    public IEnumerable<SortDescriptor> ListSort { get; set; }

    public bool IsValid()
    {
        return PageIndex > 0 && PageSize > 0 &&
               Filter != null && ListSort != null;
    }
}