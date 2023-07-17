using System.Net.Http.Headers;
using App.Ki.Business.Services.Exchanges.Internals;
using App.Ki.Business.Services.Exchanges.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace App.Ki.Business.Services.Exchanges;

internal static class ExchangesInjections
{
    internal static readonly Dictionary<string, Type> Exchanges = new();

    public static IServiceCollection AddExchanges(this IServiceCollection services, IConfiguration configuration)
    {
        Exchanges.Add(nameof(TinkoffClient), typeof(TinkoffClient));
        
        return services
            .AddScoped<Func<string, IExchange>>(sp => name => sp.GetRequiredService(Exchanges[name]) as IExchange)
            .AddScoped<IExchangeFactory, ExchangeFactory>()
            .AddExchange<KucoinExchange, KucoinSettings>(configuration, "Kucoin", "exchanges:kucoin")
            .Configure<TinkoffSettings>(opts => configuration.GetSection("exchanges:tinkoff").Bind(opts))
            .AddInvestApiClient((sp, settings) =>
            {
                var opts = sp.GetRequiredService<IOptions<TinkoffSettings>>();
                settings.AccessToken = opts.Value.ApiKey;
                settings.Sandbox = opts.Value.BaseUrl == "sandbox";
            })
            .AddScoped<TinkoffClient>();
    }

    private static IServiceCollection AddExchange<T, TS>(
        this IServiceCollection services,
        IConfiguration configuration, string name, string sectionName)
        where T : class, IExchange
        where TS : ExchangeSettings
    {
        Exchanges.Add(name, typeof(T));
        return services
            .Configure<TS>(opts => configuration.GetSection(sectionName).Bind(opts))
            .AddScoped<T>()
            .AddHttpClient(typeof(T).Name, (sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<TS>>();
                client.BaseAddress = new Uri(
                    settings.Value.BaseUrl ?? throw new ArgumentNullException(nameof(settings.Value.BaseUrl)));
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            }).Services;
    }
}