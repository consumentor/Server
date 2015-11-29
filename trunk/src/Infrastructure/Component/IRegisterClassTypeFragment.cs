using System;

namespace Consumentor.ShopGun.Component
{
    public interface IRegisterClassTypeFragment
    {
        void OfType(Type classType);
        void OfType<T>();
    }
}