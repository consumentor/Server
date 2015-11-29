using System;

namespace Consumentor.ShopGun.Component
{
    public interface IRegisterComponentKeyFragment : IRegisterServiceFragment
    {
        IRegisterServiceFragment WithKey(string key);
    }
}