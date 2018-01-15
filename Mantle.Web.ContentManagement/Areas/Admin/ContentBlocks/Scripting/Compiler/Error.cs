using System;

namespace Mantle.Web.ContentManagement.Areas.Admin.ContentBlocks.Scripting.Compiler
{
    public class Error
    {
        public string Message { get; set; }

        public Exception Exception { get; set; }

        public override string ToString()
        {
            return string.Format("Error: {0}", Message);
        }
    }
}