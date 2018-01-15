using System;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public abstract class PrimitiveType
    {
        public static PrimitiveType InstanceFor(object value)
        {
            if (value == null)
                return NullPrimitiveType.Instance;
            if (value is bool)
                return BooleanPrimitiveType.Instance;
            if (value is int)
                return IntegerPrimitiveType.Instance;
            if (value is string)
                return StringPrimitiveType.Instance;
            if (value is Error)
                return ErrorPrimitiveType.Instance;
            throw new InvalidOperationException(string.Format("Scripting engine internal error: no primitive type for value '{0}'", value));
        }

        public abstract EvaluationResult EqualityOperator(EvaluationResult value, EvaluationResult other);

        public abstract EvaluationResult ComparisonOperator(EvaluationResult value, EvaluationResult other);

        public abstract EvaluationResult LogicalAnd(EvaluationResult value, EvaluationResult other);

        public abstract EvaluationResult LogicalOr(EvaluationResult value, EvaluationResult other);

        protected EvaluationResult Result(object value)
        {
            return EvaluationResult.Result(value);
        }

        protected EvaluationResult Error(string message)
        {
            return EvaluationResult.Error(message);
        }
    }
}