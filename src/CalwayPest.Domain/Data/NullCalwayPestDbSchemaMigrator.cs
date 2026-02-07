using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace CalwayPest.Data;

/* This is used if database provider does't define
 * ICalwayPestDbSchemaMigrator implementation.
 */
public class NullCalwayPestDbSchemaMigrator : ICalwayPestDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
