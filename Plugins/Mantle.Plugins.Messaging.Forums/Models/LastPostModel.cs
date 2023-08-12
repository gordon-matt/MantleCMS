namespace Mantle.Plugins.Messaging.Forums.Models;

public class LastPostModel
{
    public int Id { get; set; }

    public int ForumTopicId { get; set; }

    public string ForumTopicSeName { get; set; }

    public string ForumTopicSubject { get; set; }

    public string UserId { get; set; }

    public bool AllowViewingProfiles { get; set; }

    public string UserName { get; set; }

    public string PostCreatedOnStr { get; set; }

    public bool ShowTopic { get; set; }
}