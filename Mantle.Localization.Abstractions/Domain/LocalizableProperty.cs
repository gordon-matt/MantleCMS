using Extenso.Data.Entity;
using System.Runtime.Serialization;

namespace Mantle.Localization.Domain
{
    [DataContract]
    public class LocalizableProperty : IEntity
    {
        [DataMember]
        public int Id { get; set; }

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

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }
}