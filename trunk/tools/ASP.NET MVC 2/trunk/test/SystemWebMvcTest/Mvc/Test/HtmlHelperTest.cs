namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class HtmlHelperTest {
        public static readonly RouteValueDictionary AttributesDictionary = new RouteValueDictionary(new { baz = "BazValue" });
        public static readonly object AttributesObjectDictionary = new { baz = "BazObjValue" };

        // AntiForgeryToken test helpers
        private static string _antiForgeryTokenCookieName = AntiForgeryData.GetAntiForgeryTokenName("/SomeAppPath");
        private const string _serializedValuePrefix = @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""Creation: ";
        private const string _someValueSuffix = @", Value: some value, Salt: some other salt, Username: username"" />";
        private readonly Regex _randomFormValueSuffixRegex = new Regex(@", Value: (?<value>[A-Za-z0-9/\+=]{24}), Salt: some other salt, Username: username"" />$");
        private readonly Regex _randomCookieValueSuffixRegex = new Regex(@", Value: (?<value>[A-Za-z0-9/\+=]{24}), Salt: ");

        [TestMethod]
        public void SerializerProperty() {
            // Arrange
            HtmlHelper helper = GetHtmlHelperForAntiForgeryToken(null);
            AntiForgeryDataSerializer newSerializer = new AntiForgeryDataSerializer();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(helper, "Serializer", newSerializer);
        }

        [TestMethod]
        public void ViewContextProperty() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            HtmlHelper htmlHelper = new HtmlHelper(viewContext, new Mock<IViewDataContainer>().Object);

            // Act
            ViewContext value = htmlHelper.ViewContext;

            // Assert
            Assert.AreEqual(viewContext, value);
        }

        [TestMethod]
        public void ViewDataContainerProperty() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            IViewDataContainer container = new Mock<IViewDataContainer>().Object;
            HtmlHelper htmlHelper = new HtmlHelper(viewContext, container);

            // Act
            IViewDataContainer value = htmlHelper.ViewDataContainer;

            // Assert
            Assert.AreEqual(container, value);
        }

        [TestMethod]
        public void ConstructorWithNullRouteCollectionThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new HtmlHelper(new Mock<ViewContext>().Object, MvcHelper.GetViewDataContainer(null), null);
                },
                "routeCollection");
        }

        [TestMethod]
        public void ConstructorWithNullViewContextThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new HtmlHelper(null, null);
                },
                "viewContext");
        }

        [TestMethod]
        public void ConstructorWithNullViewDataContainerThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new HtmlHelper(new Mock<ViewContext>().Object, null);
                },
                "viewDataContainer");
        }

        [TestMethod]
        public void AntiForgeryTokenSetsCookieValueIfDoesNotExist() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelperForAntiForgeryToken(null);

            // Act
            string formValue = htmlHelper.AntiForgeryToken("some other salt").ToHtmlString();

            // Assert
            Assert.IsTrue(formValue.StartsWith(_serializedValuePrefix), "Form value prefix did not match.");

            Match formMatch = _randomFormValueSuffixRegex.Match(formValue);
            string formTokenValue = formMatch.Groups["value"].Value;

            HttpCookie cookie = htmlHelper.ViewContext.HttpContext.Response.Cookies[_antiForgeryTokenCookieName];
            Assert.IsNotNull(cookie, "Cookie was not set correctly.");
            Assert.IsTrue(cookie.HttpOnly, "Cookie should have HTTP-only flag set.");
            Assert.IsTrue(String.IsNullOrEmpty(cookie.Domain), "Domain should not have been set.");
            Assert.AreEqual("/", cookie.Path, "Path should have remained at '/' by default.");

            Match cookieMatch = _randomCookieValueSuffixRegex.Match(cookie.Value);
            string cookieTokenValue = cookieMatch.Groups["value"].Value;

            Assert.AreEqual(formTokenValue, cookieTokenValue, "Form and cookie token values did not match.");
        }

        [TestMethod]
        public void AntiForgeryTokenSetsDomainAndPathIfSpecified() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelperForAntiForgeryToken(null);

            // Act
            string formValue = htmlHelper.AntiForgeryToken("some other salt", "theDomain", "thePath").ToHtmlString();

            // Assert
            Assert.IsTrue(formValue.StartsWith(_serializedValuePrefix), "Form value prefix did not match.");

            Match formMatch = _randomFormValueSuffixRegex.Match(formValue);
            string formTokenValue = formMatch.Groups["value"].Value;

            HttpCookie cookie = htmlHelper.ViewContext.HttpContext.Response.Cookies[_antiForgeryTokenCookieName];
            Assert.IsNotNull(cookie, "Cookie was not set correctly.");
            Assert.IsTrue(cookie.HttpOnly, "Cookie should have HTTP-only flag set.");
            Assert.AreEqual("theDomain", cookie.Domain);
            Assert.AreEqual("thePath", cookie.Path);

            Match cookieMatch = _randomCookieValueSuffixRegex.Match(cookie.Value);
            string cookieTokenValue = cookieMatch.Groups["value"].Value;

            Assert.AreEqual(formTokenValue, cookieTokenValue, "Form and cookie token values did not match.");
        }

        [TestMethod]
        public void AntiForgeryTokenUsesCookieValueIfExists() {
            // Arrange
            HtmlHelper htmlHelper = GetHtmlHelperForAntiForgeryToken("2001-01-01:some value:some salt:username");

            // Act
            string formValue = htmlHelper.AntiForgeryToken("some other salt").ToHtmlString();

            // Assert
            Assert.IsTrue(formValue.StartsWith(_serializedValuePrefix), "Form value prefix did not match.");
            Assert.IsTrue(formValue.EndsWith(_someValueSuffix), "Form value suffix did not match.");
            Assert.AreEqual(0, htmlHelper.ViewContext.HttpContext.Response.Cookies.Count, "Cookie should not have been added to response.");
        }

        [TestMethod]
        public void AttributeEncodeObject() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.AttributeEncode((object)@"<"">");

            // Assert
            Assert.AreEqual(encodedHtml, "&lt;&quot;>", "Text is not being properly HTML attribute-encoded.");
        }

        [TestMethod]
        public void AttributeEncodeObjectNull() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.AttributeEncode((object)null);

            // Assert
            Assert.AreEqual("", encodedHtml);
        }

        [TestMethod]
        public void AttributeEncodeString() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.AttributeEncode(@"<"">");

            // Assert
            Assert.AreEqual(encodedHtml, "&lt;&quot;>", "Text is not being properly HTML attribute-encoded.");
        }

        [TestMethod]
        public void AttributeEncodeStringNull() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.AttributeEncode((string)null);

            // Assert
            Assert.AreEqual("", encodedHtml);
        }

        [TestMethod]
        public void EnableClientValidation() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act & assert
            Assert.IsFalse(htmlHelper.ViewContext.ClientValidationEnabled, "Client validation should not be enabled by default.");
            htmlHelper.EnableClientValidation();
            Assert.IsTrue(htmlHelper.ViewContext.ClientValidationEnabled, "EnableClientValidation() should have set the client validation flag.");
        }

        [TestMethod]
        public void EncodeObject() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.Encode((object)"<br />");

            // Assert
            Assert.AreEqual(encodedHtml, "&lt;br /&gt;", "Text is not being properly HTML-encoded.");
        }

        [TestMethod]
        public void EncodeObjectNull() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.Encode((object)null);

            // Assert
            Assert.AreEqual("", encodedHtml);
        }

        [TestMethod]
        public void EncodeString() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.Encode("<br />");

            // Assert
            Assert.AreEqual(encodedHtml, "&lt;br /&gt;", "Text is not being properly HTML-encoded.");
        }

        [TestMethod]
        public void EncodeStringNull() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            string encodedHtml = htmlHelper.Encode((string)null);

            // Assert
            Assert.AreEqual("", encodedHtml);
        }

        [TestMethod]
        public void GenericHelperConstructorSetsProperties1() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            ViewDataDictionary<Controller> vdd = new ViewDataDictionary<Controller>(new Mock<Controller>().Object);
            Mock<IViewDataContainer> vdc = new Mock<IViewDataContainer>();
            vdc.Expect(v => v.ViewData).Returns(vdd);

            // Act
            HtmlHelper<Controller> htmlHelper = new HtmlHelper<Controller>(viewContext, vdc.Object);

            // Assert
            Assert.AreEqual(viewContext, htmlHelper.ViewContext);
            Assert.AreEqual(vdc.Object, htmlHelper.ViewDataContainer);
            Assert.AreEqual(RouteTable.Routes, htmlHelper.RouteCollection);
            Assert.AreEqual(vdd.Model, htmlHelper.ViewData.Model);
        }

        [TestMethod]
        public void GenericHelperConstructorSetsProperties2() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            ViewDataDictionary<Controller> vdd = new ViewDataDictionary<Controller>(new Mock<Controller>().Object);
            Mock<IViewDataContainer> vdc = new Mock<IViewDataContainer>();
            vdc.Expect(v => v.ViewData).Returns(vdd);
            RouteCollection rc = new RouteCollection();

            // Act
            HtmlHelper<Controller> htmlHelper = new HtmlHelper<Controller>(viewContext, vdc.Object, rc);

            // Assert
            Assert.AreEqual(viewContext, htmlHelper.ViewContext);
            Assert.AreEqual(vdc.Object, htmlHelper.ViewDataContainer);
            Assert.AreEqual(rc, htmlHelper.RouteCollection);
            Assert.AreEqual(vdd.Model, htmlHelper.ViewData.Model);
        }

        [TestMethod]
        public void GetModelStateValueReturnsNullIfModelStateHasNoValue() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.ModelState.AddModelError("foo", "some error text"); // didn't call SetModelValue()

            HtmlHelper helper = new HtmlHelper(new ViewContext(), new SimpleViewDataContainer(vdd));

            // Act
            object retVal = helper.GetModelStateValue("foo", typeof(object));

            // Assert
            Assert.IsNull(retVal);
        }

        [TestMethod]
        public void GetModelStateValueReturnsNullIfModelStateKeyNotPresent() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            HtmlHelper helper = new HtmlHelper(new ViewContext(), new SimpleViewDataContainer(vdd));

            // Act
            object retVal = helper.GetModelStateValue("key_not_present", typeof(object));

            // Assert
            Assert.IsNull(retVal);
        }

        [TestMethod]
        public void GenerateIdFromNameTests() {
            // Guard clauses
            ExceptionHelper.ExpectArgumentNullException(
                () => HtmlHelper.GenerateIdFromName(null),
                "name");
            ExceptionHelper.ExpectArgumentNullException(
                () => HtmlHelper.GenerateIdFromName(null, "?"),
                "name");
            ExceptionHelper.ExpectArgumentNullException(
                () => HtmlHelper.GenerateIdFromName("?", null),
                "idAttributeDotReplacement");

            // Default replacement tests
            Assert.AreEqual("", HtmlHelper.GenerateIdFromName(""));
            Assert.AreEqual("Foo", HtmlHelper.GenerateIdFromName("Foo"));
            Assert.AreEqual("Foo_Bar", HtmlHelper.GenerateIdFromName("Foo.Bar"));
            Assert.AreEqual("Foo_Bar_Baz", HtmlHelper.GenerateIdFromName("Foo.Bar.Baz"));

            // Custom replacement tests
            Assert.AreEqual("", HtmlHelper.GenerateIdFromName("", "?"));
            Assert.AreEqual("Foo", HtmlHelper.GenerateIdFromName("Foo", "?"));
            Assert.AreEqual("Foo?Bar", HtmlHelper.GenerateIdFromName("Foo.Bar", "?"));
            Assert.AreEqual("Foo?Bar?Baz", HtmlHelper.GenerateIdFromName("Foo.Bar.Baz", "?"));
            Assert.AreEqual("FooBarBaz", HtmlHelper.GenerateIdFromName("Foo.Bar.Baz", ""));
        }

        // RenderPartialInternal tests

        [TestMethod]
        public void NullPartialViewNameThrows() {
            // Arrange
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => helper.RenderPartialInternal(null /* partialViewName */, null /* viewData */, null /* model */, TextWriter.Null),
                "partialViewName");
        }

        [TestMethod]
        public void EmptyPartialViewNameThrows() {
            // Arrange
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => helper.RenderPartialInternal(String.Empty /* partialViewName */, null /* viewData */, null /* model */, TextWriter.Null),
                "partialViewName");
        }

        [TestMethod]
        public void EngineLookupSuccessCallsRender() {
            // Arrange
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            TextWriter writer = helper.ViewContext.Writer;
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), "partial-view", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), writer))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, _) => {
                        Assert.AreSame(helper.ViewContext.View, viewContext.View);
                        Assert.AreSame(helper.ViewContext.TempData, viewContext.TempData);
                    })
                .Verifiable();

            // Act
            helper.RenderPartialInternal("partial-view", null /* viewData */, null /* model */, writer, engine.Object);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void EngineLookupFailureThrows() {
            // Arrange
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), "partial-view", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(new[] { "location1", "location2" }))
                .Verifiable();

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => helper.RenderPartialInternal("partial-view", null /* viewData */, null /* model */, TextWriter.Null, engine.Object),
                @"The partial view 'partial-view' was not found. The following locations were searched:
