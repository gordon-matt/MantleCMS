namespace Mantle.Tasks;

/// <summary>
/// Represents task thread
/// </summary>
public partial class TaskThread : IDisposable
{
    private Timer _timer;
    private bool _disposed;
    private readonly Dictionary<string, Task> _tasks = [];

    private void Run()
    {
        if (Seconds <= 0)
        {
            return;
        }

        StartedUtc = DateTime.UtcNow;
        IsRunning = true;
        foreach (var task in _tasks.Values)
        {
            task.Execute();
        }
        IsRunning = false;
    }

    private void TimerHandler(object state)
    {
        _timer.Change(-1, -1);
        Run();
        if (RunOnlyOnce)
        {
            Dispose();
        }
        else
        {
            _timer.Change(Interval, Interval);
        }
    }

    /// <summary>
    /// Disposes the instance
    /// </summary>
    public void Dispose()
    {
        if ((_timer != null) && !_disposed)
        {
            lock (this)
            {
                _timer.Dispose();
                _timer = null;
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Inits a timer
    /// </summary>
    public void InitTimer() => _timer ??= new Timer(new TimerCallback(TimerHandler), null, Interval, Interval);

    /// <summary>
    /// Adds a task to the thread
    /// </summary>
    /// <param name="task">The task to be added</param>
    public void AddTask(Task task)
    {
        if (!_tasks.ContainsKey(task.Name))
        {
            _tasks.Add(task.Name, task);
        }
    }

    /// <summary>
    /// Gets or sets the interval in seconds at which to run the tasks
    /// </summary>
    public int Seconds { get; set; } = 10 * 60;

    /// <summary>
    /// Get or sets a datetime when thread has been started
    /// </summary>
    public DateTime StartedUtc { get; private set; }

    /// <summary>
    /// Get or sets a value indicating whether thread is running
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Get a list of tasks
    /// </summary>
    public IList<Task> Tasks
    {
        get
        {
            var list = new List<Task>();
            foreach (var task in _tasks.Values)
            {
                list.Add(task);
            }
            return new ReadOnlyCollection<Task>(list);
        }
    }

    /// <summary>
    /// Gets the interval at which to run the tasks
    /// </summary>
    public int Interval => Seconds * 1000;

    /// <summary>
    /// Gets or sets a value indicating whether the thread whould be run only once (per appliction start)
    /// </summary>
    public bool RunOnlyOnce { get; set; }
}