using Mantle.Tenants.Entities;
using System.Runtime.Serialization;

namespace Mantle.Localization.Entities;

[DataContract]
public class Language : TenantEntity<Guid>
{
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
}