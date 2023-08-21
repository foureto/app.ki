using System.Reflection;

namespace App.Ki.Clickhouse;

public interface IMigrationService
{
    Task Migrate(IEnumerable<Assembly> assemblies, bool ensureCreated = true);
}