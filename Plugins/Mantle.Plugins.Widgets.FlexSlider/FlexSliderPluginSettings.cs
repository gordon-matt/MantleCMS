using Mantle.Web.Configuration;

namespace Mantle.Plugins.Widgets.FlexSlider;

public class FlexSliderPluginSettings : BaseResourceSettings
{
    #region ISettings Members

    public override string Name => "Plugin: Flex Slider";

    public override string EditorTemplatePath => "/Plugins/Widgets.FlexSlider/Views/Shared/EditorTemplates/Settings.cshtml";

    #endregion ISettings Members

    public override ICollection<RequiredResourceCollection> Resources { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public override ICollection<RequiredResourceCollection> DefaultResources =>
    [
        new RequiredResourceCollection
        {
            Name = "FlexSlider",
            Resources =
            [
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "https://cdn.jsdelivr.net/npm/flexslider@2.7.2/demo/js/jquery.flexslider.js" },
                new RequiredResource { Type = ResourceType.Stylesheet, Order = 0, Path = "https://cdn.jsdelivr.net/npm/flexslider@2.7.2/demo/css/flexslider.css" }
            ]
        }
    ];
}