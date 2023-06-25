namespace App.Ki.Handlers;

internal static class SecretHandlers
{
    public static WebApplication AddSecretHandlers(this WebApplication builder)
    {
        var group = builder.MapGroup("/api/secret");
        group.MapPost("/api/secret", () => new { SecretData = "OKO!" }).RequireAuthorization();

        return builder;
    }
}