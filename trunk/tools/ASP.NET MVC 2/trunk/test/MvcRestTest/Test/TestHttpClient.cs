using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.Http;
using Microsoft.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.WebHost;

namespace MovieApp.Test
{
    public class MoviesTestBase
    {
        protected HttpClient client;
        protected MoviesComparer moviesComparer;

        public MoviesTestBase()
        {
            client = new TestHttpClient();
            moviesComparer = new MoviesComparer();
        }

        TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
    }

    [TestClass]
    public class TestCommon
    {
        public static int AspPort = 59341;
        public static string AspVirtualPath = "/MovieApp";
        public static string AspBaseAddress = "http://localhost:" + AspPort + AspVirtualPath + "/";

        static Server server;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // make relative to where TestResults are dropped
            string hostedRoot = Path.Combine(context.TestDir, @"..\..");
            // make relative to where the MvcRestTest folder is located
            hostedRoot = Path.Combine(hostedRoot, @"test\MvcRestTest");
            hostedRoot = new DirectoryInfo(hostedRoot).FullName;
            string hostedLocation = Path.Combine(hostedRoot, "bin");

            // check that DB files exists and are R/W or tests will fail
            EnsureExistsAndRW(hostedRoot, @"App_Data\MoviesDB.mdf");

            // update assemblies in hosted path with the ones in the current location if they're more
            // recent as they might be instrumented for code coverage
            // this will also deploy our test assembly to the ASP app, so it can load any HttpResources we define here
            string location = Path.GetDirectoryName(typeof(TestCommon).Assembly.Location);
            TestCommon.MergeBinaries(location, hostedLocation);

            // start the WebHost Http server
            server = new Server(TestCommon.AspPort, TestCommon.AspVirtualPath, hostedRoot, false, false);
            server.Start();
        }

        static void EnsureExistsAndRW(string dir, string fileName)
        {
            fileName = Path.Combine(dir, fileName);
            // check that the file exists
            FileInfo fi = new FileInfo(fileName);
            if (!fi.Exists)
            {
                throw new Exception("Required file " + fi.Name + " not found, should have been here: " + fi.FullName);
            }
            // check that the file is not ReadOnly
            if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                throw new Exception("File " + fi.Name + " is ReadOnly, it mustn't be for tests to pass: " + fi.Attributes);
            }
        }

        static void MergeBinaries(string location, string hostedLocation)
        {
            List<string> sources = new List<string>();
            sources.AddRange(Directory.GetFiles(location, "*.dll"));
            sources.AddRange(Directory.GetFiles(location, "*.exe"));
            sources.AddRange(Directory.GetFiles(location, "*.pdb"));
            foreach (string source in sources)
            {
                string destination = Path.Combine(hostedLocation, Path.GetFileName(source));
                // Debug.WriteLine("Considering: " + source + " to: " + destination);
                bool overwrite = false;
                if (File.Exists(destination))
                {
                    FileInfo s = new FileInfo(source);
                    FileInfo d = new FileInfo(destination);
                    if (s.CreationTime <= d.CreationTime)
                    {
                        // Debug.WriteLine("Skipping: " + source);
                        continue;
                    }
                    overwrite = true;
                }
                // Debug.WriteLine("Copying: " + source + " to: " + destination + " overwrite: " + overwrite);
                File.Copy(source, destination, overwrite);
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            server.Stop();
        }
    }

    class TestHttpClient : HttpClient
    {
        public static readonly ProductOrComment Mozilla40 = new ProductOrComment(new Product("Mozilla", "4.0"));

        public TestHttpClient()
            : base(TestCommon.AspBaseAddress)
        {
            ServicePointManager.Expect100Continue = false;
            this.TransportSettings.MaximumAutomaticRedirections = 0;
            this.Stages.Add(new TracingStage());
        }

        class TracingStage : HttpStage
        {
            protected override void ProcessRequestAndTryGetResponse(HttpRequestMessage request, out HttpResponseMessage response, out object state)
            {
                if (!HttpContent.IsNullOrEmpty(request.Content))
                {
                    // request.Content.LoadIntoBuffer();
                }
                response = null;
                state = null;
            }

            protected override void ProcessResponse(HttpResponseMessage response, object state)
            {
                Console.WriteLine(response.Request);
                Console.WriteLine(response.Request.Headers);
                DumpContent(response.Request.Content);
                Console.WriteLine(response);
                Console.WriteLine(response.Headers);
                DumpContent(response.Content);
                Console.WriteLine("----------------------------------");
            }

            void DumpContent(HttpContent httpContent)
            {
                if (!HttpContent.IsNullOrEmpty(httpContent))
                {
                    string content = null;
                    try
                    {
                        httpContent.LoadIntoBuffer();
                        content = httpContent.ReadAsString();
                    }
                    catch (Exception exception)
                    {
                        content = "$Exception: " + exception.ToString() + "$";
                    }
                    if (!string.IsNullOrEmpty(content))
                    {
                        Console.WriteLine(content);
                    }
                }
            }
        }
    }

    public static class HttpClientExtensions
    {
        // copied from HttpRequestExtensions
        const string XHttpMethodOverrideKey = "X-HTTP-Method-Override";

        public static HttpResponseMessage Get(this HttpClient client, string uri, string acceptContentType)
        {
            HttpRequestMessage request = new HttpRequestMessage("GET", uri);
            request.Headers.Accept.AddString(acceptContentType);
            return client.Send(request);
        }

        public static HttpResponseMessage Delete(this HttpClient client, string uri, string acceptContentType)
        {
            HttpRequestMessage request = new HttpRequestMessage("DELETE", uri);
            request.Headers.Accept.Add(new StringWithOptionalQuality(acceptContentType));
            return client.Send(request);
        }

        public static HttpResponseMessage Post(this HttpClient client, string uri, HttpContent content, string acceptContentType)
        {
            return client.Post(uri, content, acceptContentType, null);
        }

        public static HttpResponseMessage Post(this HttpClient client, string uri, HttpContent content, string acceptContentType, string httpMethod)
        {
            HttpRequestMessage request = new HttpRequestMessage("POST", new Uri(uri, UriKind.RelativeOrAbsolute), content);
            if (!string.IsNullOrEmpty(acceptContentType))
            {
                request.Headers.Accept.Add(new StringWithOptionalQuality(acceptContentType));
            }
            if (!string.IsNullOrEmpty(httpMethod))
            {
                request.Headers[XHttpMethodOverrideKey] = httpMethod;
            }
            return client.Send(request);
        }
    }

    public static class JsonContentExtensions
    {
        public static T ReadAsJsonDataContract2<T>(this HttpContent content)
        {
            string input = content.ReadAsString();
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Deserialize<T>(input);
        }

        public static HttpContent CreateJsonDataContract<T>(T value)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            StringBuilder sb = new StringBuilder();
            json.Serialize(value, sb);
            return HttpContent.Create(sb.ToString(), Encoding.UTF8, "application/json");
        }
    }
}
