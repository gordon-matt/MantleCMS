using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Mantle.Web.Mvc.Resources;

public class StyleRegistrar : ResourceRegistrar
{
    private readonly IUrlHelper urlHelper;

    public StyleRegistrar(IWorkContext workContext)
        : base(workContext)
    {
        var urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>();
        var actionContextAccessor = EngineContext.Current.Resolve<IActionContextAccessor>();
        urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
    }

    protected override string VirtualBasePath => "/css"; // TODO: Make configurable?

    protected override ResourceLocation DefaultLocation => ResourceLocation.Head;

    public override void IncludeInline(string code, ResourceLocation? location = null, bool ignoreIfExists = false) =>
        throw new NotSupportedException();

    protected override string BuildInlineResources(IEnumerable<string> resources) =>
        $"<style type=\"text/css\">{string.Join(Environment.NewLine, resources)}</style>";

    protected override string BuildResource(ResourceEntry resource)
    {
        return new FluentTagBuilder("link", TagRenderMode.SelfClosing)
            .MergeAttribute("type", "text/css")
            .MergeAttribute("rel", "stylesheet")
            .MergeAttribute("href", urlHelper.Content(resource.Path))
            .MergeAttributes(resource.HtmlAttributes)
            .ToString();
    }
}