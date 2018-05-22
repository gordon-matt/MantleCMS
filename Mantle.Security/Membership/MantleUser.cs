using Extenso.Data.Entity;
using Newtonsoft.Json;

namespace Mantle.Security.Membership
{
    public class MantleUser : IEntity
    {
        public string Id { get; set; }

        public int? TenantId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsLockedOut { get; set; }

        #region IEntity Members

        [JsonIgnore]
        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members

        public override string ToString()
        {
            return UserName;
        }
    }
}