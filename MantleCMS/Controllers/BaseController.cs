//using Microsoft.AspNetCore.Mvc;

//namespace MantleCMS.Controllers
//{
//    public class BaseController : Controller
//    {
//        public virtual string RenderRazorPartialViewToString<TModel>(string viewName, TModel model)
//        {
//            var viewEngine = EngineContext.Current.Resolve<IRazorViewEngine>();
//            var serviceProvider = EngineContext.Current.Resolve<IServiceProvider>();
//            var tempDataProvider = EngineContext.Current.Resolve<ITempDataProvider>();

//            var actionContext = GetActionContext(serviceProvider);

//            var viewEngineResult = viewEngine.FindView(actionContext, viewName, false);
//            if (!viewEngineResult.Success)
//            {
//                throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", viewName));
//            }

//            var view = viewEngineResult.View;

//            using (var output = new StringWriter())
//            {
//                var viewContext = new ViewContext(
//                    actionContext,
//                    view,
//                    new ViewDataDictionary<TModel>(
//                        metadataProvider: new EmptyModelMetadataProvider(),
//                        modelState: new ModelStateDictionary())
//                    {
//                        Model = model
//                    },
//                    new TempDataDictionary(
//                        actionContext.HttpContext,
//                        tempDataProvider),
//                    output,
//                    new HtmlHelperOptions());

//                view.RenderAsync(viewContext).GetAwaiter().GetResult();

//                return output.ToString();
//            }
//        }

//        private ActionContext GetActionContext(IServiceProvider serviceProvider)
//        {
//            var httpContext = new DefaultHttpContext();
//            httpContext.RequestServices = serviceProvider;
//            var routeData = new RouteData();
//            routeData.DataTokens.Add("area", "Admin");
//            routeData.Values.Add("controller", "Home");
//            //routeData.Values.Add("action", "Shell");
//            return new ActionContext(httpContext, routeData, new ActionDescriptor());
//        }
//    }
//}