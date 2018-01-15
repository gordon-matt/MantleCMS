namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.RuleEngine
{
    public class RuleContext
    {
        public string FunctionName { get; set; }

        public object Model { get; set; }

        public object[] Arguments { get; set; }

        public object Result { get; set; }
    }
}