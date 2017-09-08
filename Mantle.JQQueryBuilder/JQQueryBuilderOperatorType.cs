using System.Runtime.Serialization;

namespace Mantle.JQQueryBuilder
{
    public enum JQQueryBuilderOperatorType : byte
    {
        [EnumMember(Value = "equal")]
        Equal,

        [EnumMember(Value = "not_equal")]
        NotEqual,

        [EnumMember(Value = "in")]
        In,

        [EnumMember(Value = "not_in")]
        NotIn,

        [EnumMember(Value = "less")]
        LessThan,

        [EnumMember(Value = "less_or_equal")]
        LessThanOrEqual,

        [EnumMember(Value = "greater")]
        GreaterThan,

        [EnumMember(Value = "greater_or_equal")]
        GreaterThanOrEqual,

        [EnumMember(Value = "between")]
        Between,

        [EnumMember(Value = "not_between")]
        NotBetween,

        [EnumMember(Value = "begins_with")]
        BeginsWith,

        [EnumMember(Value = "not_begins_with")]
        NotBeginsWith,

        [EnumMember(Value = "contains")]
        Contains,

        [EnumMember(Value = "not_contains")]
        NotContains,

        [EnumMember(Value = "ends_with")]
        EndsWith,

        [EnumMember(Value = "not_ends_with")]
        NotEndsWith,

        [EnumMember(Value = "is_empty")]
        IsEmpty,

        [EnumMember(Value = "is_not_empty")]
        IsNotEmpty,

        [EnumMember(Value = "is_null")]
        IsNull,

        [EnumMember(Value = "is_not_null")]
        IsNotNull
    }
}