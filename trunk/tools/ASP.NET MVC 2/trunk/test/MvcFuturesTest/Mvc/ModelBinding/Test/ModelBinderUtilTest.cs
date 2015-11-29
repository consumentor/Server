namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelBinderUtilTest {

        [TestMethod]
        public void CastOrDefault_CorrectType_ReturnsInput() {
            // Act
            int retVal = ModelBinderUtil.CastOrDefault<int>(42);

            // Assert
            Assert.AreEqual(42, retVal);
        }

        [TestMethod]
        public void CastOrDefault_IncorrectType_ReturnsDefaultTModel() {
            // Act
            DateTime retVal = ModelBinderUtil.CastOrDefault<DateTime>(42);

            // Assert
            Assert.AreEqual(default(DateTime), retVal);
        }

        [TestMethod]
        public void CreateIndexModelName_EmptyParentName() {
            // Act
            string fullChildName = ModelBinderUtil.CreateIndexModelName("", 42);

            // Assert
            Assert.AreEqual("[42]", fullChildName);
        }

        [TestMethod]
        public void CreateIndexModelName_IntIndex() {
            // Act
            string fullChildName = ModelBinderUtil.CreateIndexModelName("parentName", 42);

            // Assert
            Assert.AreEqual("parentName[42]", fullChildName);
        }

        [TestMethod]
        public void CreateIndexModelName_StringIndex() {
            // Act
            string fullChildName = ModelBinderUtil.CreateIndexModelName("parentName", "index");

            // Assert
            Assert.AreEqual("parentName[index]", fullChildName);
        }

        [TestMethod]
        public void CreatePropertyModelName() {
            // Act
            string fullChildName = ModelBinderUtil.CreatePropertyModelName("parentName", "childName");

            // Assert
            Assert.AreEqual("parentName.childName", fullChildName);
        }

        [TestMethod]
        public void CreatePropertyModelName_EmptyParentName() {
            // Act
            string fullChildName = ModelBinderUtil.CreatePropertyModelName("", "childName");

            // Assert
            Assert.AreEqual("childName", fullChildName);
        }

        [TestMethod]
        public void GetPossibleBinderInstance_Match_ReturnsBinder() {
            // Act
            IExtensibleModelBinder binder = ModelBinderUtil.GetPossibleBinderInstance(typeof(List<int>), typeof(List<>), typeof(SampleGenericBinder<>));

            // Assert
            Assert.IsInstanceOfType(binder, typeof(SampleGenericBinder<int>));
        }

        [TestMethod]
        public void GetPossibleBinderInstance_NoMatch_ReturnsNull() {
            // Act
            IExtensibleModelBinder binder = ModelBinderUtil.GetPossibleBinderInstance(typeof(ArraySegment<int>), typeof(List<>), typeof(SampleGenericBinder<>));

            // Assert
            Assert.IsNull(binder);
        }

        [TestMethod]
        public void RawValueToObjectArray_RawValueIsEnumerable_ReturnsInputAsArray() {
            // Assert
            List<int> original = new List<int>() { 1, 2, 3, 4 };

            // Act
            object[] retVal = ModelBinderUtil.RawValueToObjectArray(original);

            // Assert
            CollectionAssert.AreEqual(new object[] { 1, 2, 3, 4 }, retVal);
        }

        [TestMethod]
        public void RawValueToObjectArray_RawValueIsObject_WrapsObjectInSingleElementArray() {
            // Act
            object[] retVal = ModelBinderUtil.RawValueToObjectArray(42);

            // Assert
            CollectionAssert.AreEqual(new object[] { 42 }, retVal);
        }

        [TestMethod]
        public void RawValueToObjectArray_RawValueIsObjectArray_ReturnsInputInstance() {
            // Assert
            object[] original = new object[2];

            // Act
            object[] retVal = ModelBinderUtil.RawValueToObjectArray(original);

            // Assert
            Assert.AreSame(original, retVal);
        }

        [TestMethod]
        public void RawValueToObjectArray_RawValueIsString_WrapsStringInSingleElementArray() {
            // Act
            object[] retVal = ModelBinderUtil.RawValueToObjectArray("hello");

            // Assert
            CollectionAssert.AreEqual(new string[] { "hello" }, retVal);
        }

        [TestMethod]
        public void ReplaceEmptyStringWithNull_ConvertEmptyStringToNullDisabled_ModelIsEmptyString_LeavesModelAlone() {
            // Arrange
            ModelMetadata modelMetadata = GetMetadata(typeof(string));
            modelMetadata.ConvertEmptyStringToNull = false;

            // Act
            object model = "";
            ModelBinderUtil.ReplaceEmptyStringWithNull(modelMetadata, ref model);

            // Assert
            Assert.AreEqual("", model);
        }

        [TestMethod]
        public void ReplaceEmptyStringWithNull_ConvertEmptyStringToNullEnabled_ModelIsEmptyString_ReplacesModelWithNull() {
            // Arrange
            ModelMetadata modelMetadata = GetMetadata(typeof(string));
            modelMetadata.ConvertEmptyStringToNull = true;

            // Act
            object model = "";
            ModelBinderUtil.ReplaceEmptyStringWithNull(modelMetadata, ref model);

            // Assert
            Assert.IsNull(model);
        }

        [TestMethod]
        public void ReplaceEmptyStringWithNull_ConvertEmptyStringToNullEnabled_ModelIsWhitespaceString_ReplacesModelWithNull() {
            // Arrange
            ModelMetadata modelMetadata = GetMetadata(typeof(string));
            modelMetadata.ConvertEmptyStringToNull = true;

            // Act
            object model = "     "; // whitespace
            ModelBinderUtil.ReplaceEmptyStringWithNull(modelMetadata, ref model);

            // Assert
            Assert.IsNull(model);
        }

        [TestMethod]
        public void ReplaceEmptyStringWithNull_ConvertEmptyStringToNullDisabled_ModelIsNotEmptyString_LeavesModelAlone() {
            // Arrange
            ModelMetadata modelMetadata = GetMetadata(typeof(string));
            modelMetadata.ConvertEmptyStringToNull = true;

            // Act
            object model = 42;
            ModelBinderUtil.ReplaceEmptyStringWithNull(modelMetadata, ref model);

            // Assert
            Assert.AreEqual(42, model);
        }

        [TestMethod]
        public void ValidateBindingContext_SuccessWithNonNullModel() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = GetMetadata(typeof(string))
            };
            bindingContext.ModelMetadata.Model = "hello!";

            // Act
            ModelBinderUtil.ValidateBindingContext(bindingContext, typeof(string), false);

            // Assert
            // Nothing to do - if we got this far without throwing, the test succeeded
        }

        [TestMethod]
        public void ValidateBindingContext_SuccessWithNullModel() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = GetMetadata(typeof(string))
            };

            // Act
            ModelBinderUtil.ValidateBindingContext(bindingContext, typeof(string), true);

            // Assert
            // Nothing to do - if we got this far without throwing, the test succeeded
        }

        [TestMethod]
        public void ValidateBindingContextThrowsIfBindingContextIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ModelBinderUtil.ValidateBindingContext(null, typeof(string), true);
                }, "bindingContext");
        }

        [TestMethod]
        public void ValidateBindingContextThrowsIfModelInstanceIsWrongType() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = GetMetadata(typeof(string))
            };
            bindingContext.ModelMetadata.Model = 42;

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    ModelBinderUtil.ValidateBindingContext(bindingContext, typeof(string), true);
                },
                @"The binding context has a Model of type 'System.Int32', but this binder can only operate on models of type 'System.String'.
