//using System.Collections.Generic;
//using Mantle.Web.Infrastructure;

//namespace Mantle.Web.ContentManagement.Infrastructure
//{
//    public class RequireJSConfigProvider : IRequireJSConfigProvider
//    {
//        #region IRequireJSConfigProvider Members

//        public IDictionary<string, string> Paths
//        {
//            get
//            {
//                var paths = new Dictionary<string, string>();

//                paths.Add("mantle-EXAMPLE", "/Mantle.Web.ContentManagement.Scripts.mantle-EXAMPLE");

//                return paths;
//            }
//        }

//        public IDictionary<string, string[]> Shim
//        {
//            get
//            {
//                var shim = new Dictionary<string, string[]>();

//                shim.Add("mantle-EXAMPLE", new[] { "chosen", "knockout" });

//                return shim;
//            }
//        }

//        #endregion IRequireJSConfigProvider Members
//    }
//}