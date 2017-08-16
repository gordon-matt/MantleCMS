using System;
using System.Collections.Generic;
using Mantle.Infrastructure.DependencyManagement;

namespace Mantle.Infrastructure
{
    public interface IEngine : IDisposable
    {
        IContainerManager ContainerManager { get; }

        void Initialize();

        T Resolve<T>() where T : class;

        T Resolve<T>(IDictionary<string, object> ctorArgs) where T : class;

        T ResolveNamed<T>(string name) where T : class;

        object Resolve(Type type);

        IEnumerable<T> ResolveAll<T>();

        IEnumerable<T> ResolveAllNamed<T>(string name);

        object ResolveUnregistered(Type type);

        bool TryResolve<T>(out T instance);

        bool TryResolve(Type serviceType, out object instance);

        IServiceProvider ServiceProvider { get; }
    }
}