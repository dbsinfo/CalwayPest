using CalwayPest.Samples;
using Xunit;

namespace CalwayPest.EntityFrameworkCore.Domains;

[Collection(CalwayPestTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<CalwayPestEntityFrameworkCoreTestModule>
{

}
