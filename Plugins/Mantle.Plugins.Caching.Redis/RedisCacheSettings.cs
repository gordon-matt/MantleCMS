using Mantle.Localization.ComponentModel;
using Mantle.Web.Configuration;
using System.ComponentModel.DataAnnotations;

namespace Mantle.Plugins.Caching.Redis
{
    public class RedisCacheSettings : ISettings
    {
        [Required]
        [LocalizedDisplayName(LocalizableStrings.Settings.ConnectionString)]
        public string ConnectionString { get; set; }

        #region ISettings Members

        public string Name => "Redis Cache Settings";

        public bool IsTenantRestricted => false;

        public string EditorTemplatePath => "Mantle.Plugins.Caching.Redis.Views.Shared.EditorTemplates.RedisCacheSettings.cshtml";

        #endregion ISettings Members
    }
}