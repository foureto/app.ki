using System.Runtime.Serialization;

namespace App.Ki.Commons.Models;

[DataContract]
public class PagedResult<T> : IResultEnumerable<T>
{
    [DataMember(Order = 3)] public int Count { get; set; }
    [DataMember(Order = 4)] public int Page { get; set; }
    [DataMember(Order = 5)] public int Total { get; set; }
    [DataMember(Order = 6)] public IEnumerable<T> Data { get; set; } = null!;
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 7)] public int StatusCode { get; set; }

    public static PagedResult<T> Ok(IEnumerable<T> data, int count = 0, int page = 0, int total = 0)
    {
        return New(null, 200, data, true, count, page, total);
    }


    public static PagedResult<T> Bad(string message)
    {
        return New(message, 400);
    }

    public static PagedResult<T> Forbidden(string message)
    {
        return New(message, 401);
    }

    public static PagedResult<T> UnAuthorized(string message)
    {
        return New(message, 403);
    }

    public static PagedResult<T> NotFound(string message)
    {
        return New(message, 404);
    }

    public static PagedResult<T> Failed(string message, int code = 500)
    {
        return New(message, code);
    }

    public static PagedResult<T> Internal(string message)
    {
        return New(message, 500);
    }
    
    private static PagedResult<T> New(
        string message,
        int code = 422,
        IEnumerable<T> data = null!,
        bool success = false,
        int count = 0,
        int page = 0,
        int total = 0)
    {
        return new PagedResult<T>
        {
            Message = message,
            StatusCode = code,
            Success = success,
            Data = data,
            Count = count,
            Page = page,
            Total = total
        };
    }
}