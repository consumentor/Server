namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ActionDescriptorTest {

        [TestMethod]
        public void ExtractParameterOrDefaultFromDictionary_ReturnsDefaultParameterValueIfMismatch() {
            // Arrange
            Dictionary<string, object> dictionary = new Dictionary<string, object>() {
                { "stringParameterWithDefaultValue", 42 }
            };

            // Act
            object value = ActionDescriptor.ExtractParameterOrDefaultFromDictionary(ParameterExtractionController.StringParameterWithDefaultValue, dictionary);

            // Assert
            Assert.AreEqual("hello", value);
        }

        [TestMethod]
        public void ExtractParameterOrDefaultFromDictionary_ReturnsDefaultTypeValueIfNoMatchAndNoDefaultParameterValue() {
            // Arrange
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            // Act
            object value = ActionDescriptor.ExtractParameterOrDefaultFromDictionary(ParameterExtractionController.IntParameter, dictionary);

            // Assert
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void ExtractParameterOrDefaultFromDictionary_ReturnsDictionaryValueIfTypeMatch() {
            // Arrange
            Dictionary<string, object> dictionary = new Dictionary<string, object>() {
                { "stringParameterNoDefaultValue", "someValue" }
            };

            // Act
            object value = ActionDescriptor.ExtractParameterOrDefaultFromDictionary(ParameterExtractionController.StringParameterNoDefaultValue, dictionary);

            // Assert
            Assert.AreEqual("someValue", value);
        }

        [TestMethod]
        public void GetCustomAttributesReturnsEmptyArrayOfAttributeType() {
            // Arrange
            ActionDescriptor ad = GetActionDescriptor();

            // Act
            ObsoleteAttribute[] attrs = (ObsoleteAttribute[])ad.GetCustomAttributes(typeof(ObsoleteAttribute), true);

            // Assert
            Assert.AreEqual(0, attrs.Length);
        }

        [TestMethod]
        public void GetCustomAttributesThrowsIfAttributeTypeIsNull() {
            // Arrange
            ActionDescriptor ad = GetActionDescriptor();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.GetCustomAttributes(null /* attributeType */, true);
                }, "attributeType");
        }

        [TestMethod]
        public void GetCustomAttributesWithoutAttributeTypeCallsGetCustomAttributesWithAttributeType() {
            // Arrange
            object[] expected = new object[0];
            Mock<ActionDescriptor> mockDescriptor = new Mock<ActionDescriptor>() { CallBase = true };
            mockDescriptor.Expect(d => d.GetCustomAttributes(typeof(object), true)).Returns(expected);
            ActionDescriptor ad = mockDescriptor.Object;

            // Act
            object[] returned = ad.GetCustomAttributes(true /* inherit */);

            // Assert
            Assert.AreSame(expected, returned);
        }

        [TestMethod]
        public void GetFiltersReturnsFilterInfo() {
            // Arrange
            ActionDescriptor ad = GetActionDescriptor();

            // Act
            FilterInfo filters = ad.GetFilters();
            FilterInfo filters2 = ad.GetFilters();

            // Assert
            Assert.IsNotNull(filters);
            Assert.AreNotSame(filters, filters2, "Should have returned different FilterInfo objects so that changes in one won't be reflected in the other.");
        }

        [TestMethod]
        public void GetSelectorsReturnsEmptyCollection() {
            // Arrange
            ActionDescriptor ad = GetActionDescriptor();

            // Act
            ICollection<ActionSelector> selectors = ad.GetSelectors();

            // Assert
            Assert.IsInstanceOfType(selectors, typeof(ActionSelector[]), "Should return an immutable collection.");
            Assert.AreEqual(0, selectors.Count);
        }

        [TestMethod]
        public void IsDefinedReturnsFalse() {
            // Arrange
            ActionDescriptor ad = GetActionDescriptor();

            // Act
            bool isDefined = ad.IsDefined(typeof(object), true);

            // Assert
            Assert.IsFalse(isDefined);
        }

        [TestMethod]
        public void IsDefinedThrowsIfAttributeTypeIsNull() {
            // Arrange
            ActionDescriptor ad = GetActionDescriptor();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.IsDefined(null /* attributeType */, true);
                }, "attributeType");
        }

        private static ActionDescriptor GetActionDescriptor() {
            Mock<ActionDescriptor> mockDescriptor = new Mock<ActionDescriptor>() { CallBase = true };
            return mockDescriptor.Object;
        }

        private class ParameterExtractionController : Controller {
            public static readonly ParameterInfo IntParameter = typeof(ParameterExtractionController).GetMethod("SomeMethod").GetParameters()[0];
            public static readonly ParameterInfo StringParameterNoDefaultValue = typeof(ParameterExtractionController).GetMethod("SomeMethod").GetParameters()[1];
            public static readonly ParameterInfo StringParameterWithDefaultValue = typeof(ParameterExtractionController).GetMethod("SomeMethod").GetParameters()[2];

            public void SomeMethod(int intParameter, string stringParameterNoDefaultValue, [DefaultValue("hello")] string stringParameterWithDefaultValue) {
            }
        }

    }
}
