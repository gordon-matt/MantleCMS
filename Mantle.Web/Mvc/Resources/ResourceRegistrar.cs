namespace Mantle.Web.Mvc.Resources;

public abstract class ResourceRegistrar
{
    private readonly IWorkContext workContext;

    protected readonly Dictionary<ResourceLocation, Dictionary<string, ResourceEntry>> resources = new();

    protected readonly Dictionary<ResourceLocation, List<string>> inlineResources = new();

    protected ResourceRegistrar(IWorkContext workContext)
    {
        this.workContext = workContext;
    }

    protected abstract string VirtualBasePath { get; }

    protected virtual string BundleBasePath => $"{VirtualBasePath}/bundles/";

    protected abstract ResourceLocation DefaultLocation { get; }

    public virtual ResourceEntry Include(
        string path,
        ResourceLocation? location = null,
        bool isThemePath = false,
        int? order = null,
        object htmlAttributes = null)
    {
        ResourceEntry resourceEntry;
        if (isThemePath)
        {
            var virtualBasePath = VirtualBasePath.TrimStart('/');
            resourceEntry = new ResourceEntry(
                $"/Themes/{workContext.CurrentTheme}/{virtualBasePath}/{path}",
                location ?? DefaultLocation);
        }
        else
        {
            resourceEntry = new ResourceEntry(
                $"{VirtualBasePath}/{path}",
                location ?? DefaultLocation);
        }
        RegisterResource(resourceEntry);

        if (order.HasValue)
        {
            resourceEntry.Order = order.Value;
        }

        resourceEntry.HtmlAttributes = htmlAttributes;

        return resourceEntry;
    }

    public virtual string GetBundleUrl(string bundleName) => string.Concat(BundleBasePath, bundleName);

    public virtual ResourceEntry IncludeBundle(
        string bundleName,
        ResourceLocation? location = null,
        int? order = null,
        object htmlAttributes = null)
    {
        string bundleUrl = GetBundleUrl(bundleName);

        //if (!string.IsNullOrEmpty(bundleUrl))
        //{
        var resourceEntry = new ResourceEntry(bundleUrl, location ?? DefaultLocation);
        RegisterResource(resourceEntry);

        if (order.HasValue)
        {
            resourceEntry.Order = order.Value;
        }

        resourceEntry.HtmlAttributes = htmlAttributes;

        return resourceEntry;
        //}

        //throw new UnregisteredBundleException(bundleUrl);
    }

    public void IncludeExternal(
        string path,
        ResourceLocation? location = null,
        int? order = null,
        object htmlAttributes = null)
    {
        if (order.HasValue)
        {
            RegisterResource(new ResourceEntry(path, location ?? DefaultLocation)
            {
                Order = order.Value,
                HtmlAttributes = htmlAttributes
            });
        }
        else
        {
            RegisterResource(new ResourceEntry(path, location ?? DefaultLocation)
            {
                HtmlAttributes = htmlAttributes
            });
        }
    }

    public virtual void IncludeInline(
        string code,
        ResourceLocation? location = null,
        bool ignoreIfExists = false)
    {
        RegisterInlineResource(location ?? DefaultLocation, code, ignoreIfExists);
    }

    public virtual IHtmlContent Render(ResourceLocation location)
    {
        var resources = GetResources(location);

        var inlineResources = GetInlineResources(location);

        if (!resources.Any() && !inlineResources.Any())
        {
            return null;
        }

        var sb = new StringBuilder();

        foreach (var resource in resources)
        {
            sb.AppendLine(BuildResource(resource));
        }

        if (inlineResources.Any())
        {
            sb.Append(BuildInlineResources(inlineResources));
        }

        // Clear the resources, now that they've been rendered (to prevent them being rendered again on different all other pages)
        // in which case, we can also use old method (use script/style registrars directly instead of via HTML Helpers)
        ClearResources(location);

        return new HtmlString(sb.ToString());
    }

    protected abstract string BuildInlineResources(IEnumerable<string> resources);

    protected abstract string BuildResource(ResourceEntry resource);

    protected virtual void RegisterResource(ResourceEntry resourceEntry)
    {
        if (!resources.ContainsKey(resourceEntry.Location))
        {
            resources.Add(resourceEntry.Location, new Dictionary<string, ResourceEntry>(StringComparer.InvariantCultureIgnoreCase));
        }

        if (!resources[resourceEntry.Location].ContainsKey(resourceEntry.Path))
        {
            resources[resourceEntry.Location].Add(resourceEntry.Path, resourceEntry);
        }
    }

    protected virtual void RegisterInlineResource(ResourceLocation location, string code, bool ignoreIfExists = false)
    {
        if (!inlineResources.ContainsKey(location))
        {
            inlineResources.Add(location, new List<string>());
        }

        if (string.IsNullOrEmpty(code))
        {
            return;
        }

        if (ignoreIfExists)
        {
            if (!inlineResources[location].Contains(code))
            {
                inlineResources[location].Add(code);
            }
        }
        else
        {
            inlineResources[location].Add(code);
        }
    }

    protected virtual IEnumerable<ResourceEntry> GetResources(ResourceLocation location)
    {
        if (!resources.ContainsKey(location))
        {
            return Enumerable.Empty<ResourceEntry>();
        }

        return resources[location]
            .OrderBy(x => x.Value.Order)
            .Select(x => x.Value);
    }

    protected virtual IEnumerable<string> GetInlineResources(ResourceLocation location)
    {
        if (!inlineResources.ContainsKey(location))
        {
            return Enumerable.Empty<string>();
        }

        return inlineResources[location];
    }

    protected virtual void ClearResources(ResourceLocation location)
    {
        if (resources.ContainsKey(location))
        {
            resources[location].Clear();
        }

        if (inlineResources.ContainsKey(location))
        {
            inlineResources[location].Clear();
        }
    }
}