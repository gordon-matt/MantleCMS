using Mantle.Web.Mvc.EmbeddedResources;
using Mantle.Web.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using WebOptimizer;

namespace Mantle.Web.Infrastructure;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMantleWebEmbeddedFileProviders(this WebApplication app) =>
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new CompositeFileProvider(
                new EmbeddedScriptFileProvider(),
                new EmbeddedContentFileProvider(),
                app.Environment.WebRootFileProvider),
            OnPrepareResponse = (context) =>
            {
                var headers = context.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    MaxAge = TimeSpan.FromDays(7)
                };
            }
        });

    /// <summary>
    /// Adds WebOptimizer to the <see cref="IApplicationBuilder"/> request execution pipeline
    /// </summary>
    /// <param name="app">Builder for configuring an application's request pipeline</param>
    public static IApplicationBuilder UseMantleWebOptimizer(this IApplicationBuilder app)
    {
        var webHostEnvironment = DependoResolver.Instance.Resolve<IWebHostEnvironment>();

        return app.UseWebOptimizer(webHostEnvironment, new[]
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

    public static IApplicationBuilder MapMantleEndpoints(this WebApplication app)
    {
        var routePublisher = app.Services.GetRequiredService<IRoutePublisher>();
        routePublisher.RegisterEndpoints(app);
        return app;
    }
}