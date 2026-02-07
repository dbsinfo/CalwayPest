using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace CalwayPest.Pages;

public class Index_Tests : CalwayPestWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
