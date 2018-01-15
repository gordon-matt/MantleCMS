using System;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.RuleEngine
{
    public class DisabledRuleProvider : IRuleProvider
    {
        public void Process(RuleContext ruleContext)
        {
            if (!string.Equals(ruleContext.FunctionName, "disabled", StringComparison.OrdinalIgnoreCase))
                return;
            ruleContext.Result = false;
        }
    }
}