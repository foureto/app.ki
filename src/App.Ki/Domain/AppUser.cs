using System.Security.Claims;

namespace App.Ki.Domain;

public class AppUser
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Claim> Claims { get; set; }
}