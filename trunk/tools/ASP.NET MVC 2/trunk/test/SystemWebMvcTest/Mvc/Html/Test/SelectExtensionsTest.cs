namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Mvc.Test;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class SelectExtensionsTest {
        private static readonly ViewDataDictionary<FooModel> _listBoxViewData = new ViewDataDictionary<FooModel> { { "foo", new[] { "Bravo" } } };
        private static readonly ViewDataDictionary<FooModel> _dropDownListViewData = new ViewDataDictionary<FooModel> { { "foo", "Bravo" } };

        private static ViewDataDictionary GetViewDataWithSelectList() {
            ViewDataDictionary viewData = new ViewDataDictionary();
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", "C");
            viewData["foo"] = selectList;
            viewData["foo.bar"] = selectList;
            return viewData;
        }

        // DropDownList

        [TestMethod]
        public void DropDownListUsesExplicitValueIfNotProvidedInViewData() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", "C");

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, (string)null /* optionLabel */);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListUsesViewDataDefaultValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(_dropDownListViewData);
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), "Charlie");

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, (string)null /* optionLabel */);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListUsesViewDataDefaultValueNoOptionLabel() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(_dropDownListViewData);
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), "Charlie");

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithAttributesDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, null /* optionLabel */, HtmlHelperTest.AttributesDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.DropDownList(String.Empty, (SelectList)null /* selectList */, (string)null /* optionLabel */);
                },
                "name");
        }

        [TestMethod]
        public void DropDownListWithErrors() {
            // Arrange
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });
            ViewDataDictionary viewData = GetViewDataWithErrors();
            HtmlHelper helper = MvcHelper.GetHtmlHelper(viewData);

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, null /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithErrorsAndCustomClass() {
            // Arrange
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            ViewDataDictionary viewData = GetViewDataWithErrors();
            HtmlHelper helper = MvcHelper.GetHtmlHelper(viewData);

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, null /* optionLabel */, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error foo-class"" id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.DropDownList(null /* name */, (SelectList)null /* selectList */, (string)null /* optionLabel */);
                },
                "name");
        }

        [TestMethod]
        public void DropDownListWithNullSelectListUsesViewData() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();
            helper.ViewData["foo"] = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                 @"<select id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithObjectDictionary() {
            // Arrange
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            ViewDataDictionary viewData = new ViewDataDictionary();
            HtmlHelper helper = MvcHelper.GetHtmlHelper(viewData);

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, null /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithObjectDictionaryAndSelectList() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, null /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithObjectDictionaryAndSelectListNoOptionLabel() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithObjectDictionaryAndEmptyOptionLabel() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, String.Empty /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option value=""""></option>
<option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithObjectDictionaryAndTitle() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, "[Select Something]", HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option value="""">[Select Something]</option>
<option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListUsesViewDataSelectList() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetViewDataWithSelectList());

            // Act
            MvcHtmlString html = helper.DropDownList("foo", (string)null /* optionLabel */);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListUsesModelState() {
            // Arrange
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            ViewDataDictionary viewData = GetViewDataWithErrors();
            viewData["foo"] = selectList;
            HtmlHelper helper = MvcHelper.GetHtmlHelper(viewData);

            // Act
            MvcHtmlString html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error"" id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListUsesViewDataSelectListNoOptionLabel() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetViewDataWithSelectList());

            // Act
            MvcHtmlString html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithDotReplacementForId() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(GetViewDataWithSelectList());

            // Act
            MvcHtmlString html = helper.DropDownList("foo.bar");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo_bar"" name=""foo.bar""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithIEnumerableSelectListItem() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", MultiSelectListTest.GetSampleIEnumerableObjects() } };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithIEnumerableSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", "123456789" } };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.DropDownList("foo", MultiSelectListTest.GetSampleIEnumerableObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithListOfSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", "123456789" } };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.DropDownList("foo", MultiSelectListTest.GetSampleListObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithListOfSelectListItem() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", MultiSelectListTest.GetSampleListObjects() } };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.DropDownList("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithNullViewDataValueThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    helper.DropDownList("foo", (string)null /* optionLabel */);
                },
                "There is no ViewData item of type 'IEnumerable<SelectListItem>' that has the key 'foo'.");
        }

        [TestMethod]
        public void DropDownListWithWrongViewDataTypeValueThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary { { "foo", 123 } });

            // Act
            ExceptionHelper.ExpectException<InvalidOperationException>(
                delegate {
                    helper.DropDownList("foo", (string)null /* optionLabel */);
                },
                "The ViewData item that has the key 'foo' is of type 'System.Int32' but must be of type 'IEnumerable<SelectListItem>'.");
        }

        [TestMethod]
        public void DropDownListWithPrefix() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.DropDownList("foo", selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""MyPrefix_foo"" name=""MyPrefix.foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.DropDownList("", selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""MyPrefix"" name=""MyPrefix""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        // DropDownListFor

        [TestMethod]
        public void DropDownListForWithNullExpressionThrows() {
            // Arrange
            HtmlHelper<object> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<object>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", "C");

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.DropDownListFor<object, object>(null /* expression */, selectList),
                "expression"
            );
        }

        [TestMethod]
        public void DropDownListForUsesExplicitValueIfNotProvidedInViewData() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", "C");

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, (string)null /* optionLabel */);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForUsesViewDataDefaultValue() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(_dropDownListViewData);
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), "Charlie");

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, (string)null /* optionLabel */);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForUsesViewDataDefaultValueNoOptionLabel() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(_dropDownListViewData);
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), "Charlie");

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithAttributesDictionary() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, null /* optionLabel */, HtmlHelperTest.AttributesDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithErrors() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetViewDataWithErrors());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, null /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" class=""input-validation-error"" id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithErrorsAndCustomClass() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetViewDataWithErrors());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, null /* optionLabel */, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error foo-class"" id=""foo"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithObjectDictionary() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, null /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithObjectDictionaryAndSelectListNoOptionLabel() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithObjectDictionaryAndEmptyOptionLabel() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, String.Empty /* optionLabel */, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option value=""""></option>
<option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithObjectDictionaryAndTitle() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, "[Select Something]", HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" name=""foo""><option value="""">[Select Something]</option>
<option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithIEnumerableSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary<FooModel> vdd = new ViewDataDictionary<FooModel> { { "foo", "123456789" } };
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, MultiSelectListTest.GetSampleIEnumerableObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithListOfSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary<FooModel> vdd = new ViewDataDictionary<FooModel> { { "foo", "123456789" } };
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, MultiSelectListTest.GetSampleListObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithPrefix() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m.foo, selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""MyPrefix_foo"" name=""MyPrefix.foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void DropDownListForWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            SelectList selectList = new SelectList(MultiSelectListTest.GetSampleStrings());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.DropDownListFor(m => m, selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""MyPrefix"" name=""MyPrefix""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        // ListBox

        [TestMethod]
        public void ListBoxUsesExplicitValueIfNotProvidedInViewData() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", new[] { "A", "C" });

            // Act
            MvcHtmlString html = helper.ListBox("foo", selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option selected=""selected"" value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxUsesViewDataDefaultValue() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(_listBoxViewData);
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBox("foo", selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithErrors() {
            // Arrange
            ViewDataDictionary viewData = GetViewDataWithErrors();
            HtmlHelper helper = MvcHelper.GetHtmlHelper(viewData);
            MultiSelectList list = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBox("foo", list);

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithErrorsAndCustomClass() {
            // Arrange
            ViewDataDictionary viewData = GetViewDataWithErrors();
            HtmlHelper helper = MvcHelper.GetHtmlHelper(viewData);
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBox("foo", selectList, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error foo-class"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithNameOnly() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();
            helper.ViewData["foo"] = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBox("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithAttributesDictionary() {
            // Arrange
            ViewDataDictionary viewData = new ViewDataDictionary();
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());
            //viewData["foo"] = selectList;
            HtmlHelper helper = MvcHelper.GetHtmlHelper(viewData);

            // Act
            MvcHtmlString html = helper.ListBox("foo", selectList, HtmlHelperTest.AttributesDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazValue"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithAttributesDictionaryAndMultiSelectList() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.ListBox("foo", selectList, HtmlHelperTest.AttributesDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazValue"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithAttributesDictionaryOverridesName() {
            // DevDiv Bugs #217602:
            // SelectInternal() should override the user-provided 'name' attribute

            // Arrange
            ViewDataDictionary viewData = new ViewDataDictionary();
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());
            HtmlHelper helper = MvcHelper.GetHtmlHelper(viewData);

            // Act
            MvcHtmlString html = helper.ListBox("foo", selectList, new { myAttr = "myValue", name = "theName" });

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" myAttr=""myValue"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithEmptyNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.ListBox(String.Empty, (MultiSelectList)null /* selectList */);
                },
                "name");
        }

        [TestMethod]
        public void ListBoxWithNullNameThrows() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    helper.ListBox(null /* name */, (MultiSelectList)null /* selectList */);
                },
                "name");
        }

        [TestMethod]
        public void ListBoxWithNullSelectListUsesViewData() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper();
            helper.ViewData["foo"] = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBox("foo", null);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithObjectDictionary() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.ListBox("foo", selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithIEnumerableSelectListItem() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", MultiSelectListTest.GetSampleIEnumerableObjects() } };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.ListBox("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithIEnumerableSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", "123456789" } };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.ListBox("foo", MultiSelectListTest.GetSampleIEnumerableObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithListOfSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", new string[] { "123456789", "111111111" } } };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.ListBox("foo", MultiSelectListTest.GetSampleListObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithListOfSelectListItem() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary { { "foo", MultiSelectListTest.GetSampleListObjects() } };
            HtmlHelper helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.ListBox("foo");

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithPrefix() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.ListBox("foo", selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""MyPrefix_foo"" multiple=""multiple"" name=""MyPrefix.foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxWithPrefixAndEmptyName() {
            // Arrange
            HtmlHelper helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.ListBox("", selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""MyPrefix"" multiple=""multiple"" name=""MyPrefix""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        // ListBoxFor

        [TestMethod]
        public void ListBoxForWithNullExpressionThrows() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => helper.ListBoxFor<FooModel, object>(null, null),
                "expression");
        }

        [TestMethod]
        public void ListBoxForUsesExplicitValueIfNotProvidedInViewData() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleAnonymousObjects(), "Letter", "FullWord", new[] { "A", "C" });

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option selected=""selected"" value=""A"">Alpha</option>
<option value=""B"">Bravo</option>
<option selected=""selected"" value=""C"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForUsesViewDataDefaultValue() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(_listBoxViewData);
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, selectList);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithErrors() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetViewDataWithErrors());
            MultiSelectList list = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, list);

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithErrorsAndCustomClass() {
            // Arrange
            HtmlHelper<FooBarModel> helper = MvcHelper.GetHtmlHelper(GetViewDataWithErrors());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, selectList, new { @class = "foo-class" });

            // Assert
            Assert.AreEqual(
                @"<select class=""input-validation-error foo-class"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option selected=""selected"">Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithAttributesDictionary() {
            // Arrange
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, selectList, HtmlHelperTest.AttributesDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazValue"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithAttributesDictionaryOverridesName() {
            // DevDiv Bugs #217602:
            // SelectInternal() should override the user-provided 'name' attribute

            // Arrange
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, selectList, new { myAttr = "myValue", name = "theName" });

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" myAttr=""myValue"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithNullSelectListUsesViewData() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            helper.ViewContext.ViewData["foo"] = new MultiSelectList(MultiSelectListTest.GetSampleStrings(), new[] { "Charlie" });

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, null);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option selected=""selected"">Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithObjectDictionary() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""foo"" multiple=""multiple"" name=""foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithIEnumerableSelectListItem() {
            // Arrange
            ViewDataDictionary<FooModel> vdd = new ViewDataDictionary<FooModel> { { "foo", MultiSelectListTest.GetSampleIEnumerableObjects() } };
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, null);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithIEnumerableSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary<FooModel> vdd = new ViewDataDictionary<FooModel> { { "foo", "123456789" } };
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, MultiSelectListTest.GetSampleIEnumerableObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithListOfSelectListItemSelectsDefaultFromViewData() {
            // Arrange
            ViewDataDictionary<FooModel> vdd = new ViewDataDictionary<FooModel> { { "foo", new string[] { "123456789", "111111111" } } };
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, MultiSelectListTest.GetSampleListObjects());

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option selected=""selected"" value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithListOfSelectListItem() {
            // Arrange
            ViewDataDictionary<FooModel> vdd = new ViewDataDictionary<FooModel> { { "foo", MultiSelectListTest.GetSampleListObjects() } };
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(vdd);

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, null);

            // Assert
            Assert.AreEqual(
                @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""123456789"">John</option>
<option value=""987654321"">Jane</option>
<option selected=""selected"" value=""111111111"">Joe</option>
</select>",
                html.ToHtmlString());
        }

        [TestMethod]
        public void ListBoxForWithPrefix() {
            // Arrange
            HtmlHelper<FooModel> helper = MvcHelper.GetHtmlHelper(new ViewDataDictionary<FooModel>());
            MultiSelectList selectList = new MultiSelectList(MultiSelectListTest.GetSampleStrings());
            helper.ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix = "MyPrefix";

            // Act
            MvcHtmlString html = helper.ListBoxFor(m => m.foo, selectList, HtmlHelperTest.AttributesObjectDictionary);

            // Assert
            Assert.AreEqual(
                @"<select baz=""BazObjValue"" id=""MyPrefix_foo"" multiple=""multiple"" name=""MyPrefix.foo""><option>Alpha</option>
<option>Bravo</option>
<option>Charlie</option>
</select>",
                html.ToHtmlString());
        }

        // Culture tests

        [TestMethod]
        public void SelectHelpersUseCurrentCultureToConvertValues() {
            // Arrange
            HtmlHelper defaultValueHelper = MvcHelper.GetHtmlHelper(new ViewDataDictionary { 
            { "foo", new [] {new DateTime(1900, 1, 1, 0, 0, 1)}},
            { "bar", new DateTime(1900, 1, 1, 0, 0, 1)} });
            HtmlHelper helper = MvcHelper.GetHtmlHelper();
            SelectList selectList = new SelectList(GetSampleCultureAnonymousObjects(), "Date", "FullWord", new DateTime(1900, 1, 1, 0, 0, 0));

            var tests = new[] {
                // DropDownList(name, selectList, optionLabel)
                new { 
                    Html = @"<select id=""foo"" name=""foo""><option selected=""selected"" value=""1900/01/01 12:00:00 AM"">Alpha</option>
<option value=""1900/01/01 12:00:01 AM"">Bravo</option>
<option value=""1900/01/01 12:00:02 AM"">Charlie</option>
</select>",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.DropDownList("foo", selectList, (string)null))
                },
                // DropDownList(name, selectList, optionLabel) (With default value selected from ViewData)
                new { 
                    Html = @"<select id=""bar"" name=""bar""><option value=""1900/01/01 12:00:00 AM"">Alpha</option>
<option selected=""selected"" value=""1900/01/01 12:00:01 AM"">Bravo</option>
<option value=""1900/01/01 12:00:02 AM"">Charlie</option>
</select>",
                    Action = new GenericDelegate<MvcHtmlString>(() => defaultValueHelper.DropDownList("bar", selectList, (string)null))
                },

                // ListBox(name, selectList)
                new { 
                    Html = @"<select id=""foo"" multiple=""multiple"" name=""foo""><option selected=""selected"" value=""1900/01/01 12:00:00 AM"">Alpha</option>
<option value=""1900/01/01 12:00:01 AM"">Bravo</option>
<option value=""1900/01/01 12:00:02 AM"">Charlie</option>
</select>",
                    Action = new GenericDelegate<MvcHtmlString>(() => helper.ListBox("foo", selectList))
                },

                // ListBox(name, selectList) (With default value selected from ViewData)
                new { 
                    Html = @"<select id=""foo"" multiple=""multiple"" name=""foo""><option value=""1900/01/01 12:00:00 AM"">Alpha</option>
<option selected=""selected"" value=""1900/01/01 12:00:01 AM"">Bravo</option>
<option value=""1900/01/01 12:00:02 AM"">Charlie</option>
</select>",
                    Action = new GenericDelegate<MvcHtmlString>(() => defaultValueHelper.ListBox("foo", selectList))
                }
            };

            // Act && Assert
            using (HtmlHelperTest.ReplaceCulture("en-ZA", "en-US")) {
                foreach (var test in tests) {
                    Assert.AreEqual(test.Html, test.Action().ToHtmlString());
                }
            }
        }

        // Helpers

        private class FooModel {
            public string foo { get; set; }
        }

        private class FooBarModel : FooModel {
            public string bar { get; set; }
        }

        private static ViewDataDictionary<FooBarModel> GetViewDataWithErrors() {
            ViewDataDictionary<FooBarModel> viewData = new ViewDataDictionary<FooBarModel> { { "foo", "ViewDataFoo" } };
            viewData.Model = new FooBarModel { foo = "ViewItemFoo", bar = "ViewItemBar" };

            ModelState modelStateFoo = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error 1"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            modelStateFoo.Value = new ValueProviderResult(new string[] { "Bravo", "Charlie" }, "Bravo", CultureInfo.InvariantCulture);

            return viewData;
        }

        internal static IEnumerable GetSampleCultureAnonymousObjects() {
            return new[] {
                new { Date = new DateTime(1900, 1, 1, 0, 0, 0), FullWord = "Alpha" },
                new { Date = new DateTime(1900, 1, 1, 0, 0, 1), FullWord = "Bravo" },
                new { Date = new DateTime(1900, 1, 1, 0, 0, 2), FullWord = "Charlie" }
            };
        }
    }
}
