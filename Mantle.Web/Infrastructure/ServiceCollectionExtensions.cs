using Mantle.Infrastructure.Configuration;
using Mantle.Plugins.Configuration;
using Mantle.Tasks.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void ConfigureMantleOptions(this IServiceCollection services, IConfigurationRoot configuration)
    {
        services.Configure<MantleInfrastructureOptions>(configuration.GetSection(nameof(MantleInfrastructureOptions)))
            .Configure<MantlePluginOptions>(configuration.GetSection(nameof(MantlePluginOptions)))
            .Configure<MantleTasksOptions>(configuration.GetSection(nameof(MantleTasksOptions)))
            .Configure<MantleWebOptions>(configuration.GetSection(nameof(MantleWebOptions)))
            .Configure<WebOptimizerOptions>(configuration.GetSection(nameof(WebOptimizerOptions)));
    }

    /// <summary>
    /// Adds WebOptimizer to the specified <see cref="IServiceCollection"/> and enables CSS and JavaScript minification.
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static void AddMantleWebOptimizer(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var webOptimizerOptions = configuration.GetSection("WebOptimizerOptions") as WebOptimizerOptions ?? new WebOptimizerOptions();

        //add minification & bundling
        var cssSettings = new CssBundlingSettings
        {
            FingerprintUrls = false,
            Minify = webOptimizerOptions.EnableCssBundling
        };

        var codeSettings = new CodeBundlingSettings
        {
            Minify = webOptimizerOptions.EnableJavaScriptBundling,
            AdjustRelativePaths = false //disable this feature because it breaks function names that have "Url(" at the end
        };

        services.AddWebOptimizer(null, cssSettings, codeSettings);
    }
}