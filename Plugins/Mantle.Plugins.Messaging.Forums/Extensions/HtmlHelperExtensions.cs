using Mantle.Plugins.Messaging.Forums.Models;

namespace Mantle.Plugins.Messaging.Forums.Extensions;

public static class HtmlHelperExtensions
{
    //public static HtmlString BBCodeEditor<TModel>(this HtmlHelper<TModel> html, string name)
    //{
    //    var sb = new StringBuilder(128);
    //    sb.Append("<script type=\"text/javascript\">");
    //    sb.AppendFormat("edToolbar('{0}','/Plugins/Messaging.Forums/Scripts/BBEditor/');", name);
    //    sb.Append("</script>");

    //    return HtmlString.Create(sb.ToString());
    //}

    public static IHtmlContent Pager<TModel>(this IHtmlHelper<TModel> html, PagerModel model)
    {
        if (model.TotalRecords == 0)
        {
            return null;
        }

        var localizer = EngineContext.Current.Resolve<IStringLocalizer>();

        var links = new StringBuilder();
        if (model.ShowTotalSummary && (model.TotalPages > 0))
        {
            links.Append("<li class=\"total-summary\">");
            links.Append(string.Format(model.CurrentPageText, model.PageIndex + 1, model.TotalPages, model.TotalRecords));
            links.Append("</li>");
        }
        if (model.ShowPagerItems && (model.TotalPages > 1))
        {
            if (model.ShowFirst)
            {
                //first page
                if ((model.PageIndex >= 3) && (model.TotalPages > model.IndividualPagesDisplayedCount))
                {
                    model.RouteValues.page = 1;

                    links.Append("<li class=\"first-page\">");
                    //if (model.UseRouteLinks)
                    //{
                    //    links.Append(html.RouteLink(model.FirstButtonText, model.RouteActionName, model.RouteValues, new { title = localizer[LocalizableStrings.Pager.FirstPageTitle] }));
                    //}
                    //else
                    //{
                    links.Append(html.ActionLink(model.FirstButtonText, model.RouteActionName, model.RouteValues, new { title = localizer[LocalizableStrings.Pager.FirstPageTitle] }));
                    //}
                    links.Append("</li>");
                }
            }
            if (model.ShowPrevious)
            {
                //previous page
                if (model.PageIndex > 0)
                {
                    model.RouteValues.page = (model.PageIndex);

                    links.Append("<li class=\"previous-page\">");
                    //if (model.UseRouteLinks)
                    //{
                    //    links.Append(html.RouteLink(model.PreviousButtonText, model.RouteActionName, model.RouteValues, new { title = localizer[LocalizableStrings.Pager.PreviousPageTitle] }));
                    //}
                    //else
                    //{
                    links.Append(html.ActionLink(model.PreviousButtonText, model.RouteActionName, model.RouteValues, new { title = localizer[LocalizableStrings.Pager.PreviousPageTitle] }));
                    //}
                    links.Append("</li>");
                }
            }
            if (model.ShowIndividualPages)
            {
                //individual pages
                int firstIndividualPageIndex = model.GetFirstIndividualPageIndex();
                int lastIndividualPageIndex = model.GetLastIndividualPageIndex();
                for (int i = firstIndividualPageIndex; i <= lastIndividualPageIndex; i++)
                {
                    if (model.PageIndex == i)
                    {
                        links.AppendFormat("<li class=\"current-page\"><span>{0}</span></li>", (i + 1));
                    }
                    else
                    {
                        model.RouteValues.page = (i + 1);

                        links.Append("<li class=\"individual-page\">");
                        //if (model.UseRouteLinks)
                        //{
                        //    links.Append(html.RouteLink((i + 1).ToString(), model.RouteActionName, model.RouteValues, new { title = string.Format(localizer[LocalizableStrings.Pager.PageLinkTitle], (i + 1)) }));
                        //}
                        //else
                        //{
                        links.Append(html.ActionLink((i + 1).ToString(), model.RouteActionName, model.RouteValues, new { title = string.Format(localizer[LocalizableStrings.Pager.PageLinkTitle], (i + 1)) }));
                        //}
                        links.Append("</li>");
                    }
                }
            }
            if (model.ShowNext)
            {
                //next page
                if ((model.PageIndex + 1) < model.TotalPages)
                {
                    model.RouteValues.page = (model.PageIndex + 2);

                    links.Append("<li class=\"next-page\">");
                    //if (model.UseRouteLinks)
                    //{
                    //    links.Append(html.RouteLink(model.NextButtonText, model.RouteActionName, model.RouteValues, new { title = localizer[LocalizableStrings.Pager.NextPageTitle] }));
                    //}
                    //else
                    //{
                    links.Append(html.ActionLink(model.NextButtonText, model.RouteActionName, model.RouteValues, new { title = localizer[LocalizableStrings.Pager.NextPageTitle] }));
                    //}
                    links.Append("</li>");
                }
            }
            if (model.ShowLast)
            {
                //last page
                if (((model.PageIndex + 3) < model.TotalPages) && (model.TotalPages > model.IndividualPagesDisplayedCount))
                {
                    model.RouteValues.page = model.TotalPages;

                    links.Append("<li class=\"last-page\">");
                    //if (model.UseRouteLinks)
                    //{
                    //    links.Append(html.RouteLink(model.LastButtonText, model.RouteActionName, model.RouteValues, new { title = localizer[LocalizableStrings.Pager.LastPageTitle] }));
                    //}
                    //else
                    //{
                    links.Append(html.ActionLink(model.LastButtonText, model.RouteActionName, model.RouteValues, new { title = localizer[LocalizableStrings.Pager.LastPageTitle] }));
                    //}
                    links.Append("</li>");
                }
            }
        }
        var result = links.ToString();
        if (!string.IsNullOrEmpty(result))
        {
            result = "<ul>" + result + "</ul>";
        }
        return new HtmlString(result);
    }

