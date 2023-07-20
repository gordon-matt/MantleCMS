using Extenso.AspNetCore.OData;
using Mantle.Plugins.Widgets.FullCalendar.Data.Domain;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure
{
    public class ODataRegistrar : IODataRegistrar
    {
        #region IODataRegistrar Members

        public void Register(ODataOptions options)
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Calendar>("CalendarApi");
            builder.EntitySet<CalendarEvent>("CalendarEventApi");

            options.AddRouteComponents("odata/Mantle/plugins/full-calendar", builder.GetEdmModel());
        }

        #endregion IODataRegistrar Members
    }
}