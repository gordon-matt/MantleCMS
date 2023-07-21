using Extenso.Data.Entity;

namespace Mantle.Security.Membership
{
    public class MantlePermission : BaseEntity<string>
    {
        public int? TenantId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }
    }
}