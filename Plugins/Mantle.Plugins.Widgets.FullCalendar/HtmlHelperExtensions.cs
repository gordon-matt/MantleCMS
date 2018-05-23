using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Extenso.Data.Entity;
using Mantle.Data;
using Mantle.Infrastructure;
using Mantle.Plugins.Widgets.FullCalendar.Data.Domain;
using Mantle.Web.Collections;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mantle.Plugins.Widgets.FullCalendar
{
    public static class HtmlHelperExtensions
    {
        public static FullCalendar<TModel> FullCalendar<TModel>(this IHtmlHelper<TModel> html) where TModel : class
        {
            return new FullCalendar<TModel>(html);
        }
    }

    public class FullCalendar<TModel>
        where TModel : class
    {
        private readonly IHtmlHelper<TModel> html;

        internal FullCalendar(IHtmlHelper<TModel> html)
        {
            this.html = html;
        }

        public IHtmlContent CalendarDropDownList(string name, int? selectedValue = null, string emptyText = null, object htmlAttributes = null)
        {
            var selectList = GetCalendarSelectList(selectedValue, emptyText);
            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public IHtmlContent CalendarDropDownListFor(Expression<Func<TModel, int>> expression, object htmlAttributes = null, string emptyText = null)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = GetCalendarSelectList(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        private static IEnumerable<SelectListItem> GetCalendarSelectList(int? selectedValue = null, string emptyText = null)
        {
            var repository = EngineContext.Current.Resolve<IRepository<Calendar>>();

            using (var connection = repository.OpenConnection())
            {
                return connection.Query()
                    .OrderBy(x => x.Name)
                    .ToHashSet()
                    .ToSelectList(
                        value => value.Id,
                        text => text.Name,
                        selectedValue,
                        emptyText);
            }
        }
    }
}