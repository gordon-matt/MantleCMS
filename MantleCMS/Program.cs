using Mantle.Infrastructure.Autofac;
using NLog.Web;

namespace MantleCMS;

public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Trace);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseIISIntegration();
                webBuilder.UseStartup<Startup>();
                webBuilder.UseNLog();
            })
            .UseServiceProviderFactory(new MantleDependoAutofacServiceProviderFactory())
            .UseNLog();
}