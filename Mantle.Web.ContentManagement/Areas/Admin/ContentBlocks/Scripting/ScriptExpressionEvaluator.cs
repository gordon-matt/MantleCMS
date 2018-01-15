using System.Collections.Generic;
using System.Linq;
using Mantle.Caching;
using Mantle.Exceptions;
using Mantle.Localization;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Ast;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting
{
    public class ScriptExpressionEvaluator : IScriptExpressionEvaluator
    {
        private readonly ICacheManager cacheManager;

        public ScriptExpressionEvaluator(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public object Evaluate(string expression, IEnumerable<IGlobalMethodProvider> providers, object model = null)
        {
            string key = string.Format(CmsConstants.CacheKeys.ContentBlockScriptExpression, expression);
            var expr = cacheManager.Get(key, () =>
            {
                var ast = ParseExpression(expression);
                return new { Tree = ast, Errors = ast.GetErrors().ToList() };
            });

            if (expr.Errors.Any())
            {
                throw new MantleException(string.Format("Syntax error: {0}", expr.Errors.First().Message));
            }

            var result = EvaluateExpression(expr.Tree, model, providers);
            if (result.IsError)
            {
                throw new MantleException(result.ErrorValue.Message);
            }

            return result.Value;
        }

        private AbstractSyntaxTree ParseExpression(string expression)
        {
            return new Parser(expression).Parse();
        }

        private EvaluationResult EvaluateExpression(AbstractSyntaxTree tree, object model, IEnumerable<IGlobalMethodProvider> providers)
        {
            var context = new EvaluationContext
            {
                Tree = tree,
                MethodInvocationCallback = (m, args) => Evaluate(providers, m, model, args)
            };
            return new Interpreter().Evalutate(context);
        }

        private object Evaluate(IEnumerable<IGlobalMethodProvider> globalMethodProviders, string name, object model, IEnumerable<object> args)
        {
            var globalMethodContext = new GlobalMethodContext
            {
                FunctionName = name,
                Arguments = args.ToArray(),
                Result = null
            };

            foreach (var globalMethodProvider in globalMethodProviders)
            {
                globalMethodProvider.Process(globalMethodContext, model);
            }

            return globalMethodContext.Result;
        }
    }
}