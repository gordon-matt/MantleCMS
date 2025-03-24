using Mantle.Tenants.Entities;

namespace Mantle.Tenants;

public static class TenantExtensions
{
    public static IEnumerable<string> ParseHostValues(this Tenant tenant)
    {
        if (tenant == null)
        {
            throw new ArgumentNullException(nameof(tenant));
        }

        var parsedValues = new List<string>();
        if (!string.IsNullOrEmpty(tenant.Hosts))
        {
            string[] hosts = tenant.Hosts.Split([','], StringSplitOptions.RemoveEmptyEntries);
            foreach (string host in hosts)
            {
                string tmp = host.Trim();
                if (!string.IsNullOrEmpty(tmp))
                {
                    parsedValues.Add(tmp);
                }
            }
        }
        return parsedValues;
    }

    public static bool ContainsHostValue(this Tenant tenant, string host) => tenant == null
        ? throw new ArgumentNullException(nameof(tenant))
        : !string.IsNullOrEmpty(host) && tenant.ParseHostValues().Any(x => x.Equals(host, StringComparison.OrdinalIgnoreCase));
}