namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class TypeConverterModelBinderTest {

        [TestMethod]
        public void BindModel_Error_FormatExceptionsTurnedIntoStringsInModelState() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(int));
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", "not an integer" }
            };

            TypeConverterModelBinder binder = new TypeConverterModelBinder();

            // Act
            bool retVal = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsFalse(retVal);
            Assert.AreEqual("The value 'not an integer' is not valid for Int32.", bindingContext.ModelState["theModelName"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void BindModel_Error_FormatExceptionsTurnedIntoStringsInModelState_ErrorNotAddedIfCallbackReturnsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(int));
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", "not an integer" }
            };

            TypeConverterModelBinder binder = new TypeConverterModelBinder();

            // Act
            ModelBinderErrorMessageProvider originalProvider = ModelBinderConfig.TypeConversionErrorMessageProvider;
            bool retVal;
            try {
                ModelBinderConfig.TypeConversionErrorMessageProvider = delegate { return null; };
                retVal = binder.BindModel(null, bindingContext);
            }
            finally {
                ModelBinderConfig.TypeConversionErrorMessageProvider = originalProvider;
            }

            // Assert
            Assert.IsFalse(retVal);
            Assert.IsNull(bindingContext.Model);
            Assert.IsTrue(bindingContext.ModelState.IsValid, "ModelState shouldn't be marked invalid if the callback returns null.");
        }

        [TestMethod]
        public void BindModel_Error_GeneralExceptionsSavedInModelState() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(Dummy));
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", "foo" }
            };

            TypeConverterModelBinder binder = new TypeConverterModelBinder();

            // Act
            bool retVal = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsFalse(retVal);
            Assert.IsNull(bindingContext.Model);
            Assert.AreEqual("The parameter conversion from type 'System.String' to type 'Microsoft.Web.Mvc.ModelBinding.Test.TypeConverterModelBinderTest+Dummy' failed. See the inner exception for more information.", bindingContext.ModelState["theModelName"].Errors[0].Exception.Message);
            Assert.AreEqual("From DummyTypeConverter: foo", bindingContext.ModelState["theModelName"].Errors[0].Exception.InnerException.Message);
        }

        [TestMethod]
        public void BindModel_NullValueProviderResult_ReturnsFalse() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(int));

            TypeConverterModelBinder binder = new TypeConverterModelBinder();

            // Act
            bool retVal = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsFalse(retVal, "BindModel should have returned null.");
            Assert.AreEqual(0, bindingContext.ModelState.Count, "ModelState shouldn't have been touched.");
        }

        [TestMethod]
        public void BindModel_ValidValueProviderResult_ConvertEmptyStringsToNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(string));
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", "" }
            };

            TypeConverterModelBinder binder = new TypeConverterModelBinder();

            // Act
            bool retVal = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsTrue(retVal);
            Assert.IsNull(bindingContext.Model);
            Assert.IsTrue(bindingContext.ModelState.ContainsKey("theModelName"), "ModelState should've been updated.");
        }

        [TestMethod]
        public void BindModel_ValidValueProviderResult_ReturnsModel() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = GetBindingContext(typeof(int));
            bindingContext.ValueProvider = new SimpleValueProvider() {
                { "theModelName", "42" }
            };

            TypeConverterModelBinder binder = new TypeConverterModelBinder();

            // Act
            bool retVal = binder.BindModel(null, bindingContext);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual(42, bindingContext.Model);
            Assert.IsTrue(bindingContext.ModelState.ContainsKey("theModelName"), "ModelState should've been updated.");
        }

        private static ExtensibleModelBindingContext GetBindingContext(Type modelType) {
            return new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, modelType),
                ModelName = "theModelName",
                ValueProvider = new SimpleValueProvider() // empty
            };
        }

        [TypeConverter(typeof(DummyTypeConverter))]
        private struct Dummy {
        }

        private sealed class DummyTypeConverter : TypeConverter {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
                return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
                throw new InvalidOperationException(String.Format("From DummyTypeConverter: {0}", value));
            }
        }

    }
}
