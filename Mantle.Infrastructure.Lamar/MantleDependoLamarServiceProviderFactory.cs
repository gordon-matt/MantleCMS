using Dependo;
using Dependo.Lamar;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Mantle.Infrastructure.Lamar;

public class MantleDependoLamarServiceProviderFactory : DependoLamarServiceProviderFactory
{
    /// <inheritdoc />
    public override IServiceProvider CreateServiceProvider(ServiceRegistry serviceRegistry)
    {
        ArgumentNullException.ThrowIfNull(serviceRegistry);

        if (services == null)
        {
            throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider.");
        }

        //most of API providers require TLS 1.2 nowadays
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

#pragma warning disable DF0010  // Should not be disposed here.
        var provider = services.BuildServiceProvider();
#pragma warning restore DF0010

        var hostEnvironment = provider.GetRequiredService<IHostEnvironment>();
        var pluginOptions = provider.GetRequiredService<IOptions<MantlePluginOptions>>();
        CommonHelper.BaseDirectory = hostEnvironment.ContentRootPath;

        var partManager = provider.GetService<ApplicationPartManager>();

        //initialize plugins
        PluginManager.Initialize(partManager, pluginOptions.Value);

        var configuration = provider.GetService<IConfiguration>();

#pragma warning disable DF0010 // Should not be disposed here.
        var dependoContainer = new MantleLamarDependoContainer();
#pragma warning restore DF0010

        var serviceProvider = dependoContainer.ConfigureServices(serviceRegistry, configuration);

#pragma warning disable DF0001 // Should not be disposed here.
        DependoResolver.Create(dependoContainer);
#pragma warning restore DF0001

        var infrastructureOptions = provider.GetService<IOptions<MantleInfrastructureOptions>>();
        //run startup tasks
        if (!infrastructureOptions.Value.IgnoreStartupTasks)
        {
            dependoContainer.RunStartupTasks();
        }

        //// TODO: This should not be here. Find somewhere else to put it.. problem is making sure it's after engine context has been initialized above.
        ////if (DataSettingsHelper.IsDatabaseInstalled && FrameworkConfigurationSection.Instance.ScheduledTasks.Enabled)
        ////{
        //TaskManager.Instance.Initialize();
        //TaskManager.Instance.Start();
        ////}

        return serviceProvider;
    }
}