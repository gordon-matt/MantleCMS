using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Mvc.Routing
{
    public interface IRouteProvider
    {
        void RegisterRoutes(IRouteBuilder routes);

        int Priority { get; }
    }
}