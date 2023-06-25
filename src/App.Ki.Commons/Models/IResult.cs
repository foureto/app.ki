namespace App.Ki.Commons.Models;

public interface IResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
}

public interface IResult<T> : IResult
{
    public T Data { get; set; }
}

public interface IResultEnumerable<T> : IResult
{
}