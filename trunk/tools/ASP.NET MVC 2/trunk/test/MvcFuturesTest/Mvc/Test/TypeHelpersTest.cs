namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeHelpersTest {

        [TestMethod]
        public void GetTypeArgumentsIfMatch_ClosedTypeIsGenericAndMatches_ReturnsType() {
            // Act
            Type[] typeArguments = TypeHelpers.GetTypeArgumentsIfMatch(typeof(List<int>), typeof(List<>));

            // Assert
            CollectionAssert.AreEqual(new Type[] { typeof(int) }, typeArguments);
        }

        [TestMethod]
        public void GetTypeArgumentsIfMatch_ClosedTypeIsGenericButDoesNotMatch_ReturnsNull() {
            // Act
            Type[] typeArguments = TypeHelpers.GetTypeArgumentsIfMatch(typeof(int?), typeof(List<>));

            // Assert
            Assert.IsNull(typeArguments);
        }

        [TestMethod]
        public void GetTypeArgumentsIfMatch_ClosedTypeIsNotGeneric_ReturnsNull() {
            // Act
            Type[] typeArguments = TypeHelpers.GetTypeArgumentsIfMatch(typeof(int), null);

            // Assert
            Assert.IsNull(typeArguments);
        }

        [TestMethod]
        public void IsCompatibleObjectReturnsTrueIfTypeIsNotNullableAndValueIsNull() {
            // Act
            bool retVal = TypeHelpers.IsCompatibleObject(typeof(int), null);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void IsCompatibleObjectReturnsFalseIfValueIsIncorrectType() {
            // Arrange
            object value = new string[] { "Hello", "world" };

            // Act
            bool retVal = TypeHelpers.IsCompatibleObject(typeof(int), value);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void IsCompatibleObjectReturnsTrueIfTypeIsNullableAndValueIsNull() {
            // Act
            bool retVal = TypeHelpers.IsCompatibleObject(typeof(int?), null);

            // Assert
            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void IsCompatibleObjectReturnsTrueIfValueIsOfCorrectType() {
            // Arrange
            object value = new string[] { "Hello", "world" };

            // Act
            bool retVal = TypeHelpers.IsCompatibleObject(typeof(IEnumerable<string>), value);

            // Assert
            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsFalseForNonNullableGenericValueType() {
            Assert.IsFalse(TypeHelpers.TypeAllowsNullValue(typeof(KeyValuePair<int, string>)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsFalseForNonNullableGenericValueTypeDefinition() {
            Assert.IsFalse(TypeHelpers.TypeAllowsNullValue(typeof(KeyValuePair<,>)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsFalseForNonNullableValueType() {
            Assert.IsFalse(TypeHelpers.TypeAllowsNullValue(typeof(int)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsTrueForInterfaceType() {
            Assert.IsTrue(TypeHelpers.TypeAllowsNullValue(typeof(IDisposable)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsTrueForNullableType() {
            Assert.IsTrue(TypeHelpers.TypeAllowsNullValue(typeof(int?)));
        }

        [TestMethod]
        public void TypeAllowsNullValueReturnsTrueForReferenceType() {
            Assert.IsTrue(TypeHelpers.TypeAllowsNullValue(typeof(object)));
        }

    }
}
