using Extenso.Data.Entity;

namespace Mantle.Security.Membership
{
    public class MantleUser : BaseEntity<string>
    {
        public int? TenantId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsLockedOut { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }
}