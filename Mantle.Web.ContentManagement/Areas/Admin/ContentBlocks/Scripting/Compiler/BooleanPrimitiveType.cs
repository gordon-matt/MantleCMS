namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class BooleanPrimitiveType : PrimitiveType
    {
        private static BooleanPrimitiveType instance;

        public static BooleanPrimitiveType Instance
        {
            get { return instance ?? (instance = new BooleanPrimitiveType()); }
        }

        public override EvaluationResult EqualityOperator(EvaluationResult value, EvaluationResult other)
        {
            if (value.IsBool && other.IsBool)
                return Result(value.BoolValue == other.BoolValue);
            return Error("Boolean values can only be compared to other boolean values");
        }

        public override EvaluationResult ComparisonOperator(EvaluationResult value, EvaluationResult other)
        {
            return Error("Boolean values can only be compared to other boolean values");
        }

        public override EvaluationResult LogicalAnd(EvaluationResult value, EvaluationResult other)
        {
            if (!value.BoolValue)
                return value;
            return other;
        }

        public override EvaluationResult LogicalOr(EvaluationResult value, EvaluationResult other)
        {
            if (value.BoolValue)
                return value;
            return other;
        }
    }
}