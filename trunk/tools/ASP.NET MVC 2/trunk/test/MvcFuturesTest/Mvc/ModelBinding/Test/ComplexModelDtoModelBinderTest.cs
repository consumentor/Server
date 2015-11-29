namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ComplexModelDtoModelBinderTest {

        [TestMethod]
        public void BindModel() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            MyModel model = new MyModel();
            ModelMetadata modelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => model, typeof(MyModel));
            ComplexModelDto dto = new ComplexModelDto(modelMetadata, modelMetadata.Properties);

            Mock<IExtensibleModelBinder> mockStringBinder = new Mock<IExtensibleModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        Assert.AreEqual(typeof(string), mbc.ModelType);
                        Assert.AreEqual("theModel.StringProperty", mbc.ModelName);
                        mbc.ValidationNode = new ModelValidationNode(mbc.ModelMetadata, "theModel.StringProperty");
                        mbc.Model = "someStringValue";
                        return true;
                    });

            Mock<IExtensibleModelBinder> mockIntBinder = new Mock<IExtensibleModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        Assert.AreEqual(typeof(int), mbc.ModelType);
                        Assert.AreEqual("theModel.IntProperty", mbc.ModelName);
                        mbc.ValidationNode = new ModelValidationNode(mbc.ModelMetadata, "theModel.IntProperty");
                        mbc.Model = 42;
                        return true;
                    });

            Mock<IExtensibleModelBinder> mockDateTimeBinder = new Mock<IExtensibleModelBinder>();
            mockDateTimeBinder
                .Expect(b => b.BindModel(controllerContext, It.IsAny<ExtensibleModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ExtensibleModelBindingContext mbc) {
                        Assert.AreEqual(typeof(DateTime), mbc.ModelType);
                        Assert.AreEqual("theModel.DateTimeProperty", mbc.ModelName);
                        return false;
                    });

            ModelBinderProviderCollection binders = new ModelBinderProviderCollection();
            binders.RegisterBinderForType(typeof(string), mockStringBinder.Object, true /* suppressPrefixCheck */);
            binders.RegisterBinderForType(typeof(int), mockIntBinder.Object, true /* suppressPrefixCheck */);
            binders.RegisterBinderForType(typeof(DateTime), mockDateTimeBinder.Object, true /* suppressPrefixCheck */);

            ExtensibleModelBindingContext parentBindingContext = new ExtensibleModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => dto, typeof(ComplexModelDto)),
                ModelName = "theModel",
                ModelBinderProviders = binders
            };

            ComplexModelDtoModelBinder binder = new ComplexModelDtoModelBinder();

            // Act
            bool retVal = binder.BindModel(controllerContext, parentBindingContext);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual(dto, parentBindingContext.Model, "The return model should have been the original DTO.");

            ComplexModelDtoResult stringDtoResult = dto.Results[dto.PropertyMetadata.Where(m => m.ModelType == typeof(string)).First()];
            Assert.AreEqual("someStringValue", stringDtoResult.Model);
            Assert.AreEqual("theModel.StringProperty", stringDtoResult.ValidationNode.ModelStateKey);

            ComplexModelDtoResult intDtoResult = dto.Results[dto.PropertyMetadata.Where(m => m.ModelType == typeof(int)).First()];
            Assert.AreEqual(42, intDtoResult.Model);
            Assert.AreEqual("theModel.IntProperty", intDtoResult.ValidationNode.ModelStateKey);

            ComplexModelDtoResult dateTimeDtoResult = dto.Results[dto.PropertyMetadata.Where(m => m.ModelType == typeof(DateTime)).First()];
            Assert.IsNull(dateTimeDtoResult);
        }

        private static ModelBindingContext GetBindingContext(Type modelType) {
            return new ModelBindingContext() {
                ModelMetadata = new ModelMetadata(new Mock<ModelMetadataProvider>().Object, null, null, modelType, "SomeProperty")
            };
        }

        private sealed class MyModel {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public object ObjectProperty { get; set; } // no binding should happen since no registered binder
            public DateTime DateTimeProperty { get; set; } // registered binder returns false
        }

    }
}
