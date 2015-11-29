using System;

namespace Consumentor.ShopGun
{
    public class ActionStartEventArgs : EventArgs
    {
        public DateTime DateTime { get; private set; }

        public ActionStartEventArgs()
        {
            DateTime = ShopGunTime.Now;
        }
    }
}