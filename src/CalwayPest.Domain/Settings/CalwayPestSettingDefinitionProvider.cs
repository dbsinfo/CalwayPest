using Volo.Abp.Settings;

namespace CalwayPest.Settings;

public class CalwayPestSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(CalwayPestSettings.MySetting1));
    }
}
