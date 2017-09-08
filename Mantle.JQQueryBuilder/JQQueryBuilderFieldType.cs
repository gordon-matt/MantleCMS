using System.Runtime.Serialization;

namespace Mantle.JQQueryBuilder
{
    public enum JQQueryBuilderFieldType : byte
    {
        [EnumMember(Value = "string")]
        String,

        [EnumMember(Value = "integer")]
        Integer,

        [EnumMember(Value = "double")]
        Double,

        [EnumMember(Value = "date")]
        Date,

        [EnumMember(Value = "time")]
        Time,

        [EnumMember(Value = "datetime")]
        DateTime,

        [EnumMember(Value = "boolean")]
        Boolean
    }
}