using Mantle.Infrastructure;
using Mantle.Localization.ComponentModel;
using Mantle.Security.Membership;
using Mantle.Threading;

namespace Mantle.Web.Security.Membership
{
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

        public string Name
        {
            get { return "Account"; }
        }

        public string DisplayTemplatePath
        {
            get { return "Mantle.Web.Views.Shared.DisplayTemplates.AccountUserProfileProvider"; }
        }

        public string EditorTemplatePath
        {
            get { return "Mantle.Web.Views.Shared.EditorTemplates.AccountUserProfileProvider"; }
        }

        public int Order
        {
            get { return 0; }
        }

        public IEnumerable<string> GetFieldNames()
        {
            return new[]
            {
                Fields.FamilyName,
                Fields.GivenNames,
                Fields.ShowFamilyNameFirst
            };
        }

        public void PopulateFields(string userId)
        {
            var membershipService = EngineContext.Current.Resolve<IMembershipService>();

            var profile = AsyncHelper.RunSync(() => membershipService.GetProfile(userId));

            if (profile.ContainsKey(Fields.FamilyName))
            {
                FamilyName = profile[Fields.FamilyName];
            }
            if (profile.ContainsKey(Fields.GivenNames))
            {
                GivenNames = profile[Fields.GivenNames];
            }
            if (profile.ContainsKey(Fields.ShowFamilyNameFirst))
            {
                ShowFamilyNameFirst = bool.Parse(profile[Fields.ShowFamilyNameFirst]);
            }
        }

        #endregion IUserProfileProvider Members
    }
}