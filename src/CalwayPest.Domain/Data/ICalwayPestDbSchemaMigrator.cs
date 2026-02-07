using System.Threading.Tasks;

namespace CalwayPest.Data;

public interface ICalwayPestDbSchemaMigrator
{
    Task MigrateAsync();
}
