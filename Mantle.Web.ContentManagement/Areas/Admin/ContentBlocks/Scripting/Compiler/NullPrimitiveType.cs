namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class NullPrimitiveType : PrimitiveType
    {
        private static NullPrimitiveType instance;

        public static NullPrimitiveType Instance
        {
            get { return instance ?? (instance = new NullPrimitiveType()); }
        }

        public override EvaluationResult EqualityOperator(EvaluationResult value, EvaluationResult other)
        {
            return Result(value.IsNull && other.IsNull);
        }

        public override EvaluationResult ComparisonOperator(EvaluationResult value, EvaluationResult other)
        {
            return Error("'null' values can not be compared");
        }

        public override EvaluationResult LogicalAnd(EvaluationResult value, EvaluationResult other)
        {
            return value;
        }

        public override EvaluationResult LogicalOr(EvaluationResult value, EvaluationResult other)
        {
            return other;
        }
    }
}