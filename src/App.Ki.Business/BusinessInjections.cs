﻿using App.Ki.Business.Extensions;
using App.Ki.Business.Jobs;
using App.Ki.Business.Services.Exchanges;
using App.Ki.Business.Services.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace App.Ki.Business;

public static class BusinessInjections
{
    public static IServiceCollection AddBusiness(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            // jobs
            .AddQuartz(cfg =>
            {
                cfg.UseMicrosoftDependencyInjectionJobFactory();
                cfg.AddAppJob<TickerJob>(services, configuration, null, TimeSpan.FromSeconds(15), 0);
            })
            .AddQuartzHostedService(e => e.AwaitApplicationStarted = true)
            .AddHostedService<TickersBackgroundJob>()

            // modules
            .AddSignalR().Services
            .AddAppIdentity()
            .AddExchanges(configuration)

            // infra
            .AddMediator();
    }
}