//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Mantle.Infrastructure;
//using Mantle.Web.Infrastructure;
//using Mantle.Web.Mvc.Resources;

//namespace Mantle.Web.ContentManagement.Infrastructure
//{
//    public class RequireJSConfigProvider : IRequireJSConfigProvider
//    {
//        #region IRequireJSConfigProvider Members

//        public IDictionary<string, string> Paths
//        {
//            get
//            {
//                var workContext = EngineContext.Current.Resolve<IWebWorkContext>();
//                var scriptRegister = new ScriptRegister(workContext);

//                var paths = new Dictionary<string, string>();

//                paths.Add("blog-posts", scriptRegister.GetBundleUrl("kore-cms/blog-posts"));

//                return paths;
//            }
//        }

//        public IDictionary<string, string[]> Shim
//        {
//            get
//            {
//                var shim = new Dictionary<string, string[]>();

//                shim.Add("blog", new[] { "blog-posts", "blog-categories", "blog-tags" });

//                return shim;
//            }
//        }

//        #endregion
//    }
//}

