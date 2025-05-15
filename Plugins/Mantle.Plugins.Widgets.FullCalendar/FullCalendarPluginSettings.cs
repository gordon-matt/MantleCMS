namespace Mantle.Plugins.Widgets.FullCalendar;

public class FullCalendarPluginSettings : BaseResourceSettings
{
    #region ISettings Members

    public override string Name => "Plugin: Full Calendar";

    public override string EditorTemplatePath => "/Plugins/Widgets.FullCalendar/Views/Shared/EditorTemplates/Settings.cshtml";

    #endregion ISettings Members

    public override ICollection<RequiredResourceCollection> Resources { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public override ICollection<RequiredResourceCollection> DefaultResources =>
    [
        new RequiredResourceCollection
        {
            Name = "FullCalendar",
            Resources =
            [
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "https://cdn.jsdelivr.net/npm/moment@2.29.4/moment.min.js" },
                new RequiredResource { Type = ResourceType.Script, Order = 1, Path = "/Plugins/Widgets.FullCalendar/wwwroot/js/fullcalendar-2.3.1/fullcalendar.min.js" },
                new RequiredResource { Type = ResourceType.Stylesheet, Order = 0, Path = "/Plugins/Widgets.FullCalendar/wwwroot/css/fullcalendar.min.css" },
                new RequiredResource { Type = ResourceType.Stylesheet, Order = 1, Path = "/Plugins/Widgets.FullCalendar/wwwroot/css/fullcalendar.print.css" }
            ]
        }
    ];
}