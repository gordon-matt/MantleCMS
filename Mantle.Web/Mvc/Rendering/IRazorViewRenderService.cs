using System;
using System.IO;
using System.Threading.Tasks;
using Mantle.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Mvc.Rendering
{
    // Based on: https://ppolyzos.com/2016/09/09/asp-net-core-render-view-to-string/
    public interface IRazorViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model = null, RouteData routeData = null);
    }

    public class RazorViewRenderService : IRazorViewRenderService
    {
        private readonly IRazorViewEngine razorViewEngine;
        private readonly ITempDataProvider tempDataProvider;

        public RazorViewRenderService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider)
        {
            this.razorViewEngine = razorViewEngine;
            this.tempDataProvider = tempDataProvider;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model = null, RouteData routeData = null)
        {
            ActionContext actionContext;
            if (routeData == null)
            {
                actionContext = MvcHelpers.ActionContext;
            }
            else
            {
                var serviceProvider = EngineContext.Current.Resolve<IServiceProvider>();
                actionContext = new ActionContext(
                    new DefaultHttpContext { RequestServices = serviceProvider },
                    routeData,
                    new ActionDescriptor());
            }

            using (var stringWriter = new StringWriter())
            {
                var viewResult = razorViewEngine.FindView(actionContext, viewName, false);
                if (viewResult.View == null)
                {
                    throw new ArgumentNullException("View", $"{viewName} does not match any available view");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    stringWriter,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return stringWriter.ToString();
            }
        }
    }
}