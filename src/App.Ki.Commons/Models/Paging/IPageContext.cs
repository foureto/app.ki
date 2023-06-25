namespace App.Ki.Commons.Models.Paging;

public interface IPageContext
{
    int PageIndex { get; set; }

    int PageSize { get; set; }

    IEnumerable<SortDescriptor> ListSort { get; set; }

    bool IsValid();
}

public interface IPageContext<T> : IPageContext
{
    T Filter { get; set; }
}