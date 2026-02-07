using CalwayPest.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CalwayPest.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class CalwayPestController : AbpControllerBase
{
    protected CalwayPestController()
    {
        LocalizationResource = typeof(CalwayPestResource);
    }
}
