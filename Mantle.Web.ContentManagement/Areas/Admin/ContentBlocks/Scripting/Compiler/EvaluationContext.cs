using System;
using System.Collections.Generic;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Ast;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class EvaluationContext
    {
        public AbstractSyntaxTree Tree { get; set; }

        public Func<string, IList<object>, object> MethodInvocationCallback { get; set; }
    }
}