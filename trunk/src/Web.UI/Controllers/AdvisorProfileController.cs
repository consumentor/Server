using System;
using System.Web.Mvc;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class AdvisorProfileController : BaseController
    {
        private readonly IMentorDomainService _mentorDomainService;

        public AdvisorProfileController(IMentorDomainService mentorDomainService)
        {
            _mentorDomainService = mentorDomainService;
        }

        [Authorize]
        public ActionResult Index()
        {
            return View(CurrentMentor);
        }

        [Authorize]
        public ActionResult Edit()
        {
            return View(CurrentMentor);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Mentor mentor)
        {
            try
            {
                _mentorDomainService.UpdateMentor(mentor);
            }
            catch (Exception)
            {
                return RedirectToAction("Edit");
            }

            return RedirectToAction("Index");
        }

    }
}
