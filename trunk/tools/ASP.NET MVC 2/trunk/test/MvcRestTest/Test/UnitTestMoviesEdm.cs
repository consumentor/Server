using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieApp.Models.Edm;

namespace MovieApp.Test
{
    [TestClass]
    public class UnitTestMoviesEdm : MoviesTestBase
    {
        new EdmMoviesComparer moviesComparer = new EdmMoviesComparer();

        [TestMethod]
        public void MoviesGetIndexXml()
        {
            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
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
            using (HttpResponseMessage response = client.Get("EdmMovies/99", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesGetMoviesJson()
        {
            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesNotAcceptable()
        {
            using (HttpResponseMessage response = client.Get("EdmMovies", "foo"))
            {
                Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeXml()
        {
            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            using (HttpResponseMessage response = client.Post("EdmMovies", content, "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/xml; charset=utf-8", response.Content.ContentType);
            }
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeJson()
        {
            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            using (HttpResponseMessage response = client.Post("EdmMovies", content, "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/json2; charset=utf-8", response.Content.ContentType);
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

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsJsonDataContract2<List<Movie>>();
            }

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };
            using (HttpResponseMessage response = client.Post("EdmMovies", "application/json2", JsonContentExtensions.CreateJsonDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.IsTrue(response.Headers.Location.ToString().StartsWith("/MovieApp/EdmMovies/", StringComparison.OrdinalIgnoreCase));
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract2<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.EqualsIgnoreTimeSkew(movieToInsert, insertedMovie));

            director = "Kelly"; // multiple director credits
            insertedMovie.Director = director;

            using (HttpResponseMessage response = client.Put("EdmMovies/" + insertedMovie.Id.ToString(), "application/json2", JsonContentExtensions.CreateJsonDataContract(insertedMovie)))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract2<List<Movie>>();
            }

            Movie updatedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.EqualsIgnoreTimeSkew(insertedMovie, updatedMovie));

            using (HttpResponseMessage response = client.Delete("EdmMovies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract2<List<Movie>>();
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

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie movieToInsert = new Movie() { Director = director, DateReleased = dateReleased, Title = title };
            using (HttpResponseMessage response = client.Post("EdmMovies", HttpContentExtensions.CreateDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.IsTrue(response.Headers.Location.ToString().StartsWith("/MovieApp/EdmMovies/", StringComparison.OrdinalIgnoreCase));
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.EqualsIgnoreTimeSkew(movieToInsert, insertedMovie));

            dateReleased = new DateTime(1997, 2, 14); // US re-release date
            insertedMovie.DateReleased = dateReleased;

            using (HttpResponseMessage response = client.Put("EdmMovies/" + insertedMovie.Id.ToString(), HttpContentExtensions.CreateDataContract(insertedMovie)))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie updatedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.EqualsIgnoreTimeSkew(insertedMovie, updatedMovie));

            using (HttpResponseMessage response = client.Delete("EdmMovies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
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

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            using (HttpResponseMessage response = client.Post("EdmMovies", HttpContent.Create(new HttpUrlEncodedForm() { { "Director", director }, { "Title", title }, { "DateReleased", dateReleased.ToString() } })))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsNotNull(insertedMovie);
            Assert.AreEqual(director, insertedMovie.Director);
            Assert.AreEqual(title, insertedMovie.Title);
            Assert.AreEqual(dateReleased, insertedMovie.DateReleased);

            using (HttpResponseMessage response = client.Delete("EdmMovies/" + insertedMovie.Id.ToString()))
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

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            using (HttpRequestMessage request = new HttpRequestMessage("POST", new Uri("EdmMovies", UriKind.RelativeOrAbsolute), HttpContent.Create(new HttpUrlEncodedForm() { { "Director", director }, { "Title", title }, { "DateReleased", dateReleased.ToString() } })))
            {
                // query string is only available to browsers, pretend we're a browser
                request.Headers.UserAgent.Add(TestHttpClient.Mozilla40);
                using (HttpResponseMessage response = client.Send(request))
                {
                    Assert.AreEqual(HttpStatusCode.Found, response.StatusCode);
                }
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsDataContract<List<Movie>>();
            }

            Movie insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsNotNull(insertedMovie);
            Assert.AreEqual(director, insertedMovie.Director);
            Assert.AreEqual(title, insertedMovie.Title);
            Assert.AreEqual(dateReleased, insertedMovie.DateReleased);

            using (HttpResponseMessage response = client.Delete("EdmMovies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        public class MovieDC
        {
            public MovieDC()
            {
            }

            public DateTime DateReleased { get; set; }
            public string Director { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }

            public class MoviesComparerDC : EqualityComparer<MovieDC>
            {
                public bool EqualsIgnoreTimeSkew(MovieDC x, MovieDC y)
                {
                    if (x.Director == y.Director &&
                        x.Title == y.Title)
                    {
                        if (x.DateReleased == y.DateReleased)
                        {
                            return true;
                        }
                        else if ((x.DateReleased - y.DateReleased).TotalDays < 1.0d)
                        {
                            Console.WriteLine("WARNING: ignored time delta: " + x.DateReleased.ToString("G") + " <=> " + y.DateReleased.ToString("G"));
                            return true;
                        }
                    }
                    return false;
                }

                public override bool Equals(MovieDC x, MovieDC y)
                {
                    if (x.Director == y.Director &&
                        x.Title == y.Title &&
                        x.DateReleased == y.DateReleased)
                    {
                        return true;
                    }
                    return false;
                }

                public override int GetHashCode(MovieDC obj)
                {
                    return obj.Director.GetHashCode() ^ obj.Title.GetHashCode() ^ obj.DateReleased.GetHashCode();
                }
            }
        }

        [TestMethod]
        public void CrudMoviesJsonDC()
        {
            string director = "Donen";
            string title = "Singin' in the Rain";
            DateTime dateReleased = new DateTime(1952, 4, 11);
            List<MovieDC> originalMovieList;
            List<MovieDC> updatedMovieList;
            MovieDC.MoviesComparerDC moviesComparer = new MovieDC.MoviesComparerDC();

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                originalMovieList = response.Content.ReadAsJsonDataContract<List<MovieDC>>();
            }

            MovieDC movieToInsert = new MovieDC() { Director = director, DateReleased = dateReleased, Title = title };
            using (HttpResponseMessage response = client.Post("EdmMovies", "application/json2", HttpContentExtensions.CreateJsonDataContract(movieToInsert)))
            {
                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.IsTrue(response.Headers.Location.ToString().StartsWith("/MovieApp/EdmMovies/", StringComparison.OrdinalIgnoreCase));
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<MovieDC>>();
            }

            MovieDC insertedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.EqualsIgnoreTimeSkew(movieToInsert, insertedMovie));

            director = "Kelly"; // multiple director credits
            insertedMovie.Director = director;

            using (HttpResponseMessage response = client.Put("EdmMovies/" + insertedMovie.Id.ToString(), "application/json2", HttpContentExtensions.CreateJsonDataContract(insertedMovie)))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<MovieDC>>();
            }

            MovieDC updatedMovie = updatedMovieList.Except(originalMovieList, moviesComparer).SingleOrDefault();
            Assert.IsTrue(moviesComparer.EqualsIgnoreTimeSkew(insertedMovie, updatedMovie));

            using (HttpResponseMessage response = client.Delete("EdmMovies/" + insertedMovie.Id.ToString()))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

            using (HttpResponseMessage response = client.Get("EdmMovies", "application/json2"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                updatedMovieList = response.Content.ReadAsJsonDataContract<List<MovieDC>>();
            }

            Assert.IsTrue(updatedMovieList.Union(originalMovieList, moviesComparer).Count() == updatedMovieList.Count());
        }

        public class EdmMoviesComparer : EqualityComparer<Movie>
        {
            public bool EqualsIgnoreTimeSkew(Movie x, Movie y)
            {
                if (x.Director == y.Director &&
                    x.Title == y.Title)
                {
                    if (x.DateReleased == y.DateReleased)
                    {
                        return true;
                    }
                    else if ((x.DateReleased - y.DateReleased).TotalDays < 1.0d)
                    {
                        Console.WriteLine("WARNING: ignored time delta: " + x.DateReleased.ToString("G") + " <=> " + y.DateReleased.ToString("G"));
                        return true;
                    }
                }
                return false;
            }

            public override bool Equals(Movie x, Movie y)
            {
                if (x.Director == y.Director &&
                    x.Title == y.Title &&
                    x.DateReleased == y.DateReleased)
                {
                    return true;
                }
                return false;
            }

            public override int GetHashCode(Movie obj)
            {
                return obj.Director.GetHashCode() ^ obj.Title.GetHashCode() ^ obj.DateReleased.GetHashCode();
            }
        }
    }
}
