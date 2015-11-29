namespace Consumentor.ShopGun.Component
{
    public interface IInstanceType
    {
        IRegisterComponentKeyFragment AsSingleton { get; }
        IRegisterComponentKeyFragment AsTransient { get; }
    }

    public class InstanceType : IInstanceType
    {
        private readonly RegisterComponent _componentRegistrator;

        public InstanceType(RegisterComponent componentRegistrator)
        {
            _componentRegistrator = componentRegistrator;
        }

        IRegisterComponentKeyFragment IInstanceType.AsSingleton
        {
            get
            {
                _componentRegistrator.Lifestyle = ComponentLifestyle.Singleton;
                return GetIRegisterComponentKeyFragment();
            }
        }

        IRegisterComponentKeyFragment IInstanceType.AsTransient
        {
            get 
            {
                _componentRegistrator.Lifestyle = ComponentLifestyle.Transient;
                return GetIRegisterComponentKeyFragment();
            }
        }

        private IRegisterComponentKeyFragment GetIRegisterComponentKeyFragment()
        {
            return new RegisterComponentFluentInterface(_componentRegistrator);
        }

    }
}