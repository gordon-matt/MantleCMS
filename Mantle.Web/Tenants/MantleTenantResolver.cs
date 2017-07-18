using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mantle.Tenants;
using Mantle.Tenants.Domain;
using Mantle.Tenants.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SaasKit.Multitenancy;

namespace Mantle.Web.Tenants
{
    public class MantleTenantResolver : MemoryCacheTenantResolver<Tenant>
    {
        private readonly IEnumerable<Tenant> tenants;

        public MantleTenantResolver(
            IMemoryCache cache,
            ILoggerFactory loggerFactory,
            ITenantService tenantService)
            : base(cache, loggerFactory)
        {
            tenants = tenantService.Find();
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            return context.Request.Host.Value.ToLower();
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<Tenant> context)
        {
            return context.Tenant.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        protected override Task<TenantContext<Tenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<Tenant> tenantContext = null;

            var loggerFactory = Mantle.Infrastructure.EngineContext.Current.Resolve<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(this.GetType());

            string host = context.Request.Host.Value.ToLower();

            logger.LogInformation("[Host]: " + host);

            var tenant = tenants.FirstOrDefault(s => s.ContainsHostValue(host));

            if (tenant != null)
            {
                logger.LogInformation("[Tenant]: ID: {0}, Name: {1}, Hosts: {2}", tenant.Id, tenant.Name, tenant.Hosts);
                tenantContext = new TenantContext<Tenant>(tenant);
            }

            return Task.FromResult(tenantContext);
        }

        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(new TimeSpan(24, 0, 0)); // Cache for 24 hours
        }
    }
}