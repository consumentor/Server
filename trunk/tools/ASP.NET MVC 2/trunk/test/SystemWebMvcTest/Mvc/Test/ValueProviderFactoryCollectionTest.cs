namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ValueProviderFactoryCollectionTest {

        [TestMethod]
        public void ListWrappingConstructor() {
            // Arrange
            List<ValueProviderFactory> list = new List<ValueProviderFactory>() {
                new FormValueProviderFactory()
            };

            // Act
            ValueProviderFactoryCollection collection = new ValueProviderFactoryCollection(list);

            // Assert
            CollectionAssert.AreEqual(list, collection);
        }

        [TestMethod]
        public void ListWrappingConstructorThrowsIfListIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ValueProviderFactoryCollection(null);
                },
                "list");
        }

        [TestMethod]
        public void DefaultConstructor() {
            // Act
            ValueProviderFactoryCollection collection = new ValueProviderFactoryCollection();

            // Assert
            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void AddNullValueProviderFactoryThrows() {
            // Arrange
            ValueProviderFactoryCollection collection = new ValueProviderFactoryCollection();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection.Add(null);
                },
                "item");
        }

        [TestMethod]
        public void GetValueProvider() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            IValueProvider[] expectedValueProviders = new IValueProvider[] {
                new Mock<IValueProvider>().Object,
                new Mock<IValueProvider>().Object
            };

            Mock<ValueProviderFactory> mockFactory1 = new Mock<ValueProviderFactory>();
            mockFactory1.Expect(o => o.GetValueProvider(controllerContext)).Returns(expectedValueProviders[0]);
            Mock<ValueProviderFactory> mockFactory2 = new Mock<ValueProviderFactory>();
            mockFactory2.Expect(o => o.GetValueProvider(controllerContext)).Returns(expectedValueProviders[1]);

            ValueProviderFactoryCollection factories = new ValueProviderFactoryCollection() {
                mockFactory1.Object,
                mockFactory2.Object
            };

            // Act
            ValueProviderCollection valueProviders = (ValueProviderCollection)factories.GetValueProvider(controllerContext);

            // Assert
            CollectionAssert.AreEqual(expectedValueProviders, valueProviders.ToArray());
        }

        [TestMethod]
        public void SetItem() {
            // Arrange
            ValueProviderFactoryCollection collection = new ValueProviderFactoryCollection();
            collection.Add(new Mock<ValueProviderFactory>().Object);

            ValueProviderFactory newFactory = new Mock<ValueProviderFactory>().Object;

            // Act
            collection[0] = newFactory;

            // Assert
            Assert.AreEqual(1, collection.Count);
            Assert.AreEqual(newFactory, collection[0]);
        }

        [TestMethod]
        public void SetNullValueProviderFactoryThrows() {
            // Arrange
            ValueProviderFactoryCollection collection = new ValueProviderFactoryCollection();
            collection.Add(new Mock<ValueProviderFactory>().Object);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    collection[0] = null;
                },
                "item");
        }

    }
}
