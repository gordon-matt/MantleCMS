﻿namespace Mantle.Plugins.Messaging.Forums.Models;

public class PagerModel
{
    #region Fields

    private IStringLocalizer localizer;
    private int individualPagesDisplayedCount;
    private int pageIndex = -2;
    private int pageSize;

    private bool? showFirst;
    private bool? showIndividualPages;
    private bool? showLast;
    private bool? showNext;
    private bool? showPagerItems;
    private bool? showPrevious;
    private bool? showTotalSummary;

    private string firstButtonText;
    private string lastButtonText;
    private string nextButtonText;
    private string previousButtonText;
    private string currentPageText;

    #endregion Fields

    #region Properties

    private IStringLocalizer T
    {
        get
        {
            localizer ??= DependoResolver.Instance.Resolve<IStringLocalizer>();
            return localizer;
        }
    }

    /// <summary>
    /// Gets the current page index
    /// </summary>
    public int CurrentPage => this.PageIndex + 1;

    /// <summary>
    /// Gets or sets a count of individual pages to be displayed
    /// </summary>
    public int IndividualPagesDisplayedCount
    {
        get => individualPagesDisplayedCount <= 0 ? 5 : individualPagesDisplayedCount;
        set => individualPagesDisplayedCount = value;
    }

    /// <summary>
    /// Gets the current page index
    /// </summary>
    public int PageIndex
    {
        get => this.pageIndex < 0 ? 0 : this.pageIndex;
        set => this.pageIndex = value;
    }

    /// <summary>
    /// Gets or sets a page size
    /// </summary>
    public int PageSize
    {
        get => (pageSize <= 0) ? 10 : pageSize;
        set => pageSize = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "first"
    /// </summary>
    public bool ShowFirst
    {
        get => showFirst ?? true;
        set => showFirst = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "individual pages"
    /// </summary>
    public bool ShowIndividualPages
    {
        get => showIndividualPages ?? true;
        set => showIndividualPages = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "last"
    /// </summary>
    public bool ShowLast
    {
        get => showLast ?? true;
        set => showLast = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "next"
    /// </summary>
    public bool ShowNext
    {
        get => showNext ?? true;
        set => showNext = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show pager items
    /// </summary>
    public bool ShowPagerItems
    {
        get => showPagerItems ?? true;
        set => showPagerItems = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "previous"
    /// </summary>
    public bool ShowPrevious
    {
        get => showPrevious ?? true;
        set => showPrevious = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show "total summary"
    /// </summary>
    public bool ShowTotalSummary
    {
        get => showTotalSummary ?? false;
        set => showTotalSummary = value;
    }

    /// <summary>
    /// Gets a total pages count
    /// </summary>
    public int TotalPages
    {
        get
        {
            if ((this.TotalRecords == 0) || (this.PageSize == 0))
            {
                return 0;
            }
            int num = this.TotalRecords / this.PageSize;
            if ((this.TotalRecords % this.PageSize) > 0)
            {
                num++;
            }
            return num;
        }
    }

    /// <summary>
    /// Gets or sets a total records count
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Gets or sets the first button text
    /// </summary>
    public string FirstButtonText
    {
        get => (!string.IsNullOrEmpty(firstButtonText))
            ? firstButtonText
            : T[LocalizableStrings.Models.Pager.First];
        set => firstButtonText = value;
    }

    /// <summary>
    /// Gets or sets the last button text
    /// </summary>
    public string LastButtonText
    {
        get => (!string.IsNullOrEmpty(lastButtonText))
            ? lastButtonText
            : T[LocalizableStrings.Models.Pager.Last];
        set => lastButtonText = value;
    }

    /// <summary>
    /// Gets or sets the next button text
    /// </summary>
    public string NextButtonText
    {
        get => (!string.IsNullOrEmpty(nextButtonText))
            ? nextButtonText
            : T[LocalizableStrings.Models.Pager.Next];
        set => nextButtonText = value;
    }

    /// <summary>
    /// Gets or sets the previous button text
    /// </summary>
    public string PreviousButtonText
    {
        get => (!string.IsNullOrEmpty(previousButtonText))
            ? previousButtonText
            : T[LocalizableStrings.Models.Pager.Previous];
        set => previousButtonText = value;
    }

    /// <summary>
    /// Gets or sets the current page text
    /// </summary>
    public string CurrentPageText
    {
        get => (!string.IsNullOrEmpty(currentPageText))
            ? currentPageText
            : T[LocalizableStrings.Models.Pager.CurrentPage];
        set => currentPageText = value;
    }

    /// <summary>
    /// Gets or sets the route name or action name
    /// </summary>
    public string RouteActionName { get; set; }

    ///// <summary>
    ///// Gets or sets whether the links are created using RouteLink instead of Action Link
    ///// (for additional route values such as slugs or page numbers)
    ///// </summary>
    //public bool UseRouteLinks { get; set; }

    /// <summary>
    /// Gets or sets the RouteValues object. Allows for custom route values other than page.
    /// </summary>
    public IRouteValues RouteValues { get; set; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Gets first individual page index
    /// </summary>
    /// <returns>Page index</returns>
    public int GetFirstIndividualPageIndex() =>
        (this.TotalPages < this.IndividualPagesDisplayedCount) ||
        ((this.PageIndex - (this.IndividualPagesDisplayedCount / 2)) < 0)
            ? 0
            : (this.PageIndex + (this.IndividualPagesDisplayedCount / 2)) >= this.TotalPages
                ? this.TotalPages - this.IndividualPagesDisplayedCount
                : this.PageIndex - (this.IndividualPagesDisplayedCount / 2);

    /// <summary>
    /// Get last individual page index
    /// </summary>
    /// <returns>Page index</returns>
    public int GetLastIndividualPageIndex()
    {
        int num = this.IndividualPagesDisplayedCount / 2;
        if ((this.IndividualPagesDisplayedCount % 2) == 0)
        {
            num--;
        }
        return
            (this.TotalPages < this.IndividualPagesDisplayedCount) ||
            ((this.PageIndex + num) >= this.TotalPages)
                ? this.TotalPages - 1
                : (this.PageIndex - (this.IndividualPagesDisplayedCount / 2)) < 0
                    ? this.IndividualPagesDisplayedCount - 1
                    : this.PageIndex + num;
    }

    #endregion Methods
}

public interface IRouteValues
{
    int page { get; set; }
}

/// <summary>
/// Class that has search options for route values. Used for Search result pagination
/// </summary>
public partial class ForumSearchRouteValues : IRouteValues
{
    public string searchterms { get; set; }
    public string adv { get; set; }
    public string forumId { get; set; }
    public string within { get; set; }
    public string limitDays { get; set; }
    public int page { get; set; }
}

/// <summary>
/// Class that has a slug and page for route values. Used for Topic (posts) and
/// Forum (topics) pagination
/// </summary>
public class RouteValues : IRouteValues
{
    public int id { get; set; }

    public string slug { get; set; }

    public int page { get; set; }
}

/// <summary>
/// Class that has a slug and page for route values. Used for Private Messages pagination
/// </summary>
public partial class PrivateMessageRouteValues : IRouteValues
{
    public string tab { get; set; }
    public int page { get; set; }
}

/// <summary>
/// Class that has only page for route value. Used for Active Discussions (forums) pagination
/// </summary>
public partial class ForumActiveDiscussionsRouteValues : IRouteValues
{
    public int page { get; set; }
}

/// <summary>
/// Class that has only page for route value. Used for (My Account) Forum Subscriptions pagination
/// </summary>
public partial class ForumSubscriptionsRouteValues : IRouteValues
{
    public int page { get; set; }
}