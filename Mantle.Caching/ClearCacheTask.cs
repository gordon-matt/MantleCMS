using Mantle.Tasks;

namespace Mantle.Caching
{
    /// <summary>
    /// Clear cache schedueled task implementation
    /// </summary>
    public class ClearCacheTask : ITask
    {
        public string Name
        {
            get { return "Clear Cache Task"; }
        }

        public int DefaultInterval
        {
            get { return 600; }
        }

        public void Execute()
        {
            //TODO
        }
    }
}