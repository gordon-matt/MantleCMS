using Mantle.Tenants.Domain;
using System.Runtime.Serialization;

namespace Mantle.Localization.Domain
{
    [DataContract]
    public class Language : ITenantEntity
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public int? TenantId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CultureCode { get; set; }

        [DataMember]
        public bool IsRTL { get; set; }

        [DataMember]
        public bool IsEnabled { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}