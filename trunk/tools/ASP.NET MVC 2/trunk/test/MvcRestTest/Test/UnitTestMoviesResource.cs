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
    public class UnitTestMoviesResource : MoviesTestBase
    {
        [TestMethod]
        public void MoviesGetIndexXml()
        {
            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
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
            using (HttpResponseMessage response = client.Get("Movies/99", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesGetMoviesJson()
        {
            using (HttpResponseMessage response = client.Get("Movies", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesNotAcceptable()
        {
            using (HttpResponseMessage response = client.Get("Movies", "foo"))
            {
                Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesNotAcceptableCharset()
        {
            using (HttpResponseMessage response = client.Get("Movies", "application/xml; charset=foo"))
            {
                Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesMultiAcceptWithPriorities()
        {
            using (HttpResponseMessage response = client.Get("Movies", "application/xml;q=0.9,*/*;q=0.7,text/plain;q=0.8,throw/on-this,image/png,application/json;q=1.0"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("application/json; charset=utf-8", response.Content.ContentType);
            }
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeXml()
        {
            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            using (HttpResponseMessage response = client.Post("Movies", content, "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/xml; charset=utf-8", response.Content.ContentType);
            }
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeXmlCharset()
        {
            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "application/xml; charset=foo", null);
            using (HttpResponseMessage response = client.Post("Movies", content, "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/xml; charset=utf-8", response.Content.ContentType);
            }
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeJson()
        {
            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            using (HttpResponseMessage response = client.Post("Movies", content, "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/json; charset=utf-8", response.Content.ContentType);
            }
        }

        [TestMethod]
        public void PostInvalidMovieNoTitleJson()
        {
            Movie noTitle = new Movie() { Director = "The Director", DateReleased = DateTime.Now };
            using (HttpResponseMessage response = client.Post("Movies", HttpContentExtensions.CreateJsonDataContract(noTitle)))
            {
                Assert.AreEqual(HttpStatusCode.ExpectationFailed, response.StatusCode);
                Assert.IsTrue(response.Content.ReadAsString().Contains("The Title field is required."));
            }
        }

        [TestMethod]
        public void PostInvalidMovieNoDirectorXml()
        {
            Movie noDirector = new Movie() { Title = "The Title", DateReleased = DateTime.Now };
            using (HttpResponseMessage response = client.Post("Movies", HttpContentExtensions.CreateDataContract(noDirector)))
            {
                Assert.AreEqual(HttpStatusCode.ExpectationFailed, response.StatusCode);
                Assert.IsTrue(response.Content.ReadAsString().Contains("Please enter the name of the director of this movie."));
            }
        }

        [TestMethod]
        public void PostInvalidMovieFutureXml()
        {
            Movie future = new Movie() { Director = "The Director", Title = "The Title", DateReleased = DateTime.Now.Add(TimeSpan.FromDays(300)) };
            using (HttpResponseMessage response = client.Post("Movies", HttpContentExtensions.CreateDataContract(future)))
            {
                Assert.AreEqual(HttpStatusCode.ExpectationFailed, response.StatusCode);
                Assert.IsTrue(response.Content.ReadAsString().Contains("Value for Date Released must be between"));
            }
        }

        [TestMethod]
        public void CrudMoviesJson()
        {
            string director = "Donen";
            string title = "Singin' in the Rain";
            DateTime dateReleased = new DateTime(1952, 4, 11);
            List<Movie> originalMovieList;
            List<Movie> updatedMovieList;

            using (HttpResponseMessage response = client.Get("Movies", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
            }

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };
            using (HttpResponseMessage response = client.Post("Movies", HttpContentExtensions.CreateJsonDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.IsTrue(response.Headers.Location.ToString().StartsWith("/MovieApp/movies/", StringComparison.OrdinalIgnoreCase));
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.Equals(movieToInsert, insertedMovie));

            director = "Kelly"; // multiple director credits
            insertedMovie.Director = director;

            using (HttpResponseMessage response = client.Put("Movies/" + insertedMovie.Id.ToString(), HttpContentExtensions.CreateJsonDataContract(insertedMovie)))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
            }

            Movie updatedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.Equals(insertedMovie, updatedMovie));

            using (HttpResponseMessage response = client.Delete("Movies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
            }

            Assert.IsTrue(updatedMovieList.Union(originalMovieList, moviesComparer).Count() == updatedMovieList.Count());
        }

        [TestMethod]
        public void CrudMoviesXml()
        {
            string director = "Nichols";
            string title = "The Graduate";
            DateTime dateReleased = new DateTime(1967, 12, 21);
            List<Movie> originalMovieList;
            List<Movie> updatedMovieList;

            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };
            using (HttpResponseMessage response = client.Post("Movies", HttpContentExtensions.CreateDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.IsTrue(response.Headers.Location.ToString().StartsWith("/MovieApp/movies/", StringComparison.OrdinalIgnoreCase));
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.Equals(movieToInsert, insertedMovie));

            dateReleased = new DateTime(1997, 2, 14); // US re-release date
            insertedMovie.DateReleased = dateReleased;

            using (HttpResponseMessage response = client.Put("Movies/" + insertedMovie.Id.ToString(), HttpContentExtensions.CreateDataContract(insertedMovie)))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie updatedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.Equals(insertedMovie, updatedMovie));

            using (HttpResponseMessage response = client.Delete("Movies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Assert.IsTrue(updatedMovieList.Union(originalMovieList, moviesComparer).Count() == updatedMovieList.Count());
        }

        [TestMethod]
        public void CrudMoviesForm()
        {
            string director = "Reiner";
            string title = "This is Spinal Tap";
            DateTime dateReleased = new DateTime(1984, 4, 2);
            List<Movie> originalMovieList;
            List<Movie> updatedMovieList;

            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            using (HttpResponseMessage response = client.Post("Movies", HttpContent.Create(new HttpUrlEncodedForm() { { "Director", director }, { "Title", title }, { "DateReleased", dateReleased.ToString() } })))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsNotNull(insertedMovie);
            Assert.AreEqual(director, insertedMovie.Director);
            Assert.AreEqual(title, insertedMovie.Title);
            Assert.AreEqual(dateReleased, insertedMovie.DateReleased);

            using (HttpResponseMessage response = client.Delete("Movies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void CrudMoviesFormBrowser()
        {
            string director = "Reiner";
            string title = "This is Spinal Tap";
            DateTime dateReleased = new DateTime(1984, 4, 2);
            List<Movie> originalMovieList;
            List<Movie> updatedMovieList;

            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            using (HttpRequestMessage request = new HttpRequestMessage("POST", new Uri("Movies", UriKind.RelativeOrAbsolute), HttpContent.Create(new HttpUrlEncodedForm() { { "Director", director }, { "Title", title }, { "DateReleased", dateReleased.ToString() } })))
            {
                // query string is only available to browsers, pretend we're a browser
                request.Headers.UserAgent.Add(TestHttpClient.Mozilla40);
                using (HttpResponseMessage response = client.Send(request))
                {
                    Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
                }
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsNotNull(insertedMovie);
            Assert.AreEqual(director, insertedMovie.Director);
            Assert.AreEqual(title, insertedMovie.Title);
            Assert.AreEqual(dateReleased, insertedMovie.DateReleased);

            using (HttpResponseMessage response = client.Delete("Movies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
