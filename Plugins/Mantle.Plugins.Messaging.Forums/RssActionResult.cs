using SyndicationFeed = WilderMinds.RssSyndication.Feed;

namespace Mantle.Plugins.Messaging.Forums;

public class RssActionResult : IActionResult
{
    public SyndicationFeed Feed { get; set; }

    public Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.ContentType = "application/rss+xml";
        response.StatusCode = 200;

        string content = Feed.Serialize();
        var bytes = Encoding.UTF8.GetBytes(content);
        context.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        return null;//TODO
        //return TaskCache.CompletedTask;
    }
}