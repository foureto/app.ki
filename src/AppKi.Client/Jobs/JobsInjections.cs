namespace AppKi.Client.Jobs;

internal static class JobsInjections
{
    public static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddHostedService<DataHistoryStoringJob>();
    }
}