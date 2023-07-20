using Mantle.Plugins.Messaging.Forums.Data.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Mantle.Plugins.Messaging.Forums.Models
{
    public class EditForumTopicModel
    {
        public EditForumTopicModel()
        {
            TopicPriorities = new List<SelectListItem>();
        }

        public bool IsEdit { get; set; }

        public int Id { get; set; }

        public int ForumId { get; set; }

        public string ForumName { get; set; }

        public string ForumSeName { get; set; }

        public ForumTopicType TopicType { get; set; }

        public EditorType ForumEditor { get; set; }

        //[AllowHtml]
        [Required]
        public string Subject { get; set; }

        //[AllowHtml]
        [Required]
        public string Text { get; set; }

        public bool IsUserAllowedToSetTopicPriority { get; set; }

        public IEnumerable<SelectListItem> TopicPriorities { get; set; }

        public bool IsUserAllowedToSubscribe { get; set; }

        public bool Subscribed { get; set; }
    }
}