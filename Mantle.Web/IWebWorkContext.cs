using Mantle.Web.Navigation;

namespace Mantle.Web
{
    public interface IWebWorkContext : IWorkContext
    {
        BreadcrumbCollection Breadcrumbs { get; set; }

        string CurrentTheme { get; }
    }
}