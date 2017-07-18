using System;
using System.Collections.Generic;

namespace Mantle.Infrastructure.DependencyManagement
{
    public interface IContainerManager
    {
        void AddComponent<TService>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponent(Type service, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponent<TService, TImplementation>(string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponent(Type service, Type implementation, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponentInstance<TService>(object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponentInstance(Type service, object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponentInstance(object instance, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponentWithParameters<TService, TImplementation>(IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        void AddComponentWithParameters(Type service, Type implementation, IDictionary<string, string> properties, string key = "", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton);

        T Resolve<T>(string key = "") where T : class;

        T Resolve<T>(IDictionary<string, object> ctorArgs, string key = "") where T : class;

        T ResolveNamed<T>(string name) where T : class;

        object Resolve(Type type);

        IEnumerable<T> ResolveAll<T>(string key = "");

        IEnumerable<T> ResolveAllNamed<T>(string name);

        T ResolveUnregistered<T>() where T : class;

        object ResolveUnregistered(Type type);

        bool TryResolve<T>(out T instance);

        bool TryResolve(Type serviceType, out object instance);

        bool IsRegistered(Type serviceType);

        object ResolveOptional(Type serviceType);
    }
}