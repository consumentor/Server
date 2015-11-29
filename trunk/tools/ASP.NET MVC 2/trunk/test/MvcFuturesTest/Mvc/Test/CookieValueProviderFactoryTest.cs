namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class CookieValueProviderFactoryTest {

        [TestMethod]
        public void GetValueProvider() {
            // Arrange
            HttpCookieCollection cookies = new HttpCookieCollection() {
                new HttpCookie("foo", "fooValue"),
                new HttpCookie("bar.baz", "barBazValue"),
                new HttpCookie("", "emptyValue"),
                new HttpCookie(null, "nullValue")
            };

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.Cookies).Returns(cookies);

            CookieValueProviderFactory factory = new CookieValueProviderFactory();

            // Act
            IValueProvider provider = factory.GetValueProvider(mockControllerContext.Object);

            // Assert
            Assert.IsNull(provider.GetValue(""), "Should've skipped empty key cookie.");
            Assert.IsTrue(provider.ContainsPrefix("bar"));
            Assert.AreEqual("fooValue", provider.GetValue("foo").AttemptedValue);
            Assert.AreEqual(CultureInfo.InvariantCulture, provider.GetValue("foo").Culture);
        }

    }
}
