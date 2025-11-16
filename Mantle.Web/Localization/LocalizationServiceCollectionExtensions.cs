using Mantle.Web.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class LocalizationServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMantleLocalization()
        {
            ArgumentNullException.ThrowIfNull(services);

            services.TryAdd(new ServiceDescriptor(typeof(IStringLocalizerFactory), typeof(MantleStringLocalizerFactory), ServiceLifetime.Scoped));
            services.TryAdd(new ServiceDescriptor(typeof(IStringLocalizer), typeof(MantleStringLocalizer), ServiceLifetime.Scoped));

            return services;
        }
    }
}