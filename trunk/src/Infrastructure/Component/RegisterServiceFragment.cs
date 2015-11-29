using System;

namespace Consumentor.ShopGun.Component
{
    public class RegisterServiceFragment : IRegisterServiceFragment
    {
        protected RegisterComponent Registrator { get; private set; }

        public RegisterServiceFragment(RegisterComponent registrator)
        {
            Registrator = registrator;
        }


        void IRegisterClassTypeFragment.OfType(Type classType)
        {
            Registrator.ClassType = classType;
            Registrator.Register();
        }

        void IRegisterClassTypeFragment.OfType<T>()
        {
            ((IRegisterClassTypeFragment)this).OfType(typeof(T));
        }

        IRegisterClassTypeFragment IRegisterServiceFragment.AsService(Type serviceType)
        {
            Registrator.ServiceType = serviceType;
            return this;
        }

        IRegisterClassTypeFragment IRegisterServiceFragment.AsService<T>()
        {
            return ((IRegisterServiceFragment)this).AsService(typeof(T));
        }
    }
}