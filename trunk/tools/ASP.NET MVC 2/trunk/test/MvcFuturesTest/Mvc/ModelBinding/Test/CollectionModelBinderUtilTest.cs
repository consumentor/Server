namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CollectionModelBinderUtilTest {

        [TestMethod]
        public void CreateOrReplaceCollection_OriginalModelImmutable_CreatesNewInstance() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => new ReadOnlyCollection<int>(new int[0]), typeof(ICollection<int>))
            };

            // Act
            CollectionModelBinderUtil.CreateOrReplaceCollection<int>(bindingContext, new int[] { 10, 20, 30 }, () => new List<int>());

            // Assert
            int[] newModel = (bindingContext.Model as ICollection<int>).ToArray();
            CollectionAssert.AreEqual(new int[] { 10, 20, 30 }, newModel);
        }

        [TestMethod]
        public void CreateOrReplaceCollection_OriginalModelMutable_UpdatesOriginalInstance() {
            // Arrange
            List<int> originalInstance = new List<int>() { 10, 20, 30 };
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => originalInstance, typeof(ICollection<int>))
            };

            // Act
            CollectionModelBinderUtil.CreateOrReplaceCollection<int>(bindingContext, new int[] { 40, 50, 60 }, () => new List<int>());

            // Assert
            Assert.AreSame(originalInstance, bindingContext.Model, "Original collection should have been updated.");
            CollectionAssert.AreEqual(new int[] { 40, 50, 60 }, originalInstance);
        }

        [TestMethod]
        public void CreateOrReplaceCollection_OriginalModelNotCollection_CreatesNewInstance() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(ICollection<int>))
            };

            // Act
            CollectionModelBinderUtil.CreateOrReplaceCollection<int>(bindingContext, new int[] { 10, 20, 30 }, () => new List<int>());

            // Assert
            int[] newModel = (bindingContext.Model as ICollection<int>).ToArray();
            CollectionAssert.AreEqual(new int[] { 10, 20, 30 }, newModel);
        }

        [TestMethod]
        public void CreateOrReplaceDictionary_DisallowsDuplicateKeys() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(Dictionary<string, int>))
            };

            // Act
            CollectionModelBinderUtil.CreateOrReplaceDictionary<string, int>(
                bindingContext,
                new KeyValuePair<string, int>[] {
                    new KeyValuePair<string, int>("forty-two", 40),
                    new KeyValuePair<string, int>("forty-two", 2),
                    new KeyValuePair<string, int>("forty-two", 42)
                },
                () => new Dictionary<string, int>());

            // Assert
            IDictionary<string, int> newModel = bindingContext.Model as IDictionary<string, int>;
            CollectionAssert.AreEquivalent(new string[] { "forty-two" }, newModel.Keys.ToArray());
            Assert.AreEqual(42, newModel["forty-two"]);
        }

        [TestMethod]
        public void CreateOrReplaceDictionary_DisallowsNullKeys() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(Dictionary<string, int>))
            };

            // Act
            CollectionModelBinderUtil.CreateOrReplaceDictionary<string, int>(
                bindingContext,
                new KeyValuePair<string, int>[] {
                    new KeyValuePair<string, int>("forty-two", 42),
                    new KeyValuePair<string, int>(null, 84)
                },
                () => new Dictionary<string, int>());

            // Assert
            IDictionary<string, int> newModel = bindingContext.Model as IDictionary<string, int>;
            CollectionAssert.AreEquivalent(new string[] { "forty-two" }, newModel.Keys.ToArray());
            Assert.AreEqual(42, newModel["forty-two"]);
        }

        [TestMethod]
        public void CreateOrReplaceDictionary_OriginalModelImmutable_CreatesNewInstance() {
            // Arrange
            ReadOnlyDictionary<string, string> originalModel = new ReadOnlyDictionary<string, string>();

            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => originalModel, typeof(IDictionary<string, string>))
            };

            // Act
            CollectionModelBinderUtil.CreateOrReplaceDictionary<string, string>(
                bindingContext,
                new Dictionary<string, string>() {
                    { "Hello", "World" }
                },
                () => new Dictionary<string, string>());

            // Assert
            IDictionary<string, string> newModel = bindingContext.Model as IDictionary<string, string>;
            Assert.AreNotSame(originalModel, newModel, "New instance should have been created.");
            CollectionAssert.AreEquivalent(new string[] { "Hello" }, newModel.Keys.ToArray());
            Assert.AreEqual("World", newModel["Hello"]);
        }

        [TestMethod]
        public void CreateOrReplaceDictionary_OriginalModelMutable_UpdatesOriginalInstance() {
            // Arrange
            Dictionary<string, string> originalInstance = new Dictionary<string, string>() {
                { "dog", "Canidae" },
                { "cat", "Felidae" }
            };
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => originalInstance, typeof(IDictionary<string, string>))
            };

            // Act
            CollectionModelBinderUtil.CreateOrReplaceDictionary<string, string>(
                bindingContext,
                new Dictionary<string, string>() {
                    { "horse", "Equidae" },
                    { "bear", "Ursidae" }
                },
                () => new Dictionary<string, string>());

            // Assert
            Assert.AreSame(originalInstance, bindingContext.Model, "Original collection should have been updated.");
            CollectionAssert.AreEquivalent(new string[] { "horse", "bear" }, originalInstance.Keys.ToArray());
            Assert.AreEqual("Equidae", originalInstance["horse"]);
            Assert.AreEqual("Ursidae", originalInstance["bear"]);
        }

        [TestMethod]
        public void CreateOrReplaceDictionary_OriginalModelNotDictionary_CreatesNewInstance() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(IDictionary<string, string>))
            };

            // Act
            CollectionModelBinderUtil.CreateOrReplaceDictionary<string, string>(
                bindingContext,
                new Dictionary<string, string>() {
                    { "horse", "Equidae" },
                    { "bear", "Ursidae" }
                },
                () => new Dictionary<string, string>());

            // Assert
            IDictionary<string, string> newModel = bindingContext.Model as IDictionary<string, string>;
            CollectionAssert.AreEquivalent(new string[] { "horse", "bear" }, newModel.Keys.ToArray());
            Assert.AreEqual("Equidae", newModel["horse"]);
            Assert.AreEqual("Ursidae", newModel["bear"]);
        }

        [TestMethod]
        public void GetIndexNamesFromValueProviderResult_ValueProviderResultIsNull_ReturnsNull() {
            // Act
            IEnumerable<string> indexNames = CollectionModelBinderUtil.GetIndexNamesFromValueProviderResult(null);

            // Assert
            Assert.IsNull(indexNames);
        }

        [TestMethod]
        public void GetIndexNamesFromValueProviderResult_ValueProviderResultReturnsEmptyArray_ReturnsNull() {
            // Arrange
            ValueProviderResult vpResult = new ValueProviderResult(new string[0], "", null);

            // Act
            IEnumerable<string> indexNames = CollectionModelBinderUtil.GetIndexNamesFromValueProviderResult(vpResult);

            // Assert
            Assert.IsNull(indexNames);
        }

        [TestMethod]
        public void GetIndexNamesFromValueProviderResult_ValueProviderResultReturnsNonEmptyArray_ReturnsArray() {
            // Arrange
            ValueProviderResult vpResult = new ValueProviderResult(new string[] { "foo", "bar", "baz" }, "foo,bar,baz", null);

            // Act
            IEnumerable<string> indexNames = CollectionModelBinderUtil.GetIndexNamesFromValueProviderResult(vpResult);

            // Assert
            Assert.IsNotNull(indexNames);
            CollectionAssert.AreEqual(new string[] { "foo", "bar", "baz" }, indexNames.ToArray());
        }

        [TestMethod]
        public void GetIndexNamesFromValueProviderResult_ValueProviderResultReturnsNull_ReturnsNull() {
            // Arrange
            ValueProviderResult vpResult = new ValueProviderResult(null, null, null);

            // Act
            IEnumerable<string> indexNames = CollectionModelBinderUtil.GetIndexNamesFromValueProviderResult(vpResult);

            // Assert
            Assert.IsNull(indexNames);
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ModelTypeNotGeneric_Fail() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(int));

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(null, null, modelMetadata);

            // Assert
            Assert.IsNull(typeArguments, "The given model type was not generic.");
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ModelTypeOpenGeneric_Fail() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(IList<>));

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(null, null, modelMetadata);

            // Assert
            Assert.IsNull(typeArguments, "The given model type was an open generic.");
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ModelTypeWrongNumberOfGenericArguments_Fail() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(KeyValuePair<int, string>));

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(typeof(ICollection<>), null, modelMetadata);

            // Assert
            Assert.IsNull(typeArguments, "The given model type had the wrong number of generic arguments.");
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ReadOnlyReference_ModelInstanceImmutable_Valid() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => new int[0], typeof(IList<int>));
            modelMetadata.IsReadOnly = true;

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(typeof(IList<>), typeof(List<>), modelMetadata);

            // Assert
            Assert.IsNull(typeArguments, "Collection instance is immutable and reference is readonly.");
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ReadOnlyReference_ModelInstanceMutable_Valid() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => new List<int>(), typeof(IList<int>));
            modelMetadata.IsReadOnly = true;

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(typeof(IList<>), typeof(List<>), modelMetadata);

            // Assert
            CollectionAssert.AreEqual(new Type[] { typeof(int) }, typeArguments, "Collection instance is mutable and updatable.");
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ReadOnlyReference_ModelInstanceOfWrongType_Fail() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => new HashSet<int>(), typeof(ICollection<int>));
            modelMetadata.IsReadOnly = true;

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(typeof(IList<>), typeof(List<>), modelMetadata);

            // Assert
            // HashSet<> is not an IList<>, so we can't update
            Assert.IsNull(typeArguments, "Model instance can't be converted to supported interface type.");
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ReadOnlyReference_ModelIsNull_Fail() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(IList<int>));
            modelMetadata.IsReadOnly = true;

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(typeof(ICollection<>), typeof(List<>), modelMetadata);

            // Assert
            Assert.IsNull(typeArguments, "Null read-only reference cannot be updated.");
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ReadWriteReference_NewInstanceAssignableToModelType_Success() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(IList<int>));
            modelMetadata.IsReadOnly = false;

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(typeof(ICollection<>), typeof(List<>), modelMetadata);

            // Assert
            CollectionAssert.AreEqual(new Type[] { typeof(int) }, typeArguments, "Mutable reference can be overwritten.");
        }

        [TestMethod]
        public void GetTypeArgumentsForUpdatableGenericCollection_ReadWriteReference_NewInstanceNotAssignableToModelType_MutableInstance_Success() {
            // Arrange
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => new Collection<int>(), typeof(Collection<int>));
            modelMetadata.IsReadOnly = false;

            // Act
            Type[] typeArguments = CollectionModelBinderUtil.GetTypeArgumentsForUpdatableGenericCollection(typeof(ICollection<>), typeof(List<>), modelMetadata);

            // Assert
            CollectionAssert.AreEqual(new Type[] { typeof(int) }, typeArguments, "Model updatable in-place.");
        }

        [TestMethod]
        public void GetZeroBasedIndexes() {
            // Act
            string[] indexes = CollectionModelBinderUtil.GetZeroBasedIndexes().Take(5).ToArray();

            // Assert
            CollectionAssert.AreEqual(new string[] { "0", "1", "2", "3", "4" }, indexes);
        }

        private class ReadOnlyDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>> {
            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
                get {
                    return true;
                }
            }
        }

    }
}
