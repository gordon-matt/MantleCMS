//using System;
//using System.Globalization;
//using System.Threading;
//using Mantle.Caching;
//using Mantle.Web.IO.FileSystems.AppData;

//namespace Mantle.Web.IO.FileSystems.LockFile
//{
//    public class DefaultLockFileManager : ILockFileManager
//    {
//        private readonly IAppDataFolder appDataFolder;
//        private readonly IClock clock;
//        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

//        public static TimeSpan Expiration { get; private set; }

//        public DefaultLockFileManager(IAppDataFolder appDataFolder, IClock clock)
//        {
//            this.appDataFolder = appDataFolder;
//            this.clock = clock;
//            Expiration = TimeSpan.FromMinutes(10);
//        }

//        public bool TryAcquireLock(string path, ref ILockFile lockFile)
//        {
//            if (!rwLock.TryEnterWriteLock(0))
//            {
//                return false;
//            }

//            try
//            {
//                if (IsLockedImpl(path))
//                {
//                    return false;
//                }

//                lockFile = new LockFile(appDataFolder, path, clock.UtcNow.ToString(CultureInfo.InvariantCulture), rwLock);
//                return true;
//            }
//            catch
//            {
//                // an error occured while reading/creating the lock file
//                return false;
//            }
//            finally
//            {
//                rwLock.ExitWriteLock();
//            }
//        }

//        public bool IsLocked(string path)
//        {
//            rwLock.EnterWriteLock();

//            try
//            {
//                return IsLockedImpl(path);
//            }
//            catch
//            {
//                // an error occured while reading the file
//                return true;
//            }
//            finally
//            {
//                rwLock.ExitWriteLock();
//            }
//        }

//        private bool IsLockedImpl(string path)
//        {
//            if (appDataFolder.FileExists(path))
//            {
//                var content = appDataFolder.ReadFile(path);

//                DateTime creationUtc;
//                if (DateTime.TryParse(content, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out creationUtc))
//                {
//                    // if expired the file is not removed
//                    // it should be automatically as there is a finalizer in LockFile
//                    // or the next taker can do it, unless it also fails, again
//                    return creationUtc.Add(Expiration) > clock.UtcNow;
//                }
//            }

//            return false;
//        }
//    }
//}