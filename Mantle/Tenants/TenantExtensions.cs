﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Tenants.Domain;

namespace Mantle.Tenants
{
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
                string[] hosts = tenant.Hosts.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string host in hosts)
                {
                    var tmp = host.Trim();
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        parsedValues.Add(tmp);
                    }
                }
            }
            return parsedValues;
        }

        public static bool ContainsHostValue(this Tenant tenant, string host)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }
            if (string.IsNullOrEmpty(host))
            {
                return false;
            }

            return tenant.ParseHostValues().Any(x => x.Equals(host, StringComparison.OrdinalIgnoreCase));
        }
    }
}