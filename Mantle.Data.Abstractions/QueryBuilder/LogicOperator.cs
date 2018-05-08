// Based on code by Ewout Stortenbeker
// CodeEngine Framework (http://www.code-engine.com/)
// Email: 4ewout@gmail.com
// The version used in here has been heavily modified from the original

namespace Mantle.Data.QueryBuilder
{
    /// <summary>
    /// Represents logic operators for chaining WHERE and HAVING clauses together in a statement
    /// </summary>
    public enum LogicOperator : byte
    {
        And = 0,
        Or = 1
    }
}