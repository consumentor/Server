namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientDataTypeModelValidatorProviderTest {

        private static readonly EmptyModelMetadataProvider _metadataProvider = new EmptyModelMetadataProvider();

        [TestMethod]
        public void GetValidators_ReturnsExpectedValidatorTypes() {
            // Arrange
            ClientDataTypeModelValidatorProvider validatorProvider = new ClientDataTypeModelValidatorProvider();

            Action<string, bool> checkPropertyIsNumeric = (propertyName, shouldBeNumeric) => {
                ModelValidator[] validators = validatorProvider.GetValidators(_metadataProvider.GetMetadataForProperty(null, typeof(SampleModel), propertyName), new ControllerContext()).ToArray();

                bool isNumeric = validators.OfType<ClientDataTypeModelValidatorProvider.NumericModelValidator>().Any();
                if (isNumeric && !shouldBeNumeric) {
                    Assert.Fail("Property '{0}' shouldn't have returned a numeric validator but did.", propertyName);
                }
                else if (!isNumeric && shouldBeNumeric) {
                    Assert.Fail("Property '{0}' should've returned a numeric validator but didn't.", propertyName);
                }
            };

            // Act & assert
            checkPropertyIsNumeric("Byte", true);
            checkPropertyIsNumeric("SByte", true);
            checkPropertyIsNumeric("Int16", true);
            checkPropertyIsNumeric("UInt16", true);
            checkPropertyIsNumeric("Int32", true);
            checkPropertyIsNumeric("UInt32", true);
            checkPropertyIsNumeric("Int64", true);
            checkPropertyIsNumeric("UInt64", true);
            checkPropertyIsNumeric("Single", true);
            checkPropertyIsNumeric("Double", true);
            checkPropertyIsNumeric("Decimal", true);
            checkPropertyIsNumeric("NullableInt32", true);
            checkPropertyIsNumeric("String", false);
            checkPropertyIsNumeric("Object", false);
        }

        [TestMethod]
        public void GetValidators_ThrowsIfContextIsNull() {
            // Arrange
            ClientDataTypeModelValidatorProvider validatorProvider = new ClientDataTypeModelValidatorProvider();
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(null, typeof(SampleModel));

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    validatorProvider.GetValidators(metadata, null);
                }, "context");
        }

        [TestMethod]
        public void GetValidators_ThrowsIfMetadataIsNull() {
            // Arrange
            ClientDataTypeModelValidatorProvider validatorProvider = new ClientDataTypeModelValidatorProvider();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    validatorProvider.GetValidators(null, new ControllerContext());
                }, "metadata");
        }

        [TestMethod]
        public void NumericValidator_GetClientValidationRules() {
            // Arrange
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(null, typeof(SampleModel), "Int32");
            var validator = new ClientDataTypeModelValidatorProvider.NumericModelValidator(metadata, new ControllerContext());

            // Act
            ModelClientValidationRule[] rules = validator.GetClientValidationRules().ToArray();

            // Assert
            Assert.AreEqual(1, rules.Length, "Wrong number of rules returned.");

            ModelClientValidationRule rule = rules[0];
            Assert.AreEqual("number", rule.ValidationType);
            Assert.AreEqual(0, rule.ValidationParameters.Count);
            Assert.AreEqual("The field Int32 must be a number.", rule.ErrorMessage);
        }

        [TestMethod]
        public void NumericValidator_Validate_DoesNotReadPropertyValue() {
            // Arrange
            ObservableModel model = new ObservableModel();
            ModelMetadata metadata = _metadataProvider.GetMetadataForProperty(() => model.TheProperty, typeof(ObservableModel), "TheProperty");
            ControllerContext controllerContext = new ControllerContext();

            // Act
            ModelValidator[] validators = new ClientDataTypeModelValidatorProvider().GetValidators(metadata, controllerContext).ToArray();
            ModelValidationResult[] results = validators.SelectMany(o => o.Validate(model)).ToArray();

            // Assert
            CollectionAssert.AreEqual(new Type[] { typeof(ClientDataTypeModelValidatorProvider.NumericModelValidator) }, Array.ConvertAll(validators, o => o.GetType()), "Provider did not return expected validator.");
            Assert.AreEqual(0, results.Length);
            Assert.IsFalse(model.PropertyWasRead(), "Property should not have been read by provider or validator.");
        }

        [TestMethod]
        public void NumericValidator_Validate_ReturnsEmptyCollection() {
            // Arrange
            ModelMetadata metadata = _metadataProvider.GetMetadataForType(null, typeof(object));
            var validator = new ClientDataTypeModelValidatorProvider.NumericModelValidator(metadata, new ControllerContext());

            // Act
            IEnumerable<ModelValidationResult> result = validator.Validate(null);

            // Assert
            Assert.AreEqual(0, result.Count(), "Shouldn't have been any errors.");
        }

        private class SampleModel {
            // these should have 'numeric' validators associated with them
            public byte Byte { get; set; }
            public sbyte SByte { get; set; }
            public short Int16 { get; set; }
            public ushort UInt16 { get; set; }
            public int Int32 { get; set; }
            public uint UInt32 { get; set; }
            public long Int64 { get; set; }
            public ulong UInt64 { get; set; }
            public float Single { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }

            // this should also have a 'numeric' validator
            public int? NullableInt32 { get; set; }

            // these shouldn't have any validators
            public string String { get; set; }
            public object Object { get; set; }
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

    }
}
