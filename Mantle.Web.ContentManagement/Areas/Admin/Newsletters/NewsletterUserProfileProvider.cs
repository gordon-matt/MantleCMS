using Mantle.Web.Security.Membership;

namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters;

public class NewsletterUserProfileProvider : IUserProfileProvider
{
    public class Fields
    {
        public const string SubscribeToNewsletters = "SubscribeToNewsletters";
    }

    [LocalizedDisplayName(MantleCmsLocalizableStrings.UserProfile.Newsletter.SubscribeToNewsletters)]
    public bool SubscribeToNewsletters { get; set; }

    #region IUserProfileProvider Members

    public string Name => "Newsletters";

    public string DisplayTemplatePath => "/Areas/Admin/Newsletters/Views/Shared/DisplayTemplates/NewsletterUserProfileProvider.cshtml";

    public string EditorTemplatePath => "/Areas/Admin/Newsletters/Views/Shared/EditorTemplates/NewsletterUserProfileProvider.cshtml";

    public int Order => 9999;

    public IEnumerable<string> GetFieldNames() =>
    [
        Fields.SubscribeToNewsletters
    ];

    public void PopulateFields(string userId)
    {
        var membershipService = DependoResolver.Instance.Resolve<IMembershipService>();
        string subscribeToNewsletters = AsyncHelper.RunSync(() => membershipService.GetProfileEntry(userId, Fields.SubscribeToNewsletters));
        SubscribeToNewsletters = !string.IsNullOrEmpty(subscribeToNewsletters) && bool.Parse(subscribeToNewsletters);
    }

    #endregion IUserProfileProvider Members
}