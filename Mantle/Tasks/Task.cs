using System;
using Mantle.Infrastructure;
using Mantle.Tasks.Domain;
using Mantle.Tasks.Services;
using Microsoft.Extensions.Logging;

namespace Mantle.Tasks
{
    /// <summary>
    /// Task
    /// </summary>
    public partial class Task
    {
        /// <summary>
        /// Ctor for Task
        /// </summary>
        private Task()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Ctor for Task
        /// </summary>
        /// <param name="task">Task </param>
        public Task(ScheduledTask task)
        {
            this.Type = task.Type;
            this.Enabled = task.Enabled;
            this.StopOnError = task.StopOnError;
            this.Name = task.Name;
        }

        //private ITask CreateTask(ILifetimeScope scope)
        private ITask CreateTask()
        {
            ITask task = null;
            if (this.Enabled)
            {
                var type2 = System.Type.GetType(this.Type);
                if (type2 != null)
                {
                    object instance;
                    //if (!EngineContext.Current.ContainerManager.TryResolve(type2, scope, out instance))
                    if (!EngineContext.Current.ContainerManager.TryResolve(type2, out instance))
                    {
                        //not resolved
                        //instance = EngineContext.Current.ContainerManager.ResolveUnregistered(type2, scope);
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(type2);
                    }
                    task = instance as ITask;
                }
            }
            return task;
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="throwException">A value indicating whether exception should be thrown if some error happens</param>
        /// <param name="dispose">A value indicating whether all instances hsould be disposed after task run</param>
        public void Execute(bool throwException = false)
        {
            this.IsRunning = true;

            //background tasks has an issue with Autofac
            //because scope is generated each time it's requested
            //that's why we get one single scope here
            //this way we can also dispose resources once a task is completed

            //var scope = EngineContext.Current.ContainerManager.Scope();
            //var scheduledTaskService = EngineContext.Current.ContainerManager.Resolve<IScheduledTaskService>("", scope);
            var scheduledTaskService = EngineContext.Current.Resolve<IScheduledTaskService>();
            var scheduledTask = scheduledTaskService.GetTaskByType(this.Type);

            try
            {
                //var task = this.CreateTask(scope);
                var task = this.CreateTask();
                if (task != null)
                {
                    this.LastStartUtc = DateTime.UtcNow;
                    if (scheduledTask != null)
                    {
                        //update appropriate datetime properties
                        scheduledTask.LastStartUtc = this.LastStartUtc;
                        scheduledTaskService.UpdateTask(scheduledTask);
                    }

                    //execute task
                    task.Execute();
                    this.LastEndUtc = this.LastSuccessUtc = DateTime.UtcNow;
                }
            }
            catch (Exception x)
            {
                this.Enabled = !this.StopOnError;
                this.LastEndUtc = DateTime.UtcNow;

                //log error
                var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Task>();
                logger.LogError(new EventId(), x, string.Format("Error while running the '{0}' scheduled task. {1}", Name, x.Message));
                if (throwException)
                {
                    throw;
                }
            }

            if (scheduledTask != null)
            {
                //update appropriate datetime properties
                scheduledTask.LastEndUtc = this.LastEndUtc;
                scheduledTask.LastSuccessUtc = this.LastSuccessUtc;
                scheduledTaskService.UpdateTask(scheduledTask);
            }

            this.IsRunning = false;
        }

        /// <summary>
        /// A value indicating whether a task is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Datetime of the last start
        /// </summary>
        public DateTime? LastStartUtc { get; private set; }

        /// <summary>
        /// Datetime of the last end
        /// </summary>
        public DateTime? LastEndUtc { get; private set; }

        /// <summary>
        /// Datetime of the last success
        /// </summary>
        public DateTime? LastSuccessUtc { get; private set; }

        /// <summary>
        /// A value indicating type of the task
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// A value indicating whether to stop task on error
        /// </summary>
        public bool StopOnError { get; private set; }

        /// <summary>
        /// Get the task name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A value indicating whether the task is enabled
        /// </summary>
        public bool Enabled { get; set; }
    }
}