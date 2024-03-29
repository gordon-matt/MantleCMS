﻿using Mantle.Tenants;
using Mantle.Tenants.Entities;
using Mantle.Web.Navigation.Breadcrumbs;
using System.Collections.Concurrent;

namespace Mantle.Web;

public partial class WorkContext : IWorkContext
{
    private Tenant cachedTenant;

    private readonly IWebHelper webHelper;
    private readonly ConcurrentDictionary<string, Func<object>> stateResolvers = new();
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

                // Load the first found tenant
                tenant ??= allTenants.FirstOrDefault();
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