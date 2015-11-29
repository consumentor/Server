using System;

namespace Consumentor.ShopGun
{
    public interface IActionPerformerFactory
    {
        IActionPerformer GetActionPerformer(TimeSpan retryDelay);
        IActionPerformer GetActionPerformer(TimeSpan retryDelay, int numberOfRetries);
    }
}