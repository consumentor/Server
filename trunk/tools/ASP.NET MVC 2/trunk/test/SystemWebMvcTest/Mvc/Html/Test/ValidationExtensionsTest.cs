namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Linq;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class ValidationExtensionsTest {

        // Validate

        [TestMethod]
        public void Validate_AddsClientValidationMetadata() {
            var originalProviders = ModelValidatorProviders.Providers.ToArray();
            ModelValidatorProviders.Providers.Clear();

            try {
                // Arrange
                HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
                FormContext formContext = new FormContext() {
                    FormId = "form_id"
                };
                htmlHelper.ViewContext.ClientValidationEnabled = true;
                htmlHelper.ViewContext.FormContext = formContext;

                ModelClientValidationRule[] expectedValidationRules = new ModelClientValidationRule[] {
                    new ModelClientValidationRule() { ValidationType = "ValidationRule1" },
                    new ModelClientValidationRule() { ValidationType = "ValidationRule2" }
                };

                Mock<ModelValidator> mockValidator = new Mock<ModelValidator>(ModelMetadata.FromStringExpression("", htmlHelper.ViewContext.ViewData), htmlHelper.ViewContext);
                mockValidator.Expect(v => v.GetClientValidationRules())
                             .Returns(expectedValidationRules);
                Mock<ModelValidatorProvider> mockValidatorProvider = new Mock<ModelValidatorProvider>();
                mockValidatorProvider.Expect(vp => vp.GetValidators(It.IsAny<ModelMetadata>(), It.IsAny<ControllerContext>()))
                                     .Returns(new[] { mockValidator.Object });
                ModelValidatorProviders.Providers.Add(mockValidatorProvider.Object);

                // Act
                htmlHelper.Validate("baz");

                // Assert
                Assert.IsNotNull(formContext.GetValidationMetadataForField("baz"));
                CollectionAssert.AreEqual(expectedValidationRules, formContext.FieldValidators["baz"].ValidationRules.ToArray());
            }
            finally {
                ModelValidatorProviders.Providers.Clear();
                foreach (var provider in originalProviders) {
                    ModelValidatorProviders.Providers.Add(provider);
                }
            }
        }

        [TestMethod]
        public void Validate_DoesNothingIfClientValidationIsNotEnabled() {
            // Arrange
            HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
            htmlHelper.ViewContext.FormContext = new FormContext();
            htmlHelper.ViewContext.ClientValidationEnabled = false;

            // Act 
            htmlHelper.Validate("foo");

            // Assert
            Assert.AreEqual(0, htmlHelper.ViewContext.FormContext.FieldValidators.Count);
        }

        [TestMethod]
        public void Validate_ThrowsIfModelNameIsNull() {
            // Arrange
            HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    htmlHelper.Validate((string)null /* modelName */);
                }, "modelName");
        }

        [TestMethod]
        public void ValidateFor_AddsClientValidationMetadata() {
            var originalProviders = ModelValidatorProviders.Providers.ToArray();
            ModelValidatorProviders.Providers.Clear();

            try {
                // Arrange
                HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
                FormContext formContext = new FormContext() {
                    FormId = "form_id"
                };
                htmlHelper.ViewContext.ClientValidationEnabled = true;
                htmlHelper.ViewContext.FormContext = formContext;

                ModelClientValidationRule[] expectedValidationRules = new ModelClientValidationRule[] {
                    new ModelClientValidationRule() { ValidationType = "ValidationRule1" },
                    new ModelClientValidationRule() { ValidationType = "ValidationRule2" }
                };

                Mock<ModelValidator> mockValidator = new Mock<ModelValidator>(ModelMetadata.FromStringExpression("", htmlHelper.ViewContext.ViewData), htmlHelper.ViewContext);
                mockValidator.Expect(v => v.GetClientValidationRules())
                             .Returns(expectedValidationRules);
                Mock<ModelValidatorProvider> mockValidatorProvider = new Mock<ModelValidatorProvider>();
                mockValidatorProvider.Expect(vp => vp.GetValidators(It.IsAny<ModelMetadata>(), It.IsAny<ControllerContext>()))
                                     .Returns(new[] { mockValidator.Object });
                ModelValidatorProviders.Providers.Add(mockValidatorProvider.Object);

                // Act
                htmlHelper.ValidateFor(m => m.baz);

                // Assert
                Assert.IsNotNull(formContext.GetValidationMetadataForField("baz"));
                CollectionAssert.AreEqual(expectedValidationRules, formContext.FieldValidators["baz"].ValidationRules.ToArray());
            }
            finally {
                ModelValidatorProviders.Providers.Clear();
                foreach (var provider in originalProviders) {
                    ModelValidatorProviders.Providers.Add(provider);
                }
            }
        }

        // ValidationMessage

        [TestMethod]
        public void ValidationMessageAllowsEmptyModelName() {
            // Arrange
            ViewDataDictionary vdd = new ViewDataDictionary();
            vdd.ModelState.AddModelError("", "some error text");
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(vdd);

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessage("");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">some error text</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsFirstError() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessage("foo");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsGenericMessageInsteadOfExceptionText() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessage("quux");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">The value 'quuxValue' is invalid.</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsNullForInvalidName() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("boo");

            // Assert
            Assert.IsNull(html, "html should be null if name is invalid.");
        }

        [TestMethod]
        public void ValidationMessageReturnsWithObjectAttributes() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("foo", new { bar = "bar" });

            // Assert
            Assert.AreEqual(@"<span bar=""bar"" class=""field-validation-error"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsWithCustomMessage() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("foo", "bar error");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">bar error</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageReturnsWithCustomMessageAndObjectAttributes() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("foo", "bar error", new { baz = "baz" });

            // Assert
            Assert.AreEqual(@"<span baz=""baz"" class=""field-validation-error"">bar error</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageThrowsIfModelNameIsNull() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    htmlHelper.ValidationMessage(null);
                }, "modelName");
        }

        [TestMethod]
        public void ValidationMessageWithClientValidation_DefaultMessage_Valid() {
            var originalProviders = ModelValidatorProviders.Providers.ToArray();
            ModelValidatorProviders.Providers.Clear();

            try {
                // Arrange
                HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
                FormContext formContext = new FormContext();
                htmlHelper.ViewContext.ClientValidationEnabled = true;
                htmlHelper.ViewContext.FormContext = formContext;

                ModelClientValidationRule[] expectedValidationRules = new ModelClientValidationRule[] {
                    new ModelClientValidationRule() { ValidationType = "ValidationRule1" },
                    new ModelClientValidationRule() { ValidationType = "ValidationRule2" }
                };

                Mock<ModelValidator> mockValidator = new Mock<ModelValidator>(ModelMetadata.FromStringExpression("", htmlHelper.ViewContext.ViewData), htmlHelper.ViewContext);
                mockValidator.Expect(v => v.GetClientValidationRules())
                             .Returns(expectedValidationRules);
                Mock<ModelValidatorProvider> mockValidatorProvider = new Mock<ModelValidatorProvider>();
                mockValidatorProvider.Expect(vp => vp.GetValidators(It.IsAny<ModelMetadata>(), It.IsAny<ControllerContext>()))
                                     .Returns(new[] { mockValidator.Object });
                ModelValidatorProviders.Providers.Add(mockValidatorProvider.Object);

                // Act
                MvcHtmlString html = htmlHelper.ValidationMessage("baz"); // 'baz' is valid

                // Assert
                Assert.AreEqual(@"<span class=""field-validation-valid"" id=""baz_validationMessage""></span>", html.ToHtmlString(),
                    "ValidationMessage() should always return something if client validation is enabled.");
                Assert.IsNotNull(formContext.GetValidationMetadataForField("baz"));
                Assert.AreEqual("baz_validationMessage", formContext.FieldValidators["baz"].ValidationMessageId);
                Assert.IsTrue(formContext.FieldValidators["baz"].ReplaceValidationMessageContents);
                CollectionAssert.AreEqual(expectedValidationRules, formContext.FieldValidators["baz"].ValidationRules.ToArray());
            }
            finally {
                ModelValidatorProviders.Providers.Clear();
                foreach (var provider in originalProviders) {
                    ModelValidatorProviders.Providers.Add(provider);
                }
            }
        }

        [TestMethod]
        public void ValidationMessageWithClientValidation_ExplicitMessage_Valid() {
            var originalProviders = ModelValidatorProviders.Providers.ToArray();
            ModelValidatorProviders.Providers.Clear();

            try {
                // Arrange
                HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
                FormContext formContext = new FormContext();
                htmlHelper.ViewContext.ClientValidationEnabled = true;
                htmlHelper.ViewContext.FormContext = formContext;

                ModelClientValidationRule[] expectedValidationRules = new ModelClientValidationRule[] {
                    new ModelClientValidationRule() { ValidationType = "ValidationRule1" },
                    new ModelClientValidationRule() { ValidationType = "ValidationRule2" }
                };

                Mock<ModelValidator> mockValidator = new Mock<ModelValidator>(ModelMetadata.FromStringExpression("", htmlHelper.ViewContext.ViewData), htmlHelper.ViewContext);
                mockValidator.Expect(v => v.GetClientValidationRules())
                             .Returns(expectedValidationRules);
                Mock<ModelValidatorProvider> mockValidatorProvider = new Mock<ModelValidatorProvider>();
                mockValidatorProvider.Expect(vp => vp.GetValidators(It.IsAny<ModelMetadata>(), It.IsAny<ControllerContext>()))
                                     .Returns(new[] { mockValidator.Object });
                ModelValidatorProviders.Providers.Add(mockValidatorProvider.Object);

                // Act
                MvcHtmlString html = htmlHelper.ValidationMessage("baz", "some explicit message"); // 'baz' is valid

                // Assert
                Assert.AreEqual(@"<span class=""field-validation-valid"" id=""baz_validationMessage"">some explicit message</span>", html.ToHtmlString(),
                    "ValidationMessage() should always return something if client validation is enabled.");
                Assert.IsNotNull(formContext.GetValidationMetadataForField("baz"));
                Assert.AreEqual("baz_validationMessage", formContext.FieldValidators["baz"].ValidationMessageId);
                Assert.IsFalse(formContext.FieldValidators["baz"].ReplaceValidationMessageContents);
                CollectionAssert.AreEqual(expectedValidationRules, formContext.FieldValidators["baz"].ValidationRules.ToArray());
            }
            finally {
                ModelValidatorProviders.Providers.Clear();
                foreach (var provider in originalProviders) {
                    ModelValidatorProviders.Providers.Add(provider);
                }
            }
        }

        [TestMethod]
        public void ValidationMessageWithModelStateAndNoErrors() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessage("baz");

            // Assert
            Assert.IsNull(html, "html should be null if there are no errors");
        }

        // ValidationMessageFor

        [TestMethod]
        public void ValidationMessageForThrowsIfExpressionIsNull() {
            // Arrange
            HtmlHelper<object> htmlHelper = MvcHelper.GetHtmlHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => htmlHelper.ValidationMessageFor<object, object>(null),
                "expression"
            );
        }

        [TestMethod]
        public void ValidationMessageForReturnsFirstError() {
            // Arrange
            HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessageFor(m => m.foo);

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageForReturnsGenericMessageInsteadOfExceptionText() {
            // Arrange
            HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessageFor(m => m.quux);

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">The value 'quuxValue' is invalid.</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageForReturnsWithObjectAttributes() {
            // Arrange
            HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessageFor(m => m.foo, null /* validationMessage */, new { bar = "bar" });

            // Assert
            Assert.AreEqual(@"<span bar=""bar"" class=""field-validation-error"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageForReturnsWithCustomMessage() {
            // Arrange
            HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessageFor(m => m.foo, "bar error");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">bar error</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageForReturnsWithCustomMessageAndObjectAttributes() {
            // Arrange
            HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessageFor(m => m.foo, "bar error", new { baz = "baz" });

            // Assert
            Assert.AreEqual(@"<span baz=""baz"" class=""field-validation-error"">bar error</span>", html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageForWithClientValidation() {
            var originalProviders = ModelValidatorProviders.Providers.ToArray();
            ModelValidatorProviders.Providers.Clear();

            try {
                // Arrange
                HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
                FormContext formContext = new FormContext();
                htmlHelper.ViewContext.ClientValidationEnabled = true;
                htmlHelper.ViewContext.FormContext = formContext;

                ModelClientValidationRule[] expectedValidationRules = new ModelClientValidationRule[] {
                    new ModelClientValidationRule() { ValidationType = "ValidationRule1" },
                    new ModelClientValidationRule() { ValidationType = "ValidationRule2" }
                };

                Mock<ModelValidator> mockValidator = new Mock<ModelValidator>(ModelMetadata.FromStringExpression("", htmlHelper.ViewContext.ViewData), htmlHelper.ViewContext);
                mockValidator.Expect(v => v.GetClientValidationRules())
                             .Returns(expectedValidationRules);
                Mock<ModelValidatorProvider> mockValidatorProvider = new Mock<ModelValidatorProvider>();
                mockValidatorProvider.Expect(vp => vp.GetValidators(It.IsAny<ModelMetadata>(), It.IsAny<ControllerContext>()))
                                     .Returns(new[] { mockValidator.Object });
                ModelValidatorProviders.Providers.Add(mockValidatorProvider.Object);

                // Act
                MvcHtmlString html = htmlHelper.ValidationMessageFor(m => m.baz);

                // Assert
                Assert.AreEqual(@"<span class=""field-validation-valid"" id=""baz_validationMessage""></span>", html.ToHtmlString(),
                    "ValidationMessageFor() should always return something if client validation is enabled.");
                Assert.IsNotNull(formContext.GetValidationMetadataForField("baz"));
                Assert.AreEqual("baz_validationMessage", formContext.FieldValidators["baz"].ValidationMessageId);
                CollectionAssert.AreEqual(expectedValidationRules, formContext.FieldValidators["baz"].ValidationRules.ToArray());
            }
            finally {
                ModelValidatorProviders.Providers.Clear();
                foreach (var provider in originalProviders) {
                    ModelValidatorProviders.Providers.Add(provider);
                }
            }
        }

        [TestMethod]
        public void ValidationMessageForWithModelStateAndNoErrors() {
            // Arrange
            HtmlHelper<ValidationModel> htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationMessageFor(m => m.baz);

            // Assert
            Assert.IsNull(html, "html should be null if there are no errors");
        }

        // ValidationSummary

        [TestMethod]
        public void ValidationSummary() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary();

            // Assert
            Assert.AreEqual(@"<div class=""validation-summary-errors""><ul><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryAddsIdIfClientValidationEnabled() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
            htmlHelper.ViewContext.FormContext = new FormContext();
            htmlHelper.ViewContext.ClientValidationEnabled = true;

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary();

            // Assert
            Assert.AreEqual(@"<div class=""validation-summary-errors"" id=""validationSummary""><ul><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul></div>"
                , html.ToHtmlString());
            Assert.AreEqual("validationSummary", htmlHelper.ViewContext.FormContext.ValidationSummaryId);
        }

        [TestMethod]
        public void ValidationSummaryWithDictionary() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes["class"] = "my-class";

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary(null /* message */, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<div class=""validation-summary-errors my-class""><ul><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithDictionaryAndMessage() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());
            RouteValueDictionary htmlAttributes = new RouteValueDictionary();
            htmlAttributes["class"] = "my-class";

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary("This is my message.", htmlAttributes);

            // Assert
            Assert.AreEqual(@"<div class=""validation-summary-errors my-class""><span>This is my message.</span>
<ul><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithNoErrors_ReturnsNullIfClientValidationDisabled() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary();

            // Assert
            Assert.IsNull(html);
        }

        [TestMethod]
        public void ValidationSummaryWithNoErrors_EmptyUlIfClientValidationEnabled() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(new ViewDataDictionary());
            htmlHelper.ViewContext.ClientValidationEnabled = true;
            htmlHelper.ViewContext.FormContext = new FormContext();

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary();

            // Assert
            Assert.AreEqual(@"<div class=""validation-summary-valid"" id=""validationSummary""><ul><li style=""display:none""></li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithObjectAttributes() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary(null /* message */, new { baz = "baz" });

            // Assert
            Assert.AreEqual(@"<div baz=""baz"" class=""validation-summary-errors""><ul><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithObjectAttributesAndMessage() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary("This is my message.", new { baz = "baz" });

            // Assert
            Assert.AreEqual(@"<div baz=""baz"" class=""validation-summary-errors""><span>This is my message.</span>
<ul><li>foo error &lt;1&gt;</li>
<li>foo error 2</li>
<li>bar error &lt;1&gt;</li>
<li>bar error 2</li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithNoModelErrors() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary(true /* excludePropertyErrors */, "This is my message.");

            // Assert
            Assert.AreEqual(@"<div class=""validation-summary-errors""><span>This is my message.</span>
<ul><li style=""display:none""></li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithOnlyModelErrors() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelAndPropertyErrors());

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary(true /* excludePropertyErrors */, "This is my message.");

            // Assert
            Assert.AreEqual(@"<div class=""validation-summary-errors""><span>This is my message.</span>
<ul><li>Something is wrong.</li>
<li>Something else is also wrong.</li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationSummaryWithOnlyModelErrorsAndPrefix() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors("MyPrefix"));

            // Act
            MvcHtmlString html = htmlHelper.ValidationSummary(true /* excludePropertyErrors */, "This is my message.");

            // Assert
            Assert.AreEqual(@"<div class=""validation-summary-errors""><span>This is my message.</span>
<ul><li style=""display:none""></li>
</ul></div>"
                , html.ToHtmlString());
        }

        [TestMethod]
        public void ValidationMessageWithPrefix() {
            // Arrange
            HtmlHelper htmlHelper = MvcHelper.GetHtmlHelper(GetViewDataWithModelErrors("MyPrefix"));

            // Act 
            MvcHtmlString html = htmlHelper.ValidationMessage("foo");

            // Assert
            Assert.AreEqual(@"<span class=""field-validation-error"">foo error &lt;1&gt;</span>", html.ToHtmlString());
        }

        private class ValidationModel {
            public string foo { get; set; }
            public string bar { get; set; }
            public string baz { get; set; }
            public string quux { get; set; }
        }

        private static ViewDataDictionary<ValidationModel> GetViewDataWithModelErrors() {
            ViewDataDictionary<ValidationModel> viewData = new ViewDataDictionary<ValidationModel>();
            ModelState modelStateFoo = new ModelState();
            ModelState modelStateBar = new ModelState();
            ModelState modelStateBaz = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error <1>"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            modelStateBar.Errors.Add(new ModelError("bar error <1>"));
            modelStateBar.Errors.Add(new ModelError("bar error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            viewData.ModelState["bar"] = modelStateBar;
            viewData.ModelState["baz"] = modelStateBaz;
            viewData.ModelState.SetModelValue("quux", new ValueProviderResult(null, "quuxValue", null));
            viewData.ModelState.AddModelError("quux", new InvalidOperationException("Some error text."));
            return viewData;
        }

        private static ViewDataDictionary<ValidationModel> GetViewDataWithModelAndPropertyErrors() {
            ViewDataDictionary<ValidationModel> viewData = new ViewDataDictionary<ValidationModel>();
            ModelState modelStateFoo = new ModelState();
            ModelState modelStateBar = new ModelState();
            ModelState modelStateBaz = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error <1>"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            modelStateBar.Errors.Add(new ModelError("bar error <1>"));
            modelStateBar.Errors.Add(new ModelError("bar error 2"));
            viewData.ModelState["foo"] = modelStateFoo;
            viewData.ModelState["bar"] = modelStateBar;
            viewData.ModelState["baz"] = modelStateBaz;
            viewData.ModelState.SetModelValue("quux", new ValueProviderResult(null, "quuxValue", null));
            viewData.ModelState.AddModelError("quux", new InvalidOperationException("Some error text."));
            viewData.ModelState.AddModelError(String.Empty, "Something is wrong.");
            viewData.ModelState.AddModelError(String.Empty, "Something else is also wrong.");
            return viewData;
        }

        private static ViewDataDictionary<ValidationModel> GetViewDataWithModelErrors(string prefix) {
            ViewDataDictionary<ValidationModel> viewData = new ViewDataDictionary<ValidationModel>();
            viewData.TemplateInfo.HtmlFieldPrefix = prefix;
            ModelState modelStateFoo = new ModelState();
            ModelState modelStateBar = new ModelState();
            ModelState modelStateBaz = new ModelState();
            modelStateFoo.Errors.Add(new ModelError("foo error <1>"));
            modelStateFoo.Errors.Add(new ModelError("foo error 2"));
            modelStateBar.Errors.Add(new ModelError("bar error <1>"));
            modelStateBar.Errors.Add(new ModelError("bar error 2"));
            viewData.ModelState[prefix + ".foo"] = modelStateFoo;
            viewData.ModelState[prefix + ".bar"] = modelStateBar;
            viewData.ModelState[prefix + ".baz"] = modelStateBaz;
            viewData.ModelState.SetModelValue(prefix + ".quux", new ValueProviderResult(null, "quuxValue", null));
            viewData.ModelState.AddModelError(prefix + ".quux", new InvalidOperationException("Some error text."));
            return viewData;
        }

    }
}
