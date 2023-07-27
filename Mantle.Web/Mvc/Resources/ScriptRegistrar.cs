using Extenso.AspNetCore.Mvc.Html;
using Extenso.AspNetCore.Mvc.Rendering;
using Mantle.Infrastructure;
using Mantle.Web.IO;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Mantle.Web.Mvc.Resources
{
    public class ScriptRegistrar : ResourceRegistrar
    {
        private readonly IUrlHelper urlHelper;

        public ScriptRegistrar(IWorkContext workContext)
            : base(workContext)
        {
            var urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>();
            var actionContextAccessor = EngineContext.Current.Resolve<IActionContextAccessor>();
            urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        protected override string VirtualBasePath => "/js"; // TODO: Make configurable?

        protected override ResourceLocation DefaultLocation => ResourceLocation.Foot;

        public override void IncludeInline(string code, ResourceLocation? location = null, bool ignoreIfExists = false) =>
            base.IncludeInline($"<script type=\"text/javascript\">${code}</script>", location, ignoreIfExists);

        protected override string BuildInlineResources(IEnumerable<string> resources) =>
            $"{Environment.NewLine}{string.Join(Environment.NewLine, resources)}{Environment.NewLine}";

        protected override string BuildResource(ResourceEntry resource)
        {
            return new FluentTagBuilder("script")
                .MergeAttribute("type", "text/javascript")
                .MergeAttribute("src", urlHelper.Content(resource.Path))
                .MergeAttributes(resource.HtmlAttributes)
                .ToString();
        }

        public IDisposable AtFoot(IHtmlHelper html) => new ScriptBlock(html.ViewContext, s => base.IncludeInline(s.GetString(), ResourceLocation.Foot));

        private class ScriptBlock : IDisposable
        {
            private readonly Action<IHtmlContent> callback;

            private readonly ViewContext viewContext;
            private readonly TextWriter originalWriter;
            private readonly HtmlTextWriter scriptWriter;
            private bool isDisposed;

            public ScriptBlock(ViewContext viewContext, Action<IHtmlContent> callback)
            {
                this.viewContext = viewContext;
                this.callback = callback;
                originalWriter = viewContext.Writer;
                viewContext.Writer = scriptWriter = new HtmlTextWriter();
            }

            public void Dispose()
            {
                if (isDisposed)
                {
                    return;
                }

                try
                {
                    callback(scriptWriter);
                }
                finally
                {
                    // Restore the original TextWriter
                    viewContext.Writer = originalWriter;
                    scriptWriter?.Dispose();
                    isDisposed = true;
                }
            }
        }
    }
}