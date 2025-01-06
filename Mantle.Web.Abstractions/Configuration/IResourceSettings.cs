namespace Mantle.Web.Configuration;

public interface IResourceSettings : ISettings
{
    ICollection<RequiredResourceCollection> Resources { get; set; }

    IEnumerable<RequiredResource> GetResources(ResourceType type, string name);
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