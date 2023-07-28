namespace Mantle.Caching;

/// <summary>
/// Clear cache schedueled task implementation
/// </summary>
public class ClearCacheTask : ITask
{
    public string Name => "Clear Cache Task";

    public int DefaultInterval => 600;

    public void Execute()
    {
        //TODO
    }
}