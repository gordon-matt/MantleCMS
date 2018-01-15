using System.Collections.Generic;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Ast
{
    public class ErrorNodeCollector : AstVisitor
    {
        private readonly List<ErrorAstNode> errors = new List<ErrorAstNode>();

        public IEnumerable<ErrorAstNode> Collect(AstNode root)
        {
            Visit(root);
            return errors;
        }

        public override object Visit(AstNode node)
        {
            object result = node.Accept(this);
            VisitChildren(node);
            return result;
        }

        public override object VisitError(ErrorAstNode node)
        {
            errors.Add(node);
            return null;
        }
    }
}