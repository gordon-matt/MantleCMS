namespace Mantle.Web.Mvc.Routing;

public interface IRoutePublisher
{
    void RegisterEndpoints(IEndpointRouteBuilder endpoints);
}