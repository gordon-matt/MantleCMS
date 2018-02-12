using System;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Infrastructure
{
    public interface IODataRegistrar
    {
        void Register(IRouteBuilder routes, IServiceProvider services);
    }
}