namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicViewDataDictionaryTest {

        // Property-style accessor

        [TestMethod]
        public void Property_UnknownItemReturnsEmptyString() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            dynamic dvdd = DynamicViewDataDictionary.Wrap(vdd);

            // Act
            object result = dvdd.Foo;

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void Property_CanAccessViewDataValues() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd["Foo"] = "Value for Foo";
            dynamic dvdd = DynamicViewDataDictionary.Wrap(vdd);

            // Act
            object result = dvdd.Foo;

            // Assert
            Assert.AreEqual("Value for Foo", result);
        }

        [TestMethod]
        public void Property_CanAccessModelProperties() {
            ViewDataDictionary vdd = new ViewDataDictionary(new { Foo = "Value for Foo" });
            dynamic dvdd = DynamicViewDataDictionary.Wrap(vdd);

            // Act
            object result = dvdd.Foo;

            // Assert
            Assert.AreEqual("Value for Foo", result);
        }

        // Index-style accessor

        [TestMethod]
        public void Indexer_GuardClauses() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            dynamic dvdd = DynamicViewDataDictionary.Wrap(vdd);

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => { var x = dvdd["foo", "bar"]; },
                "DynamicViewDataDictionary only supports single indexers.");

            ExceptionHelper.ExpectArgumentException(
                () => { var x = dvdd[42]; },
                "DynamicViewDataDictionary only supports string-based indexers.");
        }

        [TestMethod]
        public void Indexer_UnknownItemReturnsEmptyString() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            dynamic dvdd = DynamicViewDataDictionary.Wrap(vdd);

            // Act
            object result = dvdd["Foo"];

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void Indexer_CanAccessViewDataValues() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd["Foo"] = "Value for Foo";
            dynamic dvdd = DynamicViewDataDictionary.Wrap(vdd);

            // Act
            object result = dvdd["Foo"];

            // Assert
            Assert.AreEqual("Value for Foo", result);
        }

        [TestMethod]
        public void Indexer_CanAccessModelProperties() {
            ViewDataDictionary vdd = new ViewDataDictionary(new { Foo = "Value for Foo" });
            dynamic dvdd = DynamicViewDataDictionary.Wrap(vdd);

            // Act
            object result = dvdd["Foo"];

            // Assert
            Assert.AreEqual("Value for Foo", result);
        }

    }
}
