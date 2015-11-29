namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataErrorInfoModelValidatorProviderTest {

        private static readonly EmptyModelMetadataProvider _metadataProvider = new EmptyModelMetadataProvider();

        [TestMethod]
        public void GetValidatorsReturnsEmptyCollectionIfTypeNotIDataErrorInfo() {
            // Arrange
            DataErrorInfoModelValidatorProvider validatorProvider = new DataErrorInfoModelValidatorProvider();
            object model = new object();
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(() => model, typeof(object));

            // Act
            ModelValidator[] validators = validatorProvider.GetValidators(metadata, new ControllerContext()).ToArray();

            // Assert
            Assert.AreEqual(0, validators.Length);
        }

        [TestMethod]
        public void GetValidatorsReturnsValidatorForIDataErrorInfoProperty() {
            // Arrange
            DataErrorInfoModelValidatorProvider validatorProvider = new DataErrorInfoModelValidatorProvider();
            DataErrorInfo1 model = new DataErrorInfo1();
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => model, typeof(DataErrorInfo1), "SomeStringProperty");
            Type[] expectedTypes = new Type[] {
                typeof(DataErrorInfoModelValidatorProvider.DataErrorInfoPropertyModelValidator)
            };

            // Act
            Type[] actualTypes = validatorProvider.GetValidators(metadata, new ControllerContext()).Select(v => v.GetType()).ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedTypes, actualTypes);
        }

        [TestMethod]
        public void GetValidatorsReturnsValidatorForIDataErrorInfoRootType() {
            // Arrange
            DataErrorInfoModelValidatorProvider validatorProvider = new DataErrorInfoModelValidatorProvider();
            DataErrorInfo1 model = new DataErrorInfo1();
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(() => model, typeof(DataErrorInfo1));
            Type[] expectedTypes = new Type[] {
                typeof(DataErrorInfoModelValidatorProvider.DataErrorInfoClassModelValidator)
            };

            // Act
            Type[] actualTypes = validatorProvider.GetValidators(metadata, new ControllerContext()).Select(v => v.GetType()).ToArray();

            // Assert
            CollectionAssert.AreEqual(expectedTypes, actualTypes);
        }

        [TestMethod]
        public void GetValidatorsThrowsIfContextIsNull() {
            // Arrange
            DataErrorInfoModelValidatorProvider validatorProvider = new DataErrorInfoModelValidatorProvider();
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(null, typeof(DataErrorInfo1));

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    validatorProvider.GetValidators(metadata, null);
                }, "context");
        }

        [TestMethod]
        public void GetValidatorsThrowsIfMetadataIsNull() {
            // Arrange
            DataErrorInfoModelValidatorProvider validatorProvider = new DataErrorInfoModelValidatorProvider();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    validatorProvider.GetValidators(null, new ControllerContext());
                }, "metadata");
        }

        [TestMethod]
        public void ClassValidator_Validate_IDataErrorInfoModelWithError() {
            // Arrange
            DataErrorInfo1 model = new DataErrorInfo1() {
                Error = "This is an error message."
            };
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(() => model, typeof(DataErrorInfo1));

            var validator = new DataErrorInfoModelValidatorProvider.DataErrorInfoClassModelValidator(metadata, new ControllerContext());

            // Act
            ModelValidationResult[] result = validator.Validate(null).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("This is an error message.", result[0].Message);
        }

        [TestMethod]
        public void ClassValidator_Validate_IDataErrorInfoModelWithNoErrorReturnsEmptyResults() {
            // Arrange
            DataErrorInfo1 model = new DataErrorInfo1();
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(() => model, typeof(DataErrorInfo1));

            var validator = new DataErrorInfoModelValidatorProvider.DataErrorInfoClassModelValidator(metadata, new ControllerContext());

            // Act
            ModelValidationResult[] result = validator.Validate(null).ToArray();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void ClassValidator_Validate_NonIDataErrorInfoModelReturnsEmptyResults() {
            // Arrange
            object model = new object();
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(() => model, typeof(object));

            var validator = new DataErrorInfoModelValidatorProvider.DataErrorInfoClassModelValidator(metadata, new ControllerContext());

            // Act
            ModelValidationResult[] result = validator.Validate(null).ToArray();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void PropertyValidator_Validate_IDataErrorInfoSkipsErrorProperty() {
            // Arrange
            DataErrorInfo1 container = new DataErrorInfo1();
            container["Error"] = "This should never be shown.";
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => container, typeof(DataErrorInfo1), "Error");

            var validator = new DataErrorInfoModelValidatorProvider.DataErrorInfoPropertyModelValidator(metadata, new ControllerContext());

            // Act
            ModelValidationResult[] result = validator.Validate(container).ToArray();

            // Assert
            Assert.AreEqual(0, result.Length, "The 'Error' property should not be considered during property validation.");
        }

        [TestMethod]
        public void PropertyValidator_Validate_DoesNotReadPropertyValue() {
            // Arrange
            ObservableModel model = new ObservableModel();
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => model.TheProperty, typeof(ObservableModel), "TheProperty");
            ControllerContext controllerContext = new ControllerContext();

            // Act
            ModelValidator[] validators = new DataErrorInfoModelValidatorProvider().GetValidators(metadata, controllerContext).ToArray();
            ModelValidationResult[] results = validators.SelectMany(o => o.Validate(model)).ToArray();

            // Assert
            CollectionAssert.AreEqual(new Type[] { typeof(DataErrorInfoModelValidatorProvider.DataErrorInfoPropertyModelValidator) }, Array.ConvertAll(validators, o => o.GetType()), "Provider did not return expected validator.");
            CollectionAssert.AreEquivalent(new string[] { "TheProperty" }, model.GetColumnNamesPassed(), "Model.get_Item() was not called as expected.");
            Assert.AreEqual(0, results.Length);
            Assert.IsFalse(model.PropertyWasRead(), "Property should not have been read by provider or validator.");
        }

        [TestMethod]
        public void PropertyValidator_Validate_IDataErrorInfoContainerWithError() {
            // Arrange
            DataErrorInfo1 container = new DataErrorInfo1();
            container["SomeStringProperty"] = "This is an error message.";
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => container, typeof(DataErrorInfo1), "SomeStringProperty");

            var validator = new DataErrorInfoModelValidatorProvider.DataErrorInfoPropertyModelValidator(metadata, new ControllerContext());

            // Act
            ModelValidationResult[] result = validator.Validate(container).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("This is an error message.", result[0].Message);
        }

        [TestMethod]
        public void PropertyValidator_Validate_IDataErrorInfoContainerWithNoErrorReturnsEmptyResults() {
            // Arrange
            DataErrorInfo1 container = new DataErrorInfo1();
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => container, typeof(DataErrorInfo1), "SomeStringProperty");

            var validator = new DataErrorInfoModelValidatorProvider.DataErrorInfoPropertyModelValidator(metadata, new ControllerContext());

            // Act
            ModelValidationResult[] result = validator.Validate(container).ToArray();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void PropertyValidator_Validate_NonIDataErrorInfoContainerReturnsEmptyResults() {
            // Arrange
            DataErrorInfo1 container = new DataErrorInfo1();
            container["SomeStringProperty"] = "This is an error message.";
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => container, typeof(DataErrorInfo1), "SomeStringProperty");

            var validator = new DataErrorInfoModelValidatorProvider.DataErrorInfoPropertyModelValidator(metadata, new ControllerContext());

            // Act
            ModelValidationResult[] result = validator.Validate(new object()).ToArray();

            // Assert
            Assert.AreEqual(0, result.Length);
        }

        private class DataErrorInfo1 : IDataErrorInfo {
            private readonly Dictionary<string, string> _errors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            public string SomeStringProperty { get; set; }

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

        private class ObservableModel : IDataErrorInfo {
            private bool _propertyWasRead;
            private readonly List<string> _columnNamesPassed = new List<string>();

            public int TheProperty {
                get {
                    _propertyWasRead = true;
                    return 42;
                }
            }

            public bool PropertyWasRead() {
                return _propertyWasRead;
            }

            public string Error {
                get { throw new NotImplementedException(); }
            }

            public string this[string columnName] {
                get {
                    _columnNamesPassed.Add(columnName);
                    return null;
                }
            }

            public List<string> GetColumnNamesPassed() {
                return _columnNamesPassed;
            }
        }

    }
}
