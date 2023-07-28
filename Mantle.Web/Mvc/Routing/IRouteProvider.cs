namespace Mantle.Web.Mvc.Routing;

public interface IRouteProvider
{
    void RegisterRoutes(IEndpointRouteBuilder endpoints);

    void RegisterEndpoints(IEndpointRouteBuilder endpoints);

    int Priority { get; }
}