using System;

namespace Consumentor.ShopGun.Component
{
    public interface IRegisterServiceFragment : IRegisterClassTypeFragment
    {
        IRegisterClassTypeFragment AsService(Type serviceType);
        IRegisterClassTypeFragment AsService<T>();
    }
}