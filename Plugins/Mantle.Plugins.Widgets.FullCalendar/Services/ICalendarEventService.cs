using Mantle.Caching;
using Mantle.Data;
using Mantle.Data.Services;
using Mantle.Plugins.Widgets.FullCalendar.Data.Domain;

namespace Mantle.Plugins.Widgets.FullCalendar.Services
{
    public interface ICalendarEventService : IGenericDataService<CalendarEvent>
    {
    }

    public class CalendarEventService : GenericDataService<CalendarEvent>, ICalendarEventService
    {
        public CalendarEventService(ICacheManager cacheManager, IRepository<CalendarEvent> repository)
            : base(cacheManager, repository)
        {
        }
    }
}