namespace System.Web.Mvc.Test {
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ChildActionOnlyAttributeTest {

        [TestMethod]
        public void GuardClause() {
            // Arrange
            ChildActionOnlyAttribute attr = new ChildActionOnlyAttribute();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                () => attr.OnAuthorization(null /* filterContext */),
                "filterContext"
            );
        }

        [TestMethod]
        public void DoesNothingForChildRequest() {
            // Arrange
            ChildActionOnlyAttribute attr = new ChildActionOnlyAttribute();
            Mock<AuthorizationContext> context = new Mock<AuthorizationContext>();
            context.Expect(c => c.IsChildAction).Returns(true);

            // Act
            attr.OnAuthorization(context.Object);

            // Assert
            Assert.IsNull(context.Object.Result);
        }

        [TestMethod]
        public void ThrowsIfNotChildRequest() {
            // Arrange
            ChildActionOnlyAttribute attr = new ChildActionOnlyAttribute();
            Mock<AuthorizationContext> context = new Mock<AuthorizationContext>();
            context.Expect(c => c.IsChildAction).Returns(false);
            context.Expect(c => c.ActionDescriptor.ActionName).Returns("some name");

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    attr.OnAuthorization(context.Object);
                },
                @"The action 'some name' is accessible only by a child request.");
        }

    }
}
