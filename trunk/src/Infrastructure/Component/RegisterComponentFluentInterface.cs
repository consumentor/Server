using System;

namespace Consumentor.ShopGun.Component
{
    public class RegisterComponentFluentInterface : RegisterServiceFragment, IRegisterComponentKeyFragment
    {
        public RegisterComponentFluentInterface(RegisterComponent registrator)
            :base(registrator)
        {
        }

        IRegisterServiceFragment IRegisterComponentKeyFragment.WithKey(string key)
        {
            Registrator.Key = key;
            return new RegisterServiceFragment(Registrator);
        }
    }
}