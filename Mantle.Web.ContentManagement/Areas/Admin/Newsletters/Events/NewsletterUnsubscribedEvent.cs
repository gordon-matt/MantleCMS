namespace Mantle.Web.ContentManagement.Areas.Admin.Newsletters.Events;

public class NewsletterUnsubscribedEvent
{
    public NewsletterUnsubscribedEvent(MantleUser user)
    {
        this.User = user;
    }

    public MantleUser User { get; private set; }
}