using System.Runtime.Serialization;

namespace App.Ki.Commons.Models;

[DataContract]
public class AppResult : IAppResult
{
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 3)] public int StatusCode { get; set; }

    public static AppResult Ok(string message = null)
    {
        return New(message, 200, true);
    }

    public static AppResult Created(string message)
    {
        return New(message, 201, true);
    }

    public static AppResult Updated(string message)
    {
        return New(message, 204, true);
    }

    public static AppResult Bad(string message)
    {
        return New(message, 400);
    }

    public static AppResult UnAuthorized(string message)
    {
        return New(message, 401);
    }

    public static AppResult Forbidden(string message)
    {
        return New(message, 403);
    }

    public static AppResult NotFound(string message)
    {
        return New(message, 404);
    }

    public static AppResult ImTeapot(string message)
    {
        return New(message, 418);
    }

    public static AppResult Failed(string message, int code = 500)
    {
        return New(message, code);
    }

    public static AppResult Internal(string message)
    {
        return New(message, 500);
    }

    public static AppResult Validation(string message)
    {
        return New(message, 600);
    }

    private static AppResult New(
        string message,
        int code = 422,
        bool success = false)
    {
        return new AppResult {Message = message, StatusCode = code, Success = success};
    }
}

[DataContract]
public class AppResult<T> : IAppResult<T>
{
    [DataMember(Order = 3)] public T Data { get; set; }
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 4)] public int StatusCode { get; set; }

    public static AppResult<T> Ok(T data)
    {
        return New(null, 200, data, true);
    }

    public static AppResult<T> Created(T data, string message)
    {
        return New(message, 201, data, true);
    }

    public static AppResult<T> Updated(T data, string message)
    {
        return New(message, 204, data, true);
    }

    public static AppResult<T> Bad(string message)
    {
        return New(message, 400);
    }

    public static AppResult<T> UnAuthorized(string message)
    {
        return New(message, 401);
    }

    public static AppResult<T> Forbidden(string message)
    {
        return New(message, 403);
    }

    public static AppResult<T> NotFound(string message = "")
    {
        return New(message, 404);
    }

    public static AppResult<T> ImTeapot(string message)
    {
        return New(message, 418);
    }

    public static AppResult<T> Failed(string message, int code = 500)
    {
        return New(message, code);
    }

    public static AppResult<T> Internal(string message)
    {
        return New(message, 500);
    }

    public static AppResult<T> Validation(string message)
    {
        return New(message, 600);
    }

    private static AppResult<T> New(
        string message,
        int code = 422,
        T data = default,
        bool success = false)
    {
        return new AppResult<T> {Message = message, StatusCode = code, Success = success, Data = data};
    }
}