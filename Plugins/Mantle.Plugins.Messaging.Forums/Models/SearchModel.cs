namespace Mantle.Plugins.Messaging.Forums.Models;

public class SearchModel
{
    public bool ShowAdvancedSearch { get; set; }

    //[AllowHtml]
    public string SearchTerms { get; set; }

    public int? ForumId { get; set; }

    public int? Within { get; set; }

    public int? LimitDays { get; set; }

    public IList<ForumTopicRowModel> ForumTopics { get; set; } = [];

    public int TopicPageSize { get; set; }

    public int TopicTotalRecords { get; set; }

    public int TopicPageIndex { get; set; }

    public List<SelectListItem> LimitList { get; set; } = [];

    public List<SelectListItem> ForumList { get; set; } = [];

    public List<SelectListItem> WithinList { get; set; } = [];

    public int ForumIdSelected { get; set; }

    public int WithinSelected { get; set; }

    public int LimitDaysSelected { get; set; }

    public bool SearchResultsVisible { get; set; }

    public bool NoResultsVisisble { get; set; }

    public string Error { get; set; }

    public int PostsPageSize { get; set; }
}