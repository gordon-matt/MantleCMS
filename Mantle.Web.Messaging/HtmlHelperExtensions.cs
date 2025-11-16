namespace Mantle.Web.Messaging;

public static class HtmlHelperExtensions
{
    extension<TModel>(IHtmlHelper<TModel> html) where TModel : class
    {
        public MantleMessaging<TModel> MantleMessaging() => new(html);
    }
}

public class MantleMessaging<TModel>
    where TModel : class
{
    private readonly IHtmlHelper<TModel> html;

    internal MantleMessaging(IHtmlHelper<TModel> html)
    {
        this.html = html;
    }

    public IHtmlContent EditorDropDownList(string name, string selectedValue = null, object htmlAttributes = null, string emptyText = null)
    {
        var messageTemplateEditors = DependoResolver.Instance.ResolveAll<IMessageTemplateEditor>();

        var selectList = messageTemplateEditors
            .ToSelectList(
                value => value.Name,
                text => text.Name,
                selectedValue,
                emptyText);

        return html.DropDownList(name, selectList, htmlAttributes);
    }
}