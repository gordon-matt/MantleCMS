//using System.Collections.Generic;
//using Mantle.Infrastructure;
//using Mantle.Web.Infrastructure;

//namespace Mantle.Web.Common.Infrastructure
//{
//    public class RequireJSConfigProvider : IRequireJSConfigProvider
//    {
//        #region IRequireJSConfigProvider Members

//        public IDictionary<string, string> Paths
//        {
//            get
//            {
//                var paths = new Dictionary<string, string>();

//                paths.Add("jquery-image-mapster", "/Mantle.Web.Common.Areas.Admin.Regions.Scripts.jquery.imagemapster");

//                return paths;
//            }
//        }

//        public IDictionary<string, string[]> Shim
//        {
//            get
//            {
//                var shim = new Dictionary<string, string[]>();

//                shim.Add("jquery-image-mapster", new[] { "jquery" });

//                return shim;
//            }
//        }

//        #endregion IRequireJSConfigProvider Members
//    }
//}