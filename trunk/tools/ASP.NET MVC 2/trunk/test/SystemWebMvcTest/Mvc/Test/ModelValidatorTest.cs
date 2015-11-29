namespace System.Web.Mvc.Test {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelValidatorTest {

        [TestMethod]
        public void ConstructorGuards() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(object));
            ControllerContext context = new ControllerContext();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => new TestableModelValidator(null, context),
                "metadata");
            ExceptionHelper.ExpectArgumentNullException(
                () => new TestableModelValidator(metadata, null),
                "controllerContext");
        }

        [TestMethod]
        public void ValuesSet() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();

            // Act
            TestableModelValidator validator = new TestableModelValidator(metadata, context);

            // Assert
            Assert.AreSame(context, validator.ControllerContext);
            Assert.AreSame(metadata, validator.Metadata);
        }

        [TestMethod]
        public void NoClientRulesByDefault() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();

            // Act
            TestableModelValidator validator = new TestableModelValidator(metadata, context);

            // Assert
            Assert.IsFalse(validator.GetClientValidationRules().Any());
        }

        [TestMethod]
        public void IsRequiredFalseByDefault() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => 15, typeof(string), "Length");
            ControllerContext context = new ControllerContext();

            // Act
            TestableModelValidator validator = new TestableModelValidator(metadata, context);

            // Assert
            Assert.IsFalse(validator.IsRequired);
        }

        [TestMethod]
        public void GetModelValidator_DoesNotReadPropertyValues() {
            ModelValidatorProvider[] originalProviders = ModelValidatorProviders.Providers.ToArray();
            try {
                // Arrange
                ModelValidatorProviders.Providers.Clear();
                ModelValidatorProviders.Providers.Add(new ObservableModelValidatorProvider());

                ObservableModel model = new ObservableModel();
                ModelMetadata metadata = new EmptyModelMetadataProvider().GetMetadataForType(() => model, typeof(ObservableModel));
                ControllerContext controllerContext = new ControllerContext();

                // Act
                ModelValidator validator = ModelValidator.GetModelValidator(metadata, controllerContext);
                ModelValidationResult[] results = validator.Validate(model).ToArray();

                // Assert
                Assert.IsFalse(model.PropertyWasRead(), "Property should not have been read by provider or validator.");
            }
            finally {
                ModelValidatorProviders.Providers.Clear();
                foreach (ModelValidatorProvider provider in originalProviders) {
                    ModelValidatorProviders.Providers.Add(provider);
                }
            }
        }

        private class ObservableModelValidatorProvider : ModelValidatorProvider {
            public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context) {
                return new ModelValidator[] { new ObservableModelValidator(metadata, context) };
            }

            private class ObservableModelValidator : ModelValidator {
                public ObservableModelValidator(ModelMetadata metadata, ControllerContext controllerContext) : base(metadata, controllerContext) { }

                public override IEnumerable<ModelValidationResult> Validate(object container) {
                    return Enumerable.Empty<ModelValidationResult>();
                }
            }
        }

        private class ObservableModel {
            private bool _propertyWasRead;

            public int TheProperty {
                get {
                    _propertyWasRead = true;
                    return 42;
                }
            }

            public bool PropertyWasRead() {
                return _propertyWasRead;
            }
        }

        [TestMethod]
        public void GetModelValidatorWithTypeLevelValidator() {
            // Arrange
            ControllerContext context = new ControllerContext();
            DataErrorInfo1 model = new DataErrorInfo1 { Error = "Some Type Error" };
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            ModelValidator validator = ModelValidator.GetModelValidator(metadata, context);

            // Act
            ModelValidationResult result = validator.Validate(null).Single();

            // Assert
            Assert.AreEqual(String.Empty, result.MemberName);
            Assert.AreEqual("Some Type Error", result.Message);
        }

        [TestMethod]
        public void GetModelValidatorWithPropertyLevelValidator() {
            // Arrange
            ControllerContext context = new ControllerContext();
            DataErrorInfo1 model = new DataErrorInfo1();
            model["SomeStringProperty"] = "Some Property Error";
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            ModelValidator validator = ModelValidator.GetModelValidator(metadata, context);

            // Act
            ModelValidationResult result = validator.Validate(null).Single();

            // Assert
            Assert.AreEqual("SomeStringProperty", result.MemberName);
            Assert.AreEqual("Some Property Error", result.Message);
        }

        [TestMethod]
        public void GetModelValidatorWithFailedPropertyValidatorsPreventsTypeValidatorFromRunning() {
            // Arrange
            ControllerContext context = new ControllerContext();
            DataErrorInfo1 model = new DataErrorInfo1 { Error = "Some Type Error" };
            model["SomeStringProperty"] = "Some Property Error";
            model["SomeOtherStringProperty"] = "Some Other Property Error";
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            ModelValidator validator = ModelValidator.GetModelValidator(metadata, context);

            // Act
            List<ModelValidationResult> result = validator.Validate(null).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("SomeStringProperty", result[0].MemberName);
            Assert.AreEqual("Some Property Error", result[0].Message);
            Assert.AreEqual("SomeOtherStringProperty", result[1].MemberName);
            Assert.AreEqual("Some Other Property Error", result[1].Message);
        }

        private class TestableModelValidator : ModelValidator {
            public TestableModelValidator(ModelMetadata metadata, ControllerContext context)
                : base(metadata, context) {
            }

            public override IEnumerable<ModelValidationResult> Validate(object container) {
                throw new NotImplementedException();
            }
        }

        private class DataErrorInfo1 : IDataErrorInfo {
            private readonly Dictionary<string, string> _errors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            public string SomeStringProperty { get; set; }

            public string SomeOtherStringProperty { get; set; }

            public string Error { get; set; }

            public string this[string columnName] {
                get {
                    string outVal;
                    _errors.TryGetValue(columnName, out outVal);
                    return outVal;
                }
                set {
                    _errors[columnName] = value;
                }
            }
        }

    }
}
