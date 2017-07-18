namespace Mantle.Infrastructure
{
    public interface IStartupTask
    {
        void Execute();

        int Order { get; }
    }
}