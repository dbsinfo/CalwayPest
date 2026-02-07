using CalwayPest.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace CalwayPest.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(CalwayPestEntityFrameworkCoreModule),
    typeof(CalwayPestApplicationContractsModule)
    )]
public class CalwayPestDbMigratorModule : AbpModule
{
}
