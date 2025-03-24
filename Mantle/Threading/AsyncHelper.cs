namespace Mantle.Threading;

// Based on:
// http://www.symbolsource.org/MyGet/Metadata/aspnetwebstacknightly/Project/Microsoft.AspNet.Identity.Core/2.0.0-beta1-140203/Release/Default/Microsoft.AspNet.Identity.Core/Microsoft.AspNet.Identity.Core/AsyncHelper.cs?ImageName=Microsoft.AspNet.Identity.Core
public static class AsyncHelper
{
    public static TResult RunSync<TResult>(Func<Task<TResult>> func) => func().GetAwaiter().GetResult();

    public static void RunSync(Func<Task> func) => func().GetAwaiter().GetResult();
}