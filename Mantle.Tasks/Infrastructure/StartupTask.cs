﻿using Microsoft.Extensions.Options;

namespace Mantle.Tasks.Infrastructure;

public class StartupTask : IStartupTask
{
    #region IStartupTask Members

    public void Execute() => EnsureScheduledTasks();

    private static void EnsureScheduledTasks()
    {
        var options = DependoResolver.Instance.Resolve<IOptions<MantleTasksOptions>>();
        if (options.Value.ScheduledTasksEnabled)
        {
            var taskRepository = DependoResolver.Instance.Resolve<IRepository<ScheduledTask>>();
            var allTasks = DependoResolver.Instance.ResolveAll<ITask>();
            var allTaskNames = allTasks.Select(x => x.Name).ToList();
            var installedTasks = taskRepository.Find(new SearchOptions<ScheduledTask>
            {
                Query = x => true
            });
            var installedTaskNames = installedTasks.Select(x => x.Name).ToList();

            var tasksToAdd = allTasks
                .Where(x => !installedTaskNames.Contains(x.Name))
                .Select(x => new ScheduledTask
                {
                    Name = x.Name,
                    Type = x.GetType().AssemblyQualifiedName,
                    Seconds = x.DefaultInterval
                });

            if (!tasksToAdd.IsNullOrEmpty())
            {
                taskRepository.Insert(tasksToAdd);
            }

            var tasksToDelete = installedTasks.Where(x => !allTaskNames.Contains(x.Name));

            if (!tasksToDelete.IsNullOrEmpty())
            {
                taskRepository.Delete(tasksToDelete);
            }
        }
    }

    public int Order => 0;

    #endregion IStartupTask Members
}