using System;

namespace Consumentor.ShopGun
{
    public class ActionCompletionEventArgs : EventArgs
    {
        public bool Success { get; private set; }
        public DateTime DateTime { get; private set; }

        public ActionCompletionEventArgs(bool success)
        {
            Success = success;
            DateTime = ShopGunTime.Now;
        }
    }
}