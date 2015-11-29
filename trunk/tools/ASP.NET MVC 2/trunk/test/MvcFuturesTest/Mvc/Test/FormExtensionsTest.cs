namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class FormExtensionsTest {
        internal const string AppPathModifier = "/$(SESSION)";

        [TestMethod]
        public void FormWithPostAction() {
            // Arrange
            StringWriter writer;
            HtmlHelper htmlHelper = GetFormHelper(out writer);

            // Act
            IDisposable formDisposable = htmlHelper.BeginForm<FormController>(action => action.About());
            formDisposable.Dispose();

            // Assert
            Assert.AreEqual(@"<form action=""" + AppPathModifier + @"/Form/About"" method=""post""></form>", writer.ToString());
        }

        [TestMethod]
        public void FormWithPostActionAndObjectAttributes() {
            // Arrange
            StringWriter writer;
            HtmlHelper htmlHelper = GetFormHelper(out writer);

            // Act
            IDisposable formDisposable = htmlHelper.BeginForm<FormController>(action => action.About(), FormMethod.Get, new { baz = "baz" });
            formDisposable.Dispose();

            // Assert
            Assert.AreEqual(@"<form action=""" + AppPathModifier + @"/Form/About"" baz=""baz"" method=""get""></form>", writer.ToString());
        }

        public class FormController : Controller {
            public ActionResult About() {
                return RedirectToAction("foo");
            }
        }

        private static HtmlHelper GetFormHelper(out StringWriter writer) {
            Mock<HttpRequestBase> mockHttpRequest = new Mock<HttpRequestBase>();
            mockHttpRequest.Expect(r => r.Url).Returns(new Uri("http://www.contoso.com/some/path"));
            Mock<HttpResponseBase> mockHttpResponse = new Mock<HttpResponseBase>(MockBehavior.Strict);

            mockHttpResponse.Expect(r => r.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(r => AppPathModifier + r);
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(c => c.Request).Returns(mockHttpRequest.Object);
            mockHttpContext.Expect(c => c.Response).Returns(mockHttpResponse.Object);
            RouteCollection rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            RouteData rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");

            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.HttpContext).Returns(mockHttpContext.Object);
            mockViewContext.Expect(c => c.RouteData).Returns(rd);
            writer = new StringWriter();
            mockViewContext.Expect(c => c.Writer).Returns(writer);

            HtmlHelper helper = new HtmlHelper(
                mockViewContext.Object,
                new Mock<IViewDataContainer>().Object,
                rt);
            return helper;
        }
    }
}
