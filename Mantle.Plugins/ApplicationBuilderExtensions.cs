using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace Mantle.Plugins;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMantlePlugins(this IApplicationBuilder app) =>
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Plugins")),
            RequestPath = new PathString("/Plugins"),
            OnPrepareResponse = ctx => ctx.Context.Response.Headers.Append(HeaderNames.CacheControl, "public,max-age=604800")
        });
}