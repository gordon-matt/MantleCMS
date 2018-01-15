namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class IntegerPrimitiveType : PrimitiveType
    {
        private static IntegerPrimitiveType instance;

        public static IntegerPrimitiveType Instance
        {
            get { return instance ?? (instance = new IntegerPrimitiveType()); }
        }

        public override EvaluationResult EqualityOperator(EvaluationResult value, EvaluationResult other)
        {
            if (value.IsInt32 && other.IsInt32)
                return Result(value.Int32Value == other.Int32Value);
            return Error("Integer values can only be compared to other integer values");
        }

        public override EvaluationResult ComparisonOperator(EvaluationResult value, EvaluationResult other)
        {
            if (value.IsInt32 && other.IsInt32)
                return Result(value.Int32Value.CompareTo(other.Int32Value));
            return Error("Integer values can only be compared to other integer values");
        }

        public override EvaluationResult LogicalAnd(EvaluationResult value, EvaluationResult other)
        {
            return other;
        }

        public override EvaluationResult LogicalOr(EvaluationResult value, EvaluationResult other)
        {
            return value;
        }
    }
}