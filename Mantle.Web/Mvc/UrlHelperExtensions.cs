namespace Mantle.Web.Mvc;

public static class UrlHelperExtensions
{
    extension(IUrlHelper url)
    {
        /// <summary>
        /// Generates a fully qualified URL to an action method by using the specified action name, controller name and
        /// route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="actionName">The name of the action method.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public string AbsoluteAction(
            string actionName,
            string controllerName,
            object routeValues = null) =>
            url.Action(actionName, controllerName, routeValues, url.ActionContext.HttpContext.Request.Scheme);

        /// <summary>
        /// Generates a fully qualified URL to the specified content by using the specified content path. Converts a
        /// virtual (relative) path to an application absolute path.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="contentPath">The content path.</param>
        /// <returns>The absolute URL.</returns>
        public string AbsoluteContent(
            string contentPath)
        {
            var request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }

        /// <summary>
        /// Generates a fully qualified URL to the specified route by using the route name and route values.
        /// </summary>
        /// <param name="url">The URL helper.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routeValues">The route values.</param>
        /// <returns>The absolute URL.</returns>
        public string AbsoluteRouteUrl(
            string routeName,
            object routeValues = null) => url.RouteUrl(routeName, routeValues, url.ActionContext.HttpContext.Request.Scheme);

        public IHtmlContent Script(string src, object htmlAttributes = null)
        {
            var tagBuilder = new FluentTagBuilder("script")
                .MergeAttribute("type", "text/javascript")
                .MergeAttribute("src", src);

            if (htmlAttributes != null)
            {
                tagBuilder = tagBuilder.MergeAttributes(htmlAttributes);
            }

            return new HtmlString(tagBuilder.ToString());
        }

        public IHtmlContent ScriptInline(string rawJavaScript, object htmlAttributes = null)
        {
            var tagBuilder = new FluentTagBuilder("script")
                .MergeAttribute("type", "text/javascript")
                .SetInnerHtml(rawJavaScript);

            if (htmlAttributes != null)
            {
                tagBuilder = tagBuilder.MergeAttributes(htmlAttributes);
            }

            return new HtmlString(tagBuilder.ToString());
        }

        public IHtmlContent Style(string href, object htmlAttributes = null)
        {
            var tagBuilder = new FluentTagBuilder("link", TagRenderMode.SelfClosing)
                .MergeAttribute("rel", "stylesheet")
                .MergeAttribute("href", href);

            if (htmlAttributes != null)
            {
                tagBuilder = tagBuilder.MergeAttributes(htmlAttributes);
            }

            return new HtmlString(tagBuilder.ToString());
        }
    }
}