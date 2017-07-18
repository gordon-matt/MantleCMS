using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Data;
using Mantle.Tasks.Domain;

namespace Mantle.Tasks.Services
{
    /// <summary>
    /// Task service
    /// </summary>
    public partial class ScheduledTaskService : IScheduledTaskService
    {
        #region Fields

        private readonly IRepository<ScheduledTask> taskRepository;

        #endregion Fields

        #region Ctor

        public ScheduledTaskService(IRepository<ScheduledTask> taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        #endregion Ctor

        #region Methods

        /// <summary>
        /// Deletes a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void DeleteTask(ScheduledTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            taskRepository.Delete(task);
        }

        /// <summary>
        /// Gets a task
        /// </summary>
        /// <param name="taskId">Task identifier</param>
        /// <returns>Task</returns>
        public virtual ScheduledTask GetTaskById(int taskId)
        {
            if (taskId == 0)
                return null;

            return taskRepository.FindOne(taskId);
        }

        /// <summary>
        /// Gets a task by its type
        /// </summary>
        /// <param name="type">Task type</param>
        /// <returns>Task</returns>
        public virtual ScheduledTask GetTaskByType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return null;
            }

            using (var connection = taskRepository.OpenConnection())
            {
                return connection
                    .Query(st => st.Type == type)
                    .OrderByDescending(t => t.Id)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets all tasks
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Tasks</returns>
        public virtual IList<ScheduledTask> GetAllTasks(bool showHidden = false)
        {
            using (var connection = taskRepository.OpenConnection())
            {
                var query = connection.Query();

                if (!showHidden)
                {
                    query = query.Where(t => t.Enabled);
                }
                query = query.OrderByDescending(t => t.Seconds);

                return query.ToList();
            }
        }

        /// <summary>
        /// Inserts a task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void InsertTask(ScheduledTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            taskRepository.Insert(task);
        }

        /// <summary>
        /// Updates the task
        /// </summary>
        /// <param name="task">Task</param>
        public virtual void UpdateTask(ScheduledTask task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            taskRepository.Update(task);
        }

        #endregion Methods
    }
}