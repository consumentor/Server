namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class ImageExtensionsTest {
        [TestMethod]
        public void ImageWithEmptyRelativeUrlThrowsArgumentNullException() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(() => html.Image(null), "imageRelativeUrl");
        }

        [TestMethod]
        public void ImageStaticWithEmptyRelativeUrlThrowsArgumentNullException() {
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(() => ImageExtensions.Image((string)null, "alt", null), "imageUrl");
        }

        [TestMethod]
        public void ImageWithRelativeUrlRendersProperImageTag() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg");
            //NOTE: Although XHTML requires an alt tag, we don't construct one for you. Specify it yourself.
            Assert.AreEqual("<img src=\"/system/web/mvc.jpg\" />", imageResult.ToHtmlString());
        }

        [TestMethod]
        public void ImageWithAltValueRendersImageWithAltTag() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg", "this is an alt value");
            Assert.AreEqual("<img alt=\"this is an alt value\" src=\"/system/web/mvc.jpg\" title=\"this is an alt value\" />", imageResult.ToHtmlString());
        }

        [TestMethod]
        public void ImageWithAltValueInObjectDictionaryRendersImageWithAltAndTitleTag() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg", new { alt = "this is an alt value" });
            Assert.AreEqual("<img alt=\"this is an alt value\" src=\"/system/web/mvc.jpg\" title=\"this is an alt value\" />", imageResult.ToHtmlString());
        }

        [TestMethod]
        public void ImageWithAltValueHtmlAttributeEncodesAltTag() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg", @"<"">");
            Assert.AreEqual("<img alt=\"&lt;&quot;>\" src=\"/system/web/mvc.jpg\" title=\"&lt;&quot;>\" />", imageResult.ToHtmlString());
        }

        [TestMethod]
        public void ImageWithAltValueInObjectDictionaryHtmlAttributeEncodesAltTag() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg", new { alt = "this is an alt value" });
            Assert.AreEqual("<img alt=\"this is an alt value\" src=\"/system/web/mvc.jpg\" title=\"this is an alt value\" />", imageResult.ToHtmlString());
        }

        // TODO: Verify this behavior with others.
        [TestMethod]
        public void ImageWithAltSpecifiedAndInDictionaryRendersExplicit() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg", "specified-alt", new { alt = "object-dictionary-alt" });
            Assert.AreEqual("<img alt=\"object-dictionary-alt\" src=\"/system/web/mvc.jpg\" title=\"object-dictionary-alt\" />", imageResult.ToHtmlString());
        }

        [TestMethod]
        public void ImageWithSrcSpecifiedAndInDictionaryRendersExplicit() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg", new { src = "explicit.jpg" });
            Assert.AreEqual("<img src=\"explicit.jpg\" />", imageResult.ToHtmlString());
        }


        [TestMethod]
        public void ImageWithOtherAttributesRendersThoseAttributesCaseSensitively() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg", new { width = 100, Height = 200 });
            Assert.AreEqual("<img Height=\"200\" src=\"/system/web/mvc.jpg\" width=\"100\" />", imageResult.ToHtmlString());
        }

        [TestMethod]
        public void ImageWithUrlAndDictionaryRendersAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            var attributes = new RouteValueDictionary(new { width = 125 });
            MvcHtmlString imageResult = html.Image("/system/web/mvc.jpg", attributes);
            Assert.AreEqual("<img src=\"/system/web/mvc.jpg\" width=\"125\" />", imageResult.ToHtmlString());
        }

        [TestMethod]
        public void ImageWithTildePathAndAppPathResolvesCorrectly() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary(), "/app");
            MvcHtmlString imageResult = html.Image("~/system/web/mvc.jpg");
            Assert.AreEqual("<img src=\"/$(SESSION)/app/system/web/mvc.jpg\" />", imageResult.ToHtmlString());
        }

        [TestMethod]
        public void ImageWithTildePathWithoutAppPathResolvesCorrectly() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary(), "/");
            MvcHtmlString imageResult = html.Image("~/system/web/mvc.jpg");
            Assert.AreEqual("<img src=\"/$(SESSION)/system/web/mvc.jpg\" />", imageResult.ToHtmlString());
        }
    }
}
