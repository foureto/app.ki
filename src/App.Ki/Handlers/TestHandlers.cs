namespace App.Ki.Handlers;

public static class TestHandlers
{
    public static WebApplication AddTestHandlers(this WebApplication builder)
    {
        builder.MapGet("/api/test", () => "Ok");

        return builder;
    }
}