using System.Reflection;
using Microsoft.Extensions.Logging;

namespace App.Ki.Clickhouse.Internals;

internal class MigrationService : IMigrationService
{
    private const string MigrationsTable = "_migrations";
    private readonly ILogger<MigrationService> _logger;
    private readonly ClickhouseConnectionFactory _factory;
    private readonly IServiceProvider _provider;

    public MigrationService(
        ClickhouseConnectionFactory factory,
        IServiceProvider provider,
        ILogger<MigrationService> logger)
    {
        _factory = factory;
        _provider = provider;
        _logger = logger;
    }

    public async Task Migrate(IEnumerable<Assembly> assemblies, bool ensureCreated = true)
    {
        using var _ = _logger.BeginScope("Clickhouse.Migrations");
        _logger.LogInformation("Started migration process");

        await using var session = await _factory.GetConnection();
        if (ensureCreated)
        {
            await session.Run($"CREATE DATABASE IF NOT EXISTS {session.Database} {GetCluster(session)};");
            await CreatedTableIfNotExists();
        }

        _logger.LogInformation("Getting applied migrations");
        var appliedMigrations = await session.Get<string>($"SELECT name FROM `{MigrationsTable}`").ToListAsync();
        _logger.LogInformation("Got applied migrations ({Count}): {@Migrations}",
            appliedMigrations.Count, appliedMigrations);

        _logger.LogInformation("Getting NOT applied migrations");
        var migrations = assemblies.SelectMany(a =>
                a.GetTypes()
                    .Where(e =>
                        e.GetInterfaces().Contains(typeof(IMigration)) &&
                        !e.IsAbstract &&
                        !appliedMigrations.Contains(e.Name))
                    .OrderBy(e => e.Name))
            .Select(e => new {Instance = Activator.CreateInstance(e) as IMigration, e.Name})
            .ToArray();

        _logger.LogInformation("Got {Count} NOT applied migrations", migrations.Length);

        foreach (var migration in migrations)
        {
            bool ok;
            try
            {
                _logger.LogInformation("Running '{Name}' migration", migration.Name);
                ok = await migration.Instance.Up(session, _provider);
                _logger.LogInformation("Migration '{Name}' ran {Result}", migration.Name,
                    ok ? "successfully" : "with failure");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not Apply migration {Name}", migration.Name);
                return;
            }

            if (ok)
                await session.Run(
                    $"insert into `{MigrationsTable}` (`name`, `created`) values ('{migration.Name}', now())");
        }

        _logger.LogInformation("Migration process finished");
    }

    private async Task CreatedTableIfNotExists()
    {
        _logger.LogInformation("Try to create migrations table {Table}", MigrationsTable);
        await using var session = await _factory.GetConnection();
        var created = await session.Run($@"
                create table if not exists `{MigrationsTable}` {GetCluster(session)} (
                    `created` DateTime Default now(),
                    `name` String
                ) ENGINE = TinyLog();") > 0;

        _logger.LogInformation($"Migration table was {(created ? "" : "not ")}created");
    }


    private static string GetCluster(ClickhouseSession session)
        => string.IsNullOrWhiteSpace(session.Settings.Cluster)
            ? string.Empty
            : $"on cluster '{session.Settings.Cluster}'";
}