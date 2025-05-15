using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Mantle.Web.Configuration;
using Mantle.Web.Configuration.Services;
using Module = Autofac.Module;

namespace Mantle.Web.Infrastructure.Autofac;

public class ConfigurationModule : Module
{
    protected override void Load(ContainerBuilder builder) => builder.RegisterSource(new SettingsSource());

    private class SettingsSource : IRegistrationSource
    {
        private static readonly MethodInfo BuildMethod = typeof(SettingsSource)
            .GetMethod(nameof(BuildRegistration), BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            if (service is not TypedService typedService) yield break;

            var typeInfo = typedService.ServiceType.GetTypeInfo();
            if (typeInfo.IsClass && !typeInfo.IsAbstract && typeof(ISettings).IsAssignableFrom(typedService.ServiceType))
            {
                var registration = (IComponentRegistration)BuildMethod!
                    .MakeGenericMethod(typedService.ServiceType)
                    .Invoke(null, null);
                yield return registration;
            }
        }

        internal static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new() =>
            RegistrationBuilder
                .ForDelegate((c, p) =>
                {
                    int tenantId = c.Resolve<IWorkContext>().CurrentTenant.Id;
                    return c.Resolve<ISettingService>().GetSettings<TSettings>(tenantId);
                })
                .InstancePerLifetimeScope()
                .CreateRegistration();

        public bool IsAdapterForIndividualComponents => false;
    }
}