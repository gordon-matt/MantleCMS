namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Events;

public class NewsletterSubscribedEvent
{
    public NewsletterSubscribedEvent(MantleUser user)
    {
        this.User = user;
    }

    public MantleUser User { get; private set; }
}