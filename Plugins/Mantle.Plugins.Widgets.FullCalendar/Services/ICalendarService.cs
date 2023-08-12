using Extenso.Data.Entity;
using Mantle.Caching;
using Mantle.Data.Services;
using Mantle.Plugins.Widgets.FullCalendar.Data.Entities;

namespace Mantle.Plugins.Widgets.FullCalendar.Services
{
    public interface ICalendarService : IGenericDataService<Calendar>
    {
    }

    public class CalendarService : GenericDataService<Calendar>, ICalendarService
    {
        public CalendarService(ICacheManager cacheManager, IRepository<Calendar> repository)
            : base(cacheManager, repository)
        {
        }
    }
}