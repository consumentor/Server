namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Data.Linq;
    using System.Web.Mvc.Test;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class InputExtensionsTest {

        // CheckBox

        [TestMethod]
        public void CheckBoxDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            MvcHtmlString html = helper.CheckBox("baz", new { @checked = "checked", value = "false" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""baz"" name=""baz"" type=""checkbox"" value=""false"" />" +
                @"<input name=""baz"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxExplicitParametersOverrideDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString html = helper.CheckBox("foo", true /* isChecked */, new { @checked = "unchecked", value = "false" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""false"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxShouldNotCopyAttributesForHidden() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString html = helper.CheckBox("foo", true /* isChecked */, new { id = "myID" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""myID"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.CheckBox(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void CheckBoxWithInvalidBooleanThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectException<FormatException>(
                delegate {
                    helper.CheckBox("bar");
                },
                "String was not recognized as a valid Boolean.");
        }

        [TestMethod]
        public void CheckBoxCheckedWithOnlyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString html = helper.CheckBox("foo", true /* isChecked */);

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxShouldRespectModelStateAttemptedValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());
            helper.ViewData.ModelState.SetModelValue("foo", HtmlHelperTest.GetValueProviderResult("false", "false"));

            // Act
            MvcHtmlString html = helper.CheckBox("foo");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxWithOnlyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            MvcHtmlString html = helper.CheckBox("foo");

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxWithNameAndObjectAttribute() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            MvcHtmlString html = helper.CheckBox("foo", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxWithObjectAttribute() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString html = helper.CheckBox("foo", false /* isChecked */, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxWithAttributeDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString html = helper.CheckBox("foo", false /* isChecked */, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxWithPrefix() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.CheckBox("foo", false /* isChecked */, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""MyPrefix.foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.CheckBox("", false /* isChecked */, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""MyPrefix"" name=""MyPrefix"" type=""checkbox"" value=""true"" />" +
                @"<input name=""MyPrefix"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        // CheckBoxFor

        [TestMethod]
        public void CheckBoxForWitNullExpressionThrows() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.CheckBoxFor(null),
                "expression");
        }

        [TestMethod]
        public void CheckBoxForWithInvalidBooleanThrows() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectException<FormatException>(
                () => helper.CheckBoxFor(m => m.bar),    // "bar" in ViewData isn't a valid boolean
                "String was not recognized as a valid Boolean.");
        }

        [TestMethod]
        public void CheckBoxForDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            MvcHtmlString html = helper.CheckBoxFor(m => m.baz, new { @checked = "checked", value = "false" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""baz"" name=""baz"" type=""checkbox"" value=""false"" />" +
                @"<input name=""baz"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxForShouldNotCopyAttributesForHidden() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            MvcHtmlString html = helper.CheckBoxFor(m => m.foo, new { id = "myID" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""myID"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxForCheckedWithOnlyName() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            MvcHtmlString html = helper.CheckBoxFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxForShouldRespectModelStateAttemptedValue() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());
            helper.ViewContext.ViewData.ModelState.SetModelValue("foo", HtmlHelperTest.GetValueProviderResult("false", "false"));

            // Act
            MvcHtmlString html = helper.CheckBoxFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxForWithObjectAttribute() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            MvcHtmlString html = helper.CheckBoxFor(m => m.foo, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxForWithAttributeDictionary() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());

            // Act
            MvcHtmlString html = helper.CheckBoxFor(m => m.foo, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" checked=""checked"" id=""foo"" name=""foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        [TestMethod]
        public void CheckBoxForWithPrefix() {
            // Arrange
            HtmlHelper<FooBarBazModel> helper = MvcHelper.GetHtmlHelper(GetCheckBoxViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.CheckBoxFor(m => m.foo, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""checkbox"" value=""true"" />" +
                @"<input name=""MyPrefix.foo"" type=""hidden"" value=""false"" />",
                html.ToHtmlString());
        }

        // Culture tests

        [TestMethod]
        public void InputHelpersUseCurrentCultureToConvertValueParameter() {
            // Arrange
            DateTime dt = new DateTime(1900, 1, 1, 0, 0, 0);
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary { { "foo", dt } });

            var tests = new[] {
                // Hidden(name)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""hidden"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.Hidden("foo")) 
                },
                // Hidden(name, value)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""hidden"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.Hidden("foo", dt)) 
                },
                // Hidden(name, value, htmlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""hidden"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.Hidden("foo", dt, null)) 
                },
                // Hidden(name, value, htmlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""hidden"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.Hidden("foo", dt, new RouteValueDictionary())) 
                },

                // RadioButton(name, value)
                new { 
                    Html = @"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.RadioButton("foo", dt))
                },
                // RadioButton(name, value, isChecked)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.RadioButton("foo", dt, false))
                },
                // RadioButton(name, value, htmlAttributes)
                new { 
                    Html = @"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.RadioButton("foo", dt, null))
                },
                // RadioButton(name, value)
                new { 
                    Html = @"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.RadioButton("foo", dt, new RouteValueDictionary()))
                },
                // RadioButton(name, value, isChecked, htmlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.RadioButton("foo", dt, false, null))
                },
                // RadioButton(name, value, isChecked, htmlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""radio"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.RadioButton("foo", dt, false, new RouteValueDictionary()))
                },

                // TextBox(name)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""text"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.TextBox("foo"))
                },
                // TextBox(name, value)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""text"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.TextBox("foo", dt))
                },
                // TextBox(name, value, hmtlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""text"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.TextBox("foo", dt, null))
                },
                // TextBox(name, value, hmtlAttributes)
                new { 
                    Html = @"<input id=""foo"" name=""foo"" type=""text"" value=""1900/01/01 12:00:00 AM"" />",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.TextBox("foo", dt, new RouteValueDictionary()))
                }
            };

            // Act && Assert
            using (HtmlHelperTest.ReplaceCulture("en-ZA", "en-US")) {
                foreach (var test in tests) {
                    Assert.AreEqual(test.Html, test.Action().ToHtmlString());
                }
            }
        }

        // Hidden

        [TestMethod]
        public void HiddenWithByteArrayValueRendersBase64EncodedValue() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString result = htmlHelper.Hidden("ProductName", ByteArrayModelBinderTest.Base64TestBytes);

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"hidden\" value=\"Fys1\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithBinaryArrayValueRendersBase64EncodedValue() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act
            MvcHtmlString result = htmlHelper.Hidden("ProductName", new Binary(new byte[] { 23, 43, 53 }));

            // Assert
            Assert.AreEqual("<input id=\"ProductName\" name=\"ProductName\" type=\"hidden\" value=\"Fys1\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Hidden(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void HiddenWithExplicitValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("foo", "DefaultFoo", null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithExplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("foo", "DefaultFoo", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithExplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("foo", "DefaultFoo", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithExplicitValueNull() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("foo", (string)null /* value */, (object)null /* htmlAttributes */);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithImplicitValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("foo");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithImplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("foo", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""hidden"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithImplicitValueAndAttributesDictionaryReturnsEmptyValueIfNotFound() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("keyNotFound", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""keyNotFound"" name=""keyNotFound"" type=""hidden"" value="""" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithImplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""hidden"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithNameAndValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Hidden("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithPrefix() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.Hidden("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""hidden"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.Hidden("", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix"" name=""MyPrefix"" type=""hidden"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Hidden(null /* name */);
                },
                "name");
        }

        [TestMethod]
        public void HiddenWithViewDataErrors() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.Hidden("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""hidden"" value=""AttemptedValueFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.Hidden("foo", null, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""hidden"" value=""AttemptedValueFoo"" />", html.ToHtmlString());
        }

        // HiddenFor

        [TestMethod]
        public void HiddenForWithNullExpressionThrows() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.HiddenFor<HiddenModel, object>(null),
                "expression"
            );
        }

        [TestMethod]
        public void HiddenForWithStringValue() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewData.Model.foo = "DefaultFoo";

            // Act
            MvcHtmlString html = helper.HiddenFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithByteArrayValueRendersBase64EncodedValue() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewData.Model.bytes = ByteArrayModelBinderTest.Base64TestBytes;

            // Act
            MvcHtmlString result = helper.HiddenFor(m => m.bytes);

            // Assert
            Assert.AreEqual("<input id=\"bytes\" name=\"bytes\" type=\"hidden\" value=\"Fys1\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithBinaryValueRendersBase64EncodedValue() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewData.Model.binary = new Binary(new byte[] { 23, 43, 53 });

            // Act
            MvcHtmlString result = helper.HiddenFor(m => m.binary);

            // Assert
            Assert.AreEqual("<input id=\"binary\" name=\"binary\" type=\"hidden\" value=\"Fys1\" />", result.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithAttributesDictionary() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewData.Model.foo = "DefaultFoo";

            // Act
            MvcHtmlString html = helper.HiddenFor(m => m.foo, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithAttributesObject() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewData.Model.foo = "DefaultFoo";

            // Act
            MvcHtmlString html = helper.HiddenFor(m => m.foo, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""hidden"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithPrefix() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewData.Model.foo = "fooValue";
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.HiddenFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""hidden"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.HiddenFor(m => m);

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix"" name=""MyPrefix"" type=""hidden"" value=""{ foo = (null) }"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithViewDataErrors() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.HiddenFor(m => m.foo, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""hidden"" value=""AttemptedValueFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void HiddenForWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper<HiddenModel> helper = MvcHelper.GetHtmlHelper(GetHiddenViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.HiddenFor(m => m.foo, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""hidden"" value=""AttemptedValueFoo"" />", html.ToHtmlString());
        }

        // Password

        [TestMethod]
        public void PasswordWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Password(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void PasswordDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", "Some Value", new { type = "fooType" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""fooType"" value=""Some Value"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordExplicitParametersOverrideDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", "Some Value", new { value = "Another Value", name = "bar" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" value=""Some Value"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithExplicitValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", "DefaultFoo", (object)null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithExplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", "DefaultFoo", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""password"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithExplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", "DefaultFoo", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""password"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithExplicitValueNull() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", (string)null /* value */, (object)null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithImplicitValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithImplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithImplicitValueAndAttributesDictionaryReturnsEmptyValueIfNotFound() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("keyNotFound", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""keyNotFound"" name=""keyNotFound"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithImplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithNameAndValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.Password("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithPrefix() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.Password("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""password"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.Password("", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix"" name=""MyPrefix"" type=""password"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.Password(null /* name */);
                },
                "name");
        }

        [TestMethod]
        public void PasswordWithViewDataErrors() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.Password("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetPasswordViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.Password("foo", null, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        // PasswordFor

        [TestMethod]
        public void PasswordForWithNullExpressionThrows() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.PasswordFor<FooModel, object>(null),
                "expression");
        }

        [TestMethod]
        public void PasswordForDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.PasswordFor(m => m.foo, new { type = "fooType" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""fooType"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordForExpressionNameOverridesDictionary() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.PasswordFor(m => m.foo, new { name = "bar" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordForWithImplicitValue() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.PasswordFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordForWithAttributesDictionary() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.PasswordFor(m => m.foo, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordForWithAttributesObject() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());

            // Act
            MvcHtmlString html = helper.PasswordFor(m => m.foo, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordForWithPrefix() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.PasswordFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordForWithViewDataErrors() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.PasswordFor(m => m.foo, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void PasswordForWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(GetPasswordViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.PasswordFor(m => m.foo, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""password"" />", html.ToHtmlString());
        }

        // RadioButton

        [TestMethod]
        public void RadioButtonDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("bar", "ViewDataBar", new { @checked = "chucked", value = "baz" });

            // Assert
            Assert.AreEqual(@"<input checked=""chucked"" id=""bar"" name=""bar"" type=""radio"" value=""ViewDataBar"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonExplicitParametersOverrideDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("bar", "ViewDataBar", false, new { @checked = "checked", value = "baz" });

            // Assert
            Assert.AreEqual(@"<input id=""bar"" name=""bar"" type=""radio"" value=""ViewDataBar"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonShouldRespectModelStateAttemptedValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());
            helper.ViewData.ModelState.SetModelValue("foo", HtmlHelperTest.GetValueProviderResult("ModelStateFoo", "ModelStateFoo"));

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "ModelStateFoo", false, new { @checked = "checked", value = "baz" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ModelStateFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonValueParameterAlwaysRendered() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "ViewDataFoo");
            MvcHtmlString html2 = helper.RadioButton("foo", "fooValue2");

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ViewDataFoo"" />", html.ToHtmlString());
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""radio"" value=""fooValue2"" />", html2.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.RadioButton(String.Empty, "value");
                },
                "name");
        }

        [TestMethod]
        public void RadioButtonWithNullValueThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    helper.RadioButton("foo", null);
                },
                "value");
        }

        [TestMethod]
        public void RadioButtonWithNameAndValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "ViewDataFoo");

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithPrefix() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "ViewDataFoo");

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""radio"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.RadioButton("", "ViewDataFoo");

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix"" name=""MyPrefix"" type=""radio"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithNameAndValueNotMatched() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""radio"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithNameValueUnchecked() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("bar", "barValue", false /* isChecked */);

            // Assert
            Assert.AreEqual(@"<input id=""bar"" name=""bar"" type=""radio"" value=""barValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithNameValueChecked() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("bar", "barValue", true /* isChecked */);

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""bar"" name=""bar"" type=""radio"" value=""barValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithObjectAttribute() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "fooValue", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""radio"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithAttributeDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("bar", "barValue", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""bar"" name=""bar"" type=""radio"" value=""barValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithValueUnchecked() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "bar", false /* isChecked */);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""radio"" value=""bar"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithValueAndObjectAttributeUnchecked() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "bar", false /* isChecked */, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""radio"" value=""bar"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonWithValueAndAttributeDictionaryUnchecked() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButton("foo", "bar", false /* isChecked */, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""radio"" value=""bar"" />", html.ToHtmlString());
        }

        // RadioButtonFor

        [TestMethod]
        public void RadioButtonForWithNullExpressionThrows() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.RadioButtonFor<FooBarModel, object>(null, "value"),
                "expression");
        }

        [TestMethod]
        public void RadioButtonForWithNullValueThrows() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.RadioButtonFor(m => m.foo, null),
                "value");
        }

        [TestMethod]
        public void RadioButtonForDictionaryOverridesImplicitParameters() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButtonFor(m => m.bar, "ViewDataBar", new { @checked = "chucked", value = "baz" });

            // Assert
            Assert.AreEqual(@"<input checked=""chucked"" id=""bar"" name=""bar"" type=""radio"" value=""ViewDataBar"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonForShouldRespectModelStateAttemptedValue() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());
            helper.ViewData.ModelState.SetModelValue("foo", HtmlHelperTest.GetValueProviderResult("ModelStateFoo", "ModelStateFoo"));

            // Act
            MvcHtmlString html = helper.RadioButtonFor(m => m.foo, "ModelStateFoo", new { @checked = "checked", value = "baz" });

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ModelStateFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonForValueParameterAlwaysRendered() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act & Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ViewDataFoo"" />",
                            helper.RadioButtonFor(m => m.foo, "ViewDataFoo").ToHtmlString());
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""radio"" value=""fooValue2"" />",
                            helper.RadioButtonFor(m => m.foo, "fooValue2").ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonForWithNameAndValue() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButtonFor(m => m.foo, "ViewDataFoo");

            // Assert
            Assert.AreEqual(@"<input checked=""checked"" id=""foo"" name=""foo"" type=""radio"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonForWithPrefix() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.RadioButtonFor(m => m.foo, "ViewDataFoo");

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""radio"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonForWithNameAndValueNotMatched() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButtonFor(m => m.foo, "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""radio"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonForWithObjectAttribute() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButtonFor(m => m.foo, "fooValue", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""radio"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void RadioButtonForWithAttributeDictionary() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetRadioButtonViewData());

            // Act
            MvcHtmlString html = helper.RadioButtonFor(m => m.bar, "barValue", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""bar"" name=""bar"" type=""radio"" value=""barValue"" />", html.ToHtmlString());
        }

        // TextBox

        [TestMethod]
        public void TextBoxDictionaryOverridesImplicitValues() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", "DefaultFoo", new { type = "fooType" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""fooType"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxExplicitParametersOverrideDictionaryValues() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", "DefaultFoo", new { value = "Some other value" });

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithDotReplacementForId() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo.bar.baz", null);

            // Assert
            Assert.AreEqual(@"<input id=""foo_bar_baz"" name=""foo.bar.baz"" type=""text"" value="""" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.TextBox(String.Empty);
                },
                "name");
        }

        [TestMethod]
        public void TextBoxWithExplicitValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", "DefaultFoo", (object)null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithExplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", "DefaultFoo", _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""text"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithExplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", "DefaultFoo", _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""text"" value=""DefaultFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithExplicitValueNull() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", (string)null /* value */, (object)null);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithImplicitValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithImplicitValueAndAttributesDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""text"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithImplicitValueAndAttributesDictionaryReturnsEmptyValueIfNotFound() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("keyNotFound", null, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""keyNotFound"" name=""keyNotFound"" type=""text"" value="""" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithImplicitValueAndAttributesObject() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""text"" value=""ViewDataFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.TextBox(null /* name */);
                },
                "name");
        }

        [TestMethod]
        public void TextBoxWithNameAndValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());

            // Act
            MvcHtmlString html = helper.TextBox("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithPrefix() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.TextBox("foo", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""text"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetHiddenViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.TextBox("", "fooValue");

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix"" name=""MyPrefix"" type=""text"" value=""fooValue"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithViewDataErrors() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.TextBox("foo", null, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""text"" value=""AttemptedValueFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxWithViewDataErrorsAndCustomClass() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetTextBoxViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.TextBox("foo", null, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""text"" value=""AttemptedValueFoo"" />", html.ToHtmlString());
        }

        // TextBoxFor

        [TestMethod]
        public void TextBoxForWithNullExpressionThrows() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.TextBoxFor<FooBarModel, object>(null /* expression */),
                "expression"
            );
        }

        [TestMethod]
        public void TextBoxForWithSimpleExpression() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBoxFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<input id=""foo"" name=""foo"" type=""text"" value=""ViewItemFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxForWithAttributesDictionary() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBoxFor(m => m.foo, _attributesDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazValue"" id=""foo"" name=""foo"" type=""text"" value=""ViewItemFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxForWithAttributesObject() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());

            // Act
            MvcHtmlString html = helper.TextBoxFor(m => m.foo, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" id=""foo"" name=""foo"" type=""text"" value=""ViewItemFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxForWithPrefix() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.TextBoxFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix_foo"" name=""MyPrefix.foo"" type=""text"" value=""ViewItemFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxForWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetTextBoxViewData());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.TextBoxFor(m => m);

            // Assert
            Assert.AreEqual(@"<input id=""MyPrefix"" name=""MyPrefix"" type=""text"" value=""{ foo = ViewItemFoo, bar = ViewItemBar }"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxForWithErrors() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetTextBoxViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.TextBoxFor(m => m.foo, _attributesObjectDictionary);

            // Assert
            Assert.AreEqual(@"<input baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo"" type=""text"" value=""AttemptedValueFoo"" />", html.ToHtmlString());
        }

        [TestMethod]
        public void TextBoxForWithErrorsAndCustomClass() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetTextBoxViewDataWithErrors());

            // Act
            MvcHtmlString html = helper.TextBoxFor(m => m.foo, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(@"<input class=""input-validation-error foo-class"" id=""foo"" name=""foo"" type=""text"" value=""AttemptedValueFoo"" />", html.ToHtmlString());
        }

        // MODELS
        private class FooModel {
            public string foo { get; set; }

            public override string ToString() {
                return String.Format("{{ foo = {0} }}", foo ?? "(null)");
            }
        }

        private class FooBarModel : FooModel {
            public string bar { get; set; }

            public override string ToString() {
                return String.Format("{{ foo = {0}, bar = {1} }}", foo ?? "(null)", bar ?? "(null)");
            }
        }

        private class FooBarBazModel {
            public bool foo { get; set; }
            public bool bar { get; set; }
            public bool baz { get; set; }

            public override string ToString() {
                return String.Format("{{ foo = {0}, bar = {1}, baz = {2} }}", foo, bar, baz);
            }
        }

        private class HiddenModel : FooModel {
            public byte[] bytes { get; set; }
            public Binary binary { get; set; }
        }

        // CHECKBOX
        private static ViewDataDictionary<FooBarBazModel> GetCheckBoxViewData() {
            ViewDataDictionary<FooBarBazModel> viewData = new ViewDataDictionary<FooBarBazModel> { { "foo", true }, { "bar", "NotTrue" }, { "baz", false } };
            return viewData;
        }

        // HIDDEN
        private static ViewDataDictionary<HiddenModel> GetHiddenViewData() {
            return new ViewDataDictionary<HiddenModel>(new HiddenModel()) { { "foo", "ViewDataFoo" } };
        }

        private static ViewDataDictionary<HiddenModel> GetHiddenViewDataWithErrors() {
            ViewDataDictionary<HiddenModel> viewData = new ViewDataDictionary<HiddenModel> { { "foo", "ViewDataFoo" } };
            viewData.Model = new HiddenModel();
            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = HtmlHelperTest.GetValueProviderResult("AttemptedValueFoo", "AttemptedValueFoo");

            return viewData;
        }

        // PASSWORD
        private static ViewDataDictionary<FooModel> GetPasswordViewData() {
            return new ViewDataDictionary<FooModel> { { "foo", "ViewDataFoo" } };
        }

        private static ViewDataDictionary<FooModel> GetPasswordViewDataWithErrors() {
            ViewDataDictionary<FooModel> viewData = new ViewDataDictionary<FooModel> { { "foo", "ViewDataFoo" } };
            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = HtmlHelperTest.GetValueProviderResult("AttemptedValueFoo", "AttemptedValueFoo");

            return viewData;
        }

        // RADIO
        private static ViewDataDictionary<FooBarModel> GetRadioButtonViewData() {
            ViewDataDictionary<FooBarModel> viewData = new ViewDataDictionary<FooBarModel> { { "foo", "ViewDataFoo" } };
            viewData.Model = new FooBarModel { foo = "ViewItemFoo", bar = "ViewItemBar" };
            ModelState modelState = new ModelState();
            modelState.Value = HtmlHelperTest.GetValueProviderResult("ViewDataFoo", "ViewDataFoo");
            viewData.ModelState["foo"] = modelState;

            return viewData;
        }

        // TEXTBOX
        private static readonly RouteValueDictionary _attributesDictionary = new RouteValueDictionary(new { baz = "BazValue" });
        private static readonly object _attributesObjectDictionary = new { baz = "BazObjValue" };

        private static ViewDataDictionary<FooBarModel> GetTextBoxViewData() {
            ViewDataDictionary<FooBarModel> viewData = new ViewDataDictionary<FooBarModel> { { "foo", "ViewDataFoo" } };
            viewData.Model = new FooBarModel { foo = "ViewItemFoo", bar = "ViewItemBar" };

            return viewData;
        }

        private static ViewDataDictionary<FooBarModel> GetTextBoxViewDataWithErrors() {
            ViewDataDictionary<FooBarModel> viewData = new ViewDataDictionary<FooBarModel> { { "foo", "ViewDataFoo" } };
            viewData.Model = new FooBarModel { foo = "ViewItemFoo", bar = "ViewItemBar" };
            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = HtmlHelperTest.GetValueProviderResult(new string[] { "AttemptedValueFoo" }, "AttemptedValueFoo");

            return viewData;
        }
    }
}
