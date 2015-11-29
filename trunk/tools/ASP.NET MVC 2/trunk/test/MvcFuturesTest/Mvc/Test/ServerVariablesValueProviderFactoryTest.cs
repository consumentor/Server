namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class ServerVariablesValueProviderFactoryTest {

        [TestMethod]
        public void GetValueProvider() {
            // Arrange
            NameValueCollection serverVars = new NameValueCollection() {
                { "foo", "fooValue" },
                { "bar.baz", "barBazValue" }
            };

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.ServerVariables).Returns(serverVars);

            ServerVariablesValueProviderFactory factory = new ServerVariablesValueProviderFactory();

            // Act
            IValueProvider provider = factory.GetValueProvider(mockControllerContext.Object);

            // Assert
            Assert.IsTrue(provider.ContainsPrefix("bar"));
            Assert.AreEqual("fooValue", provider.GetValue("foo").AttemptedValue);
            Assert.AreEqual(CultureInfo.InvariantCulture, provider.GetValue("foo").Culture);
        }

    }
}
