using System;
using System.Collections.Generic;
using System.Text;
using CalwayPest.Localization;
using Volo.Abp.Application.Services;

namespace CalwayPest;

/* Inherit your application services from this class.
 */
public abstract class CalwayPestAppService : ApplicationService
{
    protected CalwayPestAppService()
    {
        LocalizationResource = typeof(CalwayPestResource);
    }
}
