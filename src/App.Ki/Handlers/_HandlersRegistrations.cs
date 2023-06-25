namespace App.Ki.Handlers;

internal static class _HandlersRegistrations
{
    public static WebApplication MapHandlers(this WebApplication builder)
        => builder
            .AddIdentityHandlers()
            .AddTestHandlers()
            .AddSecretHandlers();
}