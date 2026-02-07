using Volo.Abp.Modularity;

namespace CalwayPest;

/* Inherit from this class for your domain layer tests. */
public abstract class CalwayPestDomainTestBase<TStartupModule> : CalwayPestTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
