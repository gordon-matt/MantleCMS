// Based on code by Ewout Stortenbeker
// CodeEngine Framework (http://www.code-engine.com/)
// Email: 4ewout@gmail.com
// The version used in here has been heavily modified from the original

namespace Mantle.Data.QueryBuilder
{
    public enum JoinType : byte
    {
        InnerJoin,
        OuterJoin,
        LeftJoin,
        RightJoin
    }
}