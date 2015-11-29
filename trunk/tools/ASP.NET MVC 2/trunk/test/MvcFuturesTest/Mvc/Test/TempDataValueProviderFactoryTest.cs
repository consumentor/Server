namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class TempDataValueProviderFactoryTest {

        [TestMethod]
        public void GetValueProvider_CorrectlyRetainsOrRemovesKeys() {
            // Arrange
            string[] expectedRetainedKeys = new string[] {
                "retainMe"
            };

            TempDataDictionary tempData = new TempDataDictionary() {
                { "retainMe", "retainMeValue" },
                { "removeMe", "removeMeValue" },
                { "previouslyRemoved", "previouslyRemovedValue" }
            };
            object dummy = tempData["previouslyRemoved"]; // mark value for removal

            ControllerContext controllerContext = GetControllerContext(tempData);

            TempDataValueProviderFactory factory = new TempDataValueProviderFactory();

            // Act
            IValueProvider valueProvider = factory.GetValueProvider(controllerContext);
            ValueProviderResult nonExistentResult = valueProvider.GetValue("nonExistent");
            ValueProviderResult removeMeResult = valueProvider.GetValue("removeme");

            // Assert
            Assert.IsNull(nonExistentResult);
            Assert.AreEqual("removeMeValue", removeMeResult.AttemptedValue);
            Assert.AreEqual(CultureInfo.InvariantCulture, removeMeResult.Culture);

            // Verify that keys have been removed or retained correctly by the provider
            Mock<ITempDataProvider> mockTempDataProvider = new Mock<ITempDataProvider>();
            string[] retainedKeys = null;
            mockTempDataProvider
                .Expect(o => o.SaveTempData(controllerContext, It.IsAny<IDictionary<string, object>>()))
                .Callback(
                    delegate(ControllerContext cc, IDictionary<string, object> d) {
                        retainedKeys = d.Keys.ToArray();
                    });

            tempData.Save(controllerContext, mockTempDataProvider.Object);
            CollectionAssert.AreEquivalent(expectedRetainedKeys, retainedKeys);
        }

        [TestMethod]
        public void GetValueProvider_EmptyTempData_ReturnsNull() {
            // Arrange
            TempDataDictionary tempData = new TempDataDictionary();
            ControllerContext controllerContext = GetControllerContext(tempData);

            TempDataValueProviderFactory factory = new TempDataValueProviderFactory();

            // Act
            IValueProvider provider = factory.GetValueProvider(controllerContext);

            // Assert
            Assert.IsNull(provider);
        }

        private static ControllerContext GetControllerContext(TempDataDictionary tempData) {
            return new ControllerContext() {
                Controller = new EmptyController() {
                    TempData = tempData
                }
            };
        }

        private sealed class EmptyController : Controller {
        }

    }
}
