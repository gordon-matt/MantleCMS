namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class ErrorPrimitiveType : PrimitiveType
    {
        private static ErrorPrimitiveType instance;

        public static ErrorPrimitiveType Instance
        {
            get { return instance ?? (instance = new ErrorPrimitiveType()); }
        }

        public override EvaluationResult EqualityOperator(EvaluationResult value, EvaluationResult other)
        {
            return value;
        }

        public override EvaluationResult ComparisonOperator(EvaluationResult value, EvaluationResult other)
        {
            return value;
        }

        public override EvaluationResult LogicalAnd(EvaluationResult value, EvaluationResult other)
        {
            return value;
        }

        public override EvaluationResult LogicalOr(EvaluationResult value, EvaluationResult other)
        {
            return value;
        }
    }
}