using System.Collections.Generic;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Ast
{
    public class BinaryAstNode : AstNode, IAstNodeWithToken
    {
        private readonly AstNode left;
        private readonly Token token;
        private readonly AstNode right;

        public BinaryAstNode(AstNode left, Token token, AstNode right)
        {
            this.left = left;
            this.token = token;
            this.right = right;
        }

        public Token Token
        {
            get { return token; }
        }

        public Token Operator
        {
            get { return token; }
        }

        public override object Accept(AstVisitor visitor)
        {
            return visitor.VisitBinary(this);
        }

        public override IEnumerable<AstNode> Children
        {
            get
            {
                return new List<AstNode>(2) { left, right };
            }
        }

        public AstNode Left { get { return left; } }

        public AstNode Right { get { return right; } }
    }
}