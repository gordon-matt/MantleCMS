using System.Diagnostics;

namespace Mantle.Security.Membership;

[DebuggerDisplay("{UserName}")]
public class MantleUser : BaseEntity<string>
{
    public int? TenantId { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public bool IsLockedOut { get; set; }
}