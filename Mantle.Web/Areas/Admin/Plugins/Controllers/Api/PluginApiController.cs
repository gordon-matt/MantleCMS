namespace Mantle.Web.Areas.Admin.Plugins.Controllers.Api;

public class PluginApiController : ODataController
{
    private readonly IPluginFinder pluginFinder;
    private readonly ILogger logger;

    public PluginApiController(
        IPluginFinder pluginFinder,
        ILoggerFactory loggerFactory)
    {
        this.pluginFinder = pluginFinder;
        this.logger = loggerFactory.CreateLogger<PluginApiController>();
    }

    public virtual IEnumerable<EdmPluginDescriptor> Get(ODataQueryOptions<EdmPluginDescriptor> options)
    {
        if (!CheckPermission(StandardPermissions.FullAccess))
        {
            return Enumerable.Empty<EdmPluginDescriptor>().AsQueryable();
        }

        var query = pluginFinder.GetPluginDescriptors(LoadPluginsMode.All).Select(x => (EdmPluginDescriptor)x).AsQueryable();
        var results = options.ApplyTo(query);
        return (results as IQueryable<EdmPluginDescriptor>).ToHashSet();
    }

    [EnableQuery]
    public virtual SingleResult<EdmPluginDescriptor> Get([FromODataUri] string key)
    {
        if (!CheckPermission(StandardPermissions.FullAccess))
        {
            return SingleResult.Create(Enumerable.Empty<EdmPluginDescriptor>().AsQueryable());
        }

        string systemName = key.Replace('-', '.');
        var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
        EdmPluginDescriptor entity = pluginDescriptor;
        return SingleResult.Create(new[] { entity }.AsQueryable());
    }

    public virtual IActionResult Put([FromODataUri] string key, EdmPluginDescriptor entity)
    {
        if (!CheckPermission(StandardPermissions.FullAccess))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string systemName = key.Replace('-', '.');
            var pluginDescriptor = pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);

            if (pluginDescriptor == null)
            {
                return NotFound();
            }

            pluginDescriptor.FriendlyName = entity.FriendlyName;
            pluginDescriptor.DisplayOrder = entity.DisplayOrder;
            pluginDescriptor.LimitedToTenants.Clear();

            if (!entity.LimitedToTenants.IsNullOrEmpty())
            {
                pluginDescriptor.LimitedToTenants = entity.LimitedToTenants.ToList();
            }

            PluginManager.SavePluginDescriptor(pluginDescriptor);
        }
        catch (Exception x)
        {
            logger.LogError(new EventId(), x.Message, x);
        }

        return Updated(entity);
    }

    protected static bool CheckPermission(Permission permission)
    {
        var authorizationService = EngineContext.Current.Resolve<IMantleAuthorizationService>();
        var workContext = EngineContext.Current.Resolve<IWorkContext>();
        return authorizationService.TryCheckAccess(permission, workContext.CurrentUser);
    }
}