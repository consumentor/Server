using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieApp.Models;

namespace MovieApp.Test
{
    [TestClass]
    public class UnitTestMoviesAuth : MoviesTestBase
    {
        const string psuedoAuthHeaderForBob = "x-pseudo-auth: bob:321drowssap";  // Bob has admin rights
        const string psuedoAuthHeaderForPhil = "x-pseudo-auth: phil:drowssap321";  // Phil does not have admin rights

        [TestMethod]
        public void MoviesGetIndexXml()
        {
            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("application/xml; charset=utf-8", response.Content.ContentType);
            }
        }

        [TestMethod]
        public void MoviesGetCategory404()
        {
            // TODO: this today doesn't honor "application/xml" because a route constraint
            // on Route returns a 404 w/ text/html regardless and there's no hook to intercept
            using (HttpResponseMessage response = client.Get("AuthHome/details/99", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesGetMoviesXml()
        {
            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesGetMoviesJson()
        {
            using (HttpResponseMessage response = client.Get("AuthHome", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesNotAcceptable()
        {
            using (HttpResponseMessage response = client.Get("AuthHome", "foo"))
            {
                Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeJson()
        {
            client.DefaultHeaders.Add(psuedoAuthHeaderForBob);

            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            using (HttpResponseMessage response = client.Post("AuthHome/create", content, "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/json; charset=utf-8", response.Content.ContentType);
            }

            client.DefaultHeaders.Remove(psuedoAuthHeaderForBob);
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeXml()
        {
            client.DefaultHeaders.Add(psuedoAuthHeaderForBob);

            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            using (HttpResponseMessage response = client.Post("AuthHome/create", content, "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/xml; charset=utf-8", response.Content.ContentType);
            }

            client.DefaultHeaders.Remove(psuedoAuthHeaderForBob);
        }

        [TestMethod]
        public void PostMoviesWithoutAuth()
        {
            string director = "Wachowski";
            string title = "The Matrix";
            DateTime dateReleased = new DateTime(1999, 3, 31);

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };

            using (HttpResponseMessage response = client.Post("AuthHome/create", HttpContentExtensions.CreateJsonDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [TestMethod]
        public void PostMoviesWithInvalidAuth()
        {
            string director = "Allen";
            string title = "Annie Hall";
            DateTime dateReleased = new DateTime(1977, 4, 30);

            client.DefaultHeaders.Add(psuedoAuthHeaderForPhil);

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };

            using (HttpResponseMessage response = client.Post("AuthHome/create", HttpContentExtensions.CreateJsonDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            client.DefaultHeaders.Remove(psuedoAuthHeaderForPhil);
        }

        [TestMethod]
        public void EditMoviesWithoutAuth()
        {
            string director = "Crichton";
            string title = "A Fish Called Wanda";
            DateTime dateReleased = new DateTime(1988, 7, 15);

            Movie movieToEdit;

            using (HttpResponseMessage response = client.Get("AuthHome/details/1", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                movieToEdit = response.Content.ReadAsJsonDataContract<Movie>();
            }

            movieToEdit.DateReleased = dateReleased;
            movieToEdit.Director = director;
            movieToEdit.Title = title;

            using (HttpResponseMessage response = client.Put("AuthHome/edit/1", HttpContentExtensions.CreateJsonDataContract(movieToEdit)))
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [TestMethod]
        public void EditMoviesWithInvalidAuth()
        {
            string director = "Beresford";
            string title = "Driving Miss Daisy";
            DateTime dateReleased = new DateTime(1990, 1, 26);

            client.DefaultHeaders.Add(psuedoAuthHeaderForPhil);

            Movie movieToEdit;

            using (HttpResponseMessage response = client.Get("AuthHome/details/1", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                movieToEdit = response.Content.ReadAsJsonDataContract<Movie>();
            }

            movieToEdit.DateReleased = dateReleased;
            movieToEdit.Director = director;
            movieToEdit.Title = title;

            using (HttpResponseMessage response = client.Put("AuthHome/edit/1", HttpContentExtensions.CreateJsonDataContract(movieToEdit)))
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            client.DefaultHeaders.Remove(psuedoAuthHeaderForPhil);
        }

        [TestMethod]
        public void DeleteMoviesWithoutAuth()
        {
            using (HttpResponseMessage response = client.Delete("AuthHome/Delete/1"))
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [TestMethod]
        public void DeleteMoviesWithInvalidAuth()
        {
            client.DefaultHeaders.Add(psuedoAuthHeaderForPhil);

            using (HttpResponseMessage response = client.Delete("AuthHome/Delete/1"))
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            }

            client.DefaultHeaders.Remove(psuedoAuthHeaderForPhil);
        }

        [TestMethod]
        public void CrudMoviesJson()
        {
            string director = "Donen";
            string title = "Singin' in the Rain";
            DateTime dateReleased = new DateTime(1952, 4, 11);
            List<Movie> originalMovieList;
            List<Movie> updatedMovieList;

            client.DefaultHeaders.Add(psuedoAuthHeaderForBob);

            using (HttpResponseMessage response = client.Get("AuthHome", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
            }

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };

            using (HttpResponseMessage response = client.Post("AuthHome/create", HttpContentExtensions.CreateJsonDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("AuthHome", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.Equals(movieToInsert, insertedMovie));

            director = "Kelly"; // multiple director credits
            insertedMovie.Director = director;

            using (HttpResponseMessage response = client.Put("AuthHome/Edit/" + insertedMovie.Id.ToString(), HttpContentExtensions.CreateJsonDataContract(insertedMovie)))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("AuthHome", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
            }

            Movie updatedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.Equals(insertedMovie, updatedMovie));

            using (HttpResponseMessage response = client.Delete("AuthHome/delete/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("AuthHome", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
            }

            Assert.IsTrue(updatedMovieList.Union(originalMovieList, moviesComparer).Count() == updatedMovieList.Count());
            client.DefaultHeaders.Remove(psuedoAuthHeaderForBob);
        }

        [TestMethod]
        public void CrudMoviesXml()
        {
            string director = "Nichols";
            string title = "The Graduate";
            DateTime dateReleased = new DateTime(1967, 12, 21);
            List<Movie> originalMovieList;
            List<Movie> updatedMovieList;

            client.DefaultHeaders.Add(psuedoAuthHeaderForBob);

            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };
            using (HttpResponseMessage response = client.Post("AuthHome/create", HttpContentExtensions.CreateDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.Equals(movieToInsert, insertedMovie));

            dateReleased = new DateTime(1997, 2, 14); // US re-release date
            insertedMovie.DateReleased = dateReleased;

            using (HttpResponseMessage response = client.Put("AuthHome/Edit/" + insertedMovie.Id.ToString(), HttpContentExtensions.CreateDataContract(insertedMovie)))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie updatedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.Equals(insertedMovie, updatedMovie));

            using (HttpResponseMessage response = client.Delete("AuthHome/delete/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Assert.IsTrue(updatedMovieList.Union(originalMovieList, moviesComparer).Count() == updatedMovieList.Count());
            client.DefaultHeaders.Remove(psuedoAuthHeaderForBob);
        }

        [TestMethod]
        public void CrudMoviesForm()
        {
            string director = "Reiner";
            string title = "This is Spinal Tap";
            DateTime dateReleased = new DateTime(1984, 4, 2);
            List<Movie> originalMovieList;
            List<Movie> updatedMovieList;

            client.DefaultHeaders.Add(psuedoAuthHeaderForBob);

            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            using (HttpResponseMessage response = client.Post("AuthHome/create", HttpContent.Create(new HttpUrlEncodedForm() { { "Director", director }, { "Title", title }, { "DateReleased", dateReleased.ToString() } })))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsNotNull(insertedMovie);
            Assert.AreEqual(director, insertedMovie.Director);
            Assert.AreEqual(title, insertedMovie.Title);
            Assert.AreEqual(dateReleased, insertedMovie.DateReleased);

            using (HttpResponseMessage response = client.Delete("AuthHome/delete/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
            client.DefaultHeaders.Remove(psuedoAuthHeaderForBob);
        }

        [TestMethod]
        public void CrudMoviesFormBrowser()
        {
            string director = "Reiner";
            string title = "This is Spinal Tap";
            DateTime dateReleased = new DateTime(1984, 4, 2);
            List<Movie> originalMovieList;
            List<Movie> updatedMovieList;

            client.DefaultHeaders.Add(psuedoAuthHeaderForBob);

            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            using (HttpRequestMessage request = new HttpRequestMessage("POST", new Uri("AuthHome/create", UriKind.RelativeOrAbsolute), HttpContent.Create(new HttpUrlEncodedForm() { { "Director", director }, { "Title", title }, { "DateReleased", dateReleased.ToString() } })))
            {
                // query string is only available to browsers, pretend we're a browser
                request.Headers.UserAgent.Add(TestHttpClient.Mozilla40);
                using (HttpResponseMessage response = client.Send(request))
                {
                    Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
                }
            }
            using (HttpResponseMessage response = client.Get("AuthHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsNotNull(insertedMovie);
            Assert.AreEqual(director, insertedMovie.Director);
            Assert.AreEqual(title, insertedMovie.Title);
            Assert.AreEqual(dateReleased, insertedMovie.DateReleased);

            using (HttpResponseMessage response = client.Delete("AuthHome/delete/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
            client.DefaultHeaders.Remove(psuedoAuthHeaderForBob);
        }
    }
}
