namespace Mantle.Web.Mvc.Routing;

public interface IRouteProvider
{
    void RegisterEndpoints(IEndpointRouteBuilder endpoints);

    int Priority { get; }
}