namespace App.Ki.Business.Services.Identity;

public interface ICurrentUser<T>
{
    public T Id { get; set; }
    public string Name { get; set; }
}