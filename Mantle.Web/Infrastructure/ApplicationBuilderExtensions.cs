using Microsoft.Extensions.FileProviders;
using WebOptimizer;

namespace Mantle.Web.Infrastructure;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds WebOptimizer to the <see cref="IApplicationBuilder"/> request execution pipeline
    /// </summary>
    /// <param name="application">Builder for configuring an application's request pipeline</param>
    public static void UseMantleWebOptimizer(this IApplicationBuilder application)
    {
        var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();

        application.UseWebOptimizer(webHostEnvironment, new[]
        {
            new FileProviderOptions
            {
                RequestPath =  new PathString("/Plugins"),
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Plugins"))
            },
            new FileProviderOptions
            {
                RequestPath =  new PathString("/Themes"),
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Themes"))
            }
        });
    }
}