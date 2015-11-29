using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieApp.Models;

namespace MovieApp.Test
{
    [TestClass]
    public class UnitTestMoviesCaching : MoviesTestBase
    {
        [TestMethod]
        public void MoviesGetIndexXml()
        {
            using (HttpResponseMessage response = client.Get("CachingHome", "application/xml"))
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
            using (HttpResponseMessage response = client.Get("CachingHome/details/99", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesGetMoviesXml()
        {
            using (HttpResponseMessage response = client.Get("CachingHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesGetMoviesJson()
        {
            using (HttpResponseMessage response = client.Get("CachingHome", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesNotAcceptable()
        {
            using (HttpResponseMessage response = client.Get("CachingHome", "foo"))
            {
                Assert.AreEqual(HttpStatusCode.NotAcceptable, response.StatusCode);
            }
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeJson()
        {
            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            using (HttpResponseMessage response = client.Post("CachingHome/create", content, "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/json; charset=utf-8", response.Content.ContentType);
            }
        }

        [TestMethod]
        public void MoviesUnsupportedMediaTypeXml()
        {
            HttpContent content = HttpContent.Create(new MemoryStream(new byte[] { 1, 2, 3 }), "foo", null);
            using (HttpResponseMessage response = client.Post("CachingHome/create", content, "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.UnsupportedMediaType, response.StatusCode);
                Assert.AreEqual("application/xml; charset=utf-8", response.Content.ContentType);
            }
        }

        [TestMethod]
        public void CachingMovies()
        {
            List<Movie> movieList;

            DateTime lastModifiedJson;
            using (HttpRequestMessage request = new HttpRequestMessage("GET", "CachingHome"))
            {
                request.Headers.Accept.Add(new StringWithOptionalQuality("application/json"));
                request.Headers.CacheControl = new CacheControl { NoCache = true, MustRevalidate = true };
                using (HttpResponseMessage response = client.Send(request))
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    movieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
                    Assert.AreEqual("*", response.Headers.Vary.First());
                    lastModifiedJson = response.Headers.LastModified.Value;
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(1.1));

            DateTime lastModifiedXml;
            using (HttpRequestMessage request = new HttpRequestMessage("GET", "CachingHome"))
            {
                request.Headers.Accept.Add(new StringWithOptionalQuality("application/xml"));
                request.Headers.CacheControl = new CacheControl { NoCache = true, MustRevalidate = true };
                using (HttpResponseMessage response = client.Send(request))
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                    movieList = response.Content.ReadAsDataContract<List<Movie>>();
                    Assert.AreEqual("*", response.Headers.Vary.First());
                    lastModifiedXml = response.Headers.LastModified.Value;
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(1.1));

            using (HttpResponseMessage response = client.Get("CachingHome", "application/json"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                movieList = response.Content.ReadAsJsonDataContract<List<Movie>>();
                Assert.AreEqual("*", response.Headers.Vary.First());
                Assert.AreEqual(lastModifiedJson, response.Headers.LastModified.Value);
            }

            using (HttpResponseMessage response = client.Get("CachingHome", "application/xml"))
            {
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                movieList = response.Content.ReadAsDataContract<List<Movie>>();
                Assert.AreEqual("*", response.Headers.Vary.First());
                Assert.AreEqual(lastModifiedXml, response.Headers.LastModified.Value);
            }

            using (HttpRequestMessage request = new HttpRequestMessage("GET", "CachingHome"))
            {
                request.Headers.Accept.Add(new StringWithOptionalQuality("application/json"));
                request.Headers.IfModifiedSince = lastModifiedJson;
                using (HttpResponseMessage response = client.Send(request))
                {
                    Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode);
                }
            }

            using (HttpRequestMessage request = new HttpRequestMessage("GET", "CachingHome"))
            {
                request.Headers.Accept.Add(new StringWithOptionalQuality("application/xml"));
                request.Headers.IfModifiedSince = lastModifiedXml;
                using (HttpResponseMessage response = client.Send(request))
                {
                    Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode);
                }
            }
        }
    }
}
