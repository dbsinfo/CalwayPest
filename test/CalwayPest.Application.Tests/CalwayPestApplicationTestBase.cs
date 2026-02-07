using Volo.Abp.Modularity;

namespace CalwayPest;

public abstract class CalwayPestApplicationTestBase<TStartupModule> : CalwayPestTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
