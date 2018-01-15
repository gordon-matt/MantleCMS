namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class Interpreter
    {
        public EvaluationResult Evalutate(EvaluationContext context)
        {
            return new InterpreterVisitor(context).Evaluate();
        }
    }
}