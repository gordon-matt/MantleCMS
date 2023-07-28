namespace Mantle.Web.Mvc.EmbeddedResources;

public class EmbeddedResourceTable
{
    private static readonly object _lock = new object();
    private readonly Dictionary<string, EmbeddedResourceMetadata> resourceCache;

    public EmbeddedResourceTable()
    {
        resourceCache = new Dictionary<string, EmbeddedResourceMetadata>(StringComparer.OrdinalIgnoreCase);
    }

    public void AddResource(string resourceName, string assemblyName)
    {
        lock (_lock)
        {
            resourceCache[resourceName] = new EmbeddedResourceMetadata
            {
                ResourceName = resourceName,
                AssemblyFullName = assemblyName
            };
        }
    }

    public IEnumerable<EmbeddedResourceMetadata> Resources
    {
        get { return resourceCache.Values; }
    }

    public bool ContainsEmbeddedResource(string fullyQualifiedName)
    {
        var resource = FindEmbeddedResource(fullyQualifiedName);
        return (resource != null);
    }

    public EmbeddedResourceMetadata FindEmbeddedResource(string fullyQualifiedName)
    {
        if (string.IsNullOrEmpty(fullyQualifiedName))
        {
            return null;
        }

        lock (_lock)
        {
            return Resources.SingleOrDefault(x => x.ResourceName.ToLowerInvariant().Equals(fullyQualifiedName.ToLowerInvariant()));
        }
    }
}