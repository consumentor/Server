namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using System.Web.UI.WebControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class TemplateHelpersTest {

        // ExecuteTemplate

        private class ExecuteTemplateModel {
            public string MyProperty { get; set; }
            public Nullable<int> MyNullableProperty { get; set; }
        }

        [TestMethod]
        public void ExecuteTemplateCallsGetViewNamesWithProvidedTemplateNameAndMetadataInformation() {
            using (new MockViewEngine()) {
                // Arrange
                HtmlHelper html = MakeHtmlHelper(new ExecuteTemplateModel());
                ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
                metadata.TemplateHint = "templateHint";
                metadata.DataTypeName = "dataType";
                string[] hints = null;

                // Act
                TemplateHelpers.ExecuteTemplate(html, MakeViewData(html, metadata), "templateName", DataBoundControlMode.ReadOnly,
                    (_metadata, _hints) => { hints = _hints; return new[] { "String" }; });

                // Assert
                Assert.IsNotNull(hints);
                Assert.AreEqual(3, hints.Length);
                Assert.AreEqual("templateName", hints[0]);
                Assert.AreEqual("templateHint", hints[1]);
                Assert.AreEqual("dataType", hints[2]);
            }
        }

        [TestMethod]
        public void ExecuteTemplateUsesViewFromViewEngineInReadOnlyMode() {
            using (MockViewEngine engine = new MockViewEngine()) {
                // Arrange
                HtmlHelper html = MakeHtmlHelper(new ExecuteTemplateModel { MyProperty = "Hello" });
                ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
                ViewContext callbackViewContext = null;
                engine.Engine.Expect(e => e.FindPartialView(html.ViewContext, "DisplayTemplates/String", true))
                             .Returns(new ViewEngineResult(engine.View.Object, engine.Engine.Object))
                             .Verifiable();
                engine.View.Expect(v => v.Render(It.IsAny<ViewContext>(), It.IsAny<TextWriter>()))
                           .Callback<ViewContext, TextWriter>((vc, tw) => { callbackViewContext = vc; tw.Write("View Text"); })
                           .Verifiable();
                ViewDataDictionary viewData = MakeViewData(html, metadata);

                // Act
                string result = TemplateHelpers.ExecuteTemplate(html, viewData, "templateName", DataBoundControlMode.ReadOnly,
                    delegate { return new[] { "String" }; });

                // Assert
                engine.Engine.Verify();
                engine.View.Verify();
                Assert.AreEqual("View Text", result);
                Assert.AreSame(engine.View.Object, callbackViewContext.View);
                Assert.AreSame(viewData, callbackViewContext.ViewData);
                Assert.AreSame(html.ViewContext.TempData, callbackViewContext.TempData);
                TemplateHelpers.ActionCacheViewItem cacheItem = TemplateHelpers.GetActionCache(html)["DisplayTemplates/String"] as TemplateHelpers.ActionCacheViewItem;
                Assert.IsNotNull(cacheItem);
                Assert.AreEqual("DisplayTemplates/String", cacheItem.ViewName);
            }
        }

        [TestMethod]
        public void ExecuteTemplateUsesViewFromViewEngineInEditMode() {
            using (MockViewEngine engine = new MockViewEngine()) {
                // Arrange
                HtmlHelper html = MakeHtmlHelper(new ExecuteTemplateModel { MyProperty = "Hello" });
                ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
                ViewContext callbackViewContext = null;
                engine.Engine.Expect(e => e.FindPartialView(html.ViewContext, "EditorTemplates/String", true))
                             .Returns(new ViewEngineResult(engine.View.Object, engine.Engine.Object))
                             .Verifiable();
                engine.View.Expect(v => v.Render(It.IsAny<ViewContext>(), It.IsAny<TextWriter>()))
                           .Callback<ViewContext, TextWriter>((vc, tw) => { callbackViewContext = vc; tw.Write("View Text"); })
                           .Verifiable();
                ViewDataDictionary viewData = MakeViewData(html, metadata);

                // Act
                string result = TemplateHelpers.ExecuteTemplate(html, viewData, "templateName", DataBoundControlMode.Edit,
                    delegate { return new[] { "String" }; });

                // Assert
                engine.Engine.Verify();
                engine.View.Verify();
                Assert.AreEqual("View Text", result);
                Assert.AreSame(engine.View.Object, callbackViewContext.View);
                Assert.AreSame(viewData, callbackViewContext.ViewData);
                Assert.AreSame(html.ViewContext.TempData, callbackViewContext.TempData);
                TemplateHelpers.ActionCacheViewItem cacheItem = TemplateHelpers.GetActionCache(html)["EditorTemplates/String"] as TemplateHelpers.ActionCacheViewItem;
                Assert.IsNotNull(cacheItem);
                Assert.AreEqual("EditorTemplates/String", cacheItem.ViewName);
            }
        }

        [TestMethod]
        public void ExecuteTemplateUsesViewFromDefaultActionsInReadOnlyMode() {
            using (MockViewEngine engine = new MockViewEngine()) {
                // Arrange
                HtmlHelper html = MakeHtmlHelper(new ExecuteTemplateModel { MyProperty = "Hello" });
                ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
                engine.Engine.Expect(e => e.FindPartialView(html.ViewContext, "DisplayTemplates/String", It.IsAny<bool>()))
                             .Returns(new ViewEngineResult(new string[0]))
                             .Verifiable();
                ViewDataDictionary viewData = MakeViewData(html, metadata);

                // Act
                TemplateHelpers.ExecuteTemplate(html, viewData, "templateName", DataBoundControlMode.ReadOnly,
                    delegate { return new[] { "String" }; });

                // Assert
                engine.Engine.Verify();
                TemplateHelpers.ActionCacheCodeItem cacheItem = TemplateHelpers.GetActionCache(html)["DisplayTemplates/String"] as TemplateHelpers.ActionCacheCodeItem;
                Assert.IsNotNull(cacheItem);
                Assert.AreEqual(DefaultDisplayTemplates.StringTemplate, cacheItem.Action);
            }
        }

        [TestMethod]
        public void ExecuteTemplateUsesViewFromDefaultActionsInEditMode() {
            using (MockViewEngine engine = new MockViewEngine()) {
                // Arrange
                HtmlHelper html = MakeHtmlHelper(new ExecuteTemplateModel { MyProperty = "Hello" });
                ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
                engine.Engine.Expect(e => e.FindPartialView(html.ViewContext, "EditorTemplates/String", It.IsAny<bool>()))
                             .Returns(new ViewEngineResult(new string[0]))
                             .Verifiable();
                ViewDataDictionary viewData = MakeViewData(html, metadata);

                // Act
                TemplateHelpers.ExecuteTemplate(html, viewData, "templateName", DataBoundControlMode.Edit,
                    delegate { return new[] { "String" }; });

                // Assert
                engine.Engine.Verify();
                TemplateHelpers.ActionCacheCodeItem cacheItem = TemplateHelpers.GetActionCache(html)["EditorTemplates/String"] as TemplateHelpers.ActionCacheCodeItem;
                Assert.IsNotNull(cacheItem);
                Assert.AreEqual(DefaultEditorTemplates.StringTemplate, cacheItem.Action);
            }
        }

        [TestMethod]
        public void ExecuteTemplatePrefersExistingActionCacheItem() {
            using (MockViewEngine engine = new MockViewEngine()) {
                // Arrange
                HtmlHelper html = MakeHtmlHelper(new ExecuteTemplateModel { MyProperty = "Hello" });
                ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
                engine.Engine.Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                             .Never();
                ViewDataDictionary viewData = MakeViewData(html, metadata);
                TemplateHelpers.GetActionCache(html).Add("EditorTemplates/String",
                                                         new TemplateHelpers.ActionCacheCodeItem { Action = _ => "Action Text" });

                // Act
                string result = TemplateHelpers.ExecuteTemplate(html, viewData, "templateName", DataBoundControlMode.Edit,
                    delegate { return new[] { "String" }; });

                // Assert
                engine.Engine.Verify();
                Assert.AreEqual("Action Text", result);
            }
        }

        [TestMethod]
        public void ExecuteTemplateThrowsWhenNoTemplatesMatch() {
            using (new MockViewEngine()) {
                // Arrange
                HtmlHelper html = MakeHtmlHelper(new ExecuteTemplateModel { MyProperty = "Hello" });
                ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
                ViewDataDictionary viewData = MakeViewData(html, metadata);

                // Act & Assert
                ExceptionHelper.ExpectInvalidOperationException(
                    () => TemplateHelpers.ExecuteTemplate(html, viewData, "templateName", DataBoundControlMode.Edit, delegate { return new string[0]; }),
                    "Unable to locate an appropriate template for type System.String.");
            }
        }

        // GetActionCache

        [TestMethod]
        public void CacheIsCreatedIfNotPresent() {
            // Arrange
            Hashtable items = new Hashtable();
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.Expect(c => c.Items).Returns(items);
            Mock<ViewContext> viewContext = new Mock<ViewContext>();
            viewContext.Expect(c => c.HttpContext).Returns(context.Object);
            Mock<IViewDataContainer> viewDataContainer = new Mock<IViewDataContainer>();
            HtmlHelper helper = new HtmlHelper(viewContext.Object, viewDataContainer.Object);

            // Act
            Dictionary<string, TemplateHelpers.ActionCacheItem> cache = TemplateHelpers.GetActionCache(helper);

            // Assert
            Assert.IsNotNull(cache);
            Assert.AreEqual(0, cache.Count);
            CollectionAssert.Contains(items.Values, cache);
        }

        [TestMethod]
        public void CacheIsReusedIfPresent() {
            // Arrange
            Hashtable items = new Hashtable();
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.Expect(c => c.Items).Returns(items);
            Mock<ViewContext> viewContext = new Mock<ViewContext>();
            viewContext.Expect(c => c.HttpContext).Returns(context.Object);
            Mock<IViewDataContainer> viewDataContainer = new Mock<IViewDataContainer>();
            HtmlHelper helper = new HtmlHelper(viewContext.Object, viewDataContainer.Object);

            // Act
            Dictionary<string, TemplateHelpers.ActionCacheItem> cache1 = TemplateHelpers.GetActionCache(helper);
            Dictionary<string, TemplateHelpers.ActionCacheItem> cache2 = TemplateHelpers.GetActionCache(helper);

            // Assert
            Assert.IsNotNull(cache1);
            Assert.AreSame(cache1, cache2);
        }

        // GetViewNames

        [TestMethod]
        public void GetViewNamesFullOrderingOfBuiltInValueType() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(double));

            // Act
            List<string> result = TemplateHelpers.GetViewNames(metadata, "UIHint", "DataType").ToList();

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("UIHint", result[0]);
            Assert.AreEqual("DataType", result[1]);
            Assert.AreEqual("Double", result[2]);
            Assert.AreEqual("String", result[3]);
        }

        [TestMethod]
        public void GetViewNamesFullOrderingOfComplexType() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(System.Net.HttpWebRequest));

            // Act
            List<string> result = TemplateHelpers.GetViewNames(metadata, "UIHint", "DataType").ToList();

            // Assert
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual("UIHint", result[0]);
            Assert.AreEqual("DataType", result[1]);
            Assert.AreEqual("HttpWebRequest", result[2]);
            Assert.AreEqual("WebRequest", result[3]);
            Assert.AreEqual("MarshalByRefObject", result[4]);
            Assert.AreEqual("Object", result[5]);
        }

        [TestMethod]
        public void GetViewNamesFullOrderingOfInterface() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IDisposable));

            // Act
            List<string> result = TemplateHelpers.GetViewNames(metadata, "UIHint", "DataType").ToList();

            // Assert
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("UIHint", result[0]);
            Assert.AreEqual("DataType", result[1]);
            Assert.AreEqual("IDisposable", result[2]);
            Assert.AreEqual("Object", result[3]);
        }

        [TestMethod]
        public void GetViewNamesFullOrderingOfComplexTypeThatImplementsIEnumerable() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(List<int>));

            // Act
            List<string> result = TemplateHelpers.GetViewNames(metadata, "UIHint", "DataType").ToList();

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("UIHint", result[0]);
            Assert.AreEqual("DataType", result[1]);
            Assert.AreEqual("List`1", result[2]);
            Assert.AreEqual("Collection", result[3]);
            Assert.AreEqual("Object", result[4]);
        }

        [TestMethod]
        public void GetViewNamesFullOrderingOfInterfaceThatRequiresIEnumerable() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(IList<int>));

            // Act
            List<string> result = TemplateHelpers.GetViewNames(metadata, "UIHint", "DataType").ToList();

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual("UIHint", result[0]);
            Assert.AreEqual("DataType", result[1]);
            Assert.AreEqual("IList`1", result[2]);
            Assert.AreEqual("Collection", result[3]);
            Assert.AreEqual("Object", result[4]);
        }

        [TestMethod]
        public void GetViewNamesNullUIHintNotIncludedInList() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(Object));

            // Act
            List<string> result = TemplateHelpers.GetViewNames(metadata, null, "DataType").ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("DataType", result[0]);
            Assert.AreEqual("Object", result[1]);
        }

        [TestMethod]
        public void GetViewNamesNullDataTypeNotIncludedInList() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(Object));

            // Act
            List<string> result = TemplateHelpers.GetViewNames(metadata, "UIHint", null).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("UIHint", result[0]);
            Assert.AreEqual("Object", result[1]);
        }

        [TestMethod]
        public void GetViewNamesConvertsNullableOfTIntoT() {
            // Arrange
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(Nullable<int>));

            // Act
            List<string> result = TemplateHelpers.GetViewNames(metadata, null, null).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Int32", result[0]);
            Assert.AreEqual("String", result[1]);
        }

        // Template

        private class TemplateModel {
            public object MyProperty { get; set; }
        }

        [TestMethod]
        public void TemplateNullExpressionThrows() {
            // Arrange
            HtmlHelper<object> html = MakeHtmlHelper<object>(null);

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => TemplateHelpers.Template(html, null, "templateName", "htmlFieldName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy),
                "expression");
        }

        [TestMethod]
        public void TemplateDataNotFound() {
            // Arrange
            HtmlHelper<object> html = MakeHtmlHelper<object>(null);

            // Act
            string result = TemplateHelpers.Template(html, "UnknownObject", "templateName", null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.String, RealModelType = System.String, PropertyName = (null), HtmlFieldName = UnknownObject, TemplateName = templateName, Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateHtmlFieldNameReplacesExpression() {
            // Arrange
            HtmlHelper<object> html = MakeHtmlHelper<object>(null);

            // Act
            string result = TemplateHelpers.Template(html, "UnknownObject", "templateName", "htmlFieldName", DataBoundControlMode.ReadOnly, new { foo = "Bar" }, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.String, RealModelType = System.String, PropertyName = (null), HtmlFieldName = htmlFieldName, TemplateName = templateName, Mode = ReadOnly, AdditionalViewData = { foo: Bar }", result);
        }

        [TestMethod]
        public void TemplateDataFoundInViewDataDictionaryHasNoPropertyName() {
            // Arrange
            HtmlHelper<object> html = MakeHtmlHelper<object>(null);
            html.ViewContext.ViewData["Key"] = 42;

            // Act
            string result = TemplateHelpers.Template(html, "Key", null, null, DataBoundControlMode.Edit, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = 42, ModelType = System.Int32, RealModelType = System.Int32, PropertyName = (null), HtmlFieldName = Key, TemplateName = (null), Mode = Edit, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateDataFoundInViewDataDictionarySubPropertyHasPropertyName() {
            // Arrange
            HtmlHelper<object> html = MakeHtmlHelper<object>(null);
            html.ViewContext.ViewData["Key"] = new TemplateModel { MyProperty = "Hello!" };

            // Act
            string result = TemplateHelpers.Template(html, "Key.MyProperty", null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = Hello!, ModelType = System.Object, RealModelType = System.String, PropertyName = MyProperty, HtmlFieldName = Key.MyProperty, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateDataFoundInModelHasPropertyName() {
            // Arrange
            HtmlHelper<TemplateModel> html = MakeHtmlHelper<TemplateModel>(new TemplateModel { MyProperty = "Hello!" });

            // Act
            string result = TemplateHelpers.Template(html, "MyProperty", null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = Hello!, ModelType = System.Object, RealModelType = System.String, PropertyName = MyProperty, HtmlFieldName = MyProperty, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateNullDataFoundInModelHasPropertyTypeInsteadOfActualModelType() {
            // Arrange
            HtmlHelper<TemplateModel> html = MakeHtmlHelper<TemplateModel>(new TemplateModel());

            // Act
            string result = TemplateHelpers.Template(html, "MyProperty", null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.Object, RealModelType = System.Object, PropertyName = MyProperty, HtmlFieldName = MyProperty, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        // TemplateFor

        private class TemplateForModel {
            public int MyField = 0;
            public object MyProperty { get; set; }
            public Nullable<int> MyNullableProperty { get; set; }
        }

        [TestMethod]
        public void TemplateForNonUnsupportedExpressionTypeThrows() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper<TemplateForModel>(null);

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => TemplateHelpers.TemplateFor(html, model => new Object(), "templateName", "htmlFieldName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy),
                "Templates can be used only with field access, property access, single-dimension array index, or single-parameter custom indexer expressions.");
        }

        [TestMethod]
        public void TemplateForWithNonNullExpression() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper(new TemplateForModel { MyProperty = "Hello!" });

            // Act
            string result = TemplateHelpers.TemplateFor(html, model => model.MyProperty, "templateName", null, DataBoundControlMode.ReadOnly, new { foo = "Bar" }, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = Hello!, ModelType = System.Object, RealModelType = System.String, PropertyName = MyProperty, HtmlFieldName = MyProperty, TemplateName = templateName, Mode = ReadOnly, AdditionalViewData = { foo: Bar }", result);
        }

        [TestMethod]
        public void TemplateForHtmlFieldNameReplacesExpression() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper(new TemplateForModel { MyProperty = "Hello!" });

            // Act
            string result = TemplateHelpers.TemplateFor(html, model => model.MyProperty, "templateName", "htmlFieldName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = Hello!, ModelType = System.Object, RealModelType = System.String, PropertyName = MyProperty, HtmlFieldName = htmlFieldName, TemplateName = templateName, Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateForNullModelStillRetainsTypeInformation() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper<TemplateForModel>(null);

            // Act
            string result = TemplateHelpers.TemplateFor(html, model => model.MyProperty, null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.Object, RealModelType = System.Object, PropertyName = MyProperty, HtmlFieldName = MyProperty, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateForNullPropertyStillRetainsTypeInformation() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper(new TemplateForModel());

            // Act
            string result = TemplateHelpers.TemplateFor(html, model => model.MyProperty, null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.Object, RealModelType = System.Object, PropertyName = MyProperty, HtmlFieldName = MyProperty, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateForNullableValueTYpePropertyRetainsNullableValueTYpeForNullPropertyValue() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper(new TemplateForModel());

            // Act
            string result = TemplateHelpers.TemplateFor(html, model => model.MyNullableProperty, null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], RealModelType = System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], PropertyName = MyNullableProperty, HtmlFieldName = MyNullableProperty, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateForNullableValueTypePropertyRetainsNullableValueTypeForNonNullPropertyValue() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper(new TemplateForModel { MyNullableProperty = 42 });

            // Act
            string result = TemplateHelpers.TemplateFor(html, model => model.MyNullableProperty, null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = 42, ModelType = System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], RealModelType = System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], PropertyName = MyNullableProperty, HtmlFieldName = MyNullableProperty, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateForWithParameterExpression() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper(new TemplateForModel());

            // Act
            string result = TemplateHelpers.TemplateFor(html, model => model, null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = System.Web.Mvc.Html.Test.TemplateHelpersTest+TemplateForModel, ModelType = System.Web.Mvc.Html.Test.TemplateHelpersTest+TemplateForModel, RealModelType = System.Web.Mvc.Html.Test.TemplateHelpersTest+TemplateForModel, PropertyName = (null), HtmlFieldName = (empty), TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        [TestMethod]
        public void TemplateForWithFieldExpression() {
            // Arrange
            HtmlHelper<TemplateForModel> html = MakeHtmlHelper(new TemplateForModel { MyField = 42 });

            // Act
            string result = TemplateHelpers.TemplateFor(html, model => model.MyField, null, null, DataBoundControlMode.ReadOnly, null /* additionalViewData */, TemplateHelperSpy);

            // Assert
            Assert.AreEqual("Model = 42, ModelType = System.Int32, RealModelType = System.Int32, PropertyName = (null), HtmlFieldName = MyField, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)", result);
        }

        // TemplateHelper

        private class TemplateHelperModel {
            public string MyProperty { get; set; }
        }

        [TestMethod]
        public void TemplateHelperNonNullNonEmptyStringModel() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel { MyProperty = "Hello" });
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = Hello, ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = Hello, HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = ReadOnly", result);
        }

        [TestMethod]
        public void TemplateHelperEmptyStringModel() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel { MyProperty = "" });
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            metadata.ConvertEmptyStringToNull = false;

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = , ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = , HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = ReadOnly", result);
        }

        [TestMethod]
        public void TemplateHelperConvertsEmptyStringsToNull() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel { MyProperty = "" });
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            metadata.ConvertEmptyStringToNull = true;

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = , HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = ReadOnly", result);
        }

        [TestMethod]
        public void TemplateHelperConvertsNullModelsToNullDisplayTextInReadOnlyMode() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            metadata.NullDisplayText = "NullDisplayText";

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = NullDisplayText, HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = ReadOnly", result);
        }

        [TestMethod]
        public void TemplateHelperDoesNotConvertNullModelsToNullDisplayTextInEditMode() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            metadata.NullDisplayText = "NullDisplayText";

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.Edit, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = , HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = Edit", result);
        }

        [TestMethod]
        public void TemplateHelperAppliesDisplayFormatStringInReadOnlyMode() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel { MyProperty = "Hello" });
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            metadata.DisplayFormatString = "{0} world!";

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = Hello, ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = Hello world!, HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = ReadOnly", result);
        }

        [TestMethod]
        public void TemplateHelperDoesNotApplyDisplayFormatStringInReadOnlyModeForNullModel() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            metadata.DisplayFormatString = "{0} world!";

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = , HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = ReadOnly", result);
        }

        [TestMethod]
        public void TemplateHelperAppliesEditFormatStringInEditMode() {
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel { MyProperty = "Hello" });
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            metadata.EditFormatString = "{0} world!";

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.Edit, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = Hello, ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = Hello world!, HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = Edit", result);
        }

        [TestMethod]
        public void TemplateHelperDoesNotApplyEditFormatStringInEditModeForNullModel() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            metadata.EditFormatString = "{0} world!";

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.Edit, null /* additionalViewData */, ExecuteTemplateSpy);

            // Assert
            Assert.AreEqual("Model = (null), ModelType = System.String, RealModelType = System.String, PropertyName = MyProperty, FormattedModelValue = , HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = Edit", result);
        }

        [TestMethod]
        public void TemplateHelperAddsNonNullModelToVisitedObjects() {    // DDB #224750
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel { MyProperty = "Hello" });
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            ViewDataDictionary viewData = null;

            // Act
            TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */,
                (_html, _viewData, _templateName, _mode, _getViews) => { viewData = _viewData; return String.Empty; });

            // Assert
            Assert.IsNotNull(viewData);
            Assert.IsTrue(viewData.TemplateInfo.VisitedObjects.Contains("Hello"));
        }

        [TestMethod]
        public void TemplateHelperAddsNullModelsTypeToVisitedObjects() {    // DDB #224750
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            ViewDataDictionary viewData = null;

            // Act
            TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */,
                (_html, _viewData, _templateName, _mode, _getViews) => { viewData = _viewData; return String.Empty; });

            // Assert
            Assert.IsNotNull(viewData);
            Assert.IsTrue(viewData.TemplateInfo.VisitedObjects.Contains(typeof(string)));
        }

        [TestMethod]
        public void TemplateHelperReturnsEmptyStringForAlreadyVisitedObject() {    // DDB #224750
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel { MyProperty = "Hello" });
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            html.ViewData.TemplateInfo.VisitedObjects.Add("Hello");

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */);

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void TemplateHelperReturnsEmptyStringForAlreadyVisitedType() {    // DDB #224750
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            html.ViewData.TemplateInfo.VisitedObjects.Add(typeof(string));

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */);

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void TemplateHelperPreservesSameInstanceOfModelMetadata() {    // DDB #225858
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            ViewDataDictionary callbackViewData = null;

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */,
                (_html, _viewData, _templateName, _mode, _getViewNames) => { callbackViewData = _viewData; return String.Empty; });

            // Assert
            Assert.IsNotNull(callbackViewData);
            Assert.AreSame(metadata, callbackViewData.ModelMetadata);
        }

        [TestMethod]
        public void TemplateHelperFormatsValuesUsingCurrentCulture() {
            CultureInfo existingCulture = Thread.CurrentThread.CurrentCulture;

            try {
                // Arrange
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("es-PR");
                HtmlHelper html = MakeHtmlHelper(new { MyProperty = new DateTime(2009, 11, 18, 16, 12, 8, DateTimeKind.Utc) });
                ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
                metadata.DisplayFormatString = "{0:F}";

                // Act
                string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */, ExecuteTemplateSpy);

                // Assert
                Assert.AreEqual("Model = 18/11/2009 04:12:08 p.m., ModelType = System.DateTime, RealModelType = System.DateTime, PropertyName = MyProperty, FormattedModelValue = miércoles, 18 de noviembre de 2009 04:12:08 p.m., HtmlFieldPrefix = FieldPrefix.htmlFieldName, TemplateName = templateName, Mode = ReadOnly", result);
            }
            finally {
                Thread.CurrentThread.CurrentCulture = existingCulture;
            }
        }

        [TestMethod]
        public void TemplateHelperPreservesExistingViewData() {
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            html.ViewData["Foo"] = "Bar";
            html.ViewData["Baz"] = 42;
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            ViewDataDictionary callbackViewData = null;

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, null /* additionalViewData */,
                (_html, _viewData, _templateName, _mode, _getViewNames) => { callbackViewData = _viewData; return String.Empty; });

            // Assert
            Assert.AreNotSame(html.ViewData, callbackViewData);
            Assert.AreEqual(2, callbackViewData.Count);
            Assert.AreEqual("Bar", callbackViewData["Foo"]);
            Assert.AreEqual(42, callbackViewData["Baz"]);
        }

        [TestMethod]
        public void TemplateHelperMergesAdditionalViewData() {
            HtmlHelper html = MakeHtmlHelper(new TemplateHelperModel());
            html.ViewData["Foo"] = "Bar";
            html.ViewData["Baz"] = 42;
            ModelMetadata metadata = ModelMetadata.FromStringExpression("MyProperty", html.ViewData);
            ViewDataDictionary callbackViewData = null;

            // Act
            string result = TemplateHelpers.TemplateHelper(html, metadata, "htmlFieldName", "templateName", DataBoundControlMode.ReadOnly, new { foo = "New Foo", hello = "World!" },
                (_html, _viewData, _templateName, _mode, _getViewNames) => { callbackViewData = _viewData; return String.Empty; });

            // Assert
            Assert.AreNotSame(html.ViewData, callbackViewData);
            Assert.AreEqual(3, callbackViewData.Count);
            Assert.AreEqual("New Foo", callbackViewData["Foo"]);
            Assert.AreEqual(42, callbackViewData["Baz"]);
            Assert.AreEqual("World!", callbackViewData["Hello"]);
        }

        // Helpers

        private static string ExecuteTemplateSpy(HtmlHelper html, ViewDataDictionary viewData, string templateName, DataBoundControlMode mode, TemplateHelpers.GetViewNamesDelegate getViewNames) {
            Assert.AreSame(viewData.Model, viewData.ModelMetadata.Model);
            Assert.AreEqual<TemplateHelpers.GetViewNamesDelegate>(TemplateHelpers.GetViewNames, getViewNames);

            return String.Format("Model = {0}, ModelType = {1}, RealModelType = {2}, PropertyName = {3}, FormattedModelValue = {4}, HtmlFieldPrefix = {5}, TemplateName = {6}, Mode = {7}",
                                 viewData.ModelMetadata.Model ?? "(null)",
                                 viewData.ModelMetadata.ModelType == null ? "(null)" : viewData.ModelMetadata.ModelType.FullName,
                                 viewData.ModelMetadata.RealModelType == null ? "(null)" : viewData.ModelMetadata.RealModelType.FullName,
                                 viewData.ModelMetadata.PropertyName ?? "(null)",
                                 viewData.TemplateInfo.FormattedModelValue ?? "(null)",
                                 viewData.TemplateInfo.HtmlFieldPrefix == "" ? "(empty)" : viewData.TemplateInfo.HtmlFieldPrefix ?? "(null)",
                                 templateName ?? "(null)",
                                 mode);
        }

        private static string TemplateHelperSpy(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string templateName, DataBoundControlMode mode, object additionalViewData) {
            return String.Format("Model = {0}, ModelType = {1}, RealModelType = {2}, PropertyName = {3}, HtmlFieldName = {4}, TemplateName = {5}, Mode = {6}, AdditionalViewData = {7}",
                                 metadata.Model ?? "(null)",
                                 metadata.ModelType == null ? "(null)" : metadata.ModelType.FullName,
                                 metadata.RealModelType == null ? "(null)" : metadata.RealModelType.FullName,
                                 metadata.PropertyName ?? "(null)",
                                 htmlFieldName == String.Empty ? "(empty)" : htmlFieldName ?? "(null)",
                                 templateName ?? "(null)",
                                 mode,
                                 AnonymousObject.Inspect(additionalViewData));
        }

        private HtmlHelper<TModel> MakeHtmlHelper<TModel>(TModel model) {
            return MakeHtmlHelper<TModel>(model, model);
        }

        private HtmlHelper<TModel> MakeHtmlHelper<TModel>(TModel model, object formattedModelValue) {
            ViewDataDictionary viewData = new ViewDataDictionary(model);
            viewData.TemplateInfo.HtmlFieldPrefix = "FieldPrefix";
            viewData.TemplateInfo.FormattedModelValue = formattedModelValue;
            viewData.ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => model, typeof(TModel));

            Mock<HttpContextBase> httpContext = new Mock<HttpContextBase>();
            httpContext.Expect(c => c.Items).Returns(new Hashtable());

            Mock<ViewContext> viewContext = new Mock<ViewContext> { CallBase = true };
            viewContext.Expect(c => c.View).Returns(new DummyView());
            viewContext.Expect(c => c.ViewData).Returns(viewData);
            viewContext.Expect(c => c.HttpContext).Returns(httpContext.Object);
            viewContext.Expect(c => c.RouteData).Returns(new RouteData());
            viewContext.Expect(c => c.TempData).Returns(new TempDataDictionary());
            viewContext.Expect(c => c.Writer).Returns(TextWriter.Null);

            return new HtmlHelper<TModel>(viewContext.Object, new SimpleViewDataContainer(viewData));
        }

        private ViewDataDictionary MakeViewData(HtmlHelper html, ModelMetadata metadata) {
            return new ViewDataDictionary(html.ViewDataContainer.ViewData) {
                Model = metadata.Model,
                ModelMetadata = metadata,
                TemplateInfo = new TemplateInfo {
                    FormattedModelValue = metadata.Model,
                    HtmlFieldPrefix = "FieldPrefix",
                }
            };
        }

        private class DummyView : IView {
            public void Render(ViewContext viewContext, TextWriter writer) {
                throw new NotImplementedException();
            }
        }

        private class MockViewEngine : IDisposable {
            List<IViewEngine> oldEngines;

            public MockViewEngine() {
                oldEngines = ViewEngines.Engines.ToList();

                View = new Mock<IView>();

                Engine = new Mock<IViewEngine>();
                Engine.Expect(e => e.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                      .Returns(new ViewEngineResult(View.Object, Engine.Object));

                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(Engine.Object);
            }

            public void Dispose() {
                ViewEngines.Engines.Clear();

                foreach (IViewEngine engine in oldEngines) {
                    ViewEngines.Engines.Add(engine);
                }
            }

            public Mock<IViewEngine> Engine { get; set; }

            public Mock<IView> View { get; set; }
        }
    }
}
