using Mantle.Web.Configuration;

namespace Mantle.Plugins.Widgets.Bootstrap3;

public class Bootstrap3PluginSettings : BaseResourceSettings
{
    #region ISettings Members

    public override string Name => "Plugin: Bootsrap 3 Widgets";

    public override string EditorTemplatePath => "/Plugins/Widgets.Bootstrap3/Views/Shared/EditorTemplates/Settings.cshtml";

    #endregion ISettings Members

    public override ICollection<RequiredResourceCollection> Resources { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public override ICollection<RequiredResourceCollection> DefaultResources => new List<RequiredResourceCollection>
    {
        new RequiredResourceCollection
        {
            Name = "Bootstrap",
            Resources = new List<RequiredResource>
            {
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "https://cdn.jsdelivr.net/npm/bootstrap@3.4.1/dist/js/bootstrap.min.js" },
                new RequiredResource { Type = ResourceType.Stylesheet, Order = 0, Path = "https://cdn.jsdelivr.net/npm/bootstrap@3.4.1/dist/css/bootstrap.min.css" }
            }
        }
    };
}