Parameter name: bindingContext");
        }

        [TestMethod]
        public void ValidateBindingContextThrowsIfModelIsNullButCannotBe() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = GetMetadata(typeof(string))
            };

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    ModelBinderUtil.ValidateBindingContext(bindingContext, typeof(string), false);
                },
                @"The binding context has a null Model, but this binder requires a non-null model of type 'System.String'.
Parameter name: bindingContext");
        }

        [TestMethod]
        public void ValidateBindingContextThrowsIfModelMetadataIsNull() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext();

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    ModelBinderUtil.ValidateBindingContext(bindingContext, typeof(string), true);
                },
                @"The binding context cannot have a null ModelMetadata.
Parameter name: bindingContext");
        }

        [TestMethod]
        public void ValidateBindingContextThrowsIfModelTypeIsWrong() {
            // Arrange
            ExtensibleModelBindingContext bindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = GetMetadata(typeof(object))
            };

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    ModelBinderUtil.ValidateBindingContext(bindingContext, typeof(string), true);
                },
                @"The binding context has a ModelType of 'System.Object', but this binder can only operate on models of type 'System.String'.
Parameter name: bindingContext");
        }

        private static ModelMetadata GetMetadata(Type modelType) {
            EmptyModelMetadataProvider provider = new EmptyModelMetadataProvider();
            return provider.GetMetadataForType(null, modelType);
        }

        private class SampleGenericBinder<T> : IExtensibleModelBinder {
            public bool BindModel(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext) {
                throw new NotImplementedException();
            }
        }

    }
}
