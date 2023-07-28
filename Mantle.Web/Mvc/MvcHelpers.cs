namespace Mantle.Web.Mvc;

public static class MvcHelpers
{
    private static ActionContext actionContext;
    private static IUrlHelper urlHelper;

    public static ActionContext ActionContext
    {
        get
        {
            if (actionContext == null)
            {
                var serviceProvider = EngineContext.Current.Resolve<IServiceProvider>();

                actionContext = new ActionContext(
                    new DefaultHttpContext { RequestServices = serviceProvider },
                    new RouteData(),
                    new ActionDescriptor());
            }
            return actionContext;
        }
    }

    public static IUrlHelper UrlHelper
    {
        get
        {
            if (urlHelper == null)
            {
                var urlHelperFactory = EngineContext.Current.Resolve<IUrlHelperFactory>();
                urlHelper = urlHelperFactory.GetUrlHelper(ActionContext);
            }
            return urlHelper;
        }
    }
}