namespace Mantle.Web.Messaging.Configuration;

public class GeneralSettings : BaseResourceSettings
{
    #region ISettings Members

    public override string Name => "Messaging: General Settings";

    public override bool IsTenantRestricted => false;

    public override string EditorTemplatePath => "/Views/Shared/EditorTemplates/GeneralSettings.cshtml";

    #endregion ISettings Members

    #region IResourceSettings Members

    public override ICollection<RequiredResourceCollection> Resources { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public override ICollection<RequiredResourceCollection> DefaultResources => new List<RequiredResourceCollection>
    {
        new RequiredResourceCollection
        {
            Name = "GrapesJs",
            Resources = new List<RequiredResource>
            {
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "/lib/grapesjs/dist/grapes.min.js" },
                new RequiredResource { Type = ResourceType.Stylesheet, Order = 0, Path = "/lib/grapesjs/dist/css/grapes.min.css" }
            }
        },
        new RequiredResourceCollection
        {
            Name = "GrapesJs-Aviary",
            Resources = new List<RequiredResource>
            {
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "http://feather.aviary.com/imaging/v3/editor.js" },
                new RequiredResource { Type = ResourceType.Script, Order = 1, Path = "/lib/grapesjs-aviary/dist/grapesjs-aviary.min.js" }
            }
        },
        new RequiredResourceCollection
        {
            Name = "GrapesJs-Mjml",
            Resources = new List<RequiredResource>
            {
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "/lib/grapesjs-mjml/dist/index.min.js" }
            }
        }
    };

    #endregion IResourceSettings Members
}