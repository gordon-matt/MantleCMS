//namespace Mantle.Web.IO.FileSystems.AppData
//{
//    /// <summary>
//    /// Abstraction over the root location of "~/App_Data", mainly to enable
//    /// unit testing of AppDataFolder.
//    /// </summary>
//    public interface IAppDataFolderRoot
//    {
//        /// <summary>
//        /// Virtual path of root ("~/App_Data")
//        /// </summary>
//        string RootPath { get; }

//        /// <summary>
//        /// Physical path of root (typically: MapPath(RootPath))
//        /// </summary>
//        string RootFolder { get; }
//    }

//    public class AppDataFolderRoot : IAppDataFolderRoot
//    {
//        private readonly IWebHelper webHelper;

//        public AppDataFolderRoot(IWebHelper webHelper)
//        {
//            this.webHelper = webHelper;
//        }

//        public string RootPath
//        {
//            get { return "~/App_Data"; }
//        }

//        public string RootFolder
//        {
//            get { return webHelper.MapPath(RootPath); }
//        }
//    }
//}