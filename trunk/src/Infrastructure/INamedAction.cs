using System;

namespace Consumentor.ShopGun
{
    public interface INamedAction
    {
        Action Action { get; }
        string ActionName { get; set; }
        bool HasActionName { get; }
    }
}