namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    internal static class HttpVerbAttributeHelper {
        internal static void TestHttpVerbAttributeNullControllerContext<THttpVerb>() where THttpVerb : ActionMethodSelectorAttribute, new() {
            ActionMethodSelectorAttribute attribute = new THttpVerb();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attribute.IsValidForRequest(null, null);
                }, "controllerContext");
        }

        internal static void TestHttpVerbAttributeWithValidVerb<THttpVerb>(string validVerb) where THttpVerb : ActionMethodSelectorAttribute, new() {
            // Arrange
            ActionMethodSelectorAttribute attribute = new THttpVerb();
            ControllerContext context = AcceptVerbsAttributeTest.GetControllerContextWithHttpVerb(validVerb);

            // Act
            bool result = attribute.IsValidForRequest(context, null);

            // Assert
            Assert.IsTrue(result);
        }

        internal static void TestHttpVerbAttributeWithInvalidVerb<THttpVerb>(string invalidVerb) where THttpVerb : ActionMethodSelectorAttribute, new() {
            // Arrange
            ActionMethodSelectorAttribute attribute = new THttpVerb();
            ControllerContext context = AcceptVerbsAttributeTest.GetControllerContextWithHttpVerb(invalidVerb);

            // Act
            bool result = attribute.IsValidForRequest(context, null);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
