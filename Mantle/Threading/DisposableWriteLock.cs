namespace Mantle.Threading;

/// <summary>
/// Provides a convenience methodology for implementing locked access to resources.
/// </summary>
/// <remarks>
/// Intended as an infrastructure class.
/// </remarks>
public class DisposableWriteLock : IDisposable
{
    private readonly ReaderWriterLockSlim readerWriterLockSlim;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableWriteLock"/> class.
    /// </summary>
    /// <param name="readerWriterLockSlim">The rw lock.</param>
    public DisposableWriteLock(ReaderWriterLockSlim readerWriterLockSlim)
    {
        this.readerWriterLockSlim = readerWriterLockSlim;
        this.readerWriterLockSlim.EnterWriteLock();
    }

    void IDisposable.Dispose() => readerWriterLockSlim.ExitWriteLock();
}