location1
location2");

            engine.Verify();
        }

        [TestMethod]
        public void RenderPartialInternalWithNullModelAndNullViewData() {
            // Arrange
            object model = new object();
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            helper.ViewData["Foo"] = "Bar";
            helper.ViewData.Model = model;
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), "partial-view", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), TextWriter.Null))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreNotSame(helper.ViewData, viewContext.ViewData);  // New view data instance
                        Assert.AreEqual("Bar", viewContext.ViewData["Foo"]);       // Copy of the existing view data
                        Assert.AreSame(model, viewContext.ViewData.Model);         // Keep existing model
                    })
                .Verifiable();

            // Act
            helper.RenderPartialInternal("partial-view", null /* viewData */, null /* model */, TextWriter.Null, engine.Object);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void RenderPartialInternalWithNonNullModelAndNullViewData() {
            // Arrange
            object model = new object();
            object newModel = new object();
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            helper.ViewData["Foo"] = "Bar";
            helper.ViewData.Model = model;
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), "partial-view", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), TextWriter.Null))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreNotSame(helper.ViewData, viewContext.ViewData);  // New view data instance
                        Assert.AreEqual(0, viewContext.ViewData.Count);            // Empty (not copied)
                        Assert.AreSame(newModel, viewContext.ViewData.Model);      // New model
                    })
                .Verifiable();

            // Act
            helper.RenderPartialInternal("partial-view", null /* viewData */, newModel, TextWriter.Null, engine.Object);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void RenderPartialInternalWithNullModelAndNonNullViewData() {
            // Arrange
            object model = new object();
            object vddModel = new object();
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd["Baz"] = "Biff";
            vdd.Model = vddModel;
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            helper.ViewData["Foo"] = "Bar";
            helper.ViewData.Model = model;
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), "partial-view", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), TextWriter.Null))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreNotSame(helper.ViewData, viewContext.ViewData);  // New view data instance
                        Assert.AreEqual(1, viewContext.ViewData.Count);            // Copy of the passed view data, not original view data
                        Assert.AreEqual("Biff", viewContext.ViewData["Baz"]);
                        Assert.AreSame(vddModel, viewContext.ViewData.Model);      // Keep model from passed view data, not original view data
                    })
                .Verifiable();

            // Act
            helper.RenderPartialInternal("partial-view", vdd, null /* model */, TextWriter.Null, engine.Object);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void RenderPartialInternalWithNonNullModelAndNonNullViewData() {
            // Arrange
            object model = new object();
            object vddModel = new object();
            object newModel = new object();
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd["Baz"] = "Biff";
            vdd.Model = vddModel;
            TestableHtmlHelper helper = TestableHtmlHelper.Create();
            helper.ViewData["Foo"] = "Bar";
            helper.ViewData.Model = model;
            Mock<IViewEngine> engine = new Mock<IViewEngine>(MockBehavior.Strict);
            Mock<IView> view = new Mock<IView>(MockBehavior.Strict);
            engine
                .Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), "partial-view", It.IsAny<bool>()))
                .Returns(new ViewEngineResult(view.Object, engine.Object))
                .Verifiable();
            view
                .Expect(v => v.Render(It.IsAny<ViewContext>(), TextWriter.Null))
                .Callback<ViewContext, TextWriter>(
                    (viewContext, writer) => {
                        Assert.AreNotSame(helper.ViewData, viewContext.ViewData);  // New view data instance
                        Assert.AreEqual(1, viewContext.ViewData.Count);            // Copy of the passed view data, not original view data
                        Assert.AreEqual("Biff", viewContext.ViewData["Baz"]);
                        Assert.AreSame(newModel, viewContext.ViewData.Model);      // New model
                    })
                .Verifiable();

            // Act
            helper.RenderPartialInternal("partial-view", vdd, newModel, TextWriter.Null, engine.Object);

            // Assert
            engine.Verify();
            view.Verify();
        }

        [TestMethod]
        public void HttpMethodOverrideWithNullThrowsException() {
            // Arrange
            HtmlHelper htmlHelper = new HtmlHelper(new Mock<ViewContext>().Object, MvcHelper.GetViewDataContainer(null), new RouteCollection()); ;

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    htmlHelper.HttpMethodOverride(null);
                },
                "httpMethod");
        }

        [TestMethod]
        public void HttpMethodOverrideWithMethodRendersHiddenField() {
            // Arrange
            HtmlHelper htmlHelper = new HtmlHelper(new Mock<ViewContext>().Object, MvcHelper.GetViewDataContainer(null), new RouteCollection()); ;

            // Act
            MvcHtmlString hiddenField = htmlHelper.HttpMethodOverride("PUT");

            // Assert
            Assert.AreEqual<string>(@"<input name=""X-HTTP-Method-Override"" type=""hidden"" value=""PUT"" />", hiddenField.ToHtmlString());
        }

        [TestMethod]
        public void HttpMethodOverrideWithVerbRendersHiddenField() {
            // Arrange
            HtmlHelper htmlHelper = new HtmlHelper(new Mock<ViewContext>().Object, MvcHelper.GetViewDataContainer(null), new RouteCollection()); ;

            // Act
            MvcHtmlString hiddenField = htmlHelper.HttpMethodOverride(HttpVerbs.Delete);

            // Assert
            Assert.AreEqual<string>(@"<input name=""X-HTTP-Method-Override"" type=""hidden"" value=""DELETE"" />", hiddenField.ToHtmlString());
        }

        [TestMethod]
        public void HttpMethodOverrideWithInvalidVerbThrows() {
            TestHttpMethodOverrideVerbException(h => h.HttpMethodOverride((HttpVerbs)(-1)));
        }

        [TestMethod]
        public void HttpMethodOverrideWithGetVerbThrows() {
            TestHttpMethodOverrideVerbException(h => h.HttpMethodOverride(HttpVerbs.Get));
        }

        [TestMethod]
        public void HttpMethodOverrideWithPostVerbThrows() {
            TestHttpMethodOverrideVerbException(h => h.HttpMethodOverride(HttpVerbs.Post));
        }

        [TestMethod]
        public void HttpMethodOverrideWithGetMethodThrows() {
            TestHttpMethodOverrideMethodException(h => h.HttpMethodOverride("GeT"));
        }

        [TestMethod]
        public void HttpMethodOverrideWithPostMethodThrows() {
            TestHttpMethodOverrideMethodException(h => h.HttpMethodOverride("poST"));
        }

        private static void TestHttpMethodOverrideVerbException(Action<HtmlHelper> method) {
            // Arrange
            HtmlHelper htmlHelper = new HtmlHelper(new Mock<ViewContext>().Object, MvcHelper.GetViewDataContainer(null), new RouteCollection()); ;

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    method(htmlHelper);
                },
                @"The specified HttpVerbs value is not supported. The supported values are Delete, Head, and Put.
