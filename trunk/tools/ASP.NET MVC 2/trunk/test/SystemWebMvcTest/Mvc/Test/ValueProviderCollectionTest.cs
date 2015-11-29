namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ValueProviderCollectionTest {

        [TestMethod]
        public void ListWrappingConstructor() {
            // Arrange
            List<IValueProvider> list = new List<IValueProvider>() {
                new Mock<IValueProvider>().Object, new Mock<IValueProvider>().Object 
            };

            // Act
            ValueProviderCollection collection = new ValueProviderCollection(list);

            // Assert
            CollectionAssert.AreEqual(list, collection);
        }

        [TestMethod]
        public void ListWrappingConstructorThrowsIfListIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ValueProviderCollection(null);
                },
                "list");
        }

        [TestMethod]
        public void DefaultConstructor() {
            // Act
            ValueProviderCollection collection = new ValueProviderCollection();

            // Assert
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddNullValueProviderThrows() {
            // Arrange
            ValueProviderCollection collection = new ValueProviderCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection.Add(null);
                },
                "item");
        }

        [TestMethod]
        public void SetItem() {
            // Arrange
            ValueProviderCollection collection = new ValueProviderCollection();
            collection.Add(new Mock<IValueProvider>().Object);

            IValueProvider newProvider = new Mock<IValueProvider>().Object;

            // Act
            collection[0] = newProvider;

            // Assert
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(newProvider, collection[0]);
        }

        [TestMethod]
        public void SetNullValueProviderThrows() {
            // Arrange
            ValueProviderCollection collection = new ValueProviderCollection();
            collection.Add(new Mock<IValueProvider>().Object);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection[0] = null;
                },
                "item");
        }

        [TestMethod]
        public void ContainsPrefix() {
            // Arrange
            string prefix = "somePrefix";

            Mock<IValueProvider> mockProvider1 = new Mock<IValueProvider>();
            mockProvider1.Expect(p => p.ContainsPrefix(prefix)).Returns(false);
            Mock<IValueProvider> mockProvider2 = new Mock<IValueProvider>();
            mockProvider2.Expect(p => p.ContainsPrefix(prefix)).Returns(true);
            Mock<IValueProvider> mockProvider3 = new Mock<IValueProvider>();
            mockProvider3.Expect(p => p.ContainsPrefix(prefix)).Returns(false);

            ValueProviderCollection collection = new ValueProviderCollection() {
                mockProvider1.Object, mockProvider2.Object, mockProvider3.Object
            };

            // Act
            bool retVal = collection.ContainsPrefix(prefix);

            // Assert
            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void GetValue() {
            // Arrange
            string key = "someKey";

            Mock<IValueProvider> mockProvider1 = new Mock<IValueProvider>();
            mockProvider1.Expect(p => p.GetValue(key)).Returns((ValueProviderResult)null);
            Mock<IValueProvider> mockProvider2 = new Mock<IValueProvider>();
            mockProvider2.Expect(p => p.GetValue(key)).Returns(new ValueProviderResult("2", "2", null));
            Mock<IValueProvider> mockProvider3 = new Mock<IValueProvider>();
            mockProvider3.Expect(p => p.GetValue(key)).Returns(new ValueProviderResult("3", "3", null));

            ValueProviderCollection collection = new ValueProviderCollection() {
                mockProvider1.Object, mockProvider2.Object, mockProvider3.Object
            };

            // Act
            ValueProviderResult result = collection.GetValue(key);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.ConvertTo(typeof(int)));
        }

    }
}
