using System.Runtime.Serialization;

namespace Mantle.JQQueryBuilder
{
    public enum JQQueryBuilderCondition : byte
    {
        [EnumMember(Value = "AND")]
        And,

        [EnumMember(Value = "OR")]
        Or
    }
}