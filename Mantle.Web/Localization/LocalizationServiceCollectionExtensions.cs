using System;
using Mantle.Web.Localization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddMantleLocalization(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(new ServiceDescriptor(typeof(IStringLocalizerFactory), typeof(MantleStringLocalizerFactory), ServiceLifetime.Scoped));
            services.TryAdd(new ServiceDescriptor(typeof(IStringLocalizer), typeof(MantleStringLocalizer), ServiceLifetime.Scoped));

            return services;
        }
    }
}