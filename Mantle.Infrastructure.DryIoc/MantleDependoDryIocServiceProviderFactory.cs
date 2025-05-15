using Dependo;
using Dependo.DryIoc;
using DryIoc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Mantle.Infrastructure.DryIoc;

public class MantleDependoDryIocServiceProviderFactory : DependoDryIocServiceProviderFactory
{
    /// <inheritdoc />
    public virtual IServiceProvider CreateServiceProvider(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);

        if (_services == null)
        {
            throw new InvalidOperationException("CreateBuilder must be called before CreateServiceProvider.");
        }

        //most of API providers require TLS 1.2 nowadays
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //set base application path
        var provider = _services.BuildServiceProvider();
        var hostEnvironment = provider.GetRequiredService<IHostEnvironment>();
        var pluginOptions = provider.GetRequiredService<IOptions<MantlePluginOptions>>();
        CommonHelper.BaseDirectory = hostEnvironment.ContentRootPath;

        var partManager = provider.GetService<ApplicationPartManager>();

        //initialize plugins
        PluginManager.Initialize(partManager, pluginOptions.Value);

        var configuration = provider.GetService<IConfiguration>();

#pragma warning disable DF0010 // Should not be disposed here.
        var engine = new MantleDryIocDependoContainer();
#pragma warning restore DF0010

        var serviceProvider = engine.ConfigureServices(container, configuration);

#pragma warning disable DF0001 // Should not be disposed here.
        DependoResolver.Create(engine);
#pragma warning restore DF0001

        var infrastructureOptions = provider.GetService<IOptions<MantleInfrastructureOptions>>();
        //run startup tasks
        if (!infrastructureOptions.Value.IgnoreStartupTasks)
        {
            engine.RunStartupTasks();
        }

        //// TODO: This should not be here. Find somewhere else to put it.. problem is making sure it's after engine context has been initialized above.
        ////if (DataSettingsHelper.IsDatabaseInstalled && FrameworkConfigurationSection.Instance.ScheduledTasks.Enabled)
        ////{
        //TaskManager.Instance.Initialize();
        //TaskManager.Instance.Start();
        ////}

        return serviceProvider; // DryIocServiceProvider
    }
}