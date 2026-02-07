using CalwayPest.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace CalwayPest.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class CalwayPestPageModel : AbpPageModel
{
    protected CalwayPestPageModel()
    {
        LocalizationResourceType = typeof(CalwayPestResource);
    }
}
