namespace Mantle.Plugins.Messaging.Forums.Models;

public class ForumGroupModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string SeName { get; set; }

    public IList<ForumRowModel> Forums { get; set; } = [];
}