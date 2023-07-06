using System.Runtime.Serialization;

namespace App.Ki.Commons.Models;

[DataContract]
public class AppResultList<T> : IAppResult<IEnumerable<T>>
{
    [DataMember(Order = 3)] public IEnumerable<T> Data { get; set; }
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 4)] public int StatusCode { get; set; }

    public static AppResultList<T> Ok(IEnumerable<T> data, string message = null)
    {
        return New(message, 200, data, true);
    }

    public static AppResultList<T> Created(IEnumerable<T> data, string message = null)
    {
        return New(message, 201, data, true);
    }

    public static AppResultList<T> Updated(IEnumerable<T> data = default, string message = null)
    {
        return New(message, 204, data, true);
    }

    public static AppResultList<T> Bad(string message)
    {
        return New(message, 400);
    }

    public static AppResultList<T> UnAuthorized(string message)
    {
        return New(message, 401);
    }

    public static AppResultList<T> Forbidden(string message)
    {
        return New(message, 403);
    }

    public static AppResultList<T> NotFound(string message)
    {
        return New(message, 404);
    }

    public static AppResultList<T> Failed(string message, int code = 500)
    {
        return New(message, code);
    }

    public static AppResultList<T> Failed(IAppResult appResult)
    {
        return New(appResult.Message, appResult.StatusCode);
    }

    public static AppResultList<T> Internal(string message)
    {
        return New(message);
    }

    private static AppResultList<T> New(
        string message,
        int code = 500,
        IEnumerable<T> data = default,
        bool success = false)
    {
        return new()
        {
            Message = message,
            StatusCode = code,
            Success = success,
            Data = data,
        };
    }
}