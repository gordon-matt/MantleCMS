using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Extenso;
using Mantle.Exceptions;
using Mantle.Infrastructure;
using Mantle.Security.Membership;
using Mantle.Tenants;
using Mantle.Tenants.Domain;
using Mantle.Tenants.Services;
using Mantle.Web.Navigation;

namespace Mantle.Web
{
    public partial class WorkContext : IWorkContext
    {
        private Tenant cachedTenant;

        private readonly IWebHelper webHelper;
        private readonly ConcurrentDictionary<string, Func<object>> stateResolvers = new ConcurrentDictionary<string, Func<object>>();
        private readonly IEnumerable<IWorkContextStateProvider> workContextStateProviders;

        public WorkContext()
        {
            webHelper = EngineContext.Current.Resolve<IWebHelper>();
            workContextStateProviders = EngineContext.Current.ResolveAll<IWorkContextStateProvider>();
            Breadcrumbs = new BreadcrumbCollection();
        }

        #region IWorkContext Members

        public T GetState<T>(string name)
        {
            var resolver = stateResolvers.GetOrAdd(name, FindResolverForState<T>);
            return (T)resolver();
        }

        public void SetState<T>(string name, T value)
        {
            stateResolvers[name] = () => value;
        }

        public BreadcrumbCollection Breadcrumbs { get; set; }

        public string CurrentTheme
        {
            get => GetState<string>(MantleWebConstants.StateProviders.CurrentTheme);
            set => SetState(MantleWebConstants.StateProviders.CurrentTheme, value);
        }

        public string CurrentCultureCode => GetState<string>(MantleWebConstants.StateProviders.CurrentCultureCode);

        public MantleUser CurrentUser => GetState<MantleUser>(MantleWebConstants.StateProviders.CurrentUser);

        public virtual Tenant CurrentTenant
        {
            get
            {
                if (cachedTenant != null)
                {
                    return cachedTenant;
                }

                try
                {
                    // Try to determine the current tenant by HTTP_HOST
                    string host = webHelper.GetUrlHost();

                    if (host.Contains(":"))
                    {
                        host = host.LeftOf(':');
                    }

                    var tenantService = EngineContext.Current.Resolve<ITenantService>();
                    var allTenants = tenantService.Find();
                    var tenant = allTenants.FirstOrDefault(s => s.ContainsHostValue(host));

                    if (tenant == null)
                    {
                        // Load the first found tenant
                        tenant = allTenants.FirstOrDefault();
                    }
                    if (tenant == null)
                    {
                        throw new MantleException("No tenant could be loaded");
                    }

                    cachedTenant = tenant;
                    return cachedTenant;
                }
                catch
                {
                    return null;
                }
            }
        }

        #endregion IWorkContext Members

        private Func<object> FindResolverForState<T>(string name)
        {
            var resolver = workContextStateProviders
                .Select(wcsp => wcsp.Get<T>(name))
                .FirstOrDefault(value => !Equals(value, default(T)));

            if (resolver == null)
            {
                return () => default(T);
            }
            return () => resolver(this);
        }
    }
}