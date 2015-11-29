using System;
using System.Web.Mvc;
using Consumentor.ShopGun.Domain;
using Consumentor.ShopGun.DomainService.Server.Interfaces;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class CertificationMarkController : BaseController
    {
        private readonly ICertificationMarkDomainService _certificationMarkDomainService;

        public CertificationMarkController(ICertificationMarkDomainService certificationMarkDomainService)
        {
            _certificationMarkDomainService = certificationMarkDomainService;
        }

        public ActionResult Index()
        {
            var certificationMarks = _certificationMarkDomainService.GetCertificationMarksByCertifier(CurrentMentor);
            
            return View(certificationMarks);
        }

        public ActionResult CreateCertificationMark()
        {
            return View();
        } 

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateCertificationMark(CertificationMark certificationMarkToCreate)
        {
            try
            {
                _certificationMarkDomainService.CreateCertificationMark(CurrentMentor, certificationMarkToCreate);

                return RedirectToAction("Index");
            }
            catch(Exception exception)
            {
                return View();
            }
        }

        public ActionResult EditCertificationMark(int id)
        {
            var certificationMark = _certificationMarkDomainService.GetCertificationMarkById(id);
            return View(certificationMark);
        }

        [HttpPost]
        public ActionResult EditCertificationMark(CertificationMark updatedCertificationMark, FormCollection collection)
        {
            try
            {
                _certificationMarkDomainService.UpdateCertificationMark(updatedCertificationMark);
 
                return RedirectToAction("Index");
            }
            catch
            {
                var certificationMark = _certificationMarkDomainService.GetCertificationMarkById(updatedCertificationMark.Id);
                return View(certificationMark);
            }
        }

        public ActionResult DeleteCertificationMark(int id)
        {
            var certificationMarkToDelete = _certificationMarkDomainService.GetCertificationMarkById(id);
            _certificationMarkDomainService.DeleteCertificationMark(certificationMarkToDelete);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "SuperUser")]
        public ActionResult CertificationMarkMappings()
        {
            var opvMappings = _certificationMarkDomainService.GetOpvCertificationMarkMappings();
            return View(opvMappings);
        }

        public ActionResult AddOpvCertificationMarkMapping()
        {
            var certificationMarks = _certificationMarkDomainService.GetAllCertificationMarks();
            ViewData["CertificationMarks"] = new SelectList(certificationMarks, "Id", "CertificationName");
            return View();
        }

        [HttpPost]
        public ActionResult AddOpvCertificationMarkMapping(OpvCertificationMarkMapping opvCertificationMarkMapping)
        {
            try
            {
                _certificationMarkDomainService.AddOpvCertificationMarkMapping(opvCertificationMarkMapping);
            }
            catch (Exception)
            {
                RedirectToAction("AddOpvCertificationMarkMapping");
                throw;
            }
            return RedirectToAction("CertificationMarkMappings");
        }

        public ActionResult DeleteCertificationMarkMapping(int id)
        {
            _certificationMarkDomainService.DeleteCertificationMarkMapping(id);

            return RedirectToAction("CertificationMarkMappings");
        }
    }
}
