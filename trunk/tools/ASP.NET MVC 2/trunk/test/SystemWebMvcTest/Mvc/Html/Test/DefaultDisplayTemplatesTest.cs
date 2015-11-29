namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Objects.DataClasses;
    using System.IO;
    using System.Web.TestUtil;
    using System.Web.UI.WebControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class DefaultDisplayTemplatesTest {

        // BooleanTemplate

        [TestMethod]
        public void BooleanTemplateTests() {
            // Boolean values

            Assert.AreEqual(
                @"<input checked=""checked"" class=""check-box"" disabled=""disabled"" type=""checkbox"" />",
                DefaultDisplayTemplates.BooleanTemplate(MakeHtmlHelper<bool>(true)));

            Assert.AreEqual(
                @"<input class=""check-box"" disabled=""disabled"" type=""checkbox"" />",
                DefaultDisplayTemplates.BooleanTemplate(MakeHtmlHelper<bool>(false)));

            Assert.AreEqual(
                @"<input class=""check-box"" disabled=""disabled"" type=""checkbox"" />",
                DefaultDisplayTemplates.BooleanTemplate(MakeHtmlHelper<bool>(null)));

            // Nullable<Boolean> values

            Assert.AreEqual(
                @"<select class=""tri-state list-box"" disabled=""disabled""><option value="""">Not Set</option><option selected=""selected"" value=""true"">True</option><option value=""false"">False</option></select>",
                DefaultDisplayTemplates.BooleanTemplate(MakeHtmlHelper<Nullable<bool>>(true)));

            Assert.AreEqual(
                @"<select class=""tri-state list-box"" disabled=""disabled""><option value="""">Not Set</option><option value=""true"">True</option><option selected=""selected"" value=""false"">False</option></select>",
                DefaultDisplayTemplates.BooleanTemplate(MakeHtmlHelper<Nullable<bool>>(false)));

            Assert.AreEqual(
                @"<select class=""tri-state list-box"" disabled=""disabled""><option selected=""selected"" value="""">Not Set</option><option value=""true"">True</option><option value=""false"">False</option></select>",
                DefaultDisplayTemplates.BooleanTemplate(MakeHtmlHelper<Nullable<bool>>(null)));
        }

        // CollectionTemplate

        private static string CollectionSpyCallback(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string templateName, DataBoundControlMode mode, object additionalViewData) {
            return String.Format(Environment.NewLine + "Model = {0}, ModelType = {1}, PropertyName = {2}, HtmlFieldName = {3}, TemplateName = {4}, Mode = {5}, TemplateInfo.HtmlFieldPrefix = {6}, AdditionalViewData = {7}",
                                 metadata.Model ?? "(null)",
                                 metadata.ModelType == null ? "(null)" : metadata.ModelType.FullName,
                                 metadata.PropertyName ?? "(null)",
                                 htmlFieldName == String.Empty ? "(empty)" : htmlFieldName ?? "(null)",
                                 templateName ?? "(null)",
                                 mode,
                                 html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix,
                                 AnonymousObject.Inspect(additionalViewData));
        }

        [TestMethod]
        public void CollectionTemplateWithNullModel() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<object>(null);

            // Act
            string result = DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback);

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void CollectionTemplateNonEnumerableModelThrows() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<object>(new object());

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                () => DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback),
                "The Collection template was used with an object of type 'System.Object', which does not implement System.IEnumerable."
            );
        }

        [TestMethod]
        public void CollectionTemplateWithSingleItemCollectionWithoutPrefix() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<List<string>>(new List<string> { "foo" });
            html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = null;

            // Act
            string result = DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback);

            // Assert
            Assert.AreEqual(@"
Model = foo, ModelType = System.String, PropertyName = (null), HtmlFieldName = [0], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)",
                            result);
        }

        [TestMethod]
        public void CollectionTemplateWithSingleItemCollectionWithPrefix() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<List<string>>(new List<string> { "foo" });
            html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "ModelProperty";

            // Act
            string result = DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback);

            // Assert
            Assert.AreEqual(@"
Model = foo, ModelType = System.String, PropertyName = (null), HtmlFieldName = ModelProperty[0], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)",
                            result);
        }

        [TestMethod]
        public void CollectionTemplateWithMultiItemCollection() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<List<string>>(new List<string> { "foo", "bar", "baz" });
            html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = null;

            // Act
            string result = DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback);

            // Assert
            Assert.AreEqual(@"
Model = foo, ModelType = System.String, PropertyName = (null), HtmlFieldName = [0], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)
Model = bar, ModelType = System.String, PropertyName = (null), HtmlFieldName = [1], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)
Model = baz, ModelType = System.String, PropertyName = (null), HtmlFieldName = [2], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)",
                            result);
        }

        [TestMethod]
        public void CollectionTemplateNullITemInWeaklyTypedCollectionUsesModelTypeOfString() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<ArrayList>(new ArrayList { null });
            html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = null;

            // Act
            string result = DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback);

            // Assert
            Assert.AreEqual(@"
Model = (null), ModelType = System.String, PropertyName = (null), HtmlFieldName = [0], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)",
                            result);
        }

        [TestMethod]
        public void CollectionTemplateNullItemInStronglyTypedCollectionUsesModelTypeFromIEnumerable() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<List<IHttpHandler>>(new List<IHttpHandler> { null });
            html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = null;

            // Act
            string result = DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback);

            // Assert
            Assert.AreEqual(@"
Model = (null), ModelType = System.Web.IHttpHandler, PropertyName = (null), HtmlFieldName = [0], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)",
                            result);
        }

        [TestMethod]
        public void CollectionTemplateUsesRealObjectTypes() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<List<Object>>(new List<Object> { 1, 2.3, "Hello World" });
            html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = null;

            // Act
            string result = DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback);

            // Assert
            Assert.AreEqual(@"
Model = 1, ModelType = System.Int32, PropertyName = (null), HtmlFieldName = [0], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)
Model = 2.3, ModelType = System.Double, PropertyName = (null), HtmlFieldName = [1], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)
Model = Hello World, ModelType = System.String, PropertyName = (null), HtmlFieldName = [2], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)",
                            result);
        }

        [TestMethod]
        public void CollectionTemplateNullItemInCollectionOfNullableValueTypesDoesNotDiscardNullable() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<List<int?>>(new List<int?> { 1, null, 2 });
            html.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = null;

            // Act
            string result = DefaultDisplayTemplates.CollectionTemplate(html, CollectionSpyCallback);

            // Assert
            Assert.AreEqual(@"
Model = 1, ModelType = System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], PropertyName = (null), HtmlFieldName = [0], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)
Model = (null), ModelType = System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], PropertyName = (null), HtmlFieldName = [1], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)
Model = 2, ModelType = System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], PropertyName = (null), HtmlFieldName = [2], TemplateName = (null), Mode = ReadOnly, TemplateInfo.HtmlFieldPrefix = , AdditionalViewData = (null)",
                            result);
        }

        // DecimalTemplate

        [TestMethod]
        public void DecimalTemplateTests() {
            Assert.AreEqual(
                @"12.35",
                DefaultDisplayTemplates.DecimalTemplate(MakeHtmlHelper<decimal>(12.3456M)));

            Assert.AreEqual(
                @"Formatted Value",
                DefaultDisplayTemplates.DecimalTemplate(MakeHtmlHelper<decimal>(12.3456M, "Formatted Value")));

            Assert.AreEqual(
                @"&lt;script&gt;alert('XSS!')&lt;/script&gt;",
                DefaultDisplayTemplates.DecimalTemplate(MakeHtmlHelper<decimal>(12.3456M, "<script>alert('XSS!')</script>")));
        }

        // EmailAddressTemplate

        [TestMethod]
        public void EmailAddressTemplateTests() {
            Assert.AreEqual(
                @"<a href=""mailto:foo@bar.com"">foo@bar.com</a>",
                DefaultDisplayTemplates.EmailAddressTemplate(MakeHtmlHelper<string>("foo@bar.com")));

            Assert.AreEqual(
                @"<a href=""mailto:foo@bar.com"">The FooBar User</a>",
                DefaultDisplayTemplates.EmailAddressTemplate(MakeHtmlHelper<string>("foo@bar.com", "The FooBar User")));

            Assert.AreEqual(
                @"<a href=""mailto:&lt;script>alert('XSS!')&lt;/script>"">&lt;script&gt;alert('XSS!')&lt;/script&gt;</a>",
                DefaultDisplayTemplates.EmailAddressTemplate(MakeHtmlHelper<string>("<script>alert('XSS!')</script>")));

            Assert.AreEqual(
                @"<a href=""mailto:&lt;script>alert('XSS!')&lt;/script>"">&lt;b&gt;Encode me!&lt;/b&gt;</a>",
                DefaultDisplayTemplates.EmailAddressTemplate(MakeHtmlHelper<string>("<script>alert('XSS!')</script>", "<b>Encode me!</b>")));
        }

        // HiddenInputTemplate

        [TestMethod]
        public void HiddenInputTemplateTests() {
            Assert.AreEqual(
                @"Hidden Value",
                DefaultDisplayTemplates.HiddenInputTemplate(MakeHtmlHelper<string>("Hidden Value")));

            Assert.AreEqual(
                @"&lt;b&gt;Encode me!&lt;/b&gt;",
                DefaultDisplayTemplates.HiddenInputTemplate(MakeHtmlHelper<string>("<script>alert('XSS!')</script>", "<b>Encode me!</b>")));

            var helperWithInvisibleHtml = MakeHtmlHelper<string>("<script>alert('XSS!')</script>", "<b>Encode me!</b>");
            helperWithInvisibleHtml.ViewData.ModelMetadata.HideSurroundingHtml = true;
            Assert.AreEqual(
                String.Empty,
                DefaultDisplayTemplates.HiddenInputTemplate(helperWithInvisibleHtml));
        }

        // HtmlTemplate

        [TestMethod]
        public void HtmlTemplateTests() {
            Assert.AreEqual(
                @"Hello, world!",
                DefaultDisplayTemplates.HtmlTemplate(MakeHtmlHelper<string>("", "Hello, world!")));

            Assert.AreEqual(
                @"<b>Hello, world!</b>",
                DefaultDisplayTemplates.HtmlTemplate(MakeHtmlHelper<string>("", "<b>Hello, world!</b>")));
        }

        // ObjectTemplate

        private static string SpyCallback(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string templateName, DataBoundControlMode mode, object additionalViewData) {
            return String.Format("Model = {0}, ModelType = {1}, PropertyName = {2}, HtmlFieldName = {3}, TemplateName = {4}, Mode = {5}, AdditionalViewData = {6}",
                                 metadata.Model ?? "(null)",
                                 metadata.ModelType == null ? "(null)" : metadata.ModelType.FullName,
                                 metadata.PropertyName ?? "(null)",
                                 htmlFieldName == String.Empty ? "(empty)" : htmlFieldName ?? "(null)",
                                 templateName ?? "(null)",
                                 mode,
                                 AnonymousObject.Inspect(additionalViewData));
        }

        class ObjectTemplateModel {
            public ObjectTemplateModel() {
                ComplexInnerModel = new object();
            }

            public string Property1 { get; set; }
            public string Property2 { get; set; }
            public object ComplexInnerModel { get; set; }
        }

        [TestMethod]
        public void ObjectTemplateDisplaysSimplePropertiesOnObjectByDefault() {
            string expected = @"<div class=""display-label"">Property1</div>
<div class=""display-field"">Model = p1, ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)</div>
<div class=""display-label"">Property2</div>
<div class=""display-field"">Model = (null), ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)</div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel { Property1 = "p1", Property2 = null };
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplateWithDisplayNameMetadata() {
            string expected = @"<div class=""display-field"">Model = (null), ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)</div>
<div class=""display-label"">Custom display name</div>
<div class=""display-field"">Model = (null), ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)</div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property1") { DisplayName = String.Empty };
            ModelMetadata prop2Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property2") { DisplayName = "Custom display name" };
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata, prop2Metadata });

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplateWithShowForDisplayMetadata() {
            string expected = @"<div class=""display-label"">Property1</div>
<div class=""display-field"">Model = (null), ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)</div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property1") { ShowForDisplay = true };
            ModelMetadata prop2Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property2") { ShowForDisplay = false };
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata, prop2Metadata });

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplatePreventsRecursionOnModelValue() {
            string expected = @"<div class=""display-label"">Property2</div>
<div class=""display-field"">Model = propValue2, ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)</div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), () => "propValue1", typeof(string), "Property1");
            ModelMetadata prop2Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), () => "propValue2", typeof(string), "Property2");
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata, prop2Metadata });
            html.ViewData.TemplateInfo.VisitedObjects.Add("propValue1");

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplatePreventsRecursionOnModelTypeForNullModelValues() {
            string expected = @"<div class=""display-label"">Property2</div>
<div class=""display-field"">Model = propValue2, ModelType = System.String, PropertyName = Property2, HtmlFieldName = Property2, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)</div>
";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), null, typeof(string), "Property1");
            ModelMetadata prop2Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), () => "propValue2", typeof(string), "Property2");
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata, prop2Metadata });
            html.ViewData.TemplateInfo.VisitedObjects.Add(typeof(string));

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplateDisplaysNullDisplayTextWhenObjectIsNull() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(null);
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(ObjectTemplateModel));
            metadata.NullDisplayText = "(null value)";
            html.ViewData.ModelMetadata = metadata;

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(metadata.NullDisplayText, result);
        }

        [TestMethod]
        public void ObjectTemplateDisplaysSimpleDisplayTextWhenTemplateDepthGreaterThanOne() {
            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(ObjectTemplateModel));
            metadata.SimpleDisplayText = "Simple Display Text";
            html.ViewData.ModelMetadata = metadata;
            html.ViewData.TemplateInfo.VisitedObjects.Add("foo");
            html.ViewData.TemplateInfo.VisitedObjects.Add("bar");

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(metadata.SimpleDisplayText, result);
        }

        [TestMethod]
        public void ObjectTemplateWithHiddenHtml() {
            string expected = @"Model = propValue1, ModelType = System.String, PropertyName = Property1, HtmlFieldName = Property1, TemplateName = (null), Mode = ReadOnly, AdditionalViewData = (null)";

            // Arrange
            ObjectTemplateModel model = new ObjectTemplateModel();
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(model);
            Mock<ModelMetadataProvider> provider = new Mock<ModelMetadataProvider>();
            Func<object> accessor = () => model;
            Mock<ModelMetadata> metadata = new Mock<ModelMetadata>(provider.Object, null, accessor, typeof(ObjectTemplateModel), null);
            ModelMetadata prop1Metadata = new ModelMetadata(provider.Object, typeof(ObjectTemplateModel), () => "propValue1", typeof(string), "Property1") { HideSurroundingHtml = true };
            html.ViewData.ModelMetadata = metadata.Object;
            metadata.Expect(p => p.Properties).Returns(() => new[] { prop1Metadata });

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ObjectTemplateAllPropertiesFromEntityObjectAreHidden() {
            // Arrange
            HtmlHelper html = MakeHtmlHelper<ObjectTemplateModel>(new MyEntityObject());

            // Act
            string result = DefaultDisplayTemplates.ObjectTemplate(html, SpyCallback);

            // Assert
            Assert.AreEqual(String.Empty, result);
        }

        private class MyEntityObject : EntityObject { }

        // StringTemplate

        [TestMethod]
        public void StringTemplateTests() {
            Assert.AreEqual(
                @"Hello, world!",
                DefaultDisplayTemplates.StringTemplate(MakeHtmlHelper<string>("", "Hello, world!")));

            Assert.AreEqual(
                @"&lt;b&gt;Hello, world!&lt;/b&gt;",
                DefaultDisplayTemplates.StringTemplate(MakeHtmlHelper<string>("", "<b>Hello, world!</b>")));
        }

        // UrlTemplate

        [TestMethod]
        public void UrlTemplateTests() {
            Assert.AreEqual(
                @"<a href=""http://www.microsoft.com/testing.aspx?value1=foo&amp;value2=bar"">http://www.microsoft.com/testing.aspx?value1=foo&amp;value2=bar</a>",
                DefaultDisplayTemplates.UrlTemplate(MakeHtmlHelper<string>("http://www.microsoft.com/testing.aspx?value1=foo&value2=bar")));

            Assert.AreEqual(
                @"<a href=""http://www.microsoft.com/testing.aspx?value1=foo&amp;value2=bar"">&lt;b&gt;Microsoft!&lt;/b&gt;</a>",
                DefaultDisplayTemplates.UrlTemplate(MakeHtmlHelper<string>("http://www.microsoft.com/testing.aspx?value1=foo&value2=bar", "<b>Microsoft!</b>")));
        }

        // Helpers

        private HtmlHelper MakeHtmlHelper<TModel>(object model) {
            return MakeHtmlHelper<TModel>(model, model);
        }

        private HtmlHelper MakeHtmlHelper<TModel>(object model, object formattedModelValue) {
            ViewDataDictionary viewData = new ViewDataDictionary(model);
            viewData.TemplateInfo.HtmlFieldPrefix = "FieldPrefix";
            viewData.TemplateInfo.FormattedModelValue = formattedModelValue;
            viewData.ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(() => model, typeof(TModel));

            ViewContext viewContext = new ViewContext(new ControllerContext(), new DummyView(), viewData, new TempDataDictionary(), new StringWriter());

            return new HtmlHelper(viewContext, new SimpleViewDataContainer(viewData));
        }

        private class DummyView : IView {
            public void Render(ViewContext viewContext, TextWriter writer) {
                throw new NotImplementedException();
            }
        }
    }
}