    public static IHtmlContent ForumTopicSmallPager<TModel>(this IHtmlHelper<TModel> html, ForumTopicRowModel model)
    {
        var localizer = EngineContext.Current.Resolve<IStringLocalizer>();

        var forumTopicId = model.Id;
        var forumTopicSlug = model.SeName;
        var totalPages = model.TotalPostPages;

        if (totalPages > 0)
        {
            var links = new StringBuilder();

            if (totalPages <= 4)
            {
                for (int x = 1; x <= totalPages; x++)
                {
                    links.Append(html.ActionLink(x.ToString(), "Topic", new { id = forumTopicId, page = (x), slug = forumTopicSlug }, new { title = string.Format(localizer[LocalizableStrings.Pager.PageLinkTitle], x.ToString()) }));
                    if (x < totalPages)
                    {
                        links.Append(", ");
                    }
                }
            }
            else
            {
                links.Append(html.ActionLink("1", "Topic", new { id = forumTopicId, page = (1), slug = forumTopicSlug }, new { title = string.Format(localizer[LocalizableStrings.Pager.PageLinkTitle], 1) }));
                links.Append(" ... ");

                for (int x = (totalPages - 2); x <= totalPages; x++)
                {
                    links.Append(html.ActionLink(x.ToString(), "Topic", new { id = forumTopicId, page = (x), slug = forumTopicSlug }, new { title = string.Format(localizer[LocalizableStrings.Pager.PageLinkTitle], x.ToString()) }));

                    if (x < totalPages)
                    {
                        links.Append(", ");
                    }
                }
            }

            // Inserts the topic page links into the localized string ([Go to page: {0}])
            return new HtmlString(string.Format(localizer[LocalizableStrings.Topics_GotoPostPager], links.ToString()));
        }
        return new HtmlString(string.Empty);
    }
}