using Mantle.Data;
using Newtonsoft.Json;

namespace Mantle.Security.Membership
{
    public class MantleRole : IEntity
    {
        public string Id { get; set; }

        public int? TenantId { get; set; }

        public string Name { get; set; }

        #region IEntity Members

        [JsonIgnore]
        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}