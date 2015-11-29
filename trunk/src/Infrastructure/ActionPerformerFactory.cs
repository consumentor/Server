using System;
using Consumentor.ShopGun.Component;

namespace Consumentor.ShopGun
{
    public class ActionPerformerFactory : IActionPerformerFactory
    {
        private readonly IContainer _container;

        public ActionPerformerFactory(IContainer container)
        {
            _container = container;
        }

        IActionPerformer IActionPerformerFactory.GetActionPerformer(TimeSpan retryDelay)
        {
            return ((IActionPerformerFactory)this).GetActionPerformer(retryDelay, 3);
        }

        IActionPerformer IActionPerformerFactory.GetActionPerformer(TimeSpan retryDelay, int numberOfRetries)
        {
            var performer = _container.Resolve<IActionPerformer>();
            performer.NumberOfRetries = numberOfRetries;
            performer.RetryDelay = retryDelay;

            return performer;
        }
    }
}