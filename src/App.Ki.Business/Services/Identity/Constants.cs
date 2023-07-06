namespace App.Ki.Business.Services.Identity;

public class Constants
{
    // Schemes
    public const string TempScheme = "temp-cookie";
    public const string GeneralScheme = "cookie";

    // policies
    public const string TempPolicy = "temp-policy";
    public const string GeneralPolicy = "app-policy";

    // roles
    public const string Customer = nameof(Customer);
    public const string Admin = nameof(Admin);

    // policy claims
    private static readonly string[] AllUsers = new[] { Customer, Admin };

    public static readonly Dictionary<string, string[]> Policies = new()
    {
        { TempPolicy, AllUsers },
        { GeneralPolicy, AllUsers },
    };
}