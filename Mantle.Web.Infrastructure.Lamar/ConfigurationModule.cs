using System.Reflection;
using Dependo;
using Mantle.Web.Configuration;
using Mantle.Web.Configuration.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure.Lamar;

public static class ConfigurationModule
{
    public static IServiceCollection RegisterSettings(this IServiceCollection services, ITypeFinder typeFinder)
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
            services.AddScoped(settingsType, provider =>
            {
                var workContext = provider.GetRequiredService<IWorkContext>();
                var settingService = provider.GetRequiredService<ISettingService>();

                return methodInfo.MakeGenericMethod(settingsType)
                    .Invoke(null, [workContext, settingService]);
            });
        }

        return services;
    }

    // Make sure this EXACTLY matches what we're searching for
    private static TSettings GetSettingsGeneric<TSettings>(
        IWorkContext workContext,
        ISettingService settingService)
        where TSettings : ISettings, new()
    {
        var tenantId = workContext.CurrentTenant.Id;
        return settingService.GetSettings<TSettings>(tenantId);
    }
}