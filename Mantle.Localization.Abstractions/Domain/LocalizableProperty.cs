using System.Runtime.Serialization;

namespace Mantle.Localization.Domain;

[DataContract]
public class LocalizableProperty : BaseEntity<int>
{
    [DataMember]
    public string CultureCode { get; set; }

    [DataMember]
    public string EntityType { get; set; }

    [DataMember]
    public string EntityId { get; set; }

    [DataMember]
    public string Property { get; set; }

    [DataMember]
    public string Value { get; set; }
}