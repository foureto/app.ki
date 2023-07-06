using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace App.Ki.Business.Extensions;

internal static class QuartzExtensions
{
    public static IServiceCollectionQuartzConfigurator AddAppJob<T>(
        this IServiceCollectionQuartzConfigurator configurator,
        IServiceCollection services,
        IConfiguration configuration,
        string sectionName = null,
        TimeSpan? period = null,
        int minuteOffset = 1)
        where T : class, IJob
    {
        var tabPeriod = string.IsNullOrWhiteSpace(sectionName)
            ? period ?? TimeSpan.FromMinutes(1)
            : TimeSpan.Parse(configuration.GetSection(sectionName).Value!);

        services.AddScoped<T>();

        var now = DateTime.UtcNow;
        var startTime = (minuteOffset <= 0 ? now : now.AddMinutes(minuteOffset - now.Minute % minuteOffset))
            .AddSeconds(-now.Second);

        return configurator
            .AddJob<T>(opts => opts.WithIdentity(typeof(T).Name))
            .AddTrigger(opts =>
                opts
                    .ForJob(typeof(T).Name)
                    .WithIdentity($"{typeof(T).Name}-trigger")
                    .StartAt(startTime)
                    .WithSimpleSchedule(e => e.WithInterval(tabPeriod).RepeatForever()));
    }
}