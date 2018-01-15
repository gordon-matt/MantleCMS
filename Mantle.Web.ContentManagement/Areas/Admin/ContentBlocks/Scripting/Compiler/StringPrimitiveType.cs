namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class StringPrimitiveType : PrimitiveType
    {
        private static StringPrimitiveType instance;

        public static StringPrimitiveType Instance
        {
            get { return instance ?? (instance = new StringPrimitiveType()); }
        }

        public override EvaluationResult EqualityOperator(EvaluationResult value, EvaluationResult other)
        {
            if (value.IsString && other.IsString)
                return Result(value.StringValue == other.StringValue);
            return Result(false);
        }

        public override EvaluationResult ComparisonOperator(EvaluationResult value, EvaluationResult other)
        {
            return Error("String values can not be compared");
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