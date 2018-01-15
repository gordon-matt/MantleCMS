using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.RuleEngine
{
    public class BuiltinRuleProvider : IRuleProvider
    {
        public void Process(RuleContext ruleContext)
        {
            if (ruleContext.FunctionName == "startsWith")
            {
                ruleContext.Result = Convert.ToString(ruleContext.Arguments[0]).StartsWith(Convert.ToString(ruleContext.Arguments[1]));
                return;
            }

            if (ruleContext.FunctionName == "contains")
            {
                ruleContext.Result = Convert.ToString(ruleContext.Arguments[0]).Contains(Convert.ToString(ruleContext.Arguments[1]));
                return;
            }

            if (ruleContext.FunctionName == "date")
            {
                ruleContext.Result = DateTime.ParseExact(Convert.ToString(ruleContext.Arguments[0]), "yyyy-MM-dd", null);
                return;
            }

            if (ruleContext.FunctionName == "today")
            {
                ruleContext.Result = DateTime.UtcNow.Date;
                return;
            }

            if (ruleContext.FunctionName == "P")
            {
                if (ruleContext.Model == null)
                {
                    ruleContext.Result = null;
                    return;
                }

                var propertyName = Convert.ToString(ruleContext.Arguments[0]);
                var type = ruleContext.Model.GetType();

                if (string.IsNullOrEmpty(type.Namespace))
                {
                    // Anonymous Object
                    var dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(ruleContext.Model);
                    ruleContext.Result = dictionary.ContainsKey(propertyName) ? dictionary[propertyName] : null;
                }
                else
                {
                    var properties = type.GetProperties();
                    var property = properties.FirstOrDefault(x => x.Name == propertyName);
                    ruleContext.Result = property != null ? property.GetValue(ruleContext.Model) : null;
                }
            }
        }
    }
}