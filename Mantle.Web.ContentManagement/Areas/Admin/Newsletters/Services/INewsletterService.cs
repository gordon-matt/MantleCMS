using Mantle.Events;
using Mantle.Security;
using Mantle.Security.Membership;
using Mantle.Threading;
using Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Events;
using Mantle.Web.Security.Membership;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Services
{
    public interface INewsletterService
    {
        bool Subscribe(string email, string name, MantleUser currentUser, out string message);
    }

    public class NewsletterService : INewsletterService
    {
        private readonly Lazy<IMembershipService> membershipService;
        private readonly Lazy<MembershipSettings> membershipSettings;
        private readonly Lazy<IEventPublisher> eventPublisher;
        private readonly IStringLocalizer T;
        private readonly Lazy<IWorkContext> workContext;

        public NewsletterService(
            Lazy<IMembershipService> membershipService,
            Lazy<MembershipSettings> membershipSettings,
            Lazy<IEventPublisher> eventPublisher,
            Lazy<IWorkContext> workContext,
            IStringLocalizer localizer)
        {
            this.membershipService = membershipService;
            this.membershipSettings = membershipSettings;
            this.eventPublisher = eventPublisher;
            this.workContext = workContext;
            T = localizer;
        }

        #region INewsletterService Members

        public bool Subscribe(string email, string name, MantleUser currentUser, out string message)
        {
            // First check if valid email address
            if (!CmsConstants.RegexPatterns.Email.IsMatch(email))
            {
                message = T[MantleWebLocalizableStrings.Membership.InvalidEmailAddress].Value;
                return false;
            }

            var existingUser = AsyncHelper.RunSync(() => membershipService.Value.GetUserByEmail(workContext.Value.CurrentTenant.Id, email));

            // Check if a user exists with that email..
            if (existingUser != null)
            {
                // if user is logged in already and is the same user with that email address
                if (currentUser != null && currentUser.Id == existingUser.Id)
                {
                    //auto set "ReceiveNewsletters" in profile to true
                    AsyncHelper.RunSync(() => membershipService.Value.SaveProfileEntry(
                        currentUser.Id,
                        NewsletterUserProfileProvider.Fields.SubscribeToNewsletters,
                        bool.TrueString));

                    eventPublisher.Value.Publish(new NewsletterSubscribedEvent(existingUser));

                    message = T[MantleCmsLocalizableStrings.Newsletters.SuccessfullySignedUp].Value;
                    return true;
                }

                //else just tell user to login and set "ReceiveNewsletters" in profile to true
                message = T[MantleWebLocalizableStrings.Membership.UserEmailAlreadyExists].Value;
                return false;
            }

            //create a user and email details to him/her with random password
            string password = Password.Generate(
                membershipSettings.Value.GeneratedPasswordLength,
                membershipSettings.Value.GeneratedPasswordNumberOfNonAlphanumericChars);

            AsyncHelper.RunSync(() => membershipService.Value.InsertUser(new MantleUser
            {
                TenantId = workContext.Value.CurrentTenant.Id,
                UserName = email,
                Email = email
            }, password));

            var user = AsyncHelper.RunSync(() => membershipService.Value.GetUserByEmail(workContext.Value.CurrentTenant.Id, email));

            // and sign up for newsletter, as requested.
            AsyncHelper.RunSync(() => membershipService.Value.SaveProfileEntry(user.Id, NewsletterUserProfileProvider.Fields.SubscribeToNewsletters, bool.TrueString));

            name = name.Trim();
            if (name.Contains(" "))
            {
                string[] nameArray = name.Split(' ');
                string familyName = nameArray.Last();
                string givenNames = name.Replace(familyName, string.Empty).Trim();
                AsyncHelper.RunSync(() => membershipService.Value.SaveProfileEntry(user.Id, AccountUserProfileProvider.Fields.FamilyName, familyName));
                AsyncHelper.RunSync(() => membershipService.Value.SaveProfileEntry(user.Id, AccountUserProfileProvider.Fields.GivenNames, givenNames));
            }
            else
            {
                AsyncHelper.RunSync(() => membershipService.Value.SaveProfileEntry(user.Id, AccountUserProfileProvider.Fields.GivenNames, name));
            }

            eventPublisher.Value.Publish(new NewsletterUnsubscribedEvent(user));

            message = T[MantleCmsLocalizableStrings.Newsletters.SuccessfullySignedUp].Value;
            return true;
        }

        #endregion INewsletterService Members
    }
}