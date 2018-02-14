using System;
using Mantle.Plugins.Widgets.FullCalendar.Data.Domain;
using Mantle.Web.Infrastructure;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure
{
    public class ODataRegistrar : IODataRegistrar
    {
        #region IODataRegistrar Members

        public void Register(IRouteBuilder routes, IServiceProvider services)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder(services);
            builder.EntitySet<Calendar>("CalendarApi");
            builder.EntitySet<CalendarEvent>("CalendarEventApi");

            routes.MapODataServiceRoute("OData_Mantle_Plugin_FullCalendar", "odata/Mantle/plugins/full-calendar", builder.GetEdmModel());
        }

        #endregion IODataRegistrar Members
    }
}