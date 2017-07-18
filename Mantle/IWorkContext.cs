using Mantle.Security.Membership;
using Mantle.Tenants.Domain;

namespace Mantle
{
    public interface IWorkContext
    {
        T GetState<T>(string name);

        void SetState<T>(string name, T value);

        string CurrentCultureCode { get; }

        Tenant CurrentTenant { get; }

        MantleUser CurrentUser { get; }
    }
}