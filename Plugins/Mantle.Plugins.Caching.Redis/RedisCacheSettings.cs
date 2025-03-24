using System.ComponentModel.DataAnnotations;
using Mantle.Localization.ComponentModel;
using Mantle.Web.Configuration;

namespace Mantle.Plugins.Caching.Redis;

public class RedisCacheSettings : BaseSettings
{
    [Required]
    [LocalizedDisplayName(LocalizableStrings.Settings.ConnectionString)]
    [SettingsProperty("localhost,allowAdmin=true")]
    public string ConnectionString { get; set; }

    #region ISettings Members

    public override string Name => "Plugin: Redis Cache Settings";

    public override string EditorTemplatePath => "/Plugins/Caching.Redis/Views/Shared/EditorTemplates/RedisCacheSettings.cshtml";

    #endregion ISettings Members
}