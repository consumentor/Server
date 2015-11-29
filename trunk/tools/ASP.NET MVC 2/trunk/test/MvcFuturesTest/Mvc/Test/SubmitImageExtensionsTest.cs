namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class SubmitImageExtensionsTest {
        [TestMethod]
        public void SubmitImageWithEmptyImageSrcThrowsArgumentNullException() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            ExceptionHelper.ExpectArgumentNullException(() => html.SubmitImage("name", null), "imageSrc");
        }

        [TestMethod]
        public void SubmitImageWithTypeAttributeRendersExplicitTypeAttribute() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitImage("specified-name", "/mvc.jpg", new { type = "not-image" });
            Assert.AreEqual("<input id=\"specified-name\" name=\"specified-name\" src=\"/mvc.jpg\" type=\"not-image\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitImageWithNameAndImageUrlRendersNameAndSrcAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitImage("button-name", "/mvc.gif");
            Assert.AreEqual("<input id=\"button-name\" name=\"button-name\" src=\"/mvc.gif\" type=\"image\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitImageWithImageUrlStartingWithTildeRendersAppPath() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary(), "/app");
            MvcHtmlString button = html.SubmitImage("button-name", "~/mvc.gif");
            Assert.AreEqual("<input id=\"button-name\" name=\"button-name\" src=\"/$(SESSION)/app/mvc.gif\" type=\"image\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitImageWithNameAndIdRendersBothAttributesCorrectly() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitImage("button-name", "/mvc.png", new { id = "button-id" });
            Assert.AreEqual("<input id=\"button-id\" name=\"button-name\" src=\"/mvc.png\" type=\"image\" />", button.ToHtmlString());
        }

        [TestMethod]
        public void SubmitButtonWithNameAndValueSpecifiedAndPassedInAsAttributeChoosesExplicitAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MvcHtmlString button = html.SubmitImage("specified-name", "/specified-src.bmp"
                , new RouteValueDictionary(new { name = "name-attribute", src = "src-attribute" }));
            Assert.AreEqual("<input id=\"specified-name\" name=\"name-attribute\" src=\"src-attribute\" type=\"image\" />", button.ToHtmlString());
        }
    }
}
