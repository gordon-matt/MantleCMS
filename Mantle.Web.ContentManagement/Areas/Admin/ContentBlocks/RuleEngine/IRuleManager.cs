using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Exceptions;
using Mantle.Localization;
using Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.RuleEngine
{
    public interface IRuleManager
    {
        bool Matches(string expression, object model = null);
    }

    public class RuleManager : IRuleManager
    {
        private readonly Lazy<ILogger> logger;
        private readonly IEnumerable<IRuleProvider> ruleProviders;
        private readonly IEnumerable<IScriptExpressionEvaluator> evaluators;

        public RuleManager(
            Lazy<ILogger> logger,
            IEnumerable<IRuleProvider> ruleProviders,
            IEnumerable<IScriptExpressionEvaluator> evaluators)
        {
            this.ruleProviders = ruleProviders;
            this.evaluators = evaluators;
            T = NullLocalizer.Instance;
            this.logger = logger;
        }

        public Localizer T { get; set; }

        public bool Matches(string expression, object model = null)
        {
            var evaluator = evaluators.FirstOrDefault();
            if (evaluator == null)
            {
                throw new MantleException("There are currently no scripting engines enabled.");
            }

            object result;

            try
            {
                result = evaluator.Evaluate(expression, new List<IGlobalMethodProvider> { new GlobalMethodProvider(this) }, model);
            }
            catch
            {
                return false;
            }

            if (result == null)
            {
                logger.Value.Error("Expression is not a boolean value: " + expression);
                throw new MantleException("Expression is not a boolean value.");
            }
            return (bool)result;
        }

        private class GlobalMethodProvider : IGlobalMethodProvider
        {
            private readonly RuleManager ruleManager;

            public GlobalMethodProvider(RuleManager ruleManager)
            {
                this.ruleManager = ruleManager;
            }

            public void Process(GlobalMethodContext context, object model = null)
            {
                var ruleContext = new RuleContext
                {
                    FunctionName = context.FunctionName,
                    Model = model,
                    Arguments = context.Arguments.ToArray(),
                    Result = context.Result
                };

                foreach (var ruleProvider in ruleManager.ruleProviders)
                {
                    ruleProvider.Process(ruleContext);
                }

                context.Result = ruleContext.Result;
            }
        }
    }
}