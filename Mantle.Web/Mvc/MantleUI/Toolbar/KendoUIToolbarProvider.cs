//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Mantle.Web.Mvc.MantleUI
//{
//    // This is actually going to take quite a bit of work:
//    //  http://demos.telerik.com/kendo-ui/toolbar/index
//    public class KendoUIToolbarProvider : IToolbarProvider
//    {
//        #region IToolbarProvider Members

//        public string ToolbarTag
//        {
//            get { return "div"; }
//        }

//        public void BeginToolbar(Toolbar toolbar)
//        {
//            //toolbar.EnsureHtmlAttribute("role", "toolbar");
//        }

//        public void BeginButtonGroup(TextWriter writer)
//        {
//            writer.Write(@"<div class=""btn-group"">");
//        }

//        public void EndButtonGroup(TextWriter writer)
//        {
//            writer.Write("</div>");
//        }

//        public void AddButton(TextWriter writer, string text, State state, string onClick = null, object htmlAttributes = null)
//        {
//            var button = MantleUISettings.DefaultAdminProvider.Button(text, state, onClick, htmlAttributes);
//            writer.Write(button.ToString());
//        }

//        #endregion IToolbarProvider Members
//    }
//}