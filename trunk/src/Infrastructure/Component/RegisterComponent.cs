using System;

namespace Consumentor.ShopGun.Component
{
    public class RegisterComponent
    {
        private readonly IContainer _container;

        public RegisterComponent(IContainer container)
        {
            _container = container;
            _lifestyle = ComponentLifestyle.Singleton;
        }

        public virtual string Key { get; set; }
        public virtual Type ClassType { get; set; }
        public virtual Type ServiceType { get; set; }

        private ComponentLifestyle _lifestyle;
        public virtual ComponentLifestyle Lifestyle
        {
            get { return _lifestyle; }
            set { _lifestyle = value; }
        }

        public virtual void Register()
        {
            if (string.IsNullOrEmpty(Key))
            {
                Key = ClassType.Name;
            }
            if (ServiceType != null)
            {
                _container.RegisterComponent(Key, ServiceType, ClassType, Lifestyle);
            }
            else
            {
                _container.RegisterComponent(Key, ClassType, Lifestyle);
            }
        }
    }
}