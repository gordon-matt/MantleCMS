namespace Mantle.Web.Security.Membership;

public class AccountUserProfileProvider : IUserProfileProvider
{
    public class Fields
    {
        public const string FamilyName = "FamilyName";
        public const string GivenNames = "GivenNames";
        public const string ShowFamilyNameFirst = "ShowFamilyNameFirst";
    }

    [LocalizedDisplayName(MantleWebLocalizableStrings.UserProfile.Account.FamilyName)]
    public string FamilyName { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.UserProfile.Account.GivenNames)]
    public string GivenNames { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.UserProfile.Account.ShowFamilyNameFirst)]
    public bool ShowFamilyNameFirst { get; set; }

    #region IUserProfileProvider Members

    public string Name => "Account";

    public string DisplayTemplatePath => "/Views/Shared/DisplayTemplates/AccountUserProfileProvider.cshtml";

    public string EditorTemplatePath => "/Views/Shared/EditorTemplates/AccountUserProfileProvider.cshtml";

    public int Order => 0;

    public IEnumerable<string> GetFieldNames() =>
    [
        Fields.FamilyName,
        Fields.GivenNames,
        Fields.ShowFamilyNameFirst
    ];

    public void PopulateFields(string userId)
    {
        var membershipService = EngineContext.Current.Resolve<IMembershipService>();

        var profile = AsyncHelper.RunSync(() => membershipService.GetProfile(userId));

        if (profile.TryGetValue(Fields.FamilyName, out string familyName))
        {
            FamilyName = familyName;
        }
        if (profile.TryGetValue(Fields.GivenNames, out string givenNames))
        {
            GivenNames = givenNames;
        }
        if (profile.TryGetValue(Fields.ShowFamilyNameFirst, out string showFamilyNameFirst))
        {
            ShowFamilyNameFirst = bool.Parse(showFamilyNameFirst);
        }
    }

    #endregion IUserProfileProvider Members
}