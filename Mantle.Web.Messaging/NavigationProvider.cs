using Mantle.Web.Navigation;
using NavigationBuilder = Mantle.Web.Navigation.NavigationBuilder;

namespace Mantle.Web.Messaging;

public class NavigationProvider : INavigationProvider
{
    public NavigationProvider()
    {
        T = EngineContext.Current.Resolve<IStringLocalizer>();
    }

    public IStringLocalizer T { get; set; }

    #region INavigationProvider Members

    public string MenuName => MantleWebConstants.Areas.Admin;

    public void GetNavigation(NavigationBuilder builder) => builder.Add(T[LocalizableStrings.Messaging].Value, "2", BuildMessagingMenu);

    #endregion INavigationProvider Members

    private void BuildMessagingMenu(NavigationItemBuilder builder)
    {
        builder.Icons("fa fa-envelope-o");

        // Messaging
        builder.Add(T[LocalizableStrings.MessageTemplates].Value, "5", item => item
            .Url("#messaging/templates")
            .Icons("fa fa-crop")
            .Permission(MessagingPermissions.MessageTemplatesRead));

        // Queued Emails
        builder.Add(T[LocalizableStrings.QueuedEmails].Value, "5", item => item
            .Url("#messaging/queued-email")
            .Icons("fa fa-envelope-o")
            .Permission(MessagingPermissions.QueuedEmailsRead));
    }
}