Parameter name: httpVerb");
        }

        private static void TestHttpMethodOverrideMethodException(Action<HtmlHelper> method) {
            // Arrange
            HtmlHelper htmlHelper = new HtmlHelper(new Mock<ViewContext>().Object, MvcHelper.GetViewDataContainer(null), new RouteCollection()); ;

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    method(htmlHelper);
                },
                @"The GET and POST HTTP methods are not supported.
Parameter name: httpMethod");
        }

        private static HtmlHelper GetHtmlHelperForAntiForgeryToken(string cookieValue) {
            HttpCookieCollection requestCookies = new HttpCookieCollection();
            HttpCookieCollection responseCookies = new HttpCookieCollection();
            if (!String.IsNullOrEmpty(cookieValue)) {
                requestCookies.Set(new HttpCookie(AntiForgeryData.GetAntiForgeryTokenName("/SomeAppPath"), cookieValue));
            }

            Mock<ViewContext> mockViewContext = new Mock<ViewContext>();
            mockViewContext.Expect(c => c.HttpContext.Request.Cookies).Returns(requestCookies);
            mockViewContext.Expect(c => c.HttpContext.Request.ApplicationPath).Returns("/SomeAppPath");
            mockViewContext.Expect(c => c.HttpContext.Response.Cookies).Returns(responseCookies);
            mockViewContext.Expect(c => c.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            mockViewContext.Expect(c => c.HttpContext.User.Identity.Name).Returns("username");

            return new HtmlHelper(mockViewContext.Object, new Mock<IViewDataContainer>().Object) {
                Serializer = new SubclassedAntiForgeryTokenSerializer()
            };
        }

        internal static ValueProviderResult GetValueProviderResult(object rawValue, string attemptedValue) {
            return new ValueProviderResult(rawValue, attemptedValue, CultureInfo.InvariantCulture);
        }

        public static IDisposable ReplaceCulture(string currentCulture, string currentUICulture) {
            CultureInfo newCulture = CultureInfo.GetCultureInfo(currentCulture);
            CultureInfo newUICulture = CultureInfo.GetCultureInfo(currentUICulture);
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo originalUICulture = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentCulture = newCulture;
            Thread.CurrentThread.CurrentUICulture = newUICulture;
            return new CultureReplacement { OriginalCulture = originalCulture, OriginalUICulture = originalUICulture };
        }

        private class CultureReplacement : IDisposable {
            public CultureInfo OriginalCulture;
            public CultureInfo OriginalUICulture;
            public void Dispose() {
                Thread.CurrentThread.CurrentCulture = OriginalCulture;
                Thread.CurrentThread.CurrentUICulture = OriginalUICulture;
            }
        }

        internal class SubclassedAntiForgeryTokenSerializer : AntiForgeryDataSerializer {
            public override string Serialize(AntiForgeryData token) {
                return String.Format(CultureInfo.InvariantCulture, "Creation: {0}, Value: {1}, Salt: {2}, Username: {3}",
                        token.CreationDate, token.Value, token.Salt, token.Username);
            }
            public override AntiForgeryData Deserialize(string serializedToken) {
                string[] parts = serializedToken.Split(':');
                return new AntiForgeryData() {
                    CreationDate = DateTime.Parse(parts[0], CultureInfo.InvariantCulture),
                    Value = parts[1],
                    Salt = parts[2],
                    Username = parts[3]
                };
            }
        }

        private class TestableHtmlHelper : HtmlHelper {
            TestableHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
                : base(viewContext, viewDataContainer) { }

            public static TestableHtmlHelper Create() {
                ViewDataDictionary viewData = new ViewDataDictionary();

                Mock<ViewContext> mockViewContext = new Mock<ViewContext>() { DefaultValue = DefaultValue.Mock };
                mockViewContext.Expect(c => c.HttpContext.Response.Output).Throws(new Exception("Response.Output should never be called."));
                mockViewContext.Expect(c => c.ViewData).Returns(viewData);
                mockViewContext.Expect(c => c.Writer).Returns(new StringWriter());

                Mock<IViewDataContainer> container = new Mock<IViewDataContainer>();
                container.Expect(c => c.ViewData).Returns(viewData);

                return new TestableHtmlHelper(mockViewContext.Object, container.Object);
            }

            public void RenderPartialInternal(string partialViewName,
                                              ViewDataDictionary viewData,
                                              object model,
                                              TextWriter writer,
                                              params IViewEngine[] engines) {
                base.RenderPartialInternal(partialViewName, viewData, model, writer, new ViewEngineCollection(engines));
            }
        }
    }
}
