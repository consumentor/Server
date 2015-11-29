namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class CssExtensionsTests {
        [TestMethod]
        public void CssWithoutFileThrowsArgumentNullException() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(() => html.Css(null), "file");
        }

        [TestMethod]
        public void CssWithRootedPathRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("~/Correct/Path.css");

            // Assert
            Assert.AreEqual("<link href=\"/$(SESSION)/Correct/Path.css\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void CssWithRelativePathRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("../../Correct/Path.css");

            // Assert
            Assert.AreEqual("<link href=\"../../Correct/Path.css\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void CssWithRelativeCurrentPathRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("/Correct/Path.css");

            // Assert
            Assert.AreEqual("<link href=\"/Correct/Path.css\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void CssWithContentRelativePathRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("Correct/Path.css");

            // Assert
            Assert.AreEqual("<link href=\"/$(SESSION)/Content/Correct/Path.css\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void CssWithNullMediaTypeRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("Correct/Path.css", null);

            // Assert
            Assert.AreEqual("<link href=\"/$(SESSION)/Content/Correct/Path.css\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void CssWithEmptyMediaTypeRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("Correct/Path.css", String.Empty);

            // Assert
            Assert.AreEqual("<link href=\"/$(SESSION)/Content/Correct/Path.css\" media=\"\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void CssWithMediaTypeRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("Correct/Path.css", "Print");

            // Assert
            Assert.AreEqual("<link href=\"/$(SESSION)/Content/Correct/Path.css\" media=\"Print\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void CssWithUrlRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("http://ajax.Correct.com/Path.js");

            // Assert
            Assert.AreEqual("<link href=\"http://ajax.Correct.com/Path.js\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void CssWithSecureUrlRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Css("https://ajax.Correct.com/Path.js");

            // Assert
            Assert.AreEqual("<link href=\"https://ajax.Correct.com/Path.js\" rel=\"stylesheet\" type=\"text/css\" />", result.ToHtmlString());
        }

    }
}
