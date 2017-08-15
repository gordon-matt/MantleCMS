using System;
using System.Threading;

namespace Mantle.ComponentModel
{
    /// <summary>
    /// Provides a convenience methodology for implementing locked access to resources.
    /// </summary>
    /// <remarks>
    /// Intended as an infrastructure class.
    /// </remarks>
    public class WriteLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim readerWriterLockSlim;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteLockDisposable"/> class.
        /// </summary>
        /// <param name="readerWriterLockSlim">The rw lock.</param>
        public WriteLockDisposable(ReaderWriterLockSlim readerWriterLockSlim)
        {
            this.readerWriterLockSlim = readerWriterLockSlim;
            this.readerWriterLockSlim.EnterWriteLock();
        }

        void IDisposable.Dispose()
        {
            readerWriterLockSlim.ExitWriteLock();
        }
    }
}