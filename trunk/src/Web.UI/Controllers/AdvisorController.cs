using System;
using System.Web.Mvc;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class AdvisorController : Controller
    {
        private readonly IMentorDomainService _mentorDomainService;

        public AdvisorController(IMentorDomainService mentorDomainService)
        {
            _mentorDomainService = mentorDomainService;
        }

        public ActionResult Index()
        {
            var advisors = _mentorDomainService.GetAllMentors();

            return View(advisors);
        }

        public ActionResult CreateAdvisor()
        {
            return View();
        } 

        //
        // POST: /Advisor/Create

        [HttpPost]
        public ActionResult CreateAdvisor(Mentor newAdvisor)
        {
            try
            {
                _mentorDomainService.CreateNewMentor(newAdvisor.MentorName);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult EditAdvisor(int id)
        {
            var advisor = _mentorDomainService.GetMentorById(id);

            return View(advisor);
        }

        [HttpPost]
        public ActionResult EditAdvisor(Mentor updatedAdvisor)
        {
            try
            {
                _mentorDomainService.UpdateMentor(updatedAdvisor);
            }
            catch (Exception)
            {
                return RedirectToAction("EditAdvisor");
            }
            return RedirectToAction("Index");
        }

    }
}
