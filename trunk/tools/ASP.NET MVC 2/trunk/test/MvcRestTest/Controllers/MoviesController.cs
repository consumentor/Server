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
    public class MoviesController : Controller
    {
        const string routeName = "Movies";

        private MoviesDBModelDataContext _db = new MoviesDBModelDataContext();

        //
        // GET: /Movies/

        public ActionResult Index()
        {
            return View(_db.Movies.AsSerializable());
        }

        //
        // GET: /Movies/5

        [AcceptVerbs(HttpVerbs.Get)]
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
        // GET: /Movies/CreateForm

        public ActionResult CreateForm()
        {
            return View("Create");
        }

        //
        // POST: /Movies

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(Movie item)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View();

                _db.Movies.InsertOnSubmit(item);
                _db.SubmitChanges();

                return RedirectToRoute(this.GetResourceRouteName(routeName, ActionType.Retrieve), new RouteValueDictionary { { "id", item.Id } });
            }
            catch (Exception exception)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, "An error has occured; see details:", exception);
            }
        }

        //
        // GET: /Movies/EditForm/5

        public ActionResult EditForm(int id)
        {
            var movieToEdit = (from m in _db.Movies
                               where m.Id == id
                               select m).FirstOrDefault();

            if (movieToEdit == null)
            {
                throw new HttpException((int)HttpStatusCode.NotFound, "No movie matching '" + id + "'");
            }

            return View("Edit", movieToEdit);
        }

        //
        // POST: /Movies/5 with X-Http-Method-Override=PUT
        //
        // PUT: /Movies/5

        [AcceptVerbs(HttpVerbs.Put)]
        public ActionResult Edit(int id, Movie item)
        {
            try
            {
                item.Id = id;
                _db.Movies.Attach(item, true);
                _db.SubmitChanges();

                return RedirectToRoute(this.GetResourceRouteName(routeName, ActionType.Retrieve), new RouteValueDictionary { { "id", item.Id } });
            }
            catch (Exception exception)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, "An error has occured; see details:", exception);
            }
        }

        //
        // POST: /Movies/5 with X-Http-Method-Override=DELETE
        //
        // DELETE: /Movies/5

        [AcceptVerbs(HttpVerbs.Delete)]
        public ActionResult Delete(int id)
        {
            try
            {
                Movie movie = _db.Movies.FirstOrDefault(m => m.Id == id);
                if (movie != null)
                {
                    // Delete should is idempotent, ignore if not present
                    _db.Movies.DeleteOnSubmit(movie);
                    _db.SubmitChanges();
                }

                return RedirectToRoute(this.GetResourceRouteName(routeName, ActionType.Index));
            }
            catch (Exception exception)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, "An error has occured; see details:", exception);
            }
        }
    }
}
