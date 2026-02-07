using Volo.Abp.Modularity;

namespace CalwayPest;

[DependsOn(
    typeof(CalwayPestDomainModule),
    typeof(CalwayPestTestBaseModule)
)]
public class CalwayPestDomainTestModule : AbpModule
{

}
