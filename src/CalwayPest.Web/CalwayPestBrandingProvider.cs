using Microsoft.Extensions.Localization;
using CalwayPest.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace CalwayPest.Web;

[Dependency(ReplaceServices = true)]
public class CalwayPestBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<CalwayPestResource> _localizer;

    public CalwayPestBrandingProvider(IStringLocalizer<CalwayPestResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
