using Mantle.Tenants.Domain;
using System.Runtime.Serialization;

namespace Mantle.Localization.Domain;

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