using System;
using System.Collections;

namespace Consumentor.ShopGun.Component
{
    public interface IContainer
    {
        T Resolve<T>();
        T Resolve<T>(string key);
        void RegisterComponent(string key, Type classType);
        void RegisterComponent(string key, Type classType, ComponentLifestyle lifestyle);
        void RegisterComponent(string key, Type serviceType, Type classType);
        void RegisterComponent(string key, Type serviceType, Type classType, ComponentLifestyle lifestyle);
        void RegisterInstance(string key, Type serviceType, object instance);
        void RegisterInstance(string key, object instance);
        IInstanceType RegisterComponent();
        RegisterComponent ComponentRegistrator { get; }
        object Resolve(Type type);
        T Resolve<T>(IDictionary arguments);
        T Resolve<T>(string key, IDictionary arguments);
        void UnregisterComponent(Type serviceType);
        void UnregisterComponent(string key);
        void Release(object instance);
    }
}