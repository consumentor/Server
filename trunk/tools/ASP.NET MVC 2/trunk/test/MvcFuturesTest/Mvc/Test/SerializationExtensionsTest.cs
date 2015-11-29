namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class SerializationExtensionsTest {

        [TestMethod]
        public void SerializeFromProvidedValueOverridesViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary() {
                { "someKey", 42 }
            };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString htmlString = helper.Serialize("someKey", "Hello!", SerializationMode.Plaintext);

            // Assert
            Assert.AreEqual(@"<input name=""someKey"" type=""hidden"" value=""/wEFBkhlbGxvIQ=="" />", htmlString.ToHtmlString());
        }

        [TestMethod]
        public void SerializeFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary() {
                { "someKey", 42 }
            };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString htmlString = helper.Serialize("someKey", SerializationMode.Plaintext);

            // Assert
            Assert.AreEqual(@"<input name=""someKey"" type=""hidden"" value=""/wECKg=="" />", htmlString.ToHtmlString());
        }

        [TestMethod]
        public void SerializeThrowsIfHtmlHelperIsNull() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    SerializationExtensions.Serialize(null, "someName");
                }, "htmlHelper");
        }

        [TestMethod]
        public void SerializeThrowsIfNameIsEmpty() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Serialize("");
                }, "name");
        }

        [TestMethod]
        public void SerializeThrowsIfNameIsNull() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Serialize(null);
                }, "name");
        }

    }
}
