using System.Collections.Generic;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting
{
    public interface IGlobalMethodProvider
    {
        void Process(GlobalMethodContext context, object model = null);
    }

    public class GlobalMethodContext
    {
        public string FunctionName { get; set; }

        public IList<object> Arguments { get; set; }

        public object Result { get; set; }
    }
}