using Mantle.Web.Security.Membership;

namespace Mantle.Web.Mvc.Themes;

public class ThemeUserProfileProvider : IUserProfileProvider
{
    public class Fields
    {
        public const string PreferredTheme = "PreferredTheme";
    }

    [LocalizedDisplayName(MantleWebLocalizableStrings.UserProfile.Theme.PreferredTheme)]
    public string PreferredTheme { get; set; }

    #region IUserProfileProvider Members

    public string Name => "Theme";

    public string DisplayTemplatePath => "/Views/Shared/DisplayTemplates/ThemeUserProfileProvider.cshtml";

    public string EditorTemplatePath => "/Views/Shared/EditorTemplates/ThemeUserProfileProvider.cshtml";

    public int Order => 9999;

    public IEnumerable<string> GetFieldNames() =>
    [
        Fields.PreferredTheme
    ];

    public void PopulateFields(string userId)
    {
        var membershipService = EngineContext.Current.Resolve<IMembershipService>();
        PreferredTheme = AsyncHelper.RunSync(() => membershipService.GetProfileEntry(userId, Fields.PreferredTheme));
    }

    #endregion IUserProfileProvider Members
}