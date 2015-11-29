namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class JsonValueProviderFactoryTest {

        [TestMethod]
        public void GetValueProvider_ComplexJsonObject() {
            // Arrange
            string jsonString = @"
[ { ""FirstName"": ""John"", ""LastName"": ""Doe"", ""Age"": 32,
    ""BillingAddress"": { ""Street"": ""1 Microsoft Way"", ""City"": ""Redmond"", ""State"": ""WA"", ""ZIP"": 98052 },
    ""ShippingAddress"": { ""Street"": ""123 Anywhere Ln"", ""City"": ""Anytown"", ""State"": ""ZZ"", ""ZIP"": 99999 }
  },
  { ""Enchiladas"": [ ""Delicious"", ""Nutritious"", null ] }
]
";

            ControllerContext cc = GetJsonEnabledControllerContext(jsonString);
            JsonValueProviderFactory factory = new JsonValueProviderFactory();

            // Act & assert 1
            IValueProvider valueProvider = factory.GetValueProvider(cc);
            Assert.IsNotNull(valueProvider);

            // Act & assert 2
            Assert.IsTrue(valueProvider.ContainsPrefix("[0].billingaddress"), "[0].billingaddress prefix should have existed.");
            Assert.IsNull(valueProvider.GetValue("[0].billingaddress"), "[0].billingaddress key should not have existed.");

            ValueProviderResult vpResult1 = valueProvider.GetValue("[1].enchiladas[0]");
            Assert.IsNotNull(vpResult1);
            Assert.AreEqual("Delicious", vpResult1.AttemptedValue);
            Assert.AreEqual(CultureInfo.CurrentCulture, vpResult1.Culture);

            // null values should exist in the backing store as actual entries
            ValueProviderResult vpResult2 = valueProvider.GetValue("[1].enchiladas[2]");
            Assert.IsNotNull(vpResult2);
            Assert.IsNull(vpResult2.RawValue);
        }

        [TestMethod]
        public void GetValueProvider_NoJsonBody_ReturnsNull() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.ContentType).Returns("application/json");
            mockControllerContext.Expect(o => o.HttpContext.Request.InputStream).Returns(new MemoryStream());

            JsonValueProviderFactory factory = new JsonValueProviderFactory();

            // Act
            IValueProvider valueProvider = factory.GetValueProvider(mockControllerContext.Object);

            // Assert
            Assert.IsNull(valueProvider);
        }

        [TestMethod]
        public void GetValueProvider_NotJsonRequest_ReturnsNull() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.ContentType).Returns("not JSON");

            JsonValueProviderFactory factory = new JsonValueProviderFactory();

            // Act
            IValueProvider valueProvider = factory.GetValueProvider(mockControllerContext.Object);

            // Assert
            Assert.IsNull(valueProvider);
        }

        private static ControllerContext GetJsonEnabledControllerContext(string jsonString) {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            MemoryStream jsonStream = new MemoryStream(jsonBytes);

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.ContentType).Returns("application/json");
            mockControllerContext.Expect(o => o.HttpContext.Request.InputStream).Returns(jsonStream);
            return mockControllerContext.Object;
        }

    }
}
