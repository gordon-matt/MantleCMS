namespace Mantle.Web.Infrastructure;

public class RequireJSConfigProvider : IRequireJSConfigProvider
{
    public RequireJSConfigProvider(SiteSettings siteSettings)
    {
        Paths.Add("mantle-translations", "/admin/localization/localizable-strings/translations");

        var script = siteSettings.GetResources(ResourceType.Script, "Bootstrap-FileInput").First();
        Paths.Add("bootstrap-fileinput", script.Path.EndsWith(".js") ? script.Path[..^3] : script.Path);
    }

    public IDictionary<string, string> Paths { get; private set; } = new Dictionary<string, string>();

    public IDictionary<string, string[]> Shim => new Dictionary<string, string[]>();
}