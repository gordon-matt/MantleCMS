using System;
using System.Collections.Generic;
using System.IO;
using Mantle.Infrastructure;
using Mantle.Web.Html;
using Mantle.Web.IO;
using Mantle.Web.Mvc.Rendering;
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

        public ScriptRegistrar(IWebWorkContext workContext)
            : base(workContext)
        {
            var urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>();
            var actionContextAccessor = EngineContext.Current.Resolve<IActionContextAccessor>();
            urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        protected override string BundleBasePath => "/js/bundles/";

        protected override string VirtualBasePath => "/js"; // TODO: Make configurable?

        protected override ResourceLocation DefaultLocation => ResourceLocation.Foot;

        public override void IncludeInline(string code, ResourceLocation? location = null, bool ignoreIfExists = false)
        {
            base.IncludeInline(
                string.Concat("<script type=\"text/javascript\">", code, "</script>"),
                location,
                ignoreIfExists);
        }

        protected override string BuildInlineResources(IEnumerable<string> resources)
        {
            return string.Format("{0}{1}{0}", Environment.NewLine, string.Join(Environment.NewLine, resources));
        }

        protected override string BuildResource(ResourceEntry resource)
        {
            var builder = new FluentTagBuilder("script")
                .MergeAttribute("type", "text/javascript")
                .MergeAttribute("src", urlHelper.Content(resource.Path))
                .MergeAttributes(resource.HtmlAttributes);

            return builder.ToString();
        }
        
        public IDisposable AtFoot(IHtmlHelper html)
        {
            return new ScriptBlock(html.ViewContext, s => base.IncludeInline(s.GetString(), ResourceLocation.Foot));
        }
        
        private class ScriptBlock : IDisposable
        {
            private readonly Action<IHtmlContent> callback;

            private ViewContext viewContext;
            private TextWriter originalWriter;
            private HtmlTextWriter scriptWriter;
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
                    isDisposed = true;
                }
            }
        }
    }
}