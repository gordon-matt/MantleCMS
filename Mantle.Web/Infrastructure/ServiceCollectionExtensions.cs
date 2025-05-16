using Mantle.Infrastructure.Configuration;
using Mantle.Plugins.Configuration;
using Mantle.Tasks.Configuration;
using Mantle.Web.Mvc.EmbeddedResources;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds WebOptimizer to the specified <see cref="IServiceCollection"/> and enables CSS and JavaScript minification.
    /// </summary>
    /// <param name="services">Collection of service descriptors</param>
    public static IServiceCollection AddMantleWebOptimizer(this IServiceCollection services, IConfigurationRoot configuration)
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

        return services.AddWebOptimizer(null, cssSettings, codeSettings);
    }

    public static IServiceCollection ConfigureMantleEmbeddedFileProviders(this IServiceCollection services) =>
        services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
        {
            options.FileProviders.Add(new EmbeddedViewFileProvider());

            var embeddedFileProviders = DependoResolver.Instance.ResolveAll<IEmbeddedFileProviderRegistrar>()
                .SelectMany(x => x.EmbeddedFileProviders);

            foreach (var embeddedFileProvider in embeddedFileProviders)
            {
                options.FileProviders.Add(embeddedFileProvider);
            }
        });

    public static IServiceCollection ConfigureMantleOptions(this IServiceCollection services, IConfigurationRoot configuration) => services
        .Configure<MantleInfrastructureOptions>(configuration.GetSection(nameof(MantleInfrastructureOptions)))
        .Configure<MantlePluginOptions>(configuration.GetSection(nameof(MantlePluginOptions)))
        .Configure<MantleTasksOptions>(configuration.GetSection(nameof(MantleTasksOptions)))
        .Configure<MantleWebOptions>(configuration.GetSection(nameof(MantleWebOptions)))
        .Configure<WebOptimizerOptions>(configuration.GetSection(nameof(WebOptimizerOptions)));
}