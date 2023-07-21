using Extenso.Data.Entity;

namespace Mantle.Security.Membership
{
    public class MantleUserProfileEntry : BaseEntity<string>
    {
        public string UserId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}