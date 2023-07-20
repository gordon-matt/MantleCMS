using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Mvc.Routing
{
    public interface IRouteProvider
    {
        void RegisterRoutes(IEndpointRouteBuilder endpoints);

        int Priority { get; }
    }
}