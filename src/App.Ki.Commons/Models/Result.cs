using System.Runtime.Serialization;

namespace App.Ki.Commons.Models;

[DataContract]
public class Result : IResult
{
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 3)] public int StatusCode { get; set; }

    public static Result Ok(string message = null)
    {
        return New(message, 200, true);
    }

    public static Result Created(string message)
    {
        return New(message, 201, true);
    }

    public static Result Updated(string message)
    {
        return New(message, 204, true);
    }

    public static Result Bad(string message)
    {
        return New(message, 400);
    }

    public static Result UnAuthorized(string message)
    {
        return New(message, 401);
    }

    public static Result Forbidden(string message)
    {
        return New(message, 403);
    }

    public static Result NotFound(string message)
    {
        return New(message, 404);
    }

    public static Result ImTeapot(string message)
    {
        return New(message, 418);
    }

    public static Result Failed(string message, int code = 500)
    {
        return New(message, code);
    }

    public static Result Internal(string message)
    {
        return New(message, 500);
    }

    public static Result Validation(string message)
    {
        return New(message, 600);
    }

    private static Result New(
        string message,
        int code = 422,
        bool success = false)
    {
        return new Result {Message = message, StatusCode = code, Success = success};
    }
}

[DataContract]
public class Result<T> : IResult<T>
{
    [DataMember(Order = 3)] public T Data { get; set; }
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 4)] public int StatusCode { get; set; }

    public static Result<T> Ok(T data)
    {
        return New(null, 200, data, true);
    }

    public static Result<T> Created(T data, string message)
    {
        return New(message, 201, data, true);
    }

    public static Result<T> Updated(T data, string message)
    {
        return New(message, 204, data, true);
    }

    public static Result<T> Bad(string message)
    {
        return New(message, 400);
    }

    public static Result<T> UnAuthorized(string message)
    {
        return New(message, 401);
    }

    public static Result<T> Forbidden(string message)
    {
        return New(message, 403);
    }

    public static Result<T> NotFound(string message = "")
    {
        return New(message, 404);
    }

    public static Result<T> ImTeapot(string message)
    {
        return New(message, 418);
    }

    public static Result<T> Failed(string message, int code = 500)
    {
        return New(message, code);
    }

    public static Result<T> Internal(string message)
    {
        return New(message, 500);
    }

    public static Result<T> Validation(string message)
    {
        return New(message, 600);
    }

    private static Result<T> New(
        string message,
        int code = 422,
        T data = default,
        bool success = false)
    {
        return new Result<T> {Message = message, StatusCode = code, Success = success, Data = data};
    }
}