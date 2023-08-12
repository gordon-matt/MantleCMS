using Mantle.Web.Configuration;

namespace Mantle.Web.ContentManagement.Areas.Admin.Blog;

public class BlogSettings : BaseResourceSettings
{
    public BlogSettings()
    {
        DateFormat = "YYYY-MM-DD HH:mm:ss";
        ItemsPerPage = 5;
        PageTitle = "Blog";
        ShowOnMenus = true;
        MenuPosition = 0;
    }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.PageTitle)]
    [SettingsProperty("Blog")]
    public string PageTitle { get; set; }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.DateFormat)]
    [SettingsProperty("YYYY-MM-DD HH:mm:ss")]
    public string DateFormat { get; set; }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.ItemsPerPage)]
    [SettingsProperty(5)]
    public byte ItemsPerPage { get; set; }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.ShowOnMenus)]
    [SettingsProperty(true)]
    public bool ShowOnMenus { get; set; }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.MenuPosition)]
    [SettingsProperty(0)]
    public byte MenuPosition { get; set; }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.AccessRestrictions)]
    [SettingsProperty(
        Declaration = "viewModel.accessRestrictions = null; viewModel.roles = ko.observableArray([]);",
        Assignment =
@"if (data.AccessRestrictions) {
	viewModel.accessRestrictions = ko.mapping.fromJSON(data.AccessRestrictions);
	if (viewModel.accessRestrictions.Roles != null) {
		const split = viewModel.accessRestrictions.Roles().split(',');
		viewModel.roles(split);
	}
}",
        CleanUp = "delete viewModel.accessRestrictions; delete viewModel.roles;",
        Save = "AccessRestrictions: JSON.stringify({ Roles: viewModel.roles().join() })")]
    public string AccessRestrictions { get; set; }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.Settings.Blog.LayoutPathOverride)]
    [SettingsProperty]
    public string LayoutPathOverride { get; set; }

    #region ISettings Members

    public override string Name => "CMS: Blog Settings";

    public override string EditorTemplatePath => "/Areas/Admin/Blog/Views/Shared/EditorTemplates/BlogSettings.cshtml";

    #endregion ISettings Members

    public override ICollection<RequiredResourceCollection> Resources { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public override ICollection<RequiredResourceCollection> DefaultResources => new List<RequiredResourceCollection>
    {
        new RequiredResourceCollection
        {
            Name = "Bootpag",
            Resources = new List<RequiredResource>
            {
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "https://cdn.jsdelivr.net/npm/bootpag@1.0.7/lib/jquery.bootpag.min.js" }
            }
        },
        new RequiredResourceCollection
        {
            Name = "jQCloud",
            Resources = new List<RequiredResource>
            {
                new RequiredResource { Type = ResourceType.Script, Order = 0, Path = "https://cdn.jsdelivr.net/npm/jqcloud2@2.0.3/dist/jqcloud.min.js" },
                new RequiredResource { Type = ResourceType.Stylesheet, Order = 0, Path = "https://cdn.jsdelivr.net/npm/jqcloud2@2.0.3/dist/jqcloud.min.css" }
            }
        }
    };
}