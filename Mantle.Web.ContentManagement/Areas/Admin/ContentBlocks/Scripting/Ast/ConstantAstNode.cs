using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Ast
{
    public class ConstantAstNode : AstNode, IAstNodeWithToken
    {
        private readonly Token token;

        public ConstantAstNode(Token token)
        {
            this.token = token;
        }

        public Token Token
        {
            get { return token; }
        }

        public object Value { get { return token.Value; } }

        public override object Accept(AstVisitor visitor)
        {
            return visitor.VisitConstant(this);
        }
    }
}