namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeHelpersTest {

        [TestMethod]
        public void CreateDelegateBindsInstanceMethod() {
            // Act
            string s = "Hello, world!";
            Func<string, bool> endsWith = TypeHelpers.CreateDelegate<Func<string, bool>>(TypeHelpers.MsCorLibAssembly, "System.String", "EndsWith", s);

            // Assert
            Assert.IsNotNull(endsWith);
            Assert.IsTrue(endsWith("world!"));
        }

        [TestMethod]
        public void CreateDelegateBindsStaticMethod() {
            // Act
            Func<object, object, string> concat = TypeHelpers.CreateDelegate<Func<object, object, string>>(TypeHelpers.MsCorLibAssembly, "System.String", "Concat", null);

            // Assert
            Assert.IsNotNull(concat);
            Assert.AreEqual("45", concat(4, 5));
        }

        [TestMethod]
        public void CreateDelegateReturnsNullIfTypeDoesNotExist() {
            // Act
            Action d = TypeHelpers.CreateDelegate<Action>(TypeHelpers.MsCorLibAssembly, "System.xyz.TypeDoesNotExist", "SomeMethod", null);

            // Assert
            Assert.IsNull(d);
        }

        [TestMethod]
        public void CreateDelegateReturnsNullIfMethodDoesNotExist() {
            // Act
            Action d = TypeHelpers.CreateDelegate<Action>(TypeHelpers.MsCorLibAssembly, "System.String", "MethodDoesNotExist", null);

            // Assert
            Assert.IsNull(d);
        }

        [TestMethod]
        public void CreateTryGetValueDelegateReturnsNullForNonDictionaries() {
            // Arrange
            object notDictionary = "Hello, world";

            // Act
            TryGetValueDelegate d = TypeHelpers.CreateTryGetValueDelegate(notDictionary.GetType());

            // Assert
            Assert.IsNull(d);
        }

        [TestMethod]
        public void CreateTryGetValueDelegateWrapsGenericObjectDictionaries() {
            // Arrange
            object dictionary = new Dictionary<object, int>(){
                { "theKey", 42 }
            };

            // Act
            TryGetValueDelegate d = TypeHelpers.CreateTryGetValueDelegate(dictionary.GetType());

            object value;
            bool found = d(dictionary, "theKey", out value);

            // Assert
            Assert.IsTrue(found);
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void CreateTryGetValueDelegateWrapsGenericStringDictionaries() {
            // Arrange
            object dictionary = new Dictionary<string, int>(){
                { "theKey", 42 }
            };

            // Act
            TryGetValueDelegate d = TypeHelpers.CreateTryGetValueDelegate(dictionary.GetType());

            object value;
            bool found = d(dictionary, "theKey", out value);

            // Assert
            Assert.IsTrue(found);
            Assert.AreEqual(42, value);
        }

        [TestMethod]
        public void CreateTryGetValueDelegateWrapsNonGenericDictionaries() {
            // Arrange
            object dictionary = new Hashtable(){
                { "foo", 42 }
            };

            // Act
            TryGetValueDelegate d = TypeHelpers.CreateTryGetValueDelegate(dictionary.GetType());

            object fooValue;
            bool fooFound = d(dictionary, "foo", out fooValue);

            object barValue;
            bool barFound = d(dictionary, "bar", out barValue);

            // Assert
            Assert.IsTrue(fooFound);
            Assert.AreEqual(42, fooValue);
            Assert.IsFalse(barFound);
            Assert.IsNull(barValue);
        }

        [TestMethod]
        public void GetDefaultValue_NullableValueType() {
            // Act
            object defaultValue = TypeHelpers.GetDefaultValue(typeof(int?));

            // Assert
            Assert.AreEqual(default(int?), defaultValue);
        }

        [TestMethod]
        public void GetDefaultValue_ReferenceType() {
            // Act
            object defaultValue = TypeHelpers.GetDefaultValue(typeof(object));

            // Assert
            Assert.AreEqual(default(object), defaultValue);
        }

        [TestMethod]
        public void GetDefaultValue_ValueType() {
            // Act
            object defaultValue = TypeHelpers.GetDefaultValue(typeof(int));

            // Assert
            Assert.AreEqual(default(int), defaultValue);
        }

        [TestMethod]
        public void IsCompatibleObjectReturnsTrueIfTypeIsNotNullableAndValueIsNull() {
            // Act
            bool retVal = TypeHelpers.IsCompatibleObject<int>(null);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void IsCompatibleObjectReturnsFalseIfValueIsIncorrectType() {
            // Arrange
            object value = new string[] { "Hello", "world" };

            // Act
            bool retVal = TypeHelpers.IsCompatibleObject<int>(value);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void IsCompatibleObjectReturnsTrueIfTypeIsNullableAndValueIsNull() {
            // Act
            bool retVal = TypeHelpers.IsCompatibleObject<int?>(null);

            // Assert
            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void IsCompatibleObjectReturnsTrueIfValueIsOfCorrectType() {
            // Arrange
            object value = new string[] { "Hello", "world" };

            // Act
            bool retVal = TypeHelpers.IsCompatibleObject<IEnumerable<string>>(value);

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
