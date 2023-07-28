using Mantle.Web.ContentManagement.Areas.Admin.Menus.Domain;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MantleCMS.Extensions;

public static class HtmlHelperExtensions
{
    public static DemoApp<TModel> DemoApp<TModel>(this IHtmlHelper<TModel> html) where TModel : class
    {
        return new DemoApp<TModel>(html);
    }
}

public class DemoApp<TModel>
    where TModel : class
{
    private readonly IHtmlHelper<TModel> html;

    internal DemoApp(IHtmlHelper<TModel> html)
    {
        this.html = html;
    }

    public IHtmlContent BuildMainMenuItems(IEnumerable<MenuItem> menuItems, MenuItem menuItem, string currentUrl, bool isFirstLevel)
    {
        bool isCurrent = currentUrl.EndsWith(menuItem.Url);
        var childItems = menuItems.Where(x => x.ParentId == menuItem.Id).OrderBy(x => x.Position).ThenBy(x => x.Text).ToList();

        var urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>();
        var actionContextAccessor = EngineContext.Current.Resolve<IActionContextAccessor>();
        var urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);

        string url = menuItem.IsExternalUrl ? menuItem.Url : urlHelper.Content(menuItem.Url);
        string cssClass = (menuItem.CssClass + (isCurrent ? " active" : string.Empty)).Trim();

        var tagBuiler = new FluentTagBuilder("li")
            .AddCssClass(cssClass);

        if (isFirstLevel && childItems.Any())
        {
            tagBuiler = tagBuiler
                .AddCssClass("dropdown")
                .StartTag("a")
                    .AddCssClass("dropdown-toggle")
                    .MergeAttributes(new { aria_expanded = true, role = "button", data_toggle = "dropdown", href = "#" })
                    .SetInnerHtml(menuItem.Text)
                    .StartTag("span")
                        .AddCssClass("caret")
                    .EndTag() // </span>
                .EndTag() // </a>
                .StartTag("ul")
                    .AddCssClass("dropdown-menu");

            if (!string.IsNullOrEmpty(url))
            {
                tagBuiler = tagBuiler
                    .StartTag("li")
                        .AddCssClass(cssClass)
                        .StartTag("a")
                            .MergeAttribute("href", url)
                            .SetInnerHtml(menuItem.Text)
                        .EndTag() // </a>
                    .EndTag(); // </li>
            }
            else
            {
                foreach (var item in childItems)
                {
                    var content = BuildMainMenuItems(menuItems, item, currentUrl, false);
                    tagBuiler = tagBuiler.SetInnerHtml(content.ToString());
                }
            }

            tagBuiler = tagBuiler.EndTag(); // </ul>
        }
        else
        {
            tagBuiler = tagBuiler
                .StartTag("li")
                    .AddCssClass(cssClass)
                    .StartTag("a")
                        .MergeAttribute("href", url)
                        .SetInnerHtml(menuItem.Text)
                    .EndTag() // </a>
                .EndTag(); // </li>
        }

        return new Microsoft.AspNetCore.Html.HtmlString(tagBuiler.ToString());
    }

    public IHtmlContent BuildSubMenuItems(MenuItem menuItem, string currentUrl)
    {
        bool isCurrent = currentUrl.EndsWith(menuItem.Url);

        var urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>();
        var actionContextAccessor = EngineContext.Current.Resolve<IActionContextAccessor>();
        var urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);

        string url = menuItem.IsExternalUrl ? menuItem.Url : urlHelper.Content(menuItem.Url);
        string cssClass = (menuItem.CssClass + (isCurrent ? " active" : string.Empty)).Trim();

        var tagBuiler = new FluentTagBuilder("a")
            .AddCssClass("list-group-item")
            .AddCssClass(cssClass)
            .MergeAttribute("href", url)
            .SetInnerHtml(menuItem.Text);

        return new Microsoft.AspNetCore.Html.HtmlString(tagBuiler.ToString());
    }
}