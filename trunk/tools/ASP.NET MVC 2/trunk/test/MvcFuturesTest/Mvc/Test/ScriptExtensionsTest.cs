namespace Microsoft.Web.Mvc.Test {
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class ScriptExtensionsTest {
        [TestMethod]
        public void ScriptWithoutReleaseFileThrowsArgumentNullException() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(() => html.Script(null, "file"), "releaseFile");
        }

        [TestMethod]
        public void ScriptWithoutDebugFileThrowsArgumentNullException() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(() => html.Script("File", null), "debugFile");
        }

        [TestMethod]
        public void ScriptWithRootedPathRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("~/Correct/Path.js", "~/Correct/Debug/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"/$(SESSION)/Correct/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithRelativePathRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("../../Correct/Path.js", "../../Correct/Debug/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"../../Correct/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithRelativeCurrentPathRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("/Correct/Path.js", "/Correct/Debug/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"/Correct/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithScriptRelativePathRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("Correct/Path.js", "Correct/Debug/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"/$(SESSION)/Scripts/Correct/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithUrlRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("http://ajax.Correct.com/Path.js", "http://ajax.Debug.com/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"http://ajax.Correct.com/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithSecureUrlRendersProperElement() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());

            // Act
            MvcHtmlString result = html.Script("https://ajax.Correct.com/Path.js", "https://ajax.Debug.com/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"https://ajax.Correct.com/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

        [TestMethod]
        public void ScriptWithDebuggingOnUsesDebugUrl() {
            // Arrange
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            Mock.Get<HttpContextBase>(html.ViewContext.HttpContext).Expect(v => v.IsDebuggingEnabled).Returns(true);

            // Act
            MvcHtmlString result = html.Script("Correct/Path.js", "Correct/Debug/Path.js");

            // Assert
            Assert.AreEqual("<script src=\"/$(SESSION)/Scripts/Correct/Debug/Path.js\" type=\"text/javascript\"></script>", result.ToHtmlString());
        }

    }
}
