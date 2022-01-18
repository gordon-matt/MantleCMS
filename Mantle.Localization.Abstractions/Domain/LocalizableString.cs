using System;
using System.Runtime.Serialization;
using Mantle.Tenants.Domain;

namespace Mantle.Localization.Domain
{
    [DataContract]
    public class LocalizableString : TenantEntity<Guid>
    {
        [DataMember]
        public string CultureCode { get; set; }

        [DataMember]
        public string TextKey { get; set; }

        [DataMember]
        public string TextValue { get; set; }
    }
}