using System;
using System.Collections;
using System.Globalization;
using Castle.Core;
using Castle.MicroKernel.Releasers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Consumentor.ShopGun.Configuration;

namespace Consumentor.ShopGun.Component
{
    public class Container : IContainer
    {
        private static IWindsorContainer _container;
        private readonly IContainerConfigurationSettings _configuration;
        private readonly string _componentConfigurationFileName;


        public Container(IContainerConfigurationSettings configuration)
        {
            _configuration = configuration;
            _componentConfigurationFileName = _configuration.ComponentConfigurationFileName;
            if (_container == null)
            {
                lock (this)
                {
                    if (_container == null)
                    {
                        CreateNewContainer();
                    }
                }
            }
            _registrator = new RegisterComponent(this);
        }

        public void CreateNewContainer()
        {
            _container = new WindsorContainer(new XmlInterpreter(_componentConfigurationFileName));
            _container.Kernel.ReleasePolicy = new NoTrackingReleasePolicy(); // LifecycledComponentsReleasePolicy(); 
            RegisterSelf();
            AllowArrayResolving();
        }

        private void RegisterSelf()
        {
            _container.Kernel.AddComponentInstance("IContainer", typeof(IContainer), this);
            _container.Kernel.AddComponentInstance("IWindsorContainer", typeof(IWindsorContainer), _container);
        }

        private void AllowArrayResolving()
        {
            _container.Kernel.Resolver.AddSubResolver(new ArrayResolver(_container.Kernel));
        }

        private LifestyleType GetLifestyleType(ComponentLifestyle componentLifestyle)
        {
            var lifestyleType = (LifestyleType)Enum.Parse(typeof(LifestyleType), componentLifestyle.ToString(), true);
            return lifestyleType;
        }

        T IContainer.Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        object IContainer.Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        T IContainer.Resolve<T>(IDictionary arguments)
        {
            return _container.Resolve<T>(arguments);
        }

        T IContainer.Resolve<T>(string key, IDictionary arguments)
        {
            return _container.Resolve<T>(key, arguments);
        }

        void IContainer.UnregisterComponent(Type classType)
        {
            if (!_container.Kernel.RemoveComponent(classType.Name))
                throw new ContainerException(string.Format(CultureInfo.CurrentCulture, "Failed to unregister component '{0}'", classType.Name));
        }

        void IContainer.UnregisterComponent(string key)
        {
            if (!_container.Kernel.RemoveComponent(key))
                throw new ContainerException(string.Format(CultureInfo.CurrentCulture, "Failed to unregister component '{0}'", key));
        }

        void IContainer.Release(object instance)
        {
            _container.Release(instance);
        }

        T IContainer.Resolve<T>(string key)
        {
            return _container.Resolve<T>(key);
        }

        void IContainer.RegisterComponent(string key, Type classType)
        {
            _container.AddComponent(key, classType);
        }

        void IContainer.RegisterComponent(string key, Type classType, ComponentLifestyle componentLifestyle)
        {
            LifestyleType lifestyleType = GetLifestyleType(componentLifestyle);
            _container.AddComponentLifeStyle(key, classType, lifestyleType);
        }

        void IContainer.RegisterComponent(string key, Type serviceType, Type classType)
        {
            _container.AddComponent(key, serviceType, classType);
        }

        void IContainer.RegisterComponent(string key, Type serviceType, Type classType, ComponentLifestyle componentLifestyle)
        {
            LifestyleType lifestyleType = GetLifestyleType(componentLifestyle);
            _container.AddComponentLifeStyle(key, serviceType, classType, lifestyleType);
        }

        void IContainer.RegisterInstance(string key, Type serviceType, object instance)
        {
            _container.Kernel.AddComponentInstance(key, serviceType, instance);
        }

        void IContainer.RegisterInstance(string key, object instance)
        {
            _container.Kernel.AddComponentInstance(key, instance);
        }

        IInstanceType IContainer.RegisterComponent()
        {
            _registrator = new RegisterComponent(this);
            return new InstanceType(_registrator);
        }

        private RegisterComponent _registrator;
        RegisterComponent IContainer.ComponentRegistrator
        {
            get
            {
                return _registrator;
            }
        }
    }
}