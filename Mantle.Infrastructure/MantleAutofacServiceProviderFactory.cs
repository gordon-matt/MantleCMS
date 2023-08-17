using Microsoft.Extensions.Options;

namespace Mantle.Infrastructure;

public class MantleAutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
{
    private readonly Action<ContainerBuilder> configurationAction;
    private IServiceCollection services;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutofacServiceProviderFactory"/> class.
    /// </summary>
    /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the conatiner.</param>
    public MantleAutofacServiceProviderFactory(Action<ContainerBuilder> configurationAction = null)
    {
        this.configurationAction = configurationAction ?? (builder => { });
    }

    /// <summary>
    /// Creates a container builder from an <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
    public ContainerBuilder CreateBuilder(IServiceCollection services)
    {
        this.services = services; // To use in CreateServiceProvider()

        var builder = new ContainerBuilder();
        builder.Populate(services);
        configurationAction(builder);
        return builder;
    }

    /// <summary>
    /// Creates an <see cref="IServiceProvider" /> from the container builder.
    /// </summary>
    /// <param name="containerBuilder">The container builder.</param>
    /// <returns>An <see cref="IServiceProvider" />.</returns>
    public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
    {
        if (containerBuilder == null)
        {
            throw new ArgumentNullException(nameof(containerBuilder));
        }

        //most of API providers require TLS 1.2 nowadays
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //set base application path
        var provider = services.BuildServiceProvider();
        var hostingEnvironment = provider.GetRequiredService<IHostingEnvironment>();
        var pluginOptions = provider.GetRequiredService<IOptions<MantlePluginOptions>>();
        CommonHelper.BaseDirectory = hostingEnvironment.ContentRootPath;

        var partManager = provider.GetService<ApplicationPartManager>();

        //initialize plugins
        PluginManager.Initialize(partManager, hostingEnvironment, pluginOptions.Value);

        var configuration = provider.GetService<IConfiguration>();

        var engine = new AutofacEngine();
        var serviceProvider = engine.ConfigureServices(containerBuilder, configuration);
        EngineContext.Create(engine);

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

        return serviceProvider; // AutofacServiceProvider
    }
}