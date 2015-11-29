namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssociatedMetadataProviderTest {

        // FilterAttributes

        [TestMethod]
        public void ReadOnlyAttributeIsFilteredOffWhenContainerTypeIsViewPage() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act
            provider.GetMetadataForProperty(() => null, typeof(ViewPage<PropertyModel>), "Model");

            // Assert
            CreateMetadataParams parms = provider.CreateMetadataLog.Single();
            Assert.IsFalse(parms.Attributes.Any(a => a is ReadOnlyAttribute));
        }

        [TestMethod]
        public void ReadOnlyAttributeIsFilteredOffWhenContainerTypeIsViewUserControl() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act
            provider.GetMetadataForProperty(() => null, typeof(ViewUserControl<PropertyModel>), "Model");

            // Assert
            CreateMetadataParams parms = provider.CreateMetadataLog.Single();
            Assert.IsFalse(parms.Attributes.Any(a => a is ReadOnlyAttribute));
        }

        [TestMethod]
        public void ReadOnlyAttributeIsPreservedForReadOnlyModelProperties() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act
            provider.GetMetadataForProperty(() => null, typeof(ModelWithReadOnlyProperty), "ReadOnlyProperty");

            // Assert
            CreateMetadataParams parms = provider.CreateMetadataLog.Single();
            Assert.IsTrue(parms.Attributes.Any(a => a is ReadOnlyAttribute));
        }

        // GetMetadataForProperties

        [TestMethod]
        public void GetMetadataForPropertiesNullContainerTypeThrows() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => provider.GetMetadataForProperties(new Object(), null),
                "containerType");
        }

        [TestMethod]
        public void GetMetadataForPropertiesCreatesMetadataForAllPropertiesOnModelWithPropertyValues() {
            // Arrange
            PropertyModel model = new PropertyModel { LocalAttributes = 42, MetadataAttributes = "hello", MixedAttributes = 21.12 };
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act
            provider.GetMetadataForProperties(model, typeof(PropertyModel)).ToList();   // Call ToList() to force the lazy evaluation to evaluate

            // Assert
            CreateMetadataParams local =
                provider.CreateMetadataLog.Single(m => m.ContainerType == typeof(PropertyModel) &&
                                                       m.PropertyName == "LocalAttributes");
            Assert.AreEqual(typeof(int), local.ModelType);
            Assert.AreEqual(42, local.Model);
            Assert.IsTrue(local.Attributes.Any(a => a is RequiredAttribute));

            CreateMetadataParams metadata =
                provider.CreateMetadataLog.Single(m => m.ContainerType == typeof(PropertyModel) &&
                                                       m.PropertyName == "MetadataAttributes");
            Assert.AreEqual(typeof(string), metadata.ModelType);
            Assert.AreEqual("hello", metadata.Model);
            Assert.IsTrue(metadata.Attributes.Any(a => a is RangeAttribute));

            CreateMetadataParams mixed =
                provider.CreateMetadataLog.Single(m => m.ContainerType == typeof(PropertyModel) &&
                                                       m.PropertyName == "MixedAttributes");
            Assert.AreEqual(typeof(double), mixed.ModelType);
            Assert.AreEqual(21.12, mixed.Model);
            Assert.IsTrue(mixed.Attributes.Any(a => a is RequiredAttribute));
            Assert.IsTrue(mixed.Attributes.Any(a => a is RangeAttribute));
        }

        [TestMethod]
        public void GetMetadataForPropertyWithNullContainerReturnsMetadataWithNullValuesForProperties() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act
            provider.GetMetadataForProperties(null, typeof(PropertyModel)).ToList();   // Call ToList() to force the lazy evaluation to evaluate

            // Assert
            Assert.IsTrue(provider.CreateMetadataLog.Any());
            foreach (var parms in provider.CreateMetadataLog) {
                Assert.IsNull(parms.Model);
            }
        }

        // GetMetadataForProperty

        [TestMethod]
        public void GetMetadataForPropertyNullContainerTypeThrows() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => provider.GetMetadataForProperty(null /* model */, null /* containerType */, "propertyName"),
                "containerType");
        }

        [TestMethod]
        public void GetMetadataForPropertyNullOrEmptyPropertyNameThrows() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => provider.GetMetadataForProperty(null /* model */, typeof(object), null /* propertyName */),
                "propertyName");
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                () => provider.GetMetadataForProperty(null, typeof(object), String.Empty),
                "propertyName");
        }

        [TestMethod]
        public void GetMetadataForPropertyInvalidPropertyNameThrows() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => provider.GetMetadataForProperty(null, typeof(object), "BadPropertyName"),
                "The property System.Object.BadPropertyName could not be found.");
        }

        [TestMethod]
        public void GetMetadataForPropertyWithLocalAttributes() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();
            ModelMetadata metadata = new ModelMetadata(provider, typeof(PropertyModel), null, typeof(int), "LocalAttributes");
            provider.CreateMetadataReturnValue = metadata;

            // Act
            ModelMetadata result = provider.GetMetadataForProperty(null, typeof(PropertyModel), "LocalAttributes");

            // Assert
            Assert.AreSame(metadata, result);
            Assert.IsTrue(provider.CreateMetadataLog.Single().Attributes.Any(a => a is RequiredAttribute));
        }

        [TestMethod]
        public void GetMetadataForPropertyWithMetadataAttributes() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();
            ModelMetadata metadata = new ModelMetadata(provider, typeof(PropertyModel), null, typeof(string), "MetadataAttributes");
            provider.CreateMetadataReturnValue = metadata;

            // Act
            ModelMetadata result = provider.GetMetadataForProperty(null, typeof(PropertyModel), "MetadataAttributes");

            // Assert
            Assert.AreSame(metadata, result);
            CreateMetadataParams parms = provider.CreateMetadataLog.Single(p => p.PropertyName == "MetadataAttributes");
            Assert.IsTrue(parms.Attributes.Any(a => a is RangeAttribute));
        }

        [TestMethod]
        public void GetMetadataForPropertyWithMixedAttributes() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();
            ModelMetadata metadata = new ModelMetadata(provider, typeof(PropertyModel), null, typeof(double), "MixedAttributes");
            provider.CreateMetadataReturnValue = metadata;

            // Act
            ModelMetadata result = provider.GetMetadataForProperty(null, typeof(PropertyModel), "MixedAttributes");

            // Assert
            Assert.AreSame(metadata, result);
            CreateMetadataParams parms = provider.CreateMetadataLog.Single(p => p.PropertyName == "MixedAttributes");
            Assert.IsTrue(parms.Attributes.Any(a => a is RequiredAttribute));
            Assert.IsTrue(parms.Attributes.Any(a => a is RangeAttribute));
        }

        // GetMetadataForType

        [TestMethod]
        public void GetMetadataForTypeNullModelTypeThrows() {
            // Arrange
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => provider.GetMetadataForType(() => new Object(), null),
                "modelType");
        }

        [TestMethod]
        public void GetMetadataForTypeIncludesAttributesOnType() {
            TestableAssociatedMetadataProvider provider = new TestableAssociatedMetadataProvider();
            ModelMetadata metadata = new ModelMetadata(provider, null, null, typeof(TypeModel), null);
            provider.CreateMetadataReturnValue = metadata;

            // Act
            ModelMetadata result = provider.GetMetadataForType(null, typeof(TypeModel));

            // Assert
            Assert.AreSame(metadata, result);
            CreateMetadataParams parms = provider.CreateMetadataLog.Single(p => p.ModelType == typeof(TypeModel));
            Assert.IsTrue(parms.Attributes.Any(a => a is ReadOnlyAttribute));
        }

        // Helpers

        [MetadataType(typeof(PropertyModel.Metadata))]
        private class PropertyModel {
            [Required]
            public int LocalAttributes { get; set; }

            public string MetadataAttributes { get; set; }

            [Required]
            public double MixedAttributes { get; set; }

            private class Metadata {
                [Range(10, 100)]
                public object MetadataAttributes { get; set; }

                [Range(10, 100)]
                public object MixedAttributes { get; set; }
            }
        }

        private class ModelWithReadOnlyProperty {
            public int ReadOnlyProperty { get; private set; }
        }

        [ReadOnly(true)]
        private class TypeModel { }

        class TestableAssociatedMetadataProvider : AssociatedMetadataProvider {
            public List<CreateMetadataParams> CreateMetadataLog = new List<CreateMetadataParams>();
            public ModelMetadata CreateMetadataReturnValue = null;

            protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType,
                                                            Func<object> modelAccessor, Type modelType,
                                                            string propertyName) {
                CreateMetadataLog.Add(new CreateMetadataParams {
                    Attributes = attributes,
                    ContainerType = containerType,
                    Model = modelAccessor == null ? null : modelAccessor(),
                    ModelType = modelType,
                    PropertyName = propertyName
                });

                return CreateMetadataReturnValue;
            }
        }

        class CreateMetadataParams {
            public IEnumerable<Attribute> Attributes { get; set; }
            public Type ContainerType { get; set; }
            public object Model { get; set; }
            public Type ModelType { get; set; }
            public string PropertyName { get; set; }
        }
    }
}