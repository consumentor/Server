using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.CustomAttributes;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class ConceptAdviceController : AdviceController
    {
        private readonly IConceptApplicationService _conceptApplicationService;

        public ConceptAdviceController(IAdviceApplicationService adviceApplicationService
            , IConceptApplicationService conceptApplicationService
            , IMentorApplicationService mentorApplicationService
            , ISemaphoreApplicationService semaphoreApplicationService)
            : base(adviceApplicationService, mentorApplicationService, semaphoreApplicationService)
        {
            _conceptApplicationService = conceptApplicationService;
        }

        [HttpPost]
        public JsonResult ConceptAdvices(int? page, int? rows, string sidx, string sord, string searchMask)
        {
            var concepts = _adviceApplicationService.GetConeptsWithAdvicesByMentor(CurrentMentor);
            if (!string.IsNullOrEmpty(searchMask))
            {
                concepts = (from concept in concepts
                               where concept.ConceptTerm.ToLower().Contains(searchMask.ToLower())
                               select concept).ToList();
            }
            var conceptIdsAndNames = from c in concepts
                                     orderby c.ConceptTerm
                                     select new
                                                {
                                                    c.Id,
                                                    c.ConceptTerm,
                                                    ConceptAdvices =
                                                        c.ConceptAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id)
                                                };
            page = page.HasValue ? page.Value - 1 : 0;
            var pagedList = conceptIdsAndNames.AsQueryable().ToPagedList(page.Value, rows ?? 10);

            return BuildJsonResult(pagedList);
        }

        public ActionResult Details(int id)
        {
            var conceptAdvice = _adviceApplicationService.GetAdvice(id) as ConceptAdvice;
            var concept = _conceptApplicationService.GetConcept(conceptAdvice.ConceptsId.Value);
            ViewData["Concept"] = concept;
            return View(conceptAdvice);
        }

        [Authorize]
        public ActionResult Create(int? id)
        {
            var concepts = _conceptApplicationService.GetAllConcepts();
            ViewData["Concepts"] = new SelectList(concepts, "Id", "ConceptTerm", id);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult Create(ConceptAdvice conceptAdvice, FormCollection form)
        {
            if (conceptAdvice.ConceptsId == null)
            {
                ModelState.AddModelError("ConceptsId", "Please choose a concept...");
            }
            ValidateAdvice(conceptAdvice);

            if (ModelState.IsValid)
            {
                try
                {
                    _adviceApplicationService.AddConceptAdvice(CurrentMentor, conceptAdvice);

                    return RedirectToAction("Index", "Advice");
                }
                catch
                {
                    return RedirectToAction("Create");
                }
            }

            var concepts = _conceptApplicationService.GetAllConcepts();
            ViewData["Concepts"] = new SelectList(concepts, "Id", "ConceptTerm", conceptAdvice.ConceptsId);
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            SetAdviceTagViewData();
            return View(conceptAdvice);
        }

        //
        // GET: /ConceptAdvice/Edit/5
        [Authorize]
        [CheckPermission("id")]
        public ActionResult Edit(int id)
        {
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(id) as ConceptAdvice;
            var concept = _conceptApplicationService.GetConcept(advice.ConceptsId.Value);
            ViewData["Concept"] = concept;
            return View(advice);

        }

        //
        // POST: /ConceptAdvice/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ConceptAdvice conceptAdvice, FormCollection form)
        {
            ValidateAdvice(conceptAdvice);
            if (ModelState.IsValid)
            {
                _adviceApplicationService.UpdateAdvice(conceptAdvice);
                return RedirectToAction("Index", "Advice");
            }
            SetAdviceTagViewData();
            ViewData["Semaphores"] = _semaphoreApplicationService.GetAllSemaphores();
            var advice = _adviceApplicationService.GetAdvice(conceptAdvice.Id.Value) as ConceptAdvice;
            var concept = _conceptApplicationService.GetConcept(advice.ConceptsId.Value);
            ViewData["Concept"] = concept;
            return View(advice);
        }

        public JsonResult GetAdvicesForConcept(int? id)
        {
            var concepts = _adviceApplicationService.GetConeptsWithAdvicesByMentor(CurrentMentor);
            var concept = concepts.Single(x => x.Id == id.Value);
            var advices = concept.ConceptAdvices.Where(x => x.Mentor.Id == CurrentMentor.Id);

            var jsonData = new
            {
                rows = advices.AsQueryable().ToPagedList(0, advices.Count())
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Publish(int id)
        {
            _adviceApplicationService.PublishAdvice(id);
            return Json(null);
        }

        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Unpublish(int id)
        {
            _adviceApplicationService.UnpublishAdvice(id);
            return Json(null);
        }


        [Authorize]
        [CheckPermission("id")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            _adviceApplicationService.DeleteAdvice(id);
            return Json(null);
        }
    }
}
