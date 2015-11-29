using System;
using System.Web.Mvc;
using Consumentor.ShopGun.ApplicationService.Server.Interfaces;
using Consumentor.ShopGun.Domain;

namespace Consumentor.Shopgun.Web.UI.Controllers
{
    public class ConceptController : BaseController
    {
        private readonly IConceptApplicationService _conceptApplicationService;

        public ConceptController(IConceptApplicationService conceptApplicationService)
        {
            _conceptApplicationService = conceptApplicationService;
        }

        //
        // GET: /Concepts/
        public ActionResult Index()
        {
            var conceptList = _conceptApplicationService.GetAllConcepts();

            ViewData.Model = conceptList;
            //return View(conceptList);
            return View();
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Concept concept, FormCollection form)
        {
            //if (ModelState.IsValid)
            //{

            //Concept concept = new Concept();
            // Deserialize (Include white list!)
            bool isModelUpdated = TryUpdateModel(concept, new[] { "ConceptName" }, form.ToValueProvider());
            
            // Validate
            if (String.IsNullOrEmpty(concept.ConceptTerm))
                ModelState.AddModelError("ConceptName", "Concept Name is required!");

            if (ModelState.IsValid)
            {
                _conceptApplicationService.CreateConcept(concept);

                return RedirectToAction("Index");
            }

            return View(concept);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var concept = _conceptApplicationService.GetConcept(id);

            ViewData.Model = concept;
            return View();
        }

        [Authorize]
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(Concept concept, FormCollection form)
        {
            _conceptApplicationService.UpdateConcept(concept);
            return RedirectToAction("Index");
        }
    }
}
