using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Localization.ComponentModel;
using Mantle.Security.Membership;
using Mantle.Threading;
using Mantle.Web.Security.Membership;

namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters
{
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
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Views.Shared.DisplayTemplates.NewsletterUserProfileProvider"; }
        }

        public string EditorTemplatePath
        {
            get { return "Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Views.Shared.EditorTemplates.NewsletterUserProfileProvider"; }
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
}