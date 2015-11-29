using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieApp.Models;

namespace MovieApp.Test
{
    [TestClass]
    public class UnitTestMoviesAtomResource : MoviesTestBase
    {
        SyndicationMoviesComparer entriesComparer = new SyndicationMoviesComparer();

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
        public void MoviesGetIndexAtom()
        {
            using (HttpResponseMessage response = client.Get("Movies", "application/atom+xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("application/atom+xml; charset=utf-8", response.Content.ContentType);
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
        public void MoviesUnsupportedMediaTypeJson()
        {
            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            content.LoadIntoBuffer();
            using (HttpResponseMessage response = client.Post("Movies", content, "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/json; charset=utf-8", response.Content.ContentType);
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
        public void CrudMoviesAtom()
        {
            string director = "Donen";
            string title = "Singin' in the Rain";
            DateTime dateReleased = new DateTime(1952, 4, 11);
            SyndicationFeed originalMovieList;
            SyndicationFeed updatedMovieList;

            using (HttpResponseMessage response = client.Get("Movies", "application/atom+xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsSyndicationFeed();
            }

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };
            SyndicationItem item = new SyndicationItem() { Title = new TextSyndicationContent(movieToInsert.Title), Content = new XmlSyndicationContent(null, movieToInsert, new DataContractSerializer(typeof(Movie))) };
            HttpContent itemContent = HttpContent.Create((s) => { using (XmlWriter w = XmlWriter.Create(s)) { item.SaveAsAtom10(w); } }, "application/atom+xml;type=entry", null);
            itemContent.LoadIntoBuffer();
            using (HttpResponseMessage response = client.Post("Movies", itemContent))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.IsTrue(response.Headers.Location.ToString().StartsWith("/MovieApp/movies/", StringComparison.OrdinalIgnoreCase));
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/atom+xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsSyndicationFeed();
            }

            SyndicationItem insertedMovieItem = updatedMovieList.Items.Except(originalMovieList.Items, entriesComparer).SingleOrDefault();
            Movie insertedMovie = ((XmlSyndicationContent)insertedMovieItem.Content).ReadContent<Movie>();
            Assert.IsTrue(moviesComparer.Equals(movieToInsert, insertedMovie));

            director = "Kelly"; // multiple director credits
            insertedMovie.Director = director;
            item = new SyndicationItem() { Title = new TextSyndicationContent(insertedMovie.Title), Content = new XmlSyndicationContent(null, insertedMovie, new DataContractSerializer(typeof(Movie))) };
            itemContent = HttpContent.Create((s) => { using (XmlWriter w = XmlWriter.Create(s)) { item.SaveAsAtom10(w); } }, "application/atom+xml;type=entry", null);
            itemContent.LoadIntoBuffer();
            using (HttpResponseMessage response = client.Put("Movies/" + insertedMovie.Id.ToString(), itemContent))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/atom+xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsSyndicationFeed();
            }

            SyndicationItem updatedMovieItem = updatedMovieList.Items.Except(originalMovieList.Items, entriesComparer).SingleOrDefault();
            Movie updatedMovie = ((XmlSyndicationContent)updatedMovieItem.Content).ReadContent<Movie>();
            Assert.IsTrue(moviesComparer.Equals(insertedMovie, updatedMovie));

            using (HttpResponseMessage response = client.Delete("Movies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("Movies", "application/atom+xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsSyndicationFeed();
            }

            Assert.IsTrue(updatedMovieList.Items.Union(originalMovieList.Items, entriesComparer).Count() == updatedMovieList.Items.Count());
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

        class SyndicationMoviesComparer : EqualityComparer<SyndicationItem>
        {
            public override bool Equals(SyndicationItem x, SyndicationItem y)
            {
                Movie movieX = ((XmlSyndicationContent)x.Content).ReadContent<Movie>();
                Movie movieY = ((XmlSyndicationContent)y.Content).ReadContent<Movie>();
                return new MoviesComparer().Equals(movieX, movieY);
            }

            public override int GetHashCode(SyndicationItem item)
            {
                Movie movie = ((XmlSyndicationContent)item.Content).ReadContent<Movie>();
                return new MoviesComparer().GetHashCode(movie);
            }
        }
    }
}
