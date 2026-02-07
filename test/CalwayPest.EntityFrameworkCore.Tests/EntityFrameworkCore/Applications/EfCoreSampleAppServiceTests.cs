using CalwayPest.Samples;
using Xunit;

namespace CalwayPest.EntityFrameworkCore.Applications;

[Collection(CalwayPestTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<CalwayPestEntityFrameworkCoreTestModule>
{

}
