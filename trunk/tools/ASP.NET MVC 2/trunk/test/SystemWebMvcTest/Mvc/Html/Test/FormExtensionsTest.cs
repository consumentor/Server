namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Collections;
    using System.IO;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class FormExtensionsTest {
        private static void BeginFormHelper(Func<HtmlHelper, MvcForm> beginForm, string expectedFormTag) {
            // Arrange
            StringWriter writer;
            HtmlHelper htmlHelper = GetFormHelper(out writer);

            // Act
            IDisposable formDisposable = beginForm(htmlHelper);
            formDisposable.Dispose();

            // Assert
            Assert.AreEqual(expectedFormTag + "</form>", writer.ToString());
        }

        [TestMethod]
        public void BeginFormParameterDictionaryMerging() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("bar", "foo", FormMethod.Get, new RouteValueDictionary(new { method = "post" })),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/foo/bar"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormSetsAndRestoresFormContext() {
            // Arrange
            StringWriter writer;
            HtmlHelper htmlHelper = GetFormHelper(out writer);

            FormContext originalContext = new FormContext();
            htmlHelper.ViewContext.FormContext = originalContext;

            // Act & assert - push
            MvcForm theForm = htmlHelper.BeginForm();
            Assert.IsNotNull(htmlHelper.ViewContext.FormContext);
            Assert.AreNotEqual(originalContext, htmlHelper.ViewContext.FormContext, "FormContext should have been set to a new instance.");

            // Act & assert - pop
            theForm.Dispose();
            Assert.AreEqual(originalContext, htmlHelper.ViewContext.FormContext, "FormContext was not properly restored.");
            Assert.AreEqual(@"<form action=""/some/path"" method=""post""></form>", writer.ToString());
        }

        [TestMethod]
        public void BeginFormWithClientValidationEnabled() {
            // Arrange
            StringWriter writer;
            HtmlHelper htmlHelper = GetFormHelper(out writer);

            FormContext originalContext = new FormContext();
            htmlHelper.ViewContext.ClientValidationEnabled = true;
            htmlHelper.ViewContext.FormContext = originalContext;

            // Act & assert - push
            MvcForm theForm = htmlHelper.BeginForm();
            Assert.IsNotNull(htmlHelper.ViewContext.FormContext);
            Assert.AreNotEqual(originalContext, htmlHelper.ViewContext.FormContext, "FormContext should have been set to a new instance.");
            Assert.AreEqual("form_id", htmlHelper.ViewContext.FormContext.FormId);

            // Act & assert - pop
            theForm.Dispose();
            Assert.AreEqual(originalContext, htmlHelper.ViewContext.FormContext, "FormContext was not properly restored.");
            Assert.AreEqual(@"<form action=""/some/path"" id=""form_id"" method=""post""></form><script type=""text/javascript"">
//<![CDATA[
if (!window.mvcClientValidationMetadata) { window.mvcClientValidationMetadata = []; }
window.mvcClientValidationMetadata.push({""Fields"":[],""FormId"":""form_id"",""ReplaceValidationSummary"":false});
//]]>
</script>", writer.ToString());
        }

        [TestMethod]
        public void BeginFormWithActionControllerInvalidFormMethodHtmlValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("bar", "foo", (FormMethod)2, new RouteValueDictionary(new { baz = "baz" })),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/foo/bar"" baz=""baz"" method=""post"">");
        }

        [TestMethod]
        public void BeginFormWithActionController() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("bar", "foo"),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/foo/bar"" method=""post"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerFormMethodHtmlDictionary() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("bar", "foo", FormMethod.Get, new RouteValueDictionary(new { baz = "baz" })),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/foo/bar"" baz=""baz"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerFormMethodHtmlValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("bar", "foo", FormMethod.Get, new { baz = "baz" }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/foo/bar"" baz=""baz"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerRouteDictionaryFormMethodHtmlDictionary() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("bar", "foo", new RouteValueDictionary(new { id = "id" }), FormMethod.Get, new RouteValueDictionary(new { baz = "baz" })),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/foo/bar/id"" baz=""baz"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerRouteValuesFormMethodHtmlValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("bar", "foo", new { id = "id" }, FormMethod.Get, new { baz = "baz" }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/foo/bar/id"" baz=""baz"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerNullRouteValuesFormMethodNullHtmlValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("bar", "foo", null, FormMethod.Get, null),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/foo/bar"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormWithRouteValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm(new { action = "someOtherAction", id = "id" }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/home/someOtherAction/id"" method=""post"">");
        }

        [TestMethod]
        public void BeginFormWithRouteDictionary() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm(new RouteValueDictionary { { "action", "someOtherAction" }, { "id", "id" } }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/home/someOtherAction/id"" method=""post"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerRouteValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("myAction", "myController", new { id = "id", pageNum = "123" }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/myController/myAction/id?pageNum=123"" method=""post"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerRouteDictionary() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("myAction", "myController", new RouteValueDictionary { { "pageNum", "123" }, { "id", "id" } }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/myController/myAction/id?pageNum=123"" method=""post"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerMethod() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("myAction", "myController", FormMethod.Get),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/myController/myAction"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerRouteValuesMethod() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("myAction", "myController", new { id = "id", pageNum = "123" }, FormMethod.Get),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/myController/myAction/id?pageNum=123"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormWithActionControllerRouteDictionaryMethod() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm("myAction", "myController", new RouteValueDictionary { { "pageNum", "123" }, { "id", "id" } }, FormMethod.Get),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/myController/myAction/id?pageNum=123"" method=""get"">");
        }

        [TestMethod]
        public void BeginFormWithNoParams() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginForm(),
                @"<form action=""/some/path"" method=""post"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteNameInvalidFormMethodHtmlValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", (FormMethod)2, new RouteValueDictionary(new { baz = "baz" })),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction"" baz=""baz"" method=""post"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteName() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute"),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction"" method=""post"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteNameFormMethodHtmlDictionary() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", FormMethod.Get, new RouteValueDictionary(new { baz = "baz" })),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction"" baz=""baz"" method=""get"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteNameFormMethodHtmlValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", FormMethod.Get, new { baz = "baz" }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction"" baz=""baz"" method=""get"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteNameRouteDictionaryFormMethodHtmlDictionary() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", new RouteValueDictionary(new { id = "id" }), FormMethod.Get, new RouteValueDictionary(new { baz = "baz" })),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction/id"" baz=""baz"" method=""get"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteNameRouteValuesFormMethodHtmlValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", new { id = "id" }, FormMethod.Get, new { baz = "baz" }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction/id"" baz=""baz"" method=""get"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteNameNullRouteValuesFormMethodNullHtmlValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", null, FormMethod.Get, null),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction"" method=""get"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm(new { action = "someOtherAction", id = "id" }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/home/someOtherAction/id"" method=""post"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteDictionary() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm(new RouteValueDictionary { { "action", "someOtherAction" }, { "id", "id" } }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/home/someOtherAction/id"" method=""post"">");
        }

        [TestMethod]
        public void BeginRouteFormWithRouteNameRouteValues() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", new { id = "id", pageNum = "123" }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction/id?pageNum=123"" method=""post"">");
        }

        [TestMethod]
        public void BeginRouteFormWithActionControllerRouteDictionary() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", new RouteValueDictionary { { "pageNum", "123" }, { "id", "id" } }),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction/id?pageNum=123"" method=""post"">");
        }

        [TestMethod]
        public void BeginRouteFormCanUseNamedRouteWithoutSpecifyingDefaults() {
            // DevDiv 217072: Non-mvc specific helpers should not give default values for controller and action

            BeginFormHelper(
                htmlHelper => {
                    htmlHelper.RouteCollection.MapRoute("MyRouteName", "any/url", new { controller = "Charlie" });
                    return htmlHelper.BeginRouteForm("MyRouteName");
                }, @"<form action=""" + MvcHelper.AppPathModifier + @"/any/url"" method=""post"">");
        }

        [TestMethod]
        public void BeginRouteFormWithActionControllerMethod() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", FormMethod.Get),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction"" method=""get"">");
        }

        [TestMethod]
        public void BeginRouteFormWithActionControllerRouteValuesMethod() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", new { id = "id", pageNum = "123" }, FormMethod.Get),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction/id?pageNum=123"" method=""get"">");
        }

        [TestMethod]
        public void BeginRouteFormWithActionControllerRouteDictionaryMethod() {
            BeginFormHelper(
                htmlHelper => htmlHelper.BeginRouteForm("namedroute", new RouteValueDictionary { { "pageNum", "123" }, { "id", "id" } }, FormMethod.Get),
                @"<form action=""" + MvcHelper.AppPathModifier + @"/named/home/oldaction/id?pageNum=123"" method=""get"">");
        }

        [TestMethod]
        public void EndFormWritesCloseTag() {
            // Arrange
            StringWriter writer;
            HtmlHelper htmlHelper = GetFormHelper(out writer);

            // Act
            htmlHelper.EndForm();

            // Assert
            Assert.AreEqual("</form>", writer.ToString());
        }

        private static HtmlHelper GetFormHelper(out StringWriter writer) {
            Mock<ViewContext> mockViewContext = new Mock<ViewContext>() { CallBase = true };
            mockViewContext.Expect(c => c.HttpContext.Request.Url).Returns(new Uri("http://www.contoso.com/some/path"));
            mockViewContext.Expect(c => c.HttpContext.Request.RawUrl).Returns("/some/path");
            mockViewContext.Expect(c => c.HttpContext.Request.ApplicationPath).Returns("/");
            mockViewContext.Expect(c => c.HttpContext.Request.Path).Returns("/");
            mockViewContext.Expect(c => c.HttpContext.Response.Write(It.IsAny<string>())).Never();
            mockViewContext.Expect(c => c.HttpContext.Items).Returns(new Hashtable());

            writer = new StringWriter();
            mockViewContext.Expect(c => c.Writer).Returns(writer);

            mockViewContext.Expect(c => c.HttpContext.Response.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(r => MvcHelper.AppPathModifier + r);

            RouteCollection rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            RouteData rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");

            mockViewContext.Expect(c => c.RouteData).Returns(rd);
            HtmlHelper helper = new HtmlHelper(mockViewContext.Object, new Mock<IViewDataContainer>().Object, rt);
            helper.ViewContext.FormIdGenerator = () => "form_id";
            return helper;
        }
    }
}
