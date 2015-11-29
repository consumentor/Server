namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class NameExtensionsTest {

        [TestMethod]
        public void NonStronglyTypedWithNoPrefix() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            Assert.AreEqual("", html.IdForModel().ToHtmlString());
            Assert.AreEqual("foo", html.Id("foo").ToHtmlString());
            Assert.AreEqual("foo_bar", html.Id("foo.bar").ToHtmlString());
            Assert.AreEqual("&lt;script>alert(&quot;XSS!&quot;)&lt;/script>", html.Id("<script>alert(\"XSS!\")</script>").ToHtmlString());

            Assert.AreEqual("", html.NameForModel().ToHtmlString());
            Assert.AreEqual("foo", html.Name("foo").ToHtmlString());
            Assert.AreEqual("foo.bar", html.Name("foo.bar").ToHtmlString());
            Assert.AreEqual("&lt;script>alert(&quot;XSS!&quot;)&lt;/script>", html.Name("<script>alert(\"XSS!\")</script>").ToHtmlString());
        }

        [TestMethod]
        public void NonStronglyTypedWithPrefix() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            html.ViewData.TemplateInfo.HtmlFieldPrefix = "prefix";

            // Act & Assert
            Assert.AreEqual("prefix", html.IdForModel().ToHtmlString());
            Assert.AreEqual("prefix_foo", html.Id("foo").ToHtmlString());
            Assert.AreEqual("prefix_foo_bar", html.Id("foo.bar").ToHtmlString());

            Assert.AreEqual("prefix", html.NameForModel().ToHtmlString());
            Assert.AreEqual("prefix.foo", html.Name("foo").ToHtmlString());
            Assert.AreEqual("prefix.foo.bar", html.Name("foo.bar").ToHtmlString());
        }

        [TestMethod]
        public void StronglyTypedWithNoPrefix() {
            // Arrange
            HtmlHelper<OuterClass> html = MvcHelper.GetHtmlHelper(new ViewDataDictionary<OuterClass>());

            // Act & Assert
            Assert.AreEqual("IntValue", html.IdFor(m => m.IntValue).ToHtmlString());
            Assert.AreEqual("Inner_StringValue", html.IdFor(m => m.Inner.StringValue).ToHtmlString());

            Assert.AreEqual("IntValue", html.NameFor(m => m.IntValue).ToHtmlString());
            Assert.AreEqual("Inner.StringValue", html.NameFor(m => m.Inner.StringValue).ToHtmlString());
        }

        [TestMethod]
        public void StronglyTypedWithPrefix() {
            // Arrange
            HtmlHelper<OuterClass> html = MvcHelper.GetHtmlHelper(new ViewDataDictionary<OuterClass>());
            html.ViewData.TemplateInfo.HtmlFieldPrefix = "prefix";

            // Act & Assert
            Assert.AreEqual("prefix_IntValue", html.IdFor(m => m.IntValue).ToHtmlString());
            Assert.AreEqual("prefix_Inner_StringValue", html.IdFor(m => m.Inner.StringValue).ToHtmlString());

            Assert.AreEqual("prefix.IntValue", html.NameFor(m => m.IntValue).ToHtmlString());
            Assert.AreEqual("prefix.Inner.StringValue", html.NameFor(m => m.Inner.StringValue).ToHtmlString());
        }

        class OuterClass {
            public InnerClass Inner { get; set; }
            public int IntValue { get; set; }
        }

        class InnerClass {
            public string StringValue { get; set; }
        }

    }
}
