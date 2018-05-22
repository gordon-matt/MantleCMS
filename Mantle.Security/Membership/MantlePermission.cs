using Extenso.Data.Entity;
using Newtonsoft.Json;

namespace Mantle.Security.Membership
{
    public class MantlePermission : IEntity
    {
        public string Id { get; set; }

        public int? TenantId { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        #region IEntity Members

        [JsonIgnore]
        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}