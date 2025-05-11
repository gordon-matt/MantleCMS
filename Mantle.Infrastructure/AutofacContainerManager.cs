using Mantle.Exceptions;

namespace Mantle.Infrastructure;

public class AutofacContainerManager : IDisposable
{
    private bool disposed = false;

    public AutofacContainerManager(IContainer container)
    {
        Container = container;
    }

    public IContainer Container { get; }

    public bool IsRegistered(Type serviceType) => Container.IsRegistered(serviceType);

    public T Resolve<T>(string key = "") where T : class =>
        string.IsNullOrEmpty(key) ? Container.Resolve<T>() : Container.ResolveKeyed<T>(key);

    public T Resolve<T>(IDictionary<string, object> ctorArgs, string key = "") where T : class
    {
        var ctorParams = ctorArgs.Select(x => new NamedParameter(x.Key, x.Value)).ToArray();

        return string.IsNullOrEmpty(key)
            ? Container.Resolve<T>(ctorParams)
            : Container.ResolveKeyed<T>(key, ctorParams);
    }

    public object Resolve(Type type) => Container.Resolve(type);

    public IEnumerable<T> ResolveAll<T>(string key = "") => string.IsNullOrEmpty(key)
        ? Container.Resolve<IEnumerable<T>>().ToArray()
        : Container.ResolveKeyed<IEnumerable<T>>(key).ToArray();

    public IEnumerable<T> ResolveAllNamed<T>(string name) =>
        Container.ResolveKeyed<IEnumerable<T>>(name).ToArray();

    public T ResolveNamed<T>(string name) where T : class =>
        Container.ResolveNamed<T>(name);

    public object ResolveOptional(Type serviceType) =>
        Container.ResolveOptional(serviceType);

    public T ResolveUnregistered<T>() where T : class =>
        ResolveUnregistered(typeof(T)) as T;

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
                    return service ?? throw new MantleException("Unknown dependency");
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

    public bool TryResolve<T>(out T instance) where T : class =>
        Container.TryResolve<T>(out instance);

    public bool TryResolve(Type serviceType, out object instance) =>
        Container.TryResolve(serviceType, out instance);

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