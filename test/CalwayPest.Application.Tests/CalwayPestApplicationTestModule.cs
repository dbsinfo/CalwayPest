using Volo.Abp.Modularity;

namespace CalwayPest;

[DependsOn(
    typeof(CalwayPestApplicationModule),
    typeof(CalwayPestDomainTestModule)
)]
public class CalwayPestApplicationTestModule : AbpModule
{

}
