using System.Reflection;
using Dependo;
using DryIoc;
using Mantle.Web.Configuration;
using Mantle.Web.Configuration.Services;

namespace Mantle.Web.Infrastructure.DryIoc;

public static class ConfigurationModule
{
    public static void RegisterSettings(this IRegistrator registrator, ITypeFinder typeFinder)
    {
        var settingsTypes = typeFinder.FindClassesOfType<ISettings>();
        foreach (var settingsType in settingsTypes)
        {
            registrator.RegisterDelegate(
                settingsType,
                r => GetSettingsDynamic(r, settingsType),
                setup: Setup.With(asResolutionCall: true)
            );
        }
    }

    private static object GetSettingsDynamic(IResolverContext resolver, Type settingsType)
    {
        var method = typeof(ConfigurationModule)
            .GetMethod(nameof(GetSettingsGeneric), BindingFlags.Static | BindingFlags.NonPublic)
            .MakeGenericMethod(settingsType);

        using var scope = resolver.OpenScope();
        var workContext = scope.Resolve<IWorkContext>();
        var settingService = scope.Resolve<ISettingService>();

        return method.Invoke(null, [workContext, settingService]);
    }

    private static TSettings GetSettingsGeneric<TSettings>(
        IWorkContext workContext,
        ISettingService settingService)
        where TSettings : ISettings, new()
    {
        int tenantId = workContext.CurrentTenant.Id;
        return settingService.GetSettings<TSettings>(tenantId);
    }
}