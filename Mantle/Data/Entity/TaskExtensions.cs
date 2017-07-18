using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mantle.EntityFramework.Data
{
    internal static class TaskExtensions
    {
        public static TaskExtensions.CultureAwaiter<T> WithCurrentCulture<T>(this Task<T> task)
        {
            return new TaskExtensions.CultureAwaiter<T>(task);
        }

        public static TaskExtensions.CultureAwaiter WithCurrentCulture(this Task task)
        {
            return new TaskExtensions.CultureAwaiter(task);
        }

        public struct CultureAwaiter<T> : ICriticalNotifyCompletion, INotifyCompletion
        {
            private readonly Task<T> _task;

            public bool IsCompleted
            {
                get
                {
                    return this._task.IsCompleted;
                }
            }

            public CultureAwaiter(Task<T> task)
            {
                this._task = task;
            }

            public TaskExtensions.CultureAwaiter<T> GetAwaiter()
            {
                return this;
            }

            public T GetResult()
            {
                return this._task.GetAwaiter().GetResult();
            }

            public void OnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                this._task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted((Action)(() =>
                {
                    CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
                    CultureInfo.CurrentCulture = currentCulture;
                    try
                    {
                        continuation();
                    }
                    finally
                    {
                        CultureInfo.CurrentCulture = currentCulture1;
                    }
                }));
            }
        }

        public struct CultureAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {
            private readonly Task _task;

            public bool IsCompleted
            {
                get
                {
                    return this._task.IsCompleted;
                }
            }

            public CultureAwaiter(Task task)
            {
                this._task = task;
            }

            public TaskExtensions.CultureAwaiter GetAwaiter()
            {
                return this;
            }

            public void GetResult()
            {
                this._task.GetAwaiter().GetResult();
            }

            public void OnCompleted(Action continuation)
            {
                throw new NotImplementedException();
            }

            public void UnsafeOnCompleted(Action continuation)
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;
                this._task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted((Action)(() =>
                {
                    CultureInfo currentCulture1 = CultureInfo.CurrentCulture;
                    CultureInfo.CurrentCulture = currentCulture;
                    try
                    {
                        continuation();
                    }
                    finally
                    {
                        CultureInfo.CurrentCulture = currentCulture1;
                    }
                }));
            }
        }
    }
}