using System;
using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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

        public override void IncludeInline(string code, ResourceLocation? location = null, bool ignoreExists = false)
        {
            base.IncludeInline(
                string.Concat("<script type=\"text/javascript\">", code, "</script>"),
                location,
                ignoreExists);
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
    }
}