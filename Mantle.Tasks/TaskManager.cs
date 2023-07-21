using Mantle.Infrastructure;
using Mantle.Tasks.Services;
using System.Collections.ObjectModel;

namespace Mantle.Tasks
{
    /// <summary>
    /// Represents task manager
    /// </summary>
    public partial class TaskManager
    {
        private readonly List<TaskThread> taskThreads = new List<TaskThread>();
        private int notRunTasksInterval = 60 * 30; //30 minutes

        public TaskManager()
        {
        }

        /// <summary>
        /// Initializes the task manager with the property values specified in the configuration file.
        /// </summary>
        public void Initialize()
        {
            var taskService = EngineContext.Current.Resolve<IScheduledTaskService>();

            taskThreads.Clear();

            var scheduledTasks = taskService
                .GetAllTasks()
                .OrderBy(x => x.Seconds)
                .ToList();

            //group by threads with the same seconds
            foreach (var scheduledTaskGrouped in scheduledTasks.GroupBy(x => x.Seconds))
            {
                //create a thread
                var taskThread = new TaskThread()
                {
                    Seconds = scheduledTaskGrouped.Key
                };
                foreach (var scheduledTask in scheduledTaskGrouped)
                {
                    var task = new Task(scheduledTask);
                    taskThread.AddTask(task);
                }
                taskThreads.Add(taskThread);
            }

            //sometimes a task period could be set to several hours (or even days).
            //in this case a probability that it'll be run is quite small (an application could be restarted)
            //we should manually run the tasks which weren't run for a long time
            var notRunTasks = scheduledTasks
                .Where(x => x.Seconds >= notRunTasksInterval)
                .Where(x => !x.LastStartUtc.HasValue || x.LastStartUtc.Value.AddSeconds(notRunTasksInterval) < DateTime.UtcNow)
                .ToList();
            //create a thread for the tasks which weren't run for a long time
            if (notRunTasks.Count > 0)
            {
                var taskThread = new TaskThread()
                {
                    RunOnlyOnce = true,
                    Seconds = 60 * 5 //let's run such tasks in 5 minutes after application start
                };
                foreach (var scheduledTask in notRunTasks)
                {
                    var task = new Task(scheduledTask);
                    taskThread.AddTask(task);
                }
                taskThreads.Add(taskThread);
            }
        }

        /// <summary>
        /// Starts the task manager
        /// </summary>
        public void Start()
        {
            foreach (var taskThread in taskThreads)
            {
                taskThread.InitTimer();
            }
        }

        /// <summary>
        /// Stops the task manager
        /// </summary>
        public void Stop()
        {
            foreach (var taskThread in taskThreads)
            {
                taskThread.Dispose();
            }
        }

        /// <summary>
        /// Gets the task mamanger instance
        /// </summary>
        public static TaskManager Instance { get; } = new TaskManager();

        /// <summary>
        /// Gets a list of task threads of this task manager
        /// </summary>
        public IList<TaskThread> TaskThreads => new ReadOnlyCollection<TaskThread>(taskThreads);
    }
}