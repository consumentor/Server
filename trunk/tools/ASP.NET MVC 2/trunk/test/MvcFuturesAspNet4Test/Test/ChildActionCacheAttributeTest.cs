namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ChildActionCacheAttributeTest {

        [TestMethod]
        public void OnActionExecuting_CacheHit_ReturnsContentResult() {
            // Arrange
            Mock<ActionExecutingContext> mockFilterContext = new Mock<ActionExecutingContext>();
            mockFilterContext.Expect(o => o.IsChildAction).Returns(true);
            mockFilterContext.Expect(o => o.ActionDescriptor.ActionName).Returns("theActionName");
            mockFilterContext.Expect(o => o.ActionDescriptor.ControllerDescriptor.ControllerName).Returns("theControllerName");
            mockFilterContext.Expect(o => o.HttpContext.Items).Never();
            mockFilterContext.Expect(o => o.HttpContext.Response.ContentEncoding).Returns(Encoding.UTF8);
            mockFilterContext.Expect(o => o.ActionParameters).Returns(new Dictionary<string, object> {
                { "forty-two", 42 },
                { "eighty-four", 84 }
            });

            Mock<ITestableCache> mockCache = new Mock<ITestableCache>();
            mockCache.Expect(o => o.GetCacheItem(It.IsAny<string>())).Returns("some cached content");

            ChildActionCacheAttribute attr = new TestableChildActionCacheAttribute(mockCache.Object) {
                Duration = 10
            };

            // Act
            ActionExecutingContext filterContext = mockFilterContext.Object;
            attr.OnActionExecuting(filterContext);

            // Assert
            Assert.IsInstanceOfType(filterContext.Result, typeof(ContentResult));
            Assert.AreEqual("some cached content", ((ContentResult)filterContext.Result).Content);
        }

        [TestMethod]
        public void OnActionExecuting_CacheMiss_SetsFlagInItems() {
            // Arrange
            string expectedCacheKey = null;

            Mock<ActionExecutingContext> mockFilterContext = new Mock<ActionExecutingContext>();
            mockFilterContext.Expect(o => o.IsChildAction).Returns(true);
            mockFilterContext.Expect(o => o.ActionDescriptor.ActionName).Returns("theActionName");
            mockFilterContext.Expect(o => o.ActionDescriptor.ControllerDescriptor.ControllerName).Returns("theControllerName");
            mockFilterContext.Expect(o => o.HttpContext.Items).Returns(new Hashtable());
            mockFilterContext.Expect(o => o.HttpContext.Response.ContentEncoding).Returns(Encoding.UTF8);
            mockFilterContext.Expect(o => o.ActionParameters).Returns(new Dictionary<string, object> {
                { "forty-two", 42 },
                { "eighty-four", 84 }
            });

            Mock<ITestableCache> mockCache = new Mock<ITestableCache>();
            mockCache
                .Expect(o => o.GetCacheItem(It.IsAny<string>()))
                .Callback((string cacheKey) => { expectedCacheKey = cacheKey; })
                .Returns((object)null);

            ChildActionCacheAttribute attr = new TestableChildActionCacheAttribute(mockCache.Object) {
                Duration = 10
            };

            // Act
            ActionExecutingContext filterContext = mockFilterContext.Object;
            attr.OnActionExecuting(filterContext);

            // Assert
            Assert.IsNull(filterContext.Result, "Result shouldn't have been set if cache miss.");
            Assert.AreEqual(expectedCacheKey, filterContext.HttpContext.Items.Values.Cast<object>().Single(), "Flag was not correctly set in HttpContext.Items.");
        }

        [TestMethod]
        public void OnActionExecuting_NegativeDuration_Throws() {
            // Arrange
            Mock<ActionExecutingContext> mockFilterContext = new Mock<ActionExecutingContext>();
            mockFilterContext.Expect(o => o.IsChildAction).Returns(true);

            ChildActionCacheAttribute attr = new ChildActionCacheAttribute() {
                Duration = -1
            };

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    attr.OnActionExecuting(mockFilterContext.Object);
                },
                @"The 'Duration' property must be a positive number.");
        }

        [TestMethod]
        public void OnActionExecuting_NotChildAction_DoesNothing() {
            // Arrange
            Mock<ActionExecutingContext> mockFilterContext = new Mock<ActionExecutingContext>();
            mockFilterContext.Expect(o => o.IsChildAction).Returns(false).Verifiable();

            ChildActionCacheAttribute attr = new ChildActionCacheAttribute() {
                Duration = 10
            };

            // Act
            attr.OnActionExecuting(mockFilterContext.Object);

            // Assert
            mockFilterContext.Verify();
        }

        [TestMethod]
        public void OnResultExecuted_ItemsFlagNotSet_DoesNothing() {
            // Arrange
            Mock<ResultExecutedContext> mockFilterContext = new Mock<ResultExecutedContext>();
            mockFilterContext.Expect(o => o.HttpContext.Items).Returns(new Hashtable()).Verifiable(); // empty hashtable has no keys set
            mockFilterContext.Expect(o => o.HttpContext.Response.Output).Never();

            // Act
            ChildActionCacheAttribute attr = new ChildActionCacheAttribute() {
                Duration = 10
            };

            // Act
            attr.OnResultExecuted(mockFilterContext.Object);

            // Assert
            mockFilterContext.Verify();
        }

        [TestMethod]
        public void OnResultExecuted_ItemsFlagSet_Exception_DumpsContent_NoAddToCache() {
            // Arrange
            TextWriter originalWriter = TextWriter.Null;

            ChildActionCacheAttribute.WrappedStringWriter wrappedWriter = new ChildActionCacheAttribute.WrappedStringWriter(originalWriter);
            wrappedWriter.Write("captured text");

            Mock<ResultExecutedContext> mockFilterContext = new Mock<ResultExecutedContext>();
            mockFilterContext.Expect(o => o.HttpContext.Items[It.IsAny<object>()]).Returns("theCacheKey");
            mockFilterContext.Expect(o => o.Exception).Returns(new Exception("Sample exception."));

            TextWriter currentResponseWriter = wrappedWriter;
            mockFilterContext.Expect(o => o.HttpContext.Response.Output).Returns(() => currentResponseWriter);
            mockFilterContext
                .ExpectSet(o => o.HttpContext.Response.Output)
                .Callback(w => { currentResponseWriter = w; });
            mockFilterContext
                .Expect(o => o.HttpContext.Response.Write("captured text"))
                .Callback(() => { Assert.AreEqual(originalWriter, currentResponseWriter, "Response.Write() called while wrong writer was active."); })
                .AtMostOnce()
                .Verifiable();

            Mock<ITestableCache> mockCache = new Mock<ITestableCache>();
            mockCache.Expect(o => o.SetCacheItem(It.IsAny<string>(), It.IsAny<string>())).Never();

            // Act
            ChildActionCacheAttribute attr = new TestableChildActionCacheAttribute(mockCache.Object) {
                Duration = 10
            };

            // Act
            attr.OnResultExecuted(mockFilterContext.Object);

            // Assert
            mockFilterContext.Verify();
            mockCache.Verify();
            Assert.AreEqual(originalWriter, currentResponseWriter);
            Assert.IsFalse(mockFilterContext.Object.ExceptionHandled, "Exception shouldn't have been marked as handled.");
        }

        [TestMethod]
        public void OnResultExecuted_ItemsFlagSet_NoException_DumpsContentAndAddsToCache() {
            // Arrange
            TextWriter originalWriter = TextWriter.Null;

            ChildActionCacheAttribute.WrappedStringWriter wrappedWriter = new ChildActionCacheAttribute.WrappedStringWriter(originalWriter);
            wrappedWriter.Write("captured text");

            Mock<ResultExecutedContext> mockFilterContext = new Mock<ResultExecutedContext>();
            mockFilterContext.Expect(o => o.HttpContext.Items[It.IsAny<object>()]).Returns("theCacheKey");
            mockFilterContext.Expect(o => o.Exception).Returns((Exception)null);

            TextWriter currentResponseWriter = wrappedWriter;
            mockFilterContext.Expect(o => o.HttpContext.Response.Output).Returns(() => currentResponseWriter);
            mockFilterContext
                .ExpectSet(o => o.HttpContext.Response.Output)
                .Callback(w => { currentResponseWriter = w; });
            mockFilterContext
                .Expect(o => o.HttpContext.Response.Write("captured text"))
                .Callback(() => { Assert.AreEqual(originalWriter, currentResponseWriter, "Response.Write() called while wrong writer was active."); })
                .AtMostOnce()
                .Verifiable();

            Mock<ITestableCache> mockCache = new Mock<ITestableCache>();
            mockCache.Expect(o => o.SetCacheItem("theCacheKey", "captured text")).Verifiable();

            // Act
            ChildActionCacheAttribute attr = new TestableChildActionCacheAttribute(mockCache.Object) {
                Duration = 10
            };

            // Act
            attr.OnResultExecuted(mockFilterContext.Object);

            // Assert
            mockFilterContext.Verify();
            mockCache.Verify();
            Assert.AreEqual(originalWriter, currentResponseWriter);
        }

        [TestMethod]
        public void OnResultExecuting_ItemsFlagNotSet_DoesNothing() {
            // Arrange
            Mock<ResultExecutingContext> mockFilterContext = new Mock<ResultExecutingContext>();
            mockFilterContext.Expect(o => o.HttpContext.Items).Returns(new Hashtable()).Verifiable(); // empty hashtable has no keys set
            mockFilterContext.Expect(o => o.HttpContext.Response.Output).Never();

            // Act
            ChildActionCacheAttribute attr = new ChildActionCacheAttribute() {
                Duration = 10
            };

            // Act
            attr.OnResultExecuting(mockFilterContext.Object);

            // Assert
            mockFilterContext.Verify();
        }

        [TestMethod]
        public void OnResultExecuting_ItemsFlagSet_ReplacesOutputWriter() {
            // Arrange
            TextWriter originalWriter = TextWriter.Null;
            TextWriter newWriter = null;

            Mock<ResultExecutingContext> mockFilterContext = new Mock<ResultExecutingContext>();
            mockFilterContext.Expect(o => o.HttpContext.Items[It.IsAny<object>()]).Returns("theCacheKey");
            mockFilterContext.Expect(o => o.HttpContext.Response.Output).Returns(originalWriter);
            mockFilterContext
                .ExpectSet(o => o.HttpContext.Response.Output)
                .Callback(w => { newWriter = w; });

            // Act
            ChildActionCacheAttribute attr = new ChildActionCacheAttribute() {
                Duration = 10
            };

            // Act
            attr.OnResultExecuting(mockFilterContext.Object);

            // Assert
            ChildActionCacheAttribute.WrappedStringWriter stringWriter = newWriter as ChildActionCacheAttribute.WrappedStringWriter;
            Assert.IsNotNull(stringWriter);
            Assert.AreEqual(originalWriter, stringWriter.OriginalWriter);
        }

        public interface ITestableCache {
            object GetCacheItem(string cacheKey);
            void SetCacheItem(string cacheKey, object value);
        }

        private class TestableChildActionCacheAttribute : ChildActionCacheAttribute {
            private readonly ITestableCache _testableCache;

            public TestableChildActionCacheAttribute(ITestableCache testableCache) {
                _testableCache = testableCache;
            }

            internal override object GetCacheItem(HttpContextBase httpContext, string cacheKey) {
                return _testableCache.GetCacheItem(cacheKey);
            }

            internal override void SetCacheItem(HttpContextBase httpContext, string cacheKey, object value) {
                _testableCache.SetCacheItem(cacheKey, value);
            }
        }

    }
}
