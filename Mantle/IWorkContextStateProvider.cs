using System;

namespace Mantle
{
    public interface IWorkContextStateProvider
    {
        Func<IWorkContext, T> Get<T>(string name);
    }
}