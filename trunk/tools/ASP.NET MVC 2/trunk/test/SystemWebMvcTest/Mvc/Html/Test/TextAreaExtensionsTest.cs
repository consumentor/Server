namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Web.Mvc.Test;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class TextAreaExtensionsTest {
        private static readonly RouteValueDictionary _textAreaAttributesDictionary = new RouteValueDictionary(new { rows = "15", cols = "12" });
        private static readonly object _textAreaAttributesObjectDictionary = new { rows = "15", cols = "12" };

        private class TextAreaModel {
            public string foo { get; set; }
            public string bar { get; set; }
        }

        private static ViewDataDictionary<TextAreaModel> GetTextAreaViewData() {
            ViewDataDictionary<TextAreaModel> viewData = new ViewDataDictionary<TextAreaModel> { { "foo", "ViewDataFoo" } };
            viewData.Model = new TextAreaModel { foo = "ViewItemFoo", bar = "ViewItemBar" };
            return viewData;
        }

        private static ViewDataDictionary<TextAreaModel> GetTextAreaViewDataWithErrors() {
            ViewDataDictionary<TextAreaModel> viewData = new ViewDataDictionary<TextAreaModel> { { "foo", "ViewDataFoo" } };
            viewData.Model = new TextAreaModel { foo = "ViewItemFoo", bar = "ViewItemBar" };

            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = HtmlHelperTest.GetValueProviderResult(new string[] { "AttemptedValueFoo" }, "AttemptedValueFoo");

            return viewData;
        }

        // TextArea

        [TestMethod]
        public void TextAreaParameterDictionaryMerging() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString html = helper.TextArea("foo", new { rows = "30" });

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""foo"" name=""foo"" rows=""30"">
</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaParameterDictionaryMergingExplicitParameters() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString html = helper.TextArea("foo", "bar", 10, 25, new { rows = "30" });

            // Assert
            Assert.AreEqual(@"<textarea cols=""25"" id=""foo"" name=""foo"" rows=""10"">
bar</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.TextArea(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void TextAreaWithOutOfRangeColsThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    helper.TextArea("Foo", null /* value */, 0, -1, null /* htmlAttributes */);
                },
                "columns",
                @"The value must be greater than or equal to zero.
Parameter name: columns");
        }

        [TestMethod]
        public void TextAreaWithOutOfRangeRowsThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    helper.TextArea("Foo", null /* value */, -1, 0, null /* htmlAttributes */);
                },
                "rows",
                @"The value must be greater than or equal to zero.
Parameter name: rows");
        }

        [TestMethod]
        public void TextAreaWithExplicitValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString html = helper.TextArea("foo", "bar");

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""foo"" name=""foo"" rows=""2"">
bar</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithDefaultAttributes() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("foo");

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""foo"" name=""foo"" rows=""2"">
ViewDataFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithZeroRowsAndColumns() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("foo", null, 0, 0, null);

            // Assert
            Assert.AreEqual(@"<textarea id=""foo"" name=""foo"">
ViewDataFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithDotReplacementForId() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("foo.bar.baz");

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""foo_bar_baz"" name=""foo.bar.baz"" rows=""2"">
</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithObjectAttributes() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("foo", _textAreaAttributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<textarea cols=""12"" id=""foo"" name=""foo"" rows=""15"">
ViewDataFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithDictionaryAttributes() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("foo", _textAreaAttributesDictionary);

            // Assert
            Assert.AreEqual(@"<textarea cols=""12"" id=""foo"" name=""foo"" rows=""15"">
ViewDataFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithExplicitValueAndObjectAttributes() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("foo", "Hello World", _textAreaAttributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<textarea cols=""12"" id=""foo"" name=""foo"" rows=""15"">
Hello World</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithExplicitValueAndDictionaryAttributes() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("foo", "<Hello World>", _textAreaAttributesDictionary);

            // Assert
            Assert.AreEqual(@"<textarea cols=""12"" id=""foo"" name=""foo"" rows=""15"">
&lt;Hello World&gt;</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithNoValueAndObjectAttributes() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("baz", _textAreaAttributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<textarea cols=""12"" id=""baz"" name=""baz"" rows=""15"">
</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithNullValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextArea("foo", null, null);

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""foo"" name=""foo"" rows=""2"">
ViewDataFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithViewDataErrors() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.TextArea("foo", _textAreaAttributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<textarea class=""input-validation-error"" cols=""12"" id=""foo"" name=""foo"" rows=""15"">
AttemptedValueFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextAreaViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.TextArea("foo", new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<textarea class=""input-validation-error foo-class"" cols=""20"" id=""foo"" name=""foo"" rows=""2"">
AttemptedValueFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithPrefix() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.TextArea("foo", "bar");

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""MyPrefix_foo"" name=""MyPrefix.foo"" rows=""2"">
bar</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.TextArea("", "bar");

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""MyPrefix"" name=""MyPrefix"" rows=""2"">
bar</textarea>", html.ToHtmlString());
        }

        // TextAreaFor

        [TestMethod]
        public void TextAreaForWithNullExpression() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.TextAreaFor<TextAreaModel, object>(null),
                "expression"
            );
        }

        [TestMethod]
        public void TextAreaForWithOutOfRangeColsThrows() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                () => helper.TextAreaFor(m => m.foo, 0, -1, null /* htmlAttributes */),
                "columns",
                "The value must be greater than or equal to zero.\r\nParameter name: columns"
            );
        }

        [TestMethod]
        public void TextAreaForWithOutOfRangeRowsThrows() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                () => helper.TextAreaFor(m => m.foo, -1, 0, null /* htmlAttributes */),
                "rows",
                "The value must be greater than or equal to zero.\r\nParameter name: rows"
            );
        }

        [TestMethod]
        public void TextAreaForParameterDictionaryMerging() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo, new { rows = "30" });

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""foo"" name=""foo"" rows=""30"">
ViewItemFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForWithDefaultAttributes() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""foo"" name=""foo"" rows=""2"">
ViewItemFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForWithZeroRowsAndColumns() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo, 0, 0, null);

            // Assert
            Assert.AreEqual(@"<textarea id=""foo"" name=""foo"">
ViewItemFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForWithObjectAttributes() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo, _textAreaAttributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<textarea cols=""12"" id=""foo"" name=""foo"" rows=""15"">
ViewItemFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForWithDictionaryAttributes() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo, _textAreaAttributesDictionary);

            // Assert
            Assert.AreEqual(@"<textarea cols=""12"" id=""foo"" name=""foo"" rows=""15"">
ViewItemFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForWithViewDataErrors() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo, _textAreaAttributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<textarea class=""input-validation-error"" cols=""12"" id=""foo"" name=""foo"" rows=""15"">
AttemptedValueFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<textarea class=""input-validation-error foo-class"" cols=""20"" id=""foo"" name=""foo"" rows=""2"">
AttemptedValueFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForWithPrefix() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""MyPrefix_foo"" name=""MyPrefix.foo"" rows=""2"">
ViewItemFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m);

            // Assert
            Assert.AreEqual(@"<textarea cols=""20"" id=""MyPrefix"" name=""MyPrefix"" rows=""2"">
System.Web.Mvc.Html.Test.TextAreaExtensionsTest+TextAreaModel</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForParameterDictionaryMergingWithObjectValues() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo, 10, 25, new { rows = "30" });

            // Assert
            Assert.AreEqual(@"<textarea cols=""25"" id=""foo"" name=""foo"" rows=""10"">
ViewItemFoo</textarea>", html.ToHtmlString());
        }

        [TestMethod]
        public void TextAreaForParameterDictionaryMergingWithDictionaryValues() {
            // Arrange
            HtmlHelper<TextAreaModel> helper = MvcHelper.GetHtmlHelper(GetTextAreaViewData());

            // Act
            MvcHtmlString html = helper.TextAreaFor(m => m.foo, 10, 25, new RouteValueDictionary(new { rows = "30" }));

            // Assert
            Assert.AreEqual(@"<textarea cols=""25"" id=""foo"" name=""foo"" rows=""10"">
ViewItemFoo</textarea>", html.ToHtmlString());
        }

    }
}
