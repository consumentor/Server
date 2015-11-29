namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;
    using Moq.Protected;

    [TestClass]
    [CLSCompliant(false)]
    public class DefaultModelBinderTest {

        [TestMethod]
        public void BindComplexElementalModelReturnsIfOnModelUpdatingReturnsFalse() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            MyModel model = new MyModel() { ReadWriteProperty = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
            };

            Mock<DefaultModelBinderHelper> mockHelper = new Mock<DefaultModelBinderHelper>() { CallBase = true };
            mockHelper.Expect(b => b.PublicOnModelUpdating(controllerContext, It.IsAny<ModelBindingContext>())).Returns(false);
            mockHelper.Expect(b => b.PublicGetModelProperties(controllerContext, It.IsAny<ModelBindingContext>())).Never();
            mockHelper.Expect(b => b.PublicBindProperty(controllerContext, It.IsAny<ModelBindingContext>(), It.IsAny<PropertyDescriptor>())).Never();
            DefaultModelBinderHelper helper = mockHelper.Object;

            // Act
            helper.BindComplexElementalModel(controllerContext, bindingContext, model);

            // Assert
            Assert.AreEqual(3, model.ReadWriteProperty, "Model should not have been updated.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void BindComplexModelCanBindArrays() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int[])),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", null },
                    { "foo[1]", null },
                    { "foo[2]", null }
                }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object newModel = binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(newModel, typeof(int[]));
            int[] newIntArray = (int[])newModel;

            Assert.AreEqual(3, newIntArray.Length, "Model is not of correct length.");
            Assert.AreEqual(0, newIntArray[0]);
            Assert.AreEqual(1, newIntArray[1]);
            Assert.AreEqual(2, newIntArray[2]);
        }

        [TestMethod]
        public void BindComplexModelCanBindCollections() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IList<int>)),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", null },
                    { "foo[1]", null },
                    { "foo[2]", null } 
                }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object newModel = binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(newModel, typeof(IList<int>));
            IList<int> modelAsList = (IList<int>)newModel;

            Assert.AreEqual(3, modelAsList.Count, "Model is not of correct length.");
            Assert.AreEqual(0, modelAsList[0]);
            Assert.AreEqual(1, modelAsList[1]);
            Assert.AreEqual(2, modelAsList[2]);
        }

        [TestMethod]
        public void BindComplexModelCanBindDictionaries() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IDictionary<int, string>)),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0].key", null }, { "foo[0].value", null },
                    { "foo[1].key", null }, { "foo[1].value", null },
                    { "foo[2].key", null }, { "foo[2].value", null }
                }
            };

            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(new ModelBindingContext().PropertyFilter, bc.PropertyFilter, "PropertyFilter should not have been set.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10;
                    });

            Mock<IModelBinder> mockStringBinder = new Mock<IModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(string), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockIntBinder.Object },
                    { typeof(string), mockStringBinder.Object }
                }
            };

            // Act
            object newModel = binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.IsInstanceOfType(newModel, typeof(IDictionary<int, string>));
            IDictionary<int, string> modelAsDictionary = (IDictionary<int, string>)newModel;

            Assert.AreEqual(3, modelAsDictionary.Count, "Model is not of correct length.");
            Assert.AreEqual("10Value", modelAsDictionary[10]);
            Assert.AreEqual("11Value", modelAsDictionary[11]);
            Assert.AreEqual("12Value", modelAsDictionary[12]);
        }

        [TestMethod]
        public void BindComplexModelCanBindObjects() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() { { "Foo", null }, { "Bar", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return bc.ModelName + "PostValue";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockInnerBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.AreSame(model, updatedModel, "Should have returned same instance of the model.");
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindComplexModelReturnsNullArrayIfNoValuesProvided() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int[])),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() { { "foo", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object newModel = binder.BindComplexModel(null, bindingContext);

            // Assert
            Assert.IsNull(newModel, "Method should have returned null.");
        }

        [TestMethod]
        public void BindComplexModelWhereModelTypeContainsBindAttribute() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelWithBindAttribute model = new ModelWithBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() { { "Foo", null }, { "Bar", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return bc.ModelName + "PostValue";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockInnerBinder.Object }
                }
            };

            // Act
            binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual("FooPreValue", model.Foo, "Foo property shouldn't have been updated since it was in the exclusion list.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindComplexModelWhereModelTypeDoesNotContainBindAttribute() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() { { "Foo", null }, { "Bar", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return bc.ModelName + "PostValue";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockInnerBinder.Object }
                }
            };

            // Act
            binder.BindComplexModel(controllerContext, bindingContext);

            // Assert
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        // BindModel tests

        [TestMethod]
        public void BindModelCanBindObjects() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() { { "Foo", null }, { "Bar", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return bc.ModelName + "PostValue";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockInnerBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.BindModel(controllerContext, bindingContext);

            // Assert
            Assert.AreSame(model, updatedModel, "Should have returned same instance of the model.");
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindModelCanBindSimpleTypes() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo", "42" } 
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

            // Assert
            Assert.AreEqual(42, updatedModel);
        }

        [TestMethod]
        public void BindModelReturnsNullIfKeyNotFound() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider()
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedModel = binder.BindModel(new ControllerContext(), bindingContext);

            // Assert
            Assert.IsNull(returnedModel);
        }

        [TestMethod]
        public void BindModelThrowsIfBindingContextIsNull() {
            // Arrange
            DefaultModelBinder binder = new DefaultModelBinder();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(new ControllerContext(), null);
                }, "bindingContext");
        }

        [TestMethod]
        public void BindModelValuesCanBeOverridden() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => new ModelWithoutBindAttribute(), typeof(ModelWithoutBindAttribute)),
                ModelName = "",
                ValueProvider = new SimpleValueProvider() {
                    { "foo", "FooPostValue" },
                    { "bar", "BarPostValue" },
                    { "baz", "BazPostValue" }
                }
            };
            Mock<DefaultModelBinder> binder = new Mock<DefaultModelBinder> { CallBase = true };
            binder.Protected().Expect<object>("GetPropertyValue",
                                              ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>(),
                                              ItExpr.IsAny<PropertyDescriptor>(), ItExpr.IsAny<IModelBinder>())
                              .Returns("Hello, world!");

            // Act
            ModelWithoutBindAttribute model = (ModelWithoutBindAttribute)binder.Object.BindModel(new ControllerContext(), bindingContext);

            // Assert
            Assert.AreEqual("Hello, world!", model.Bar);
            Assert.AreEqual("Hello, world!", model.Baz);
            Assert.AreEqual("Hello, world!", model.Foo);
        }

        [TestMethod]
        public void BindModelWithTypeConversionErrorUpdatesModelStateMessage() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => new PropertyTestingModel(), typeof(PropertyTestingModel)),
                ModelName = "",
                ValueProvider = new SimpleValueProvider() {
                    { "IntReadWrite", "foo" }
                },
            };
            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            PropertyTestingModel model = (PropertyTestingModel)binder.BindModel(new ControllerContext(), bindingContext);

            // Assert
            ModelState modelState = bindingContext.ModelState["IntReadWrite"];
            Assert.IsNotNull(modelState);
            Assert.AreEqual(1, modelState.Errors.Count);
            Assert.AreEqual("The value 'foo' is not valid for IntReadWrite.", modelState.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void BindModelWithPrefix() {
            // Arrange
            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "prefix",
                ValueProvider = new SimpleValueProvider() {
                    { "prefix.foo", "FooPostValue" },
                    { "prefix.bar", "BarPostValue" }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

            // Assert
            Assert.AreSame(model, updatedModel, "Should have returned same instance of the model.");
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindModelWithPrefixAndFallback() {
            // Arrange
            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                FallbackToEmptyPrefix = true,
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "prefix",
                ValueProvider = new SimpleValueProvider() {
                    { "foo", "FooPostValue" },
                    { "bar", "BarPostValue" }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

            // Assert
            Assert.AreSame(model, updatedModel, "Should have returned same instance of the model.");
            Assert.AreEqual("FooPostValue", model.Foo, "Foo property should have been updated.");
            Assert.AreEqual("BarPostValue", model.Bar, "Bar property should have been updated.");
            Assert.AreEqual("BazPreValue", model.Baz, "Baz property shouldn't have been updated since it wasn't part of the request.");
        }

        [TestMethod]
        public void BindModelWithPrefixReturnsNullIfFallbackNotSpecifiedAndValueProviderContainsNoEntries() {
            // Arrange
            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute() {
                Foo = "FooPreValue",
                Bar = "BarPreValue",
                Baz = "BazPreValue",
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "prefix",
                ValueProvider = new SimpleValueProvider() {
                    { "foo", "FooPostValue" },
                    { "bar", "BarPostValue" }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

            // Assert
            Assert.IsNull(updatedModel);
        }

        [TestMethod]
        public void BindModelReturnsNullIfSimpleTypeNotFound() {
            // DevDiv 216165: ModelBinders should not try and instantiate simple types

            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(string)),
                ModelName = "prefix",
                ValueProvider = new SimpleValueProvider() {
                    { "prefix.foo", "foo" },
                    { "prefix.bar", "bar" }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.BindModel(new ControllerContext(), bindingContext);

            // Assert
            Assert.IsNull(updatedModel);
        }

        // BindProperty tests

        [TestMethod]
        public void BindPropertyCanUpdateComplexReadOnlyProperties() {
            // Arrange
            // the Customer type contains a single read-only Address property
            Customer model = new Customer();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() { { "Address", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Address address = (Address)bc.Model;
                        address.Street = "1 Microsoft Way";
                        address.Zip = "98052";
                        return address;
                    });

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["Address"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper() {
                Binders = new ModelBinderDictionary() {
                    { typeof(Address), mockInnerBinder.Object }
                }
            };

            // Act
            helper.PublicBindProperty(new ControllerContext(), bindingContext, pd);

            // Assert
            Assert.AreEqual("1 Microsoft Way", model.Address.Street, "Property should have been updated.");
            Assert.AreEqual("98052", model.Address.Zip, "Property should have been updated.");
        }

        [TestMethod]
        public void BindPropertyDoesNothingIfValueProviderContainsNoEntryForProperty() {
            // Arrange
            MyModel2 model = new MyModel2() { IntReadWrite = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider()
            };

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicBindProperty(new ControllerContext(), bindingContext, pd);

            // Assert
            Assert.AreEqual(3, model.IntReadWrite, "Property should not have been changed.");
        }

        [TestMethod]
        public void BindProperty() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            MyModel2 model = new MyModel2() { IntReadWrite = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() {
                    { "IntReadWrite", "42" }
                }
            };

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];

            Mock<DefaultModelBinderHelper> mockHelper = new Mock<DefaultModelBinderHelper>() { CallBase = true };
            mockHelper.Expect(b => b.PublicOnPropertyValidating(controllerContext, bindingContext, pd, 42)).Returns(true).Verifiable();
            mockHelper.Expect(b => b.PublicSetProperty(controllerContext, bindingContext, pd, 42)).Verifiable();
            mockHelper.Expect(b => b.PublicOnPropertyValidated(controllerContext, bindingContext, pd, 42)).Verifiable();
            DefaultModelBinderHelper helper = mockHelper.Object;

            // Act
            helper.PublicBindProperty(controllerContext, bindingContext, pd);

            // Assert
            mockHelper.Verify();
        }

        [TestMethod]
        public void BindPropertyReturnsIfOnPropertyValidatingReturnsFalse() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            MyModel2 model = new MyModel2() { IntReadWrite = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() {
                    { "IntReadWrite", "42" }
                }
            };

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];

            Mock<DefaultModelBinderHelper> mockHelper = new Mock<DefaultModelBinderHelper>() { CallBase = true };
            mockHelper.Expect(b => b.PublicOnPropertyValidating(controllerContext, bindingContext, pd, 42)).Returns(false);
            mockHelper.Expect(b => b.PublicSetProperty(controllerContext, bindingContext, pd, 42)).Never();
            mockHelper.Expect(b => b.PublicOnPropertyValidated(controllerContext, bindingContext, pd, 42)).Never();
            DefaultModelBinderHelper helper = mockHelper.Object;

            // Act
            helper.PublicBindProperty(controllerContext, bindingContext, pd);

            // Assert
            Assert.AreEqual(3, model.IntReadWrite, "Property should not have been changed.");
            mockHelper.Verify();
        }

        [TestMethod]
        public void BindPropertySetsPropertyToNullIfUserLeftTextEntryFieldBlankForOptionalValue() {
            // Arrange
            MyModel2 model = new MyModel2() { NullableIntReadWrite = 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() { { "NullableIntReadWrite", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder.Expect(b => b.BindModel(new ControllerContext(), It.IsAny<ModelBindingContext>())).Returns((object)null);

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["NullableIntReadWrite"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int?), mockInnerBinder.Object }
                }
            };

            // Act
            helper.PublicBindProperty(new ControllerContext(), bindingContext, pd);

            // Assert
            Assert.AreEqual(0, bindingContext.ModelState.Count, "Should not have been an error.");
            Assert.AreEqual(null, model.NullableIntReadWrite, "Property should not have been updated.");
        }

        [TestMethod]
        public void BindPropertyUpdatesPropertyOnFailureIfInnerBinderReturnsNonNullObject() {
            // Arrange
            MyModel2 model = new MyModel2() { IntReadWriteNonNegative = 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ValueProvider = new SimpleValueProvider() { { "IntReadWriteNonNegative", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        bc.ModelState.AddModelError("IntReadWriteNonNegative", "Some error text.");
                        return 4;
                    });

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWriteNonNegative"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            helper.PublicBindProperty(new ControllerContext(), bindingContext, pd);

            // Assert
            Assert.AreEqual(false, bindingContext.ModelState.IsValidField("IntReadWriteNonNegative"), "Error should have propagated.");
            Assert.AreEqual(1, bindingContext.ModelState["IntReadWriteNonNegative"].Errors.Count, "Wrong number of errors.");
            Assert.AreEqual("Some error text.", bindingContext.ModelState["IntReadWriteNonNegative"].Errors[0].ErrorMessage, "Wrong error text.");
            Assert.AreEqual(4, model.IntReadWriteNonNegative, "Property should have been updated.");
        }

        [TestMethod]
        public void BindPropertyUpdatesPropertyOnSuccess() {
            // Arrange
            // Effectively, this is just testing updating a single property named "IntReadWrite"
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            MyModel2 model = new MyModel2() { IntReadWrite = 3 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "foo",
                ModelState = new ModelStateDictionary() { { "blah", new ModelState() } },
                ValueProvider = new SimpleValueProvider() { { "foo.IntReadWrite", null } }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(3, bc.Model, "Original model was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "Model type was not forwarded correctly.");
                        Assert.AreEqual("foo.IntReadWrite", bc.ModelName, "Model name was not forwarded correctly.");
                        Assert.AreEqual(new ModelBindingContext().PropertyFilter, bc.PropertyFilter, "Property filter property should not have been set.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "Value provider was not forwarded correctly.");
                        return 4;
                    });

            PropertyDescriptor pd = TypeDescriptor.GetProperties(model)["IntReadWrite"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            helper.PublicBindProperty(controllerContext, bindingContext, pd);

            // Assert
            Assert.AreEqual(4, model.IntReadWrite, "Property should have been updated.");
        }

        // BindSimpleModel tests

        [TestMethod]
        public void BindSimpleModelCanReturnArrayTypes() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(42, null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int[])),
                ModelName = "foo",
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.IsInstanceOfType(returnedValue, typeof(int[]), "Returned value was of incorrect type.");

            int[] returnedValueAsIntArray = (int[])returnedValue;
            Assert.AreEqual(1, returnedValueAsIntArray.Length);
            Assert.AreEqual(42, returnedValueAsIntArray[0]);
        }

        [TestMethod]
        public void BindSimpleModelCanReturnCollectionTypes() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(new string[] { "42", "82" }, null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IEnumerable<int>)),
                ModelName = "foo",
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.IsInstanceOfType(returnedValue, typeof(IEnumerable<int>), "Returned value was of incorrect type.");
            List<int> returnedValueAsList = ((IEnumerable<int>)returnedValue).ToList();

            Assert.AreEqual(2, returnedValueAsList.Count);
            Assert.AreEqual(42, returnedValueAsList[0]);
            Assert.AreEqual(82, returnedValueAsList[1]);
        }

        [TestMethod]
        public void BindSimpleModelCanReturnElementalTypes() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult("42", null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
                ModelName = "foo",
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.AreEqual(42, returnedValue);
        }

        [TestMethod]
        public void BindSimpleModelCanReturnStrings() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(new object[] { "42" }, null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(string)),
                ModelName = "foo",
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.AreEqual("42", returnedValue);
        }

        [TestMethod]
        public void BindSimpleModelChecksValueProviderResultRawValueType() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult(new MemoryStream(), null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(Stream)),
                ModelName = "foo",
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.AreEqual(result, bindingContext.ModelState["foo"].Value, "ModelState should have been set.");
            Assert.AreSame(result.RawValue, returnedValue, "Should have returned the RawValue since it was of the correct type.");
        }

        [TestMethod]
        public void BindSimpleModelPropagatesErrorsOnFailure() {
            // Arrange
            ValueProviderResult result = new ValueProviderResult("invalid", null, null);
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(int)),
                ModelName = "foo",
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object returnedValue = binder.BindSimpleModel(null, bindingContext, result);

            // Assert
            Assert.IsFalse(bindingContext.ModelState.IsValidField("foo"), "Foo should be an invalid field.");
            Assert.IsInstanceOfType(bindingContext.ModelState["foo"].Errors[0].Exception, typeof(InvalidOperationException));
            Assert.AreEqual("The parameter conversion from type 'System.String' to type 'System.Int32' failed. See the inner exception for more information.", bindingContext.ModelState["foo"].Errors[0].Exception.Message);
            Assert.IsNull(returnedValue, "Should have returned null on failure.");
        }

        [TestMethod]
        public void CreateComplexElementalModelBindingContext_ReadsBindAttributeFromBuddyClass() {
            // Arrange
            ModelBindingContext originalBindingContext = new ModelBindingContext() {
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(null, typeof(CreateComplexElementalModelBindingContext_ReadsBindAttributeFromBuddyClass_Model)),
                ModelName = "someName",
                ValueProvider = new SimpleValueProvider()
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            ModelBindingContext newBindingContext = binder.CreateComplexElementalModelBindingContext(new ControllerContext(), originalBindingContext, null);

            // Assert
            Assert.IsTrue(newBindingContext.PropertyFilter("foo"));
            Assert.IsFalse(newBindingContext.PropertyFilter("bar"));
        }

        [MetadataType(typeof(CreateComplexElementalModelBindingContext_ReadsBindAttributeFromBuddyClass_Model_BuddyClass))]
        private class CreateComplexElementalModelBindingContext_ReadsBindAttributeFromBuddyClass_Model {

            [Bind(Include = "foo")]
            private class CreateComplexElementalModelBindingContext_ReadsBindAttributeFromBuddyClass_Model_BuddyClass { }
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstance() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(Guid));

            // Assert
            Assert.AreEqual(Guid.Empty, modelObj);
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstanceForGenericICollection() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(ICollection<Guid>));

            // Assert
            Assert.IsInstanceOfType(modelObj, typeof(ICollection<Guid>));
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstanceForGenericIDictionary() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(IDictionary<string, Guid>));

            // Assert
            Assert.IsInstanceOfType(modelObj, typeof(IDictionary<string, Guid>));
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstanceForGenericIEnumerable() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(IEnumerable<Guid>));

            // Assert
            Assert.IsInstanceOfType(modelObj, typeof(ICollection<Guid>), "We must actually create an ICollection<> when asked to create an IEnumerable<>.");
        }

        [TestMethod]
        public void CreateInstanceCreatesModelInstanceForGenericIList() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            object modelObj = helper.PublicCreateModel(null, null, typeof(IList<Guid>));

            // Assert
            Assert.IsInstanceOfType(modelObj, typeof(IList<Guid>));
        }

        [TestMethod]
        public void CreateSubIndexNameReturnsPrefixPlusIndex() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            string newName = helper.PublicCreateSubIndexName("somePrefix", 2);

            // Assert
            Assert.AreEqual("somePrefix[2]", newName);
        }

        [TestMethod]
        public void CreateSubPropertyNameReturnsPrefixPlusPropertyName() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            string newName = helper.PublicCreateSubPropertyName("somePrefix", "someProperty");

            // Assert
            Assert.AreEqual("somePrefix.someProperty", newName);
        }

        [TestMethod]
        public void CreateSubPropertyNameReturnsPropertyNameIfPrefixIsEmpty() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            string newName = helper.PublicCreateSubPropertyName(String.Empty, "someProperty");

            // Assert
            Assert.AreEqual("someProperty", newName);
        }

        [TestMethod]
        public void CreateSubPropertyNameReturnsPropertyNameIfPrefixIsNull() {
            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            string newName = helper.PublicCreateSubPropertyName(null, "someProperty");

            // Assert
            Assert.AreEqual("someProperty", newName);
        }

        [TestMethod]
        public void GetFilteredModelPropertiesFiltersNonUpdateableProperties() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(PropertyTestingModel)),
                PropertyFilter = new BindAttribute() { Exclude = "Blacklisted" }.IsPropertyAllowed
            };

            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            PropertyDescriptorCollection properties = new PropertyDescriptorCollection(helper.PublicGetFilteredModelProperties(null, bindingContext).ToArray());

            // Assert
            Assert.IsNotNull(properties["StringReadWrite"], "StringReadWrite: Read+write string properties are updateable.");
            Assert.IsNull(properties["StringReadOnly"], "StringReadOnly: Read-only string properties are not updateable.");
            Assert.IsNotNull(properties["IntReadWrite"], "IntReadWrite: Read+write ValueType properties are updateable.");
            Assert.IsNull(properties["IntReadOnly"], "IntReadOnly: Read-only string properties are not updateable.");
            Assert.IsNotNull(properties["ArrayReadWrite"], "ArrayReadWrite: Read+write array properties are updateable.");
            Assert.IsNull(properties["ArrayReadOnly"], "ArrayReadOnly: Read-only array properties are not updateable.");
            Assert.IsNotNull(properties["AddressReadWrite"], "AddressReadWrite: Read+write complex properties are updateable.");
            Assert.IsNotNull(properties["AddressReadOnly"], "AddressReadOnly: Read-only complex properties are updateable.");
            Assert.IsNotNull(properties["Whitelisted"], "Whitelisted: Whitelisted properties are updateable.");
            Assert.IsNull(properties["Blacklisted"], "Blacklisted: Blacklisted properties are not updateable.");
            Assert.AreEqual(6, properties.Count, "Incorrect number of properties returned.");
        }

        [TestMethod]
        public void GetModelPropertiesReturnsUnfilteredPropertyList() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(PropertyTestingModel)),
                PropertyFilter = new BindAttribute() { Exclude = "Blacklisted" }.IsPropertyAllowed
            };

            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            PropertyDescriptorCollection properties = helper.PublicGetModelProperties(null, bindingContext);

            // Assert
            Assert.IsNotNull(properties["StringReadWrite"]);
            Assert.IsNotNull(properties["StringReadOnly"]);
            Assert.IsNotNull(properties["IntReadWrite"]);
            Assert.IsNotNull(properties["IntReadOnly"]);
            Assert.IsNotNull(properties["ArrayReadWrite"]);
            Assert.IsNotNull(properties["ArrayReadOnly"]);
            Assert.IsNotNull(properties["AddressReadWrite"]);
            Assert.IsNotNull(properties["AddressReadOnly"]);
            Assert.IsNotNull(properties["Whitelisted"]);
            Assert.IsNotNull(properties["Blacklisted"]);
            Assert.AreEqual(10, properties.Count);
        }

        [TestMethod]
        public void IsModelValidWithNullBindingContextThrows() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => DefaultModelBinderHelper.PublicIsModelValid(null),
                "bindingContext");
        }

        [TestMethod]
        public void IsModelValidReturnsModelStateIsValidWhenModelNameIsEmpty() {
            // Arrange
            ModelBindingContext contextWithNoErrors = new ModelBindingContext { ModelName = "" };
            ModelBindingContext contextWithErrors = new ModelBindingContext { ModelName = "" };
            contextWithErrors.ModelState.AddModelError("foo", "bar");

            // Act & Assert
            Assert.IsTrue(DefaultModelBinderHelper.PublicIsModelValid(contextWithNoErrors));
            Assert.IsFalse(DefaultModelBinderHelper.PublicIsModelValid(contextWithErrors));
        }

        [TestMethod]
        public void IsModelValidReturnsValidityOfSubModelStateWhenModelNameIsNotEmpty() {
            // Arrange
            ModelBindingContext contextWithNoErrors = new ModelBindingContext { ModelName = "foo" };
            ModelBindingContext contextWithErrors = new ModelBindingContext { ModelName = "foo" };
            contextWithErrors.ModelState.AddModelError("foo.bar", "baz");
            ModelBindingContext contextWithUnrelatedErrors = new ModelBindingContext { ModelName = "foo" };
            contextWithUnrelatedErrors.ModelState.AddModelError("biff", "baz");

            // Act & Assert
            Assert.IsTrue(DefaultModelBinderHelper.PublicIsModelValid(contextWithNoErrors));
            Assert.IsFalse(DefaultModelBinderHelper.PublicIsModelValid(contextWithErrors));
            Assert.IsTrue(DefaultModelBinderHelper.PublicIsModelValid(contextWithUnrelatedErrors));
        }

        [TestMethod]
        public void OnModelUpdatingReturnsTrue() {
            // By default, this method does nothing, so we just want to make sure it returns true

            // Arrange
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            bool returned = helper.PublicOnModelUpdating(null, null);

            // Arrange
            Assert.IsTrue(returned);
        }

        // OnModelUpdated tests

        [TestMethod]
        public void OnModelUpdatedCalledWhenOnModelUpdatingReturnsTrue() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => new ModelWithoutBindAttribute(), typeof(ModelWithoutBindAttribute)),
                ModelName = "",
                ValueProvider = new SimpleValueProvider()
            };
            Mock<DefaultModelBinder> binder = new Mock<DefaultModelBinder> { CallBase = true };
            binder.Protected().Expect<bool>("OnModelUpdating",
                                            ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>())
                              .Returns(true);
            binder.Protected().Expect("OnModelUpdated",
                                      ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>())
                              .Verifiable();

            // Act
            binder.Object.BindModel(new ControllerContext(), bindingContext);

            // Assert
            binder.Verify();
        }

        [TestMethod]
        public void OnModelUpdatedNotCalledWhenOnModelUpdatingReturnsFalse() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => new ModelWithoutBindAttribute(), typeof(ModelWithoutBindAttribute)),
                ModelName = "",
                ValueProvider = new SimpleValueProvider()
            };
            Mock<DefaultModelBinder> binder = new Mock<DefaultModelBinder> { CallBase = true };
            binder.Protected().Expect<bool>("OnModelUpdating",
                                            ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>())
                              .Returns(false);
            binder.Protected().Expect("OnModelUpdated",
                                      ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>())
                              .Never();

            // Act
            binder.Object.BindModel(new ControllerContext(), bindingContext);

            // Assert
            binder.Verify();
        }

        [TestMethod]
        public void OnModelUpdatedDoesntAddNewMessagesWhenMessagesAlreadyExist() {
            // Arrange
            var binder = new TestableDefaultModelBinder<SetPropertyModel>();
            binder.Context.ModelState.AddModelError(BASE_MODEL_NAME + ".NonNullableStringWithAttribute", "Some pre-existing error");

            // Act
            binder.OnModelUpdated();

            // Assert
            var modelState = binder.Context.ModelState[BASE_MODEL_NAME + ".NonNullableStringWithAttribute"];
            Assert.AreEqual(1, modelState.Errors.Count);
            Assert.AreEqual("Some pre-existing error", modelState.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void OnPropertyValidatingNotCalledOnPropertiesWithErrors() {
            // Arrange
            ModelWithoutBindAttribute model = new ModelWithoutBindAttribute();
            ModelBindingContext bindingContext = new ModelBindingContext {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "",
                ValueProvider = new SimpleValueProvider() {
                    { "foo", "foo" }
                },
            };
            bindingContext.ModelState.AddModelError("foo", "Pre-existing error");
            Mock<DefaultModelBinder> binder = new Mock<DefaultModelBinder> { CallBase = true };
            binder.Protected().Expect("OnPropertyValidating",
                                      ItExpr.IsAny<ControllerContext>(), ItExpr.IsAny<ModelBindingContext>(),
                                      ItExpr.IsAny<PropertyDescriptor>(), ItExpr.IsAny<object>())
                              .Never();

            // Act
            binder.Object.BindModel(new ControllerContext(), bindingContext);

            // Assert
            binder.Verify();
        }

        public class ExtraValueModel {
            public int RequiredValue { get; set; }
        }

        [TestMethod]
        public void ExtraValueRequiredMessageNotAddedForAlreadyInvalidProperty() {
            // Arrange
            DefaultModelBinder binder = new DefaultModelBinder();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
                ModelName = "theModel",
                ValueProvider = new SimpleValueProvider()
            };
            bindingContext.ModelState.AddModelError("theModel.ReadWriteProperty", "Existing Error Message");

            // Act
            binder.BindModel(new ControllerContext(), bindingContext);

            // Assert
            ModelState modelState = bindingContext.ModelState["theModel.ReadWriteProperty"];
            Assert.AreEqual(1, modelState.Errors.Count);
            Assert.AreEqual("Existing Error Message", modelState.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void OnPropertyValidatingReturnsTrueOnSuccess() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();
            bindingContext.PropertyMetadata["ReadWriteProperty"].Model = 42;

            // Act
            bool returned = helper.PublicOnPropertyValidating(new ControllerContext(), bindingContext, property, 42);

            // Assert
            Assert.IsTrue(returned, "Value should have passed validation.");
            Assert.AreEqual(0, bindingContext.ModelState.Count);
        }

        [TestMethod]
        public void UpdateCollectionCreatesDefaultEntriesForInvalidElements() {
            // Arrange
            List<int> model = new List<int>() { 4, 5, 6, 7, 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", null },
                    { "foo[1]", null },
                    { "foo[2]", null }
                }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                        return (fooIdx == 1) ? (object)null : fooIdx;
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateCollection(null, bindingContext, typeof(int));

            // Assert
            Assert.AreEqual(3, model.Count, "Model is not of correct length.");
            Assert.AreEqual(false, bindingContext.ModelState.IsValidField("foo[1]"), "Conversion should have failed.");
            Assert.AreEqual("A value is required.", bindingContext.ModelState["foo[1]"].Errors[0].ErrorMessage, "Error message did not propagate correctly.");
            Assert.AreEqual(0, model[0]);
            Assert.AreEqual(0, model[1]);
            Assert.AreEqual(2, model[2]);
        }

        [TestMethod]
        public void UpdateCollectionReturnsModifiedCollectionOnSuccess_ExplicitIndex() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            List<int> model = new List<int>() { 4, 5, 6, 7, 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new SimpleValueProvider() {
                    { "foo.index", new string[] { "alpha", "bravo", "charlie" } }, // 'bravo' will be skipped
                    { "foo[alpha]", "10" },
                    { "foo[charlie]", "30" }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.UpdateCollection(controllerContext, bindingContext, typeof(int));

            // Assert
            Assert.AreSame(model, updatedModel, "Should have updated the provided model object.");
            Assert.AreEqual(2, model.Count, "Model is not of correct length.");
            Assert.AreEqual(10, model[0]);
            Assert.AreEqual(30, model[1]);
        }

        [TestMethod]
        public void UpdateCollectionReturnsModifiedCollectionOnSuccess_ZeroBased() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            List<int> model = new List<int>() { 4, 5, 6, 7, 8 };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0]", null },
                    { "foo[1]", null },
                    { "foo[2]", null }
                }
            };

            Mock<IModelBinder> mockInnerBinder = new Mock<IModelBinder>();
            mockInnerBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockInnerBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateCollection(controllerContext, bindingContext, typeof(int));

            // Assert
            Assert.AreSame(model, updatedModel, "Should have updated the provided model object.");
            Assert.AreEqual(3, model.Count, "Model is not of correct length.");
            Assert.AreEqual(0, model[0]);
            Assert.AreEqual(1, model[1]);
            Assert.AreEqual(2, model[2]);
        }

        [TestMethod]
        public void UpdateCollectionReturnsNullIfZeroIndexNotFound() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ValueProvider = new SimpleValueProvider()
            };
            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.UpdateCollection(null, bindingContext, typeof(object));

            // Assert
            Assert.IsNull(updatedModel, "Method should return null if no values exist as part of the request.");
        }

        [TestMethod]
        public void UpdateDictionaryCreatesDefaultEntriesForInvalidValues() {
            // Arrange
            Dictionary<string, int> model = new Dictionary<string, int>{
                { "one", 1 },
                { "two", 2 }
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0].key", null }, { "foo[0].value", null },
                    { "foo[1].key", null }, { "foo[1].value", null },
                    { "foo[2].key", null }, { "foo[2].value", null }
                }
            };

            Mock<IModelBinder> mockStringBinder = new Mock<IModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
                    });

            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                        return (fooIdx == 1) ? (object)null : fooIdx;
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(string), mockStringBinder.Object },
                    { typeof(int), mockIntBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateDictionary(null, bindingContext, typeof(string), typeof(int));

            // Assert
            Assert.AreEqual(3, model.Count, "Model is not of correct length.");
            Assert.AreEqual(false, bindingContext.ModelState.IsValidField("foo[1].value"), "Conversion should have failed.");
            Assert.AreEqual("A value is required.", bindingContext.ModelState["foo[1].value"].Errors[0].ErrorMessage, "Error message did not propagate correctly.");
            Assert.AreEqual(0, model["10Value"]);
            Assert.AreEqual(0, model["11Value"]);
            Assert.AreEqual(2, model["12Value"]);
        }

        [TestMethod]
        public void UpdateDictionaryReturnsModifiedDictionaryOnSuccess_ExplicitIndex() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            Dictionary<int, string> model = new Dictionary<int, string>{
                { 1, "one" },
                { 2, "two" }
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new SimpleValueProvider() {
                    { "foo.index", new string[] { "alpha", "bravo", "charlie" } }, // 'bravo' will be skipped
                    { "foo[alpha].key", "10" }, { "foo[alpha].value", "ten" },
                    { "foo[charlie].key", "30" }, { "foo[charlie].value", "thirty" }
                }
            };

            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.UpdateDictionary(controllerContext, bindingContext, typeof(int), typeof(string));

            // Assert
            Assert.AreSame(model, updatedModel, "Should have updated the provided model object.");
            Assert.AreEqual(2, model.Count, "Model is not of correct length.");
            Assert.AreEqual("ten", model[10]);
            Assert.AreEqual("thirty", model[30]);
        }

        [TestMethod]
        public void UpdateDictionaryReturnsModifiedDictionaryOnSuccess_ZeroBased() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;

            Dictionary<int, string> model = new Dictionary<int, string>{
                { 1, "one" },
                { 2, "two" }
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "foo",
                PropertyFilter = _ => false,
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0].key", null }, { "foo[0].value", null },
                    { "foo[1].key", null }, { "foo[1].value", null },
                    { "foo[2].key", null }, { "foo[2].value", null }
                }
            };

            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(int), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(new ModelBindingContext().PropertyFilter, bc.PropertyFilter, "PropertyFilter should not have been set.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10;
                    });

            Mock<IModelBinder> mockStringBinder = new Mock<IModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        Assert.AreEqual(controllerContext, cc, "ControllerContext was not forwarded correctly.");
                        Assert.AreEqual(typeof(string), bc.ModelType, "ModelType was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ModelState, bc.ModelState, "ModelState was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.PropertyFilter, bc.PropertyFilter, "PropertyFilter was not forwarded correctly.");
                        Assert.AreEqual(bindingContext.ValueProvider, bc.ValueProvider, "ValueProvider was not forwarded correctly.");
                        return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockIntBinder.Object },
                    { typeof(string), mockStringBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateDictionary(controllerContext, bindingContext, typeof(int), typeof(string));

            // Assert
            Assert.AreSame(model, updatedModel, "Should have updated the provided model object.");
            Assert.AreEqual(3, model.Count, "Model is not of correct length.");
            Assert.AreEqual("10Value", model[10]);
            Assert.AreEqual("11Value", model[11]);
            Assert.AreEqual("12Value", model[12]);
        }

        [TestMethod]
        public void UpdateDictionaryReturnsNullIfNoValidElementsFound() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ValueProvider = new SimpleValueProvider()
            };
            DefaultModelBinder binder = new DefaultModelBinder();

            // Act
            object updatedModel = binder.UpdateDictionary(null, bindingContext, typeof(object), typeof(object));

            // Assert
            Assert.IsNull(updatedModel, "Method should return null if no values exist as part of the request.");
        }

        [TestMethod]
        public void UpdateDictionarySkipsInvalidKeys() {
            // Arrange
            Dictionary<int, string> model = new Dictionary<int, string>{
                { 1, "one" },
                { 2, "two" }
            };
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "foo",
                ValueProvider = new SimpleValueProvider() {
                    { "foo[0].key", null }, { "foo[0].value", null },
                    { "foo[1].key", null }, { "foo[1].value", null },
                    { "foo[2].key", null }, { "foo[2].value", null }
                }
            };

            Mock<IModelBinder> mockIntBinder = new Mock<IModelBinder>();
            mockIntBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        int fooIdx = Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture);
                        return (fooIdx == 1) ? (object)null : fooIdx;
                    });

            Mock<IModelBinder> mockStringBinder = new Mock<IModelBinder>();
            mockStringBinder
                .Expect(b => b.BindModel(It.IsAny<ControllerContext>(), It.IsAny<ModelBindingContext>()))
                .Returns(
                    delegate(ControllerContext cc, ModelBindingContext bc) {
                        return (Int32.Parse(bc.ModelName.Substring(4, 1), CultureInfo.InvariantCulture) + 10) + "Value";
                    });

            DefaultModelBinder binder = new DefaultModelBinder() {
                Binders = new ModelBinderDictionary() {
                    { typeof(int), mockIntBinder.Object },
                    { typeof(string), mockStringBinder.Object }
                }
            };

            // Act
            object updatedModel = binder.UpdateDictionary(null, bindingContext, typeof(int), typeof(string));

            // Assert
            Assert.AreEqual(2, model.Count, "Model is not of correct length.");
            Assert.AreEqual(false, bindingContext.ModelState.IsValidField("foo[1].key"), "Conversion should have failed.");
            Assert.AreEqual("A value is required.", bindingContext.ModelState["foo[1].key"].Errors[0].ErrorMessage, "Error message did not propagate correctly.");
            Assert.AreEqual("10Value", model[0]);
            Assert.AreEqual("12Value", model[2]);
        }

        [ModelBinder(typeof(DefaultModelBinder))]
        private class MyModel {
            public int ReadOnlyProperty {
                get { return 4; }
            }
            public int ReadWriteProperty { get; set; }
            public int ReadWriteProperty2 { get; set; }
        }

        private class MyClassWithoutConverter {
        }

        [Bind(Exclude = "Alpha,Echo")]
        private class MyOtherModel {
            public string Alpha { get; set; }
            public string Bravo { get; set; }
            public string Charlie { get; set; }
            public string Delta { get; set; }
            public string Echo { get; set; }
            public string Foxtrot { get; set; }
        }

        public class Customer {
            private Address _address = new Address();
            public Address Address {
                get {
                    return _address;
                }
            }
        }

        public class Address {
            public string Street { get; set; }
            public string Zip { get; set; }
        }

        public class IntegerContainer {
            public int Integer { get; set; }
            public int? NullableInteger { get; set; }
        }

        [TypeConverter(typeof(CultureAwareConverter))]
        public class StringContainer {
            public StringContainer(string value) {
                Value = value;
            }
            public string Value {
                get;
                private set;
            }
        }

        private class CultureAwareConverter : TypeConverter {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
                return (sourceType == typeof(string));
            }
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
                return (destinationType == typeof(string));
            }
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
                string stringValue = value as string;
                if (stringValue == null || stringValue.Length < 3) {
                    throw new Exception("Value must have at least 3 characters.");
                }
                return new StringContainer(AppendCultureName(stringValue, culture));
            }
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
                StringContainer container = value as StringContainer;
                if (container.Value == null || container.Value.Length < 3) {
                    throw new Exception("Value must have at least 3 characters.");
                }

                return AppendCultureName(container.Value, culture);
            }

            private static string AppendCultureName(string value, CultureInfo culture) {
                string cultureName = (!String.IsNullOrEmpty(culture.Name)) ? culture.Name : culture.ThreeLetterWindowsLanguageName;
                return value + " (" + cultureName + ")";
            }
        }

        [ModelBinder(typeof(MyStringModelBinder))]
        private class MyStringModel {
            public string Value { get; set; }
        }

        private class MyStringModelBinder : IModelBinder {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                MyStringModel castModel = bindingContext.Model as MyStringModel;
                if (castModel != null) {
                    castModel.Value += "_Update";
                }
                else {
                    castModel = new MyStringModel() { Value = bindingContext.ModelName + "_Create" };
                }
                return castModel;
            }
        }

        public class DefaultModelBinderHelper : DefaultModelBinder {
            public virtual void PublicBindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property) {
                base.BindProperty(controllerContext, bindingContext, property);
            }
            protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property) {
                PublicBindProperty(controllerContext, bindingContext, property);
            }
            public virtual object PublicCreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType) {
                return base.CreateModel(controllerContext, bindingContext, modelType);
            }
            protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType) {
                return PublicCreateModel(controllerContext, bindingContext, modelType);
            }
            public virtual IEnumerable<PropertyDescriptor> PublicGetFilteredModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return base.GetFilteredModelProperties(controllerContext, bindingContext);
            }
            public virtual PropertyDescriptorCollection PublicGetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return base.GetModelProperties(controllerContext, bindingContext);
            }
            protected override PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return PublicGetModelProperties(controllerContext, bindingContext);
            }
            public string PublicCreateSubIndexName(string prefix, int indexName) {
                return CreateSubIndexName(prefix, indexName);
            }
            public string PublicCreateSubPropertyName(string prefix, string propertyName) {
                return CreateSubPropertyName(prefix, propertyName);
            }
            public static bool PublicIsModelValid(ModelBindingContext bindingContext) {
                return DefaultModelBinder.IsModelValid(bindingContext);
            }
            public virtual bool PublicOnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return base.OnModelUpdating(controllerContext, bindingContext);
            }
            protected override bool OnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                return PublicOnModelUpdating(controllerContext, bindingContext);
            }
            public virtual void PublicOnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                base.OnModelUpdated(controllerContext, bindingContext);
            }
            protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext) {
                PublicOnModelUpdated(controllerContext, bindingContext);
            }
            public virtual bool PublicOnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                return base.OnPropertyValidating(controllerContext, bindingContext, property, value);
            }
            protected override bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                return PublicOnPropertyValidating(controllerContext, bindingContext, property, value);
            }
            public virtual void PublicOnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                base.OnPropertyValidated(controllerContext, bindingContext, property, value);
            }
            protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                PublicOnPropertyValidated(controllerContext, bindingContext, property, value);
            }
            public virtual void PublicSetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                base.SetProperty(controllerContext, bindingContext, property, value);
            }
            protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor property, object value) {
                PublicSetProperty(controllerContext, bindingContext, property, value);
            }
        }

        private class MyModel2 {

            private int _intReadWriteNonNegative;

            public int IntReadOnly { get { return 4; } }
            public int IntReadWrite { get; set; }
            public int IntReadWriteNonNegative {
                get {
                    return _intReadWriteNonNegative;
                }
                set {
                    if (value < 0) {
                        throw new ArgumentOutOfRangeException("value", "Value must be non-negative.");
                    }
                    _intReadWriteNonNegative = value;
                }
            }
            public int? NullableIntReadWrite { get; set; }

        }

        [Bind(Exclude = "Foo")]
        private class ModelWithBindAttribute : ModelWithoutBindAttribute {
        }

        private class ModelWithoutBindAttribute {
            public string Foo { get; set; }
            public string Bar { get; set; }
            public string Baz { get; set; }
        }

        private class PropertyTestingModel {
            public string StringReadWrite { get; set; }
            public string StringReadOnly { get; private set; }
            public int IntReadWrite { get; set; }
            public int IntReadOnly { get; private set; }
            public object[] ArrayReadWrite { get; set; }
            public object[] ArrayReadOnly { get; private set; }
            public Address AddressReadWrite { get; set; }
            public Address AddressReadOnly { get; private set; }
            public string Whitelisted { get; set; }
            public string Blacklisted { get; set; }
        }

        // --------------------------------------------------------------------------------
        //  DataAnnotations tests

        public const string BASE_MODEL_NAME = "BaseModelName";

        // GetModelProperties tests

        [MetadataType(typeof(GetModelPropertiesModel.Metadata))]
        class GetModelPropertiesModel {
            [Required]
            public int LocalAttributes { get; set; }

            public int MetadataAttributes { get; set; }

            [Required]
            public int MixedAttributes { get; set; }

            class Metadata {
                [Range(10, 100)]
                public int MetadataAttributes { get; set; }

                [Range(10, 100)]
                public int MixedAttributes { get; set; }
            }
        }

        [TestMethod]
        public void GetModelPropertiesWithLocalAttributes() {
            // Arrange
            TestableDefaultModelBinder<GetModelPropertiesModel> modelBinder = new TestableDefaultModelBinder<GetModelPropertiesModel>();

            // Act
            PropertyDescriptor property = modelBinder.GetModelProperties()
                                                     .Cast<PropertyDescriptor>()
                                                     .Where(pd => pd.Name == "LocalAttributes")
                                                     .Single();

            // Assert
            Assert.IsTrue(property.Attributes.Cast<Attribute>().Any(a => a is RequiredAttribute));
        }

        [TestMethod]
        public void GetModelPropertiesWithMetadataAttributes() {
            // Arrange
            TestableDefaultModelBinder<GetModelPropertiesModel> modelBinder = new TestableDefaultModelBinder<GetModelPropertiesModel>();

            // Act
            PropertyDescriptor property = modelBinder.GetModelProperties()
                                                     .Cast<PropertyDescriptor>()
                                                     .Where(pd => pd.Name == "MetadataAttributes")
                                                     .Single();

            // Assert
            Assert.IsTrue(property.Attributes.Cast<Attribute>().Any(a => a is RangeAttribute));
        }

        [TestMethod]
        public void GetModelPropertiesWithMixedAttributes() {
            // Arrange
            TestableDefaultModelBinder<GetModelPropertiesModel> modelBinder = new TestableDefaultModelBinder<GetModelPropertiesModel>();

            // Act
            PropertyDescriptor property = modelBinder.GetModelProperties()
                                                     .Cast<PropertyDescriptor>()
                                                     .Where(pd => pd.Name == "MixedAttributes")
                                                     .Single();

            // Assert
            Assert.IsTrue(property.Attributes.Cast<Attribute>().Any(a => a is RequiredAttribute));
            Assert.IsTrue(property.Attributes.Cast<Attribute>().Any(a => a is RangeAttribute));
        }

        // GetPropertyValue tests

        class GetPropertyValueModel {
            public string NoAttribute { get; set; }

            [DisplayFormat(ConvertEmptyStringToNull = false)]
            public string AttributeWithoutConversion { get; set; }

            [DisplayFormat(ConvertEmptyStringToNull = true)]
            public string AttributeWithConversion { get; set; }
        }

        [TestMethod]
        public void GetPropertyValueWithNoAttributeConvertsEmptyStringToNull() {
            // Arrange
            TestableDefaultModelBinder<GetPropertyValueModel> binder = new TestableDefaultModelBinder<GetPropertyValueModel>();
            binder.Context.ModelMetadata = binder.Context.PropertyMetadata["NoAttribute"];

            // Act
            object result = binder.GetPropertyValue("NoAttribute", String.Empty);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetPropertyValueWithFalseAttributeDoesNotConvertEmptyStringToNull() {
            // Arrange
            TestableDefaultModelBinder<GetPropertyValueModel> binder = new TestableDefaultModelBinder<GetPropertyValueModel>();
            binder.Context.ModelMetadata = binder.Context.PropertyMetadata["AttributeWithoutConversion"];

            // Act
            object result = binder.GetPropertyValue("AttributeWithoutConversion", String.Empty);

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void GetPropertyValueWithTrueAttributeConvertsEmptyStringToNull() {
            // Arrange
            TestableDefaultModelBinder<GetPropertyValueModel> binder = new TestableDefaultModelBinder<GetPropertyValueModel>();
            binder.Context.ModelMetadata = binder.Context.PropertyMetadata["AttributeWithConversion"];

            // Act
            object result = binder.GetPropertyValue("AttributeWithConversion", String.Empty);

            // Assert
            Assert.IsNull(result);
        }

        // OnModelUpdated tests

        [TestMethod]
        public void OnModelUpdatedPassesNullContainerToValidate() {
            Mock<ModelValidatorProvider> provider = null;

            try {
                // Arrange
                ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(object));
                ControllerContext context = new ControllerContext();
                Mock<ModelValidator> validator = new Mock<ModelValidator>(metadata, context);
                provider = new Mock<ModelValidatorProvider>();
                provider.Expect(p => p.GetValidators(It.IsAny<ModelMetadata>(), It.IsAny<ControllerContext>()))
                        .Returns(new ModelValidator[] { validator.Object });
                ModelValidatorProviders.Providers.Add(provider.Object);
                object model = new object();
                TestableDefaultModelBinder<object> modelBinder = new TestableDefaultModelBinder<object>(model);

                // Act
                modelBinder.OnModelUpdated();

                // Assert
                validator.Verify(v => v.Validate(null));
            }
            finally {
                if (provider != null) {
                    ModelValidatorProviders.Providers.Remove(provider.Object);
                }
            }
        }

        public class MinMaxValidationAttribute : ValidationAttribute {
            public MinMaxValidationAttribute() : base("Minimum must be less than or equal to Maximum") { }

            public override bool IsValid(object value) {
                OnModelUpdatedModelMultipleParameters model = (OnModelUpdatedModelMultipleParameters)value;
                return model.Minimum <= model.Maximum;
            }
        }

        [MinMaxValidation]
        public class OnModelUpdatedModelMultipleParameters {
            public int Minimum { get; set; }
            public int Maximum { get; set; }
        }

        [TestMethod]
        public void OnModelUpdatedWithValidationAttributeMultipleParameters() {
            // Arrange
            OnModelUpdatedModelMultipleParameters model = new OnModelUpdatedModelMultipleParameters { Minimum = 250, Maximum = 100 };
            TestableDefaultModelBinder<OnModelUpdatedModelMultipleParameters> modelBinder = new TestableDefaultModelBinder<OnModelUpdatedModelMultipleParameters>(model);

            // Act
            modelBinder.OnModelUpdated();

            // Assert
            Assert.AreEqual(1, modelBinder.ModelState.Count);
            ModelState stateModel = modelBinder.ModelState[BASE_MODEL_NAME];
            Assert.IsNotNull(stateModel);
            Assert.AreEqual("Minimum must be less than or equal to Maximum", stateModel.Errors.Single().ErrorMessage);
        }

        [TestMethod]
        public void OnModelUpdatedWithInvalidPropertyValidationWillNotRunEntityLevelValidation() {
            // Arrange
            OnModelUpdatedModelMultipleParameters model = new OnModelUpdatedModelMultipleParameters { Minimum = 250, Maximum = 100 };
            TestableDefaultModelBinder<OnModelUpdatedModelMultipleParameters> modelBinder = new TestableDefaultModelBinder<OnModelUpdatedModelMultipleParameters>(model);
            modelBinder.ModelState.AddModelError(BASE_MODEL_NAME + ".Minimum", "The minimum value was invalid.");

            // Act
            modelBinder.OnModelUpdated();

            // Assert
            Assert.IsNull(modelBinder.ModelState[BASE_MODEL_NAME]);
        }

        public class AlwaysInvalidAttribute : ValidationAttribute {
            public AlwaysInvalidAttribute() { }
            public AlwaysInvalidAttribute(string message) : base(message) { }

            public override bool IsValid(object value) {
                return false;
            }
        }

        [AlwaysInvalid("The object just isn't right")]
        public class OnModelUpdatedModelNoParameters { }

        [TestMethod]
        public void OnModelUpdatedWithValidationAttributeNoParameters() {
            // Arrange
            TestableDefaultModelBinder<OnModelUpdatedModelNoParameters> modelBinder = new TestableDefaultModelBinder<OnModelUpdatedModelNoParameters>();

            // Act
            modelBinder.OnModelUpdated();

            // Assert
            Assert.AreEqual(1, modelBinder.ModelState.Count);
            ModelState stateModel = modelBinder.ModelState[BASE_MODEL_NAME];
            Assert.IsNotNull(stateModel);
            Assert.AreEqual("The object just isn't right", stateModel.Errors.Single().ErrorMessage);
        }

        [AlwaysInvalid]
        public class OnModelUpdatedModelNoValidationResult { }

        [TestMethod]
        public void OnModelUpdatedWithValidationAttributeNoValidationMessage() {
            // Arrange
            TestableDefaultModelBinder<OnModelUpdatedModelNoValidationResult> modelBinder = new TestableDefaultModelBinder<OnModelUpdatedModelNoValidationResult>();

            // Act
            modelBinder.OnModelUpdated();

            // Assert
            Assert.AreEqual(1, modelBinder.ModelState.Count);
            ModelState stateModel = modelBinder.ModelState[BASE_MODEL_NAME];
            Assert.IsNotNull(stateModel);
            Assert.AreEqual("The field OnModelUpdatedModelNoValidationResult is invalid.", stateModel.Errors.Single().ErrorMessage);
        }

        [TestMethod]
        public void OnModelUpdatedDoesNotPlaceErrorMessagesInModelStateWhenSubPropertiesHaveErrors() {
            // Arrange
            TestableDefaultModelBinder<OnModelUpdatedModelNoValidationResult> modelBinder = new TestableDefaultModelBinder<OnModelUpdatedModelNoValidationResult>();
            modelBinder.ModelState.AddModelError("Foo.Bar", "Foo.Bar is invalid");
            modelBinder.Context.ModelName = "Foo";

            // Act
            modelBinder.OnModelUpdated();

            // Assert
            Assert.IsNull(modelBinder.ModelState["Foo"]);
        }

        // OnPropertyValidating tests

        class OnPropertyValidatingModel {
            public string NotValidated { get; set; }

            [Range(10, 65)]
            public int RangedInteger { get; set; }

            [Required(ErrorMessage = "Custom Required Message")]
            public int RequiredInteger { get; set; }
        }

        [TestMethod]
        public void OnPropertyValidatingWithoutValidationAttribute() {
            // Arrange
            TestableDefaultModelBinder<OnPropertyValidatingModel> modelBinder = new TestableDefaultModelBinder<OnPropertyValidatingModel>();

            // Act
            modelBinder.OnPropertyValidating("NotValidated", 42);

            // Assert
            Assert.IsFalse(modelBinder.ModelState.Any());
        }

        [TestMethod]
        public void OnPropertyValidatingWithValidationAttributePassing() {
            // Arrange
            TestableDefaultModelBinder<OnPropertyValidatingModel> modelBinder = new TestableDefaultModelBinder<OnPropertyValidatingModel>();
            modelBinder.Context.PropertyMetadata["RangedInteger"].Model = 42;

            // Act
            bool result = modelBinder.OnPropertyValidating("RangedInteger", 42);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(modelBinder.ModelState.Any());
        }

        // SetProperty tests

        [TestMethod]
        public void SetPropertyWithRequiredOnValueTypeOnlyResultsInSingleMessage() {    // DDB #225150
            // Arrange
            TestableDefaultModelBinder<OnPropertyValidatingModel> modelBinder = new TestableDefaultModelBinder<OnPropertyValidatingModel>();
            modelBinder.Context.ModelMetadata.Model = null;

            // Act
            modelBinder.SetProperty("RequiredInteger", null);

            // Assert
            ModelState modelState = modelBinder.ModelState[BASE_MODEL_NAME + ".RequiredInteger"];
            ModelError modelStateError = modelState.Errors.Single();
            Assert.AreEqual("Custom Required Message", modelStateError.ErrorMessage);
        }

        [TestMethod]
        public void SetPropertyAddsDefaultMessageForNonBindableNonNullableValueTypes() {
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            try {
                // Arrange
                TestableDefaultModelBinder<List<String>> modelBinder = new TestableDefaultModelBinder<List<String>>();
                modelBinder.Context.ModelMetadata.Model = null;

                // Act
                modelBinder.SetProperty("Count", null);

                // Assert
                ModelState modelState = modelBinder.ModelState[BASE_MODEL_NAME + ".Count"];
                ModelError modelStateError = modelState.Errors.Single();
                Assert.AreEqual("A value is required.", modelStateError.ErrorMessage);
            }
            finally {
                DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = true;
            }
        }

        [TestMethod]
        public void SetPropertyCreatesValueRequiredErrorIfNecessary() {
            // Arrange
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MyModel)),
                ModelName = "theModel",
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(MyModel))["ReadWriteProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicSetProperty(new ControllerContext(), bindingContext, property, null);

            // Assert
            Assert.AreEqual("The ReadWriteProperty field is required.", bindingContext.ModelState["theModel.ReadWriteProperty"].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void SetPropertyWithThrowingSetter() {
            // Arrange
            TestableDefaultModelBinder<SetPropertyModel> binder = new TestableDefaultModelBinder<SetPropertyModel>();

            // Act
            binder.SetProperty("NonNullableString", null);

            // Assert
            ModelState modelState = binder.Context.ModelState[BASE_MODEL_NAME + ".NonNullableString"];
            Assert.AreEqual(1, modelState.Errors.Count);
            Assert.IsInstanceOfType(modelState.Errors[0].Exception, typeof(ArgumentNullException));
        }

        [TestMethod]
        public void SetPropertyWithNullValueAndThrowingSetterWithRequiredAttribute() {    // DDB #227809
            // Arrange
            TestableDefaultModelBinder<SetPropertyModel> binder = new TestableDefaultModelBinder<SetPropertyModel>();

            // Act
            binder.SetProperty("NonNullableStringWithAttribute", null);

            // Assert
            ModelState modelState = binder.Context.ModelState[BASE_MODEL_NAME + ".NonNullableStringWithAttribute"];
            Assert.AreEqual(1, modelState.Errors.Count);
            Assert.AreEqual("My custom required message", modelState.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void SetPropertyDoesNothingIfPropertyIsReadOnly() {
            // Arrange
            MyModel model = new MyModel();
            ModelBindingContext bindingContext = new ModelBindingContext() {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelName = "theModel"
            };

            PropertyDescriptor property = TypeDescriptor.GetProperties(model)["ReadOnlyProperty"];
            DefaultModelBinderHelper helper = new DefaultModelBinderHelper();

            // Act
            helper.PublicSetProperty(new ControllerContext(), bindingContext, property, 42);

            // Assert
            Assert.AreEqual(0, bindingContext.ModelState.Count, "ModelState should remain untouched.");
        }

        [TestMethod]
        public void SetPropertySuccess() {
            // Arrange
            TestableDefaultModelBinder<SetPropertyModel> binder = new TestableDefaultModelBinder<SetPropertyModel>();

            // Act
            binder.SetProperty("NullableString", "The new value");

            // Assert
            Assert.AreEqual("The new value", ((SetPropertyModel)binder.Context.Model).NullableString);
            Assert.AreEqual(0, binder.Context.ModelState.Count, "ModelState should remain untouched.");
        }

        class SetPropertyModel {
            public string NullableString { get; set; }

            public string NonNullableString {
                get {
                    return null;
                }
                set {
                    if (value == null) {
                        throw new ArgumentNullException("value");
                    }
                }
            }

            [Required(ErrorMessage = "My custom required message")]
            public string NonNullableStringWithAttribute {
                get {
                    return null;
                }
                set {
                    if (value == null) {
                        throw new ArgumentNullException("value");
                    }
                }
            }
        }

        // Helper methods

        static PropertyDescriptor GetProperty<T>(string propertyName) {
            return TypeDescriptor.GetProperties(typeof(T))
                                 .Cast<PropertyDescriptor>()
                                 .Where(p => p.Name == propertyName)
                                 .Single();
        }

        static ICustomTypeDescriptor GetType<T>() {
            return TypeDescriptor.GetProvider(typeof(T)).GetTypeDescriptor(typeof(T));
        }

        // Helper classes

        class TestableDefaultModelBinder<TModel> : DefaultModelBinder where TModel : new() {
            public TestableDefaultModelBinder() : this(new TModel()) { }

            public TestableDefaultModelBinder(TModel model) {
                ModelState = new ModelStateDictionary();

                Context = new ModelBindingContext();
                Context.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(TModel));
                Context.ModelName = BASE_MODEL_NAME;
                Context.ModelState = ModelState;
            }

            public ModelBindingContext Context { get; private set; }

            public ModelStateDictionary ModelState { get; set; }

            public void BindProperty(string propertyName) {
                base.BindProperty(new ControllerContext(), Context, GetProperty<TModel>(propertyName));
            }

            public PropertyDescriptorCollection GetModelProperties() {
                return base.GetModelProperties(new ControllerContext(), Context);
            }

            public object GetPropertyValue(string propertyName, object existingValue) {
                Mock<IModelBinder> mockModelBinder = new Mock<IModelBinder>();
                mockModelBinder.Expect(b => b.BindModel(It.IsAny<ControllerContext>(), Context)).Returns(existingValue);
                return base.GetPropertyValue(new ControllerContext(), Context, GetProperty<TModel>(propertyName), mockModelBinder.Object);
            }

            public bool OnPropertyValidating(string propertyName, object value) {
                return base.OnPropertyValidating(new ControllerContext(), Context, GetProperty<TModel>(propertyName), value);
            }

            public void OnPropertyValidated(string propertyName, object value) {
                base.OnPropertyValidated(new ControllerContext(), Context, GetProperty<TModel>(propertyName), value);
            }

            public void OnModelUpdated() {
                base.OnModelUpdated(new ControllerContext(), Context);
            }

            public void SetProperty(string propertyName, object value) {
                base.SetProperty(new ControllerContext(), Context, GetProperty<TModel>(propertyName), value);
            }
        }
    }
}
