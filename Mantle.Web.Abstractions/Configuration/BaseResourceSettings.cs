namespace Mantle.Web.Configuration;

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