using Mantle.Security.Membership;
using Mantle.Tenants.Domain;
using Mantle.Web.Navigation;

namespace Mantle.Web
{
    public interface IWorkContext
    {
        T GetState<T>(string name);

        void SetState<T>(string name, T value);

        string CurrentCultureCode { get; }

        Tenant CurrentTenant { get; }

        MantleUser CurrentUser { get; }

        BreadcrumbCollection Breadcrumbs { get; set; }

        string CurrentTheme { get; }
    }
}