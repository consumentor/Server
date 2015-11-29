namespace Microsoft.Web.Mvc.AspNet4.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicViewPageTest {

        // DynamicViewPage

        [TestMethod]
        public void AnonymousObjectsAreWrapped() {
            // Arrange
            DynamicViewPage page = new DynamicViewPage();
            page.ViewData.Model = new { foo = "Hello world!" };

            // Act & Assert
            Assert.AreEqual("Microsoft.Web.Mvc.AspNet4.DynamicReflectionObject", page.Model.GetType().FullName);
        }

        [TestMethod]
        public void NonAnonymousObjectsAreNotWrapped() {
            // Arrange
            DynamicViewPage page = new DynamicViewPage();
            page.ViewData.Model = "Hello world!";

            // Act & Assert
            Assert.AreEqual(typeof(string), page.Model.GetType());
        }

        [TestMethod]
        public void ViewDataDictionaryIsWrapped() {
            // Arrange
            DynamicViewPage page = new DynamicViewPage();

            // Act & Assert
            Assert.AreEqual("Microsoft.Web.Mvc.AspNet4.DynamicViewDataDictionary", page.ViewData.GetType().FullName);
        }

        // DynamicViewPage<T>

        [TestMethod]
        public void Generic_ViewDataDictionaryIsWrapped() {
            // Arrange
            DynamicViewPage<object> page = new DynamicViewPage<object>();

            // Act & Assert
            Assert.AreEqual("Microsoft.Web.Mvc.AspNet4.DynamicViewDataDictionary", page.ViewData.GetType().FullName);
        }

    }
}
