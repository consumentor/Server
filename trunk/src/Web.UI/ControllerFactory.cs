using System.Web.Mvc;
using System.Web.Routing;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;

namespace Consumentor.Shopgun.Web.UI
{
    public class ControllerFactory : IControllerFactory
    {

        private readonly IConfiguration _configuration = new BasicConfiguration();
        private readonly IContainer _container;

        public ControllerFactory()
        {
            _configuration = new BasicConfiguration();
            _container = _configuration.Container;
        }

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            return _container.Resolve<IController>(controllerName);
        }

        public void ReleaseController(IController controller)
        {
            _container.Release(controller);
        }
    }
}