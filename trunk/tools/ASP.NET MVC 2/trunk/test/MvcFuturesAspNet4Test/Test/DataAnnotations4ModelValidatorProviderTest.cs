namespace Microsoft.Web.Mvc.AspNet4.Test {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class DataAnnotations4ModelValidatorProviderTest {

        // Validation attribute adapter registration

        class MyValidationAttribute : ValidationAttribute {
            public override bool IsValid(object value) {
                throw new NotImplementedException();
            }
        }

        class MyValidationAttributeAdapter : DataAnnotations4ModelValidator<MyValidationAttribute> {
            public MyValidationAttributeAdapter(ModelMetadata metadata, ControllerContext context, MyValidationAttribute attribute)
                : base(metadata, context, attribute) { }
        }

        class MyValidationAttributeAdapterBadCtor : ModelValidator {
            public MyValidationAttributeAdapterBadCtor(ModelMetadata metadata, ControllerContext context)
                : base(metadata, context) { }

            public override IEnumerable<ModelValidationResult> Validate(object container) {
                throw new NotImplementedException();
            }
        }

        class MyDefaultValidationAttributeAdapter : DataAnnotations4ModelValidator {
            public MyDefaultValidationAttributeAdapter(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute)
                : base(metadata, context, attribute) { }
        }

        [MyValidation]
        class MyValidatedClass { }

        [TestMethod]
        public void RegisterAdapter() {
            var oldFactories = DataAnnotations4ModelValidatorProvider.AttributeFactories;

            try {
                // Arrange
                DataAnnotations4ModelValidatorProvider.AttributeFactories = new Dictionary<Type, DataAnnotationsModelValidationFactory>();

                // Act
                DataAnnotations4ModelValidatorProvider.RegisterAdapter(typeof(MyValidationAttribute), typeof(MyValidationAttributeAdapter));

                // Assert
                var type = DataAnnotations4ModelValidatorProvider.AttributeFactories.Keys.Single();
                Assert.AreEqual(typeof(MyValidationAttribute), type);

                var factory = DataAnnotations4ModelValidatorProvider.AttributeFactories.Values.Single();
                var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(object));
                var context = new ControllerContext();
                var attribute = new MyValidationAttribute();
                var validator = factory(metadata, context, attribute);
                Assert.AreEqual(typeof(MyValidationAttributeAdapter), validator.GetType());
            }
            finally {
                DataAnnotations4ModelValidatorProvider.AttributeFactories = oldFactories;
            }
        }

        [TestMethod]
        public void RegisterAdapterGuardClauses() {
            // Attribute type cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterAdapter(null, typeof(MyValidationAttributeAdapter)),
                "attributeType");

            // Adapter type cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterAdapter(typeof(MyValidationAttribute), null),
                "adapterType");

            // Validation attribute must derive from ValidationAttribute
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterAdapter(typeof(object), typeof(MyValidationAttributeAdapter)),
                "The type System.Object must derive from System.ComponentModel.DataAnnotations.ValidationAttribute.\r\nParameter name: attributeType");

            // Adapter must derive from ModelValidator
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterAdapter(typeof(MyValidationAttribute), typeof(object)),
                "The type System.Object must derive from System.Web.Mvc.ModelValidator.\r\nParameter name: adapterType");

            // Adapter must have the expected constructor
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterAdapter(typeof(MyValidationAttribute), typeof(MyValidationAttributeAdapterBadCtor)),
                "The type Microsoft.Web.Mvc.AspNet4.Test.DataAnnotations4ModelValidatorProviderTest+MyValidationAttributeAdapterBadCtor must have a public constructor which accepts three parameters of types System.Web.Mvc.ModelMetadata, System.Web.Mvc.ControllerContext, and Microsoft.Web.Mvc.AspNet4.Test.DataAnnotations4ModelValidatorProviderTest+MyValidationAttribute.\r\nParameter name: adapterType");
        }

        [TestMethod]
        public void RegisterAdapterFactory() {
            var oldFactories = DataAnnotations4ModelValidatorProvider.AttributeFactories;

            try {
                // Arrange
                DataAnnotations4ModelValidatorProvider.AttributeFactories = new Dictionary<Type, DataAnnotationsModelValidationFactory>();
                DataAnnotationsModelValidationFactory factory = delegate { return null; };

                // Act
                DataAnnotations4ModelValidatorProvider.RegisterAdapterFactory(typeof(MyValidationAttribute), factory);

                // Assert
                var type = DataAnnotations4ModelValidatorProvider.AttributeFactories.Keys.Single();
                Assert.AreEqual(typeof(MyValidationAttribute), type);
                Assert.AreSame(factory, DataAnnotations4ModelValidatorProvider.AttributeFactories.Values.Single());
            }
            finally {
                DataAnnotations4ModelValidatorProvider.AttributeFactories = oldFactories;
            }
        }

        [TestMethod]
        public void RegisterAdapterFactoryGuardClauses() {
            DataAnnotationsModelValidationFactory factory = (metadata, context, attribute) => null;

            // Attribute type cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterAdapterFactory(null, factory),
                "attributeType");

            // Factory cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterAdapterFactory(typeof(MyValidationAttribute), null),
                "factory");

            // Validation attribute must derive from ValidationAttribute
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterAdapterFactory(typeof(object), factory),
                "The type System.Object must derive from System.ComponentModel.DataAnnotations.ValidationAttribute.\r\nParameter name: attributeType");
        }

        [TestMethod]
        public void RegisterDefaultAdapter() {
            var oldFactory = DataAnnotations4ModelValidatorProvider.DefaultAttributeFactory;

            try {
                // Arrange
                var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(MyValidatedClass));
                var context = new ControllerContext();
                DataAnnotations4ModelValidatorProvider.RegisterDefaultAdapter(typeof(MyDefaultValidationAttributeAdapter));

                // Act
                var result = new DataAnnotations4ModelValidatorProvider().GetValidators(metadata, context).Single();

                // Assert
                Assert.AreEqual(typeof(MyDefaultValidationAttributeAdapter), result.GetType());
            }
            finally {
                DataAnnotations4ModelValidatorProvider.DefaultAttributeFactory = oldFactory;
            }
        }

        [TestMethod]
        public void RegisterDefaultAdapterGuardClauses() {
            // Adapter type cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterDefaultAdapter(null),
                "adapterType");

            // Adapter must derive from ModelValidator
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterDefaultAdapter(typeof(object)),
                "The type System.Object must derive from System.Web.Mvc.ModelValidator.\r\nParameter name: adapterType");

            // Adapter must have the expected constructor
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterDefaultAdapter(typeof(MyValidationAttributeAdapterBadCtor)),
                "The type Microsoft.Web.Mvc.AspNet4.Test.DataAnnotations4ModelValidatorProviderTest+MyValidationAttributeAdapterBadCtor must have a public constructor which accepts three parameters of types System.Web.Mvc.ModelMetadata, System.Web.Mvc.ControllerContext, and System.ComponentModel.DataAnnotations.ValidationAttribute.\r\nParameter name: adapterType");
        }

        [TestMethod]
        public void RegisterDefaultAdapterFactory() {
            var oldFactory = DataAnnotations4ModelValidatorProvider.DefaultAttributeFactory;

            try {
                // Arrange
                var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(MyValidatedClass));
                var context = new ControllerContext();
                ModelValidator validator = new Mock<ModelValidator>(metadata, context).Object;
                DataAnnotationsModelValidationFactory factory = delegate { return validator; };
                DataAnnotations4ModelValidatorProvider.RegisterDefaultAdapterFactory(factory);

                // Act
                var result = new DataAnnotations4ModelValidatorProvider().GetValidators(metadata, context).Single();

                // Assert
                Assert.AreSame(validator, result);
            }
            finally {
                DataAnnotations4ModelValidatorProvider.DefaultAttributeFactory = oldFactory;
            }
        }

        [TestMethod]
        public void RegisterDefaultAdapterFactoryGuardClauses() {
            // Factory cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterDefaultAdapterFactory(null),
                "factory");
        }

        // IValidatableObject adapter registration

        class MyValidatableAdapter : ModelValidator {
            public MyValidatableAdapter(ModelMetadata metadata, ControllerContext context)
                : base(metadata, context) { }

            public override IEnumerable<ModelValidationResult> Validate(object container) {
                throw new NotImplementedException();
            }
        }

        class MyValidatableAdapterBadCtor : ModelValidator {
            public MyValidatableAdapterBadCtor(ModelMetadata metadata, ControllerContext context, int unused)
                : base(metadata, context) { }

            public override IEnumerable<ModelValidationResult> Validate(object container) {
                throw new NotImplementedException();
            }
        }

        class MyValidatableClass : IValidatableObject {
            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void RegisterValidatableObjectAdapter() {
            var oldFactories = DataAnnotations4ModelValidatorProvider.ValidatableFactories;

            try {
                // Arrange
                DataAnnotations4ModelValidatorProvider.ValidatableFactories = new Dictionary<Type, DataAnnotations4ValidatableObjectAdapterFactory>();
                IValidatableObject validatable = new Mock<IValidatableObject>().Object;

                // Act
                DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapter(validatable.GetType(), typeof(MyValidatableAdapter));

                // Assert
                var type = DataAnnotations4ModelValidatorProvider.ValidatableFactories.Keys.Single();
                Assert.AreEqual(validatable.GetType(), type);

                var factory = DataAnnotations4ModelValidatorProvider.ValidatableFactories.Values.Single();
                var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(object));
                var context = new ControllerContext();
                var validator = factory(metadata, context);
                Assert.AreEqual(typeof(MyValidatableAdapter), validator.GetType());
            }
            finally {
                DataAnnotations4ModelValidatorProvider.ValidatableFactories = oldFactories;
            }
        }

        [TestMethod]
        public void RegisterValidatableObjectAdapterGuardClauses() {
            // Attribute type cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapter(null, typeof(MyValidatableAdapter)),
                "modelType");

            // Adapter type cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapter(typeof(MyValidatableClass), null),
                "adapterType");

            // Validation attribute must derive from ValidationAttribute
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapter(typeof(object), typeof(MyValidatableAdapter)),
                "The type System.Object must derive from System.ComponentModel.DataAnnotations.IValidatableObject.\r\nParameter name: modelType");

            // Adapter must derive from ModelValidator
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapter(typeof(MyValidatableClass), typeof(object)),
                "The type System.Object must derive from System.Web.Mvc.ModelValidator.\r\nParameter name: adapterType");

            // Adapter must have the expected constructor
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapter(typeof(MyValidatableClass), typeof(MyValidatableAdapterBadCtor)),
                "The type Microsoft.Web.Mvc.AspNet4.Test.DataAnnotations4ModelValidatorProviderTest+MyValidatableAdapterBadCtor must have a public constructor which accepts two parameters of types System.Web.Mvc.ModelMetadata and System.Web.Mvc.ControllerContext.\r\nParameter name: adapterType");
        }

        [TestMethod]
        public void RegisterValidatableObjectAdapterFactory() {
            var oldFactories = DataAnnotations4ModelValidatorProvider.ValidatableFactories;

            try {
                // Arrange
                DataAnnotations4ModelValidatorProvider.ValidatableFactories = new Dictionary<Type, DataAnnotations4ValidatableObjectAdapterFactory>();
                DataAnnotations4ValidatableObjectAdapterFactory factory = delegate { return null; };

                // Act
                DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapterFactory(typeof(MyValidatableClass), factory);

                // Assert
                var type = DataAnnotations4ModelValidatorProvider.ValidatableFactories.Keys.Single();
                Assert.AreEqual(typeof(MyValidatableClass), type);
                Assert.AreSame(factory, DataAnnotations4ModelValidatorProvider.ValidatableFactories.Values.Single());
            }
            finally {
                DataAnnotations4ModelValidatorProvider.ValidatableFactories = oldFactories;
            }
        }

        [TestMethod]
        public void RegisterValidatableObjectAdapterFactoryGuardClauses() {
            DataAnnotations4ValidatableObjectAdapterFactory factory = (metadata, context) => null;

            // Attribute type cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapterFactory(null, factory),
                "modelType");

            // Factory cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapterFactory(typeof(MyValidatableClass), null),
                "factory");

            // Validation attribute must derive from ValidationAttribute
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterValidatableObjectAdapterFactory(typeof(object), factory),
                "The type System.Object must derive from System.ComponentModel.DataAnnotations.IValidatableObject.\r\nParameter name: modelType");
        }

        [TestMethod]
        public void RegisterDefaultValidatableObjectAdapter() {
            var oldFactory = DataAnnotations4ModelValidatorProvider.DefaultValidatableFactory;

            try {
                // Arrange
                var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(MyValidatableClass));
                var context = new ControllerContext();
                DataAnnotations4ModelValidatorProvider.RegisterDefaultValidatableObjectAdapter(typeof(MyValidatableAdapter));

                // Act
                var result = new DataAnnotations4ModelValidatorProvider().GetValidators(metadata, context).Single();

                // Assert
                Assert.AreEqual(typeof(MyValidatableAdapter), result.GetType());
            }
            finally {
                DataAnnotations4ModelValidatorProvider.DefaultValidatableFactory = oldFactory;
            }
        }

        [TestMethod]
        public void RegisterDefaultValidatableObjectAdapterGuardClauses() {
            // Adapter type cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterDefaultValidatableObjectAdapter(null),
                "adapterType");

            // Adapter must derive from ModelValidator
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterDefaultValidatableObjectAdapter(typeof(object)),
                "The type System.Object must derive from System.Web.Mvc.ModelValidator.\r\nParameter name: adapterType");

            // Adapter must have the expected constructor
            ExceptionHelper.ExpectArgumentException(
                () => DataAnnotations4ModelValidatorProvider.RegisterDefaultValidatableObjectAdapter(typeof(MyValidatableAdapterBadCtor)),
                "The type Microsoft.Web.Mvc.AspNet4.Test.DataAnnotations4ModelValidatorProviderTest+MyValidatableAdapterBadCtor must have a public constructor which accepts two parameters of types System.Web.Mvc.ModelMetadata and System.Web.Mvc.ControllerContext.\r\nParameter name: adapterType");
        }

        [TestMethod]
        public void RegisterDefaultValidatableObjectAdapterFactory() {
            var oldFactory = DataAnnotations4ModelValidatorProvider.DefaultValidatableFactory;

            try {
                // Arrange
                var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(MyValidatableClass));
                var context = new ControllerContext();
                ModelValidator validator = new Mock<ModelValidator>(metadata, context).Object;
                DataAnnotations4ValidatableObjectAdapterFactory factory = delegate { return validator; };
                DataAnnotations4ModelValidatorProvider.RegisterDefaultValidatableObjectAdapterFactory(factory);

                // Act
                var result = new DataAnnotations4ModelValidatorProvider().GetValidators(metadata, context).Single();

                // Assert
                Assert.AreSame(validator, result);
            }
            finally {
                DataAnnotations4ModelValidatorProvider.DefaultValidatableFactory = oldFactory;
            }
        }

        [TestMethod]
        public void RegisterDefaultValidatableObjectAdapterFactoryGuardClauses() {
            // Factory cannot be null
            ExceptionHelper.ExpectArgumentNullException(
                () => DataAnnotations4ModelValidatorProvider.RegisterDefaultValidatableObjectAdapterFactory(null),
                "factory");
        }

        // Pre-configured adapters

        [TestMethod]
        public void AdapterForRangeAttributeRegistered() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(object));
            var context = new ControllerContext();
            var adapters = DataAnnotations4ModelValidatorProvider.AttributeFactories;
            var adapterFactory = adapters.Single(kvp => kvp.Key == typeof(RangeAttribute)).Value;

            // Act
            var adapter = adapterFactory(metadata, context, new RangeAttribute(1, 100));

            // Assert
            Assert.IsInstanceOfType(adapter, typeof(RangeAttribute4Adapter));
        }

        [TestMethod]
        public void AdapterForRegularExpressionAttributeRegistered() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(object));
            var context = new ControllerContext();
            var adapters = DataAnnotations4ModelValidatorProvider.AttributeFactories;
            var adapterFactory = adapters.Single(kvp => kvp.Key == typeof(RegularExpressionAttribute)).Value;

            // Act
            var adapter = adapterFactory(metadata, context, new RegularExpressionAttribute("abc"));

            // Assert
            Assert.IsInstanceOfType(adapter, typeof(RegularExpressionAttribute4Adapter));
        }

        [TestMethod]
        public void AdapterForRequiredAttributeRegistered() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(object));
            var context = new ControllerContext();
            var adapters = DataAnnotations4ModelValidatorProvider.AttributeFactories;
            var adapterFactory = adapters.Single(kvp => kvp.Key == typeof(RequiredAttribute)).Value;

            // Act
            var adapter = adapterFactory(metadata, context, new RequiredAttribute());

            // Assert
            Assert.IsInstanceOfType(adapter, typeof(RequiredAttribute4Adapter));
        }

        [TestMethod]
        public void AdapterForStringLengthAttributeRegistered() {
            // Arrange
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(object));
            var context = new ControllerContext();
            var adapters = DataAnnotations4ModelValidatorProvider.AttributeFactories;
            var adapterFactory = adapters.Single(kvp => kvp.Key == typeof(StringLengthAttribute)).Value;

            // Act
            var adapter = adapterFactory(metadata, context, new StringLengthAttribute(6));

            // Assert
            Assert.IsInstanceOfType(adapter, typeof(StringLengthAttribute4Adapter));
        }

        // Default adapter factory for unknown attribute type

        [TestMethod]
        public void UnknownValidationAttributeGetsDefaultAdapter() {
            // Arrange
            var provider = new DataAnnotations4ModelValidatorProvider();
            var context = new ControllerContext();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(DummyClassWithDummyValidationAttribute));

            // Act
            IEnumerable<ModelValidator> validators = provider.GetValidators(metadata, context);

            // Assert
            var validator = validators.Single();
            Assert.AreEqual(typeof(DataAnnotations4ModelValidator), validator.GetType());
        }

        private class DummyValidationAttribute : ValidationAttribute { }

        [DummyValidation]
        private class DummyClassWithDummyValidationAttribute { }

        // Default IValidatableObject adapter factory

        [TestMethod]
        public void IValidatableObjectGetsAValidator() {
            // Arrange
            var provider = new DataAnnotations4ModelValidatorProvider();
            var mockValidatable = new Mock<IValidatableObject>();
            var context = new ControllerContext();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, mockValidatable.Object.GetType());

            // Act
            IEnumerable<ModelValidator> validators = provider.GetValidators(metadata, context);

            // Assert
            Assert.AreEqual(1, validators.Count());
        }

        // Implicit [Required] attribute

        [TestMethod]
        public void ReferenceTypesDontGetImplicitRequiredAttribute() {
            // Arrange
            var provider = new DataAnnotations4ModelValidatorProvider();
            var context = new ControllerContext();
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, typeof(string));

            // Act
            IEnumerable<ModelValidator> validators = provider.GetValidators(metadata, context);

            // Assert
            Assert.AreEqual(0, validators.Count());
        }

        [TestMethod]
        public void NonNullableValueTypesGetImplicitRequiredAttribute() {
            // Arrange
            var provider = new DataAnnotations4ModelValidatorProvider();
            var context = new ControllerContext();
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(DummyRequiredAttributeHelperClass), "WithoutAttribute");

            // Act
            IEnumerable<ModelValidator> validators = provider.GetValidators(metadata, context);

            // Assert
            ModelValidator validator = validators.Single();
            ModelClientValidationRule rule = validator.GetClientValidationRules().Single();
            Assert.AreEqual(typeof(ModelClientValidationRequiredRule), rule.GetType());
        }

        [TestMethod]
        public void NonNullableValueTypesWithExplicitRequiredAttributeDoesntGetImplictRequiredAttribute() {
            // Arrange
            var provider = new DataAnnotations4ModelValidatorProvider();
            var context = new ControllerContext();
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(DummyRequiredAttributeHelperClass), "WithAttribute");

            // Act
            IEnumerable<ModelValidator> validators = provider.GetValidators(metadata, context);

            // Assert
            ModelValidator validator = validators.Single();
            ModelClientValidationRule rule = validator.GetClientValidationRules().Single();
            Assert.AreEqual(typeof(ModelClientValidationRequiredRule), rule.GetType());
            Assert.AreEqual("Custom Required Message", rule.ErrorMessage);
        }

        [TestMethod]
        public void NonNullableValueTypeDoesntGetImplicitRequiredAttributeWhenFlagIsOff() {
            // Arrange
            var provider = new DataAnnotations4ModelValidatorProvider { AddImpliedRequiredAttributes = false };
            var context = new ControllerContext();
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => null, typeof(DummyRequiredAttributeHelperClass), "WithoutAttribute");

            // Act
            IEnumerable<ModelValidator> validators = provider.GetValidators(metadata, context);

            // Assert
            Assert.AreEqual(0, validators.Count());
        }

        private class DummyRequiredAttributeHelperClass {
            [Required(ErrorMessage = "Custom Required Message")]
            public int WithAttribute { get; set; }

            public int WithoutAttribute { get; set; }
        }

        // Registration helper

        [TestMethod]
        public void RegisterProviderRemovesExistingDA35ProviderAndRegistersItself() {
            var oldProviders = ModelValidatorProviders.Providers.ToList();
            var oldProvider = oldProviders.Single(p => p is DataAnnotationsModelValidatorProvider);
            int oldProviderIndex = oldProviders.IndexOf(oldProvider);

            try {
                // Act
                DataAnnotations4ModelValidatorProvider.RegisterProvider();

                // Assert
                Assert.AreEqual(oldProviders.Count, ModelValidatorProviders.Providers.Count);
                Assert.IsTrue(ModelValidatorProviders.Providers[oldProviderIndex] is DataAnnotations4ModelValidatorProvider);
            }
            finally {
                ModelValidatorProviders.Providers.Clear();

                foreach (ModelValidatorProvider p in oldProviders) {
                    ModelValidatorProviders.Providers.Add(p);
                }
            }
        }

        [TestMethod]
        public void RegisterProviderPutsNewProviderAtEndOfListWhenDA35ProviderNotPresent() {
            var oldProviders = ModelValidatorProviders.Providers.ToList();
            var oldProvider = oldProviders.Single(p => p is DataAnnotationsModelValidatorProvider);

            try {
                // Arrange
                ModelValidatorProviders.Providers.Remove(oldProvider);
                int oldCount = ModelValidatorProviders.Providers.Count;

                // Act
                DataAnnotations4ModelValidatorProvider.RegisterProvider();

                // Assert
                Assert.AreEqual(oldCount + 1, ModelValidatorProviders.Providers.Count);
                Assert.IsTrue(ModelValidatorProviders.Providers[oldCount] is DataAnnotations4ModelValidatorProvider);
            }
            finally {
                ModelValidatorProviders.Providers.Clear();

                foreach (ModelValidatorProvider p in oldProviders) {
                    ModelValidatorProviders.Providers.Add(p);
                }
            }
        }
    }
}
