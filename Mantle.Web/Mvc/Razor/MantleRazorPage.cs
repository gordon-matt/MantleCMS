using System.Collections.Generic;
using System.Globalization;
using Mantle.Infrastructure;
using Mantle.Web.Configuration;
using Mantle.Web.Navigation;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Mvc.Razor
{
    public abstract class MantleRazorPage<TModel> : RazorPage<TModel>
    {
        public IEnumerable<MenuItem> GetMenu(string menuName)
        {
            return EngineContext.Current.Resolve<INavigationManager>().BuildMenu(menuName);
        }

        [RazorInject]
        public SiteSettings SiteSettings { get; set; }

        [RazorInject]
        public IStringLocalizer T { get; set; }

        [RazorInject]
        public IWebWorkContext WorkContext { get; set; }

        public bool IsRightToLeft
        {
            get { return CultureInfo.CurrentCulture.TextInfo.IsRightToLeft; }
        }
    }

    public abstract class MantleRazorPage : MantleRazorPage<dynamic>
    {
    }
}