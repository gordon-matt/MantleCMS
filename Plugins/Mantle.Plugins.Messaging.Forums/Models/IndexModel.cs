using System.Collections.Generic;

namespace Mantle.Plugins.Messaging.Forums.Models
{
    public class IndexModel
    {
        public IndexModel()
        {
            this.ForumGroups = new List<ForumGroupModel>();
        }

        public IList<ForumGroupModel> ForumGroups { get; set; }
    }
}