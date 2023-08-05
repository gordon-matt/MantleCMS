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

    public string Name
    {
        get { return "Newsletters"; }
    }

    public string DisplayTemplatePath
    {
        get { return "/Areas/Admin/Newsletters/Views/Shared/DisplayTemplates/NewsletterUserProfileProvider.cshtml"; }
    }

    public string EditorTemplatePath
    {
        get { return "/Areas/Admin/Newsletters/Views/Shared/EditorTemplates/NewsletterUserProfileProvider.cshtml"; }
    }

    public int Order
    {
        get { return 9999; }
    }

    public IEnumerable<string> GetFieldNames()
    {
        return new[]
        {
            Fields.SubscribeToNewsletters
        };
    }

    public void PopulateFields(string userId)
    {
        var membershipService = EngineContext.Current.Resolve<IMembershipService>();
        string subscribeToNewsletters = AsyncHelper.RunSync(() => membershipService.GetProfileEntry(userId, Fields.SubscribeToNewsletters));
        SubscribeToNewsletters = !string.IsNullOrEmpty(subscribeToNewsletters) && bool.Parse(subscribeToNewsletters);
    }

    #endregion IUserProfileProvider Members
}