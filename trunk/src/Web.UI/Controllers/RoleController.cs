using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class RoleController : BaseController
    {
        private readonly IRoleProviderApplicationService _roleProviderApplicationService;

        public RoleController(IRoleProviderApplicationService roleProviderApplicationService)
        {
            _roleProviderApplicationService = roleProviderApplicationService;
        }

        //
        // GET: /Role/

        public ActionResult Index()
        {
            var roles = _roleProviderApplicationService.GetAllRoles();
            return View(roles);
        }

        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRole(Role roleToCreate)
        {
            try
            {
                _roleProviderApplicationService.CreateRole(roleToCreate.RoleName);
            }
            catch (Exception)
            {    
                throw;
            }
            return RedirectToAction("Index");
        }


    }
}
