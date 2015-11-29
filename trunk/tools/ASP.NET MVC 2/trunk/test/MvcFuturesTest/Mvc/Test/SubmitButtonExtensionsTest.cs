namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class SubmitButtonExtensionsTest {
        [TestMethod]
        public void SubmitButtonRendersWithJustTypeAttribute() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitButton();
            Assert.AreEqual("<input type=\"submit\" />", button.ToHtmlString());
        }
        
        [TestMethod]
        public void SubmitButtonWithNameRendersButtonWithNameAttribute() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitButton("button-name");
            Assert.AreEqual("<input id=\"button-name\" name=\"button-name\" type=\"submit\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitButtonWithIdDifferentFromNameRendersButtonWithId() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitButton("button-name", "blah", new {id="foo" });
            Assert.AreEqual("<input id=\"foo\" name=\"button-name\" type=\"submit\" value=\"blah\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitButtonWithNameAndTextRendersAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitButton("button-name", "button-text");
            Assert.AreEqual("<input id=\"button-name\" name=\"button-name\" type=\"submit\" value=\"button-text\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitButtonWithNameAndValueRendersBothAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitButton("button-name", "button-value", new { id = "button-id" });
            Assert.AreEqual("<input id=\"button-id\" name=\"button-name\" type=\"submit\" value=\"button-value\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitButtonWithNameAndIdRendersBothAttributesCorrectly() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitButton("button-name", "button-value", new { id = "button-id" });
            Assert.AreEqual("<input id=\"button-id\" name=\"button-name\" type=\"submit\" value=\"button-value\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitButtonWithTypeAttributeRendersCorrectType() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitButton("specified-name", "button-value", new {type="not-submit"});
            Assert.AreEqual("<input id=\"specified-name\" name=\"specified-name\" type=\"not-submit\" value=\"button-value\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitButtonWithNameAndValueSpecifiedAndPassedInAsAttributeChoosesSpecified() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitButton("specified-name", "button-value"
                , new RouteValueDictionary(new { name = "name-attribute-value", value="value-attribute" }));
            Assert.AreEqual("<input id=\"specified-name\" name=\"name-attribute-value\" type=\"submit\" value=\"value-attribute\" />", button.ToHtmlString());
        }
    }
}
