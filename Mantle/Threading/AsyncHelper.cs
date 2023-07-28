namespace Mantle.Threading;

// Taken from:
// http://www.symbolsource.org/MyGet/Metadata/aspnetwebstacknightly/Project/Microsoft.AspNet.Identity.Core/2.0.0-beta1-140203/Release/Default/Microsoft.AspNet.Identity.Core/Microsoft.AspNet.Identity.Core/AsyncHelper.cs?ImageName=Microsoft.AspNet.Identity.Core
public static class AsyncHelper
{
    private static readonly TaskFactory taskFactory = new(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        return taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
    }

    public static void RunSync(Func<Task> func)
    {
        taskFactory.StartNew(func).Unwrap().GetAwaiter().GetResult();
    }
}