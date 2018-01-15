//using System.Threading;
//using Mantle.Web.IO.FileSystems.AppData;

//namespace Mantle.Web.IO.FileSystems.LockFile
//{
//    /// <summary>
//    /// Represents a Lock File acquired on the file system
//    /// </summary>
//    /// <remarks>
//    /// The instance needs to be disposed in order to release the lock explicitly
//    /// </remarks>
//    public class LockFile : ILockFile
//    {
//        private readonly IAppDataFolder appDataFolder;
//        private readonly string path;
//        private readonly string content;
//        private readonly ReaderWriterLockSlim rwLock;
//        private bool isReleased;

//        public LockFile(IAppDataFolder appDataFolder, string path, string content, ReaderWriterLockSlim rwLock)
//        {
//            this.appDataFolder = appDataFolder;
//            this.path = path;
//            this.content = content;
//            this.rwLock = rwLock;

//            // create the physical lock file
//            this.appDataFolder.CreateFile(path, content);
//        }

//        public void Dispose()
//        {
//            Release();
//        }

//        public void Release()
//        {
//            rwLock.EnterWriteLock();

//            try
//            {
//                if (isReleased || !appDataFolder.FileExists(path))
//                {
//                    // nothing to do, might happen if re-granted, and already released
//                    return;
//                }

//                isReleased = true;

//                // check it has not been granted in the meantime
//                var current = appDataFolder.ReadFile(path);
//                if (current == content)
//                {
//                    appDataFolder.DeleteFile(path);
//                }
//            }
//            finally
//            {
//                rwLock.ExitWriteLock();
//            }
//        }
//    }
//}