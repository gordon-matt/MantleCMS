using Extenso.Data.Entity;

namespace Mantle.Security.Membership
{
    public class MantleRole : BaseEntity<string>
    {
        public int? TenantId { get; set; }

        public string Name { get; set; }
    }
}