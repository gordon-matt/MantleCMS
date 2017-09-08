using System.Runtime.Serialization;

namespace Mantle.JQQueryBuilder
{
    public enum JQQueryBuilderInputType : byte
    {
        [EnumMember(Value = "text")]
        Text,

        [EnumMember(Value = "textarea")]
        TextArea,

        [EnumMember(Value = "radio")]
        Radio,

        [EnumMember(Value = "checkbox")]
        Checkbox,

        [EnumMember(Value = "select")]
        Select
    }
}