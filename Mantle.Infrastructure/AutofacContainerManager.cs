using Mantle.Exceptions;

namespace Mantle.Infrastructure;

public class AutofacContainerManager : IDisposable
{
    private bool disposed = false;

    public IContainer Container { get; }

    public AutofacContainerManager(IContainer container)
    {
        Container = container;
    }

    public T Resolve<T>(string key = "") where T : class
    {
        if (string.IsNullOrEmpty(key))
        {
            return Container.Resolve<T>();
        }
        return Container.ResolveKeyed<T>(key);
    }

    public T Resolve<T>(IDictionary<string, object> ctorArgs, string key = "") where T : class
    {
        var ctorParams = ctorArgs.Select(x => new NamedParameter(x.Key, x.Value)).ToArray();

        if (string.IsNullOrEmpty(key))
        {
            return Container.Resolve<T>(ctorParams);
        }
        return Container.ResolveKeyed<T>(key, ctorParams);
    }

    public T ResolveNamed<T>(string name) where T : class
    {
        return Container.ResolveNamed<T>(name);
    }

    public object Resolve(Type type)
    {
        return Container.Resolve(type);
    }

    public IEnumerable<T> ResolveAll<T>(string key = "")
    {
        if (string.IsNullOrEmpty(key))
        {
            return Container.Resolve<IEnumerable<T>>().ToArray();
        }
        return Container.ResolveKeyed<IEnumerable<T>>(key).ToArray();
    }

    public IEnumerable<T> ResolveAllNamed<T>(string name)
    {
        return Container.ResolveKeyed<IEnumerable<T>>(name).ToArray();
    }

    public T ResolveUnregistered<T>() where T : class
    {
        return ResolveUnregistered(typeof(T)) as T;
    }

    public object ResolveUnregistered(Type type)
    {
        Exception innerException = null;
        foreach (var constructor in type.GetConstructors())
        {
            try
            {
                //try to resolve constructor parameters
                var parameters = constructor.GetParameters().Select(parameter =>
                {
                    object service = Resolve(parameter.ParameterType);
                    if (service == null)
                    {
                        throw new MantleException("Unknown dependency");
                    }
                    return service;
                });

                //all is ok, so create instance
                return Activator.CreateInstance(type, parameters.ToArray());
            }
            catch (Exception ex)
            {
                innerException = ex;
            }
        }
        throw new MantleException("No constructor was found that had all the dependencies satisfied.", innerException);
    }

    public bool TryResolve<T>(out T instance) where T : class
    {
        return Container.TryResolve<T>(out instance);
    }

    public bool TryResolve(Type serviceType, out object instance)
    {
        return Container.TryResolve(serviceType, out instance);
    }

    public bool IsRegistered(Type serviceType)
    {
        return Container.IsRegistered(serviceType);
    }

    public object ResolveOptional(Type serviceType)
    {
        return Container.ResolveOptional(serviceType);
    }

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }

        if (disposing)
        {
            Container.Dispose();
            // Free any other managed objects here.
        }

        // Free any unmanaged objects here.
        disposed = true;
    }

    #endregion IDisposable Members
}