namespace System.Web.Mvc.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MvcHtmlStringTest {

        // IsNullOrEmpty

        [TestMethod]
        public void IsNullOrEmptyTests() {
            // Act & Assert
            Assert.IsTrue(MvcHtmlString.IsNullOrEmpty(null));
            Assert.IsTrue(MvcHtmlString.IsNullOrEmpty(MvcHtmlString.Empty));
            Assert.IsTrue(MvcHtmlString.IsNullOrEmpty(MvcHtmlString.Create("")));
            Assert.IsFalse(MvcHtmlString.IsNullOrEmpty(MvcHtmlString.Create(" ")));
        }

        // ToHtmlString

        [TestMethod]
        public void ToHtmlStringReturnsOriginalString() {
            // Arrange
            MvcHtmlString htmlString = MvcHtmlString.Create("some value");

            // Act
            string retVal = htmlString.ToHtmlString();

            // Assert
            Assert.AreEqual("some value", retVal);
        }

        // ToString

        [TestMethod]
        public void ToStringReturnsOriginalString() {
            // Arrange
            MvcHtmlString htmlString = MvcHtmlString.Create("some value");

            // Act
            string retVal = htmlString.ToString();

            // Assert
            Assert.AreEqual("some value", retVal);
        }

        [TestMethod]
        public void ToStringReturnsEmptyStringIfOriginalStringWasNull() {
            // Arrange
            MvcHtmlString htmlString = MvcHtmlString.Create(null);

            // Act
            string retVal = htmlString.ToString();

            // Assert
            Assert.AreEqual("", retVal);
        }

    }
}
