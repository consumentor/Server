namespace Microsoft.Web.Mvc.AspNet4.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicReflectionObjectTest {

        [TestMethod]
        public void NoPropertiesThrows() {
            // Arrange
            dynamic dro = DynamicReflectionObject.Wrap(new { });

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => dro.baz,
                "The property baz doesn't exist. There are no public properties on this object.");
        }

        [TestMethod]
        public void UnknownPropertyThrows() {
            // Arrange
            dynamic dro = DynamicReflectionObject.Wrap(new { foo = 3.4, biff = "Two", bar = 1 });

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => dro.baz,
                "The property baz doesn't exist. Supported properties are: bar, biff, foo.");
        }

        [TestMethod]
        public void CanAccessProperties() {
            // Arrange
            dynamic dro = DynamicReflectionObject.Wrap(new { foo = "Hello world!", bar = 42 });

            // Act & Assert
            Assert.AreEqual("Hello world!", dro.foo);
            Assert.AreEqual(42, dro.bar);
        }

        [TestMethod]
        public void CanAccessNestedAnonymousProperties() {
            // Arrange
            dynamic dro = DynamicReflectionObject.Wrap(new { foo = new { bar = "Hello world!" } });

            // Act & Assert
            Assert.AreEqual("Hello world!", dro.foo.bar);
        }

    }
}
