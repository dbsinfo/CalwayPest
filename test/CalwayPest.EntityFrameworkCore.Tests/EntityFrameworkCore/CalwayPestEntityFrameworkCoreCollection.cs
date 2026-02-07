using Xunit;

namespace CalwayPest.EntityFrameworkCore;

[CollectionDefinition(CalwayPestTestConsts.CollectionDefinitionName)]
public class CalwayPestEntityFrameworkCoreCollection : ICollectionFixture<CalwayPestEntityFrameworkCoreFixture>
{

}
