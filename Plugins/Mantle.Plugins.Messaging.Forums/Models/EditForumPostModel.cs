using System.ComponentModel.DataAnnotations;
using Mantle.Plugins.Messaging.Forums.Data.Domain;

namespace Mantle.Plugins.Messaging.Forums.Models
{
    public class EditForumPostModel
    {
        public int Id { get; set; }

        public int ForumTopicId { get; set; }

        public bool IsEdit { get; set; }

        //[AllowHtml]
        [Required]
        public string Text { get; set; }

        public EditorType ForumEditor { get; set; }

        public string ForumName { get; set; }

        public string ForumTopicSubject { get; set; }

        public string ForumTopicSeName { get; set; }

        public bool IsUserAllowedToSubscribe { get; set; }

        public bool Subscribed { get; set; }
    }
}