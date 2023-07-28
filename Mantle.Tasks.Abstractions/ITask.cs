namespace Mantle.Tasks;

/// <summary>
/// Interface that should be implemented by each task
/// </summary>
public partial interface ITask
{
    string Name { get; }

    int DefaultInterval { get; }

    void Execute();
}