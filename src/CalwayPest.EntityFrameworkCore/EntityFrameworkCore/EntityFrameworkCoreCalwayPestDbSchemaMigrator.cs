using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CalwayPest.Data;
using Volo.Abp.DependencyInjection;

namespace CalwayPest.EntityFrameworkCore;

public class EntityFrameworkCoreCalwayPestDbSchemaMigrator
    : ICalwayPestDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreCalwayPestDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the CalwayPestDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<CalwayPestDbContext>()
            .Database
            .MigrateAsync();
    }
}
