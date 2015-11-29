namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Specialized;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ValidateAntiForgeryTokenAttributeTest {

        private static string _antiForgeryTokenCookieName = AntiForgeryData.GetAntiForgeryTokenName("/SomeAppPath");

        [TestMethod]
        public void OnAuthorizationDoesNothingIfTokensAreValid() {
            // Arrange
            AuthorizationContext authContext = GetAuthorizationContext("2001-01-01:some value::username", "2001-01-02:some value:the real salt:username", "username");
            ValidateAntiForgeryTokenAttribute attribute = GetAttribute();

            // Act
            attribute.OnAuthorization(authContext);

            // Assert
            // If we got to this point, no exception was thrown, so success.
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfCookieMissing() {
            ExpectValidationException(null, "2001-01-01:some other value:the real salt:username");
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfCookieValueDoesNotMatchFormValue() {
            ExpectValidationException("2001-01-01:some value:the real salt:username", "2001-01-01:some other value:the real salt:username");
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfFilterContextIsNull() {
            // Arrange
            ValidateAntiForgeryTokenAttribute attribute = new ValidateAntiForgeryTokenAttribute();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    attribute.OnAuthorization(null);
                }, "filterContext");
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfFormSaltDoesNotMatchAttributeSalt() {
            ExpectValidationException("2001-01-01:some value:some salt:username", "2001-01-01:some value:some other salt:username");
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfFormValueMissing() {
            ExpectValidationException("2001-01-01:some value:the real salt:username", null);
        }

        [TestMethod]
        public void OnAuthorizationThrowsIfUsernameInFormIsIncorrect() {
            ExpectValidationException("2001-01-01:value:salt:username", "2001-01-01:some value:some other salt:different username");
        }

        [TestMethod]
        public void SaltProperty() {
            // Arrange
            ValidateAntiForgeryTokenAttribute attribute = new ValidateAntiForgeryTokenAttribute();

            // Act & Assert
            MemberHelper.TestStringProperty(attribute, "Salt", String.Empty, false /* testDefaultValue */, true /* allowNullAndEmpty */);
        }

        [TestMethod]
        public void SerializerProperty() {
            // Arrange
            ValidateAntiForgeryTokenAttribute attribute = new ValidateAntiForgeryTokenAttribute();
            AntiForgeryDataSerializer newSerializer = new AntiForgeryDataSerializer();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(attribute, "Serializer", newSerializer);
        }

        public static AuthorizationContext GetAuthorizationContext(string cookieValue, string formValue, string username) {
            HttpCookieCollection requestCookies = new HttpCookieCollection();
            NameValueCollection formCollection = new NameValueCollection();

            Mock<AuthorizationContext> mockAuthContext = new Mock<AuthorizationContext>();
            mockAuthContext.Expect(c => c.HttpContext.Request.ApplicationPath).Returns("/SomeAppPath");
            mockAuthContext.Expect(c => c.HttpContext.Request.Cookies).Returns(requestCookies);
            mockAuthContext.Expect(c => c.HttpContext.Request.Form).Returns(formCollection);
            mockAuthContext.Expect(c => c.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            mockAuthContext.Expect(c => c.HttpContext.User.Identity.Name).Returns(username);

            if (!String.IsNullOrEmpty(cookieValue)) {
                requestCookies.Set(new HttpCookie(_antiForgeryTokenCookieName, cookieValue));
            }
            if (!String.IsNullOrEmpty(formValue)) {
                formCollection.Set(AntiForgeryData.GetAntiForgeryTokenName(null), formValue);
            }

            return mockAuthContext.Object;
        }

        private static ValidateAntiForgeryTokenAttribute GetAttribute() {
            AntiForgeryDataSerializer serializer = new HtmlHelperTest.SubclassedAntiForgeryTokenSerializer();
            return new ValidateAntiForgeryTokenAttribute() {
                Salt = "the real salt",
                Serializer = serializer
            };
        }

        private static void ExpectValidationException(string cookieValue, string formValue) {
            ExpectValidationException(cookieValue, formValue, "username");
        }

        private static void ExpectValidationException(string cookieValue, string formValue, string username) {
            // Arrange
            ValidateAntiForgeryTokenAttribute attribute = GetAttribute();
            AuthorizationContext authContext = GetAuthorizationContext(cookieValue, formValue, username);

            // Act & Assert
            ExceptionHelper.ExpectException<HttpAntiForgeryException>(
                delegate {
                    attribute.OnAuthorization(authContext);
                }, "A required anti-forgery token was not supplied or was invalid.");
        }

    }
}
