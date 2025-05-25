using System.Reflection;
using Dependo;
using LightInject;
using Mantle.Web.Configuration;
using Mantle.Web.Configuration.Services;

namespace Mantle.Web.Infrastructure.LightInject;

public static class ConfigurationModule
{
    public static IServiceContainer RegisterSettings(this IServiceContainer container, ITypeFinder typeFinder)
    {
        // Get the method info FIRST with exact parameter types
        var methodInfo = typeof(ConfigurationModule)
            .GetMethod(
                nameof(GetSettingsGeneric),
                BindingFlags.Static | BindingFlags.NonPublic,
                null,
                [typeof(IWorkContext), typeof(ISettingService)],
                null)
            ?? throw new InvalidOperationException("GetSettingsGeneric method not found. Check method signature.");

        var settingsTypes = typeFinder.FindClassesOfType<ISettings>(onlyConcreteClasses: true);

        foreach (var settingsType in settingsTypes)
        {
            container.Register(settingsType, factory =>
            {
                var workContext = factory.GetInstance<IWorkContext>();
                var settingService = factory.GetInstance<ISettingService>();

                return methodInfo.MakeGenericMethod(settingsType)
                    .Invoke(null, [workContext, settingService]);
            }, new PerScopeLifetime());
        }

        return container;
    }

    // Make sure this EXACTLY matches what we're searching for
    private static TSettings GetSettingsGeneric<TSettings>(
        IWorkContext workContext,
        ISettingService settingService)
        where TSettings : ISettings, new()
    {
        int tenantId = workContext.CurrentTenant.Id;
        return settingService.GetSettings<TSettings>(tenantId);
    }
}