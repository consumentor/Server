namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class CollectionModelBinderTest {

        [TestMethod]
        public void BindComplexCollectionFromIndexes_FiniteIndexes() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider() {
                    { "someName[foo]", "42" },
                    { "someName[baz]", "200" }
                }
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        mbc.Model = mbc.ValueProvider.GetValue(mbc.ModelName).ConvertTo(mbc.ModelType);
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, false /* suppressPrefixCheck */);

            // Act
            List<int> boundCollection = CollectionModelBinder<int>.BindComplexCollectionFromIndexes(controllerContext, bindingContext, new string[] { "foo", "bar", "baz" });

            // Assert
            CollectionAssert.AreEqual(new int[] { 42, 0, 200 }, boundCollection, "Missing element should have been converted to default(Int32).");
            CollectionAssert.AreEquivalent(new string[] { "someName[foo]", "someName[baz]" }, bindingContext.ValidationNode.ChildNodes.Select(o => o.ModelStateKey).ToArray());
        }

        [TestMethod]
        public void BindComplexCollectionFromIndexes_InfiniteIndexes() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider() {
                    { "someName[0]", "42" },
                    { "someName[1]", "100" },
                    { "someName[3]", "400" }
                }
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        mbc.Model = mbc.ValueProvider.GetValue(mbc.ModelName).ConvertTo(mbc.ModelType);
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, false /* suppressPrefixCheck */);

            // Act
            List<int> boundCollection = CollectionModelBinder<int>.BindComplexCollectionFromIndexes(controllerContext, bindingContext, null /* indexNames */);

            // Assert
            CollectionAssert.AreEqual(new int[] { 42, 100 }, boundCollection, "Binding should have halted at missing element.");
            CollectionAssert.AreEquivalent(new string[] { "someName[0]", "someName[1]" }, bindingContext.ValidationNode.ChildNodes.Select(o => o.ModelStateKey).ToArray());
        }

        [TestMethod]
        public void BindModel_ComplexCollection() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider() {
                    { "someName.index", new string[] { "foo", "bar", "baz" } },
                    { "someName[foo]", "42" },
                    { "someName[bar]", "100" },
                    { "someName[baz]", "200" }
                }
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        mbc.Model = mbc.ValueProvider.GetValue(mbc.ModelName).ConvertTo(mbc.ModelType);
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, true /* suppressPrefixCheck */);

            CollectionModelBinder<int> modelBinder = new CollectionModelBinder<int>();

            // Act
            bool retVal = modelBinder.BindModel(controllerContext, bindingContext);

            // Assert
            CollectionAssert.AreEqual(new int[] { 42, 100, 200 }, bindingContext.Model as ICollection);
        }

        [TestMethod]
        public void BindModel_SimpleCollection() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider() {
                    { "someName", new string[] { "42", "100", "200" } }
                }
            };

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        mbc.Model = mbc.ValueProvider.GetValue(mbc.ModelName).ConvertTo(mbc.ModelType);
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, true /* suppressPrefixCheck */);

            CollectionModelBinder<int> modelBinder = new CollectionModelBinder<int>();

            // Act
            bool retVal = modelBinder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.IsTrue(retVal);
            CollectionAssert.AreEqual(new int[] { 42, 100, 200 }, bindingContext.Model as ICollection);
        }

        [TestMethod]
        public void BindSimpleCollection_RawValueIsEmptyCollection_ReturnsEmptyList() {
            // Act
            List<int> boundCollection = CollectionModelBinder<int>.BindSimpleCollection(null, null, new object[0], null);

            // Assert
            Assert.IsNotNull(boundCollection);
            Assert.AreEqual(0, boundCollection.Count);
        }

        [TestMethod]
        public void BindSimpleCollection_RawValueIsNull_ReturnsNull() {
            // Act
            List<int> boundCollection = CollectionModelBinder<int>.BindSimpleCollection(null, null, null, null);

            // Assert
            Assert.IsNull(boundCollection);
        }

        [TestMethod]
        public void BindSimpleCollection_SubBinderDoesNotExist() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider()
            };

            // Act
            List<int> boundCollection = CollectionModelBinder<int>.BindSimpleCollection(controllerContext, bindingContext, new int[1], culture);

            // Assert
            CollectionAssert.AreEqual(new int[] { 0 }, boundCollection, "default(int) was not correctly added to collection.");
            Assert.AreEqual(0, bindingContext.ValidationNode.ChildNodes.Count, "No child validation node should have been added.");
        }

        [TestMethod]
        public void BindSimpleCollection_SubBindingSucceeds() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int)),
                ModelName = "someName",
                ModelBinderProviders = new ModelBinderProviderCollection(),
                ValueProvider = new SimpleValueProvider()
            };

            ModelValidationNode childValidationNode = null;
            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(o => o.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        Assert.AreEqual("someName", mbc.ModelName);
                        childValidationNode = mbc.ValidationNode;
                        mbc.Model = 42;
                        return true;
                    });
            bindingContext.ModelBinderProviders.RegisterBinderForType(typeof(int), mockIntBinder.Object, true /* suppressPrefixCheck */);

            // Act
            List<int> boundCollection = CollectionModelBinder<int>.BindSimpleCollection(controllerContext, bindingContext, new int[1], culture);

            // Assert
            CollectionAssert.AreEqual(new int[] { 42 }, boundCollection);
            CollectionAssert.AreEqual(new object[] { childValidationNode }, bindingContext.ValidationNode.ChildNodes.ToArray(), "Child validation node was not added.");
        }

    }
}
