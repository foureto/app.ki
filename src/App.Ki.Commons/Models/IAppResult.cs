namespace App.Ki.Commons.Models;

public interface IAppResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
}

public interface IAppResult<T> : IAppResult
{
    public T Data { get; set; }
}

public interface IAppResultEnumerable<T> : IAppResult
{
}