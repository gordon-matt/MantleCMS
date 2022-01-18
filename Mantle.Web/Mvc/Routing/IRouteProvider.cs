using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Mvc.Routing
{
    public interface IRouteProvider
    {
        void RegisterRoutes(IRouteBuilder routes);

        void RegisterEndpoints(IEndpointRouteBuilder endpoints);

        int Priority { get; }
    }
}