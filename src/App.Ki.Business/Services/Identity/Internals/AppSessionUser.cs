namespace App.Ki.Business.Services.Identity.Internals;

internal class AppSessionUser : ICurrentUser<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
}