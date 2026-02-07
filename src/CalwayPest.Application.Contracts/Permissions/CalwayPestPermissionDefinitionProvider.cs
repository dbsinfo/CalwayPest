using CalwayPest.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace CalwayPest.Permissions;

public class CalwayPestPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(CalwayPestPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(CalwayPestPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CalwayPestResource>(name);
    }
}
