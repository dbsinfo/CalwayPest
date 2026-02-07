using Microsoft.AspNetCore.Builder;
using CalwayPest;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();

builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("CalwayPest.Web.csproj");
await builder.RunAbpModuleAsync<CalwayPestWebTestModule>(applicationName: "CalwayPest.Web" );

public partial class Program
{
}
