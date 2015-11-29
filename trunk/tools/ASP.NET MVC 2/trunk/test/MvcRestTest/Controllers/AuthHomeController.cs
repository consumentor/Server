using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Web.Mvc.Resources;
using MovieApp.Models;

namespace MovieApp.Controllers
{
    [WebApiEnabled]
    public class AuthHomeController : Controller
    {
        private MoviesDBModelDataContext _db = new MoviesDBModelDataContext();

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(_db.Movies.AsSerializable());
        }

        //
        // GET: /Home/About

        public ActionResult About()
        {
            return View();
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(int id)
        {
            var movieToDisplay = (from m in _db.Movies
                                  where m.Id == id
                                  select m).FirstOrDefault();

            if (movieToDisplay == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "No movie matching '" + id + "'");
            }

            return View(movieToDisplay);
        }

        //
        // GET: /Home/Create

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create

        [Authorize(Roles = "admin")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([Bind(Exclude = "Id")] Movie movieToCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View();

                _db.Movies.InsertOnSubmit(movieToCreate);
                _db.SubmitChanges();

                return RedirectToAction("Details", new RouteValueDictionary { { "id", movieToCreate.Id } });
            }
            catch (Exception exception)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, "An error has occured; see details:", exception);
            }
        }

        //
        // GET: /Home/Edit/5

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            var movieToEdit = (from m in _db.Movies
                               where m.Id == id
                               select m).FirstOrDefault();

            if (movieToEdit == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "No movie matching '" + id + "'");
            }

            return View(movieToEdit);
        }

        //
        // POST: /Home/Edit/5
        //
        // PUT: /Home/Edit/5

        [Authorize(Roles = "admin")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
        public ActionResult Edit(Movie movieToEdit)
        {
            try
            {
                _db.Movies.Attach(movieToEdit, true);
                _db.SubmitChanges();

                return RedirectToAction("Details", new RouteValueDictionary { { "id", movieToEdit.Id } });
            }
            catch (Exception exception)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, "An error has occured; see details:", exception);
            }
        }

        //
        // POST: /Home/Delete/5
        //
        // DELETE: /Home/Delete/5

        [Authorize(Roles = "admin")]
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Delete)]
        public ActionResult Delete(int id)
        {
            try
            {
                Movie movieToDelete = _db.Movies.FirstOrDefault(m => m.Id == id);
                if (movieToDelete != null)
                {
                    // Delete should is idempotent, ignore if not present
                    _db.Movies.DeleteOnSubmit(movieToDelete);
                    _db.SubmitChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, "An error has occured; see details:", exception);
            }
        }
    }
}
