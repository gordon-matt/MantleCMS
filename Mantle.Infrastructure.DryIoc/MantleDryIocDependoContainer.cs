using Dependo;
using Dependo.DryIoc;

namespace Mantle.Infrastructure.DryIoc;

public class MantleDryIocDependoContainer : DryIocDependoContainer
{
    /// <inheritdoc />
    public override IServiceProvider ConfigureServices(IContainer containerBuilder, IConfiguration configuration)
    {
        //find startup configurations provided by other assemblies
        var typeFinder = new WebAppTypeFinder();

        //create and sort instances of startup configurations
        var startupConfigs = typeFinder.FindClassesOfType<IMantleStartup>()
            .Where(startup => PluginManager.FindPlugin(startup)?.Installed ?? true) //ignore not installed plugins
            .Select(startup => (IMantleStartup)Activator.CreateInstance(startup))
            .OrderBy(startup => startup.Order);

        //configure services
        foreach (var startupConfig in startupConfigs)
        {
            startupConfig.ConfigureServices(new DryIocContainerBuilder(containerBuilder), configuration);
        }

        //register dependencies
        RegisterDependencies(containerBuilder, typeFinder, configuration);

        //resolve assemblies here. otherwise, plugins can throw an exception when rendering views
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

        //set App_Data path as base data directory (required to create and save SQL Server Compact database file in App_Data folder)
        AppDomain.CurrentDomain.SetData("DataDirectory", CommonHelper.MapPath("~/App_Data/"));

        return ServiceProvider;
    }

    #region Non-Public Methods

    /// <summary>
    /// Run startup tasks
    /// </summary>
    /// <param name="typeFinder">Type finder</param>
    internal virtual void RunStartupTasks()
    {
        if (!DataSettingsHelper.IsDatabaseInstalled)
        {
            return;
        }

        var typeFinder = Resolve<ITypeFinder>();

        //find startup tasks provided by other assemblies
        var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

        //create and sort instances of startup tasks
        //we startup this interface even for not installed plugins.
        //otherwise, DbContext initializers won't run and a plugin installation won't work
        var instances = startupTasks
            .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
            .OrderBy(startupTask => startupTask.Order);

        //execute tasks
        foreach (var task in instances)
        {
            task.Execute();
        }
    }

    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        //check for assembly already loaded
        var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
        if (assembly != null)
        {
            return assembly;
        }

        //get assembly from TypeFinder
        var tf = Resolve<ITypeFinder>();
        assembly = tf.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
        return assembly;
    }

    #endregion Non-Public Methods
}