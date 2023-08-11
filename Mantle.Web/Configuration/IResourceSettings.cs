namespace Mantle.Web.Configuration;

public interface IResourceSettings : ISettings
{
    ICollection<RequiredResourceCollection> Resources { get; set; }

    IEnumerable<RequiredResource> GetResources(ResourceType type, string name);
}

public abstract class BaseResourceSettings : BaseSettings, IResourceSettings
{
    public BaseResourceSettings()
    {
        Resources = DefaultResources;
    }

    public override bool IsTenantRestricted => false;

    public abstract ICollection<RequiredResourceCollection> Resources { get; set; }

    public abstract ICollection<RequiredResourceCollection> DefaultResources { get; }

    public IEnumerable<RequiredResource> GetResources(ResourceType type, string name) =>
        Resources.FirstOrDefault(r => r.Name == name)?.Resources.Where(x => x.Type == type).OrderBy(x => x.Order);
}

public class RequiredResourceCollection
{
    public string Name { get; set; }

    public ICollection<RequiredResource> Resources { get; set; } = new List<RequiredResource>();
}

public class RequiredResource
{
    public ResourceType Type { get; set; }

    public string Path { get; set; }

    public int Order { get; set; }
}

public enum ResourceType : byte
{
    Script = 0,
    Stylesheet = 1
}