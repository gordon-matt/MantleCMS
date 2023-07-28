using Extenso.Data.Entity;

namespace Mantle.Tenants.Domain;

public class Tenant : BaseEntity<int>
{
    public string Name { get; set; }

    public string Url { get; set; }

    // TODO: Support SSL
    //public bool SslEnabled { get; set; }

    //public string SecureUrl { get; set; }

    public string Hosts { get; set; }
}