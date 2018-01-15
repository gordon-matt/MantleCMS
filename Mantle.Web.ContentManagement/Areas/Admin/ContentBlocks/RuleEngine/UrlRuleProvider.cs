using System;
using Mantle.Web.Environment;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.RuleEngine
{
    public class UrlRuleProvider : IRuleProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UrlRuleProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public void Process(RuleContext ruleContext)
        {
            if (!string.Equals(ruleContext.FunctionName, "url", StringComparison.OrdinalIgnoreCase))
                return;

            var context = httpContextAccessor.Current();
            var url = Convert.ToString(ruleContext.Arguments[0]);
            if (url.StartsWith("~/"))
            {
                url = url.Substring(2);
                var appPath = context.Request.ApplicationPath;
                if (appPath == "/")
                    appPath = "";

                url = String.Concat(appPath, "/", url);
            }

            if (!url.Contains("?"))
                url = url.TrimEnd('/');

            var requestPath = context.Request.Path;
            if (!requestPath.Contains("?"))
                requestPath = requestPath.TrimEnd('/');

            ruleContext.Result = url.EndsWith("*")
                ? requestPath.StartsWith(url.TrimEnd('*'), StringComparison.OrdinalIgnoreCase)
                : string.Equals(requestPath, url, StringComparison.OrdinalIgnoreCase);
        }
    }
}