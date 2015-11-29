namespace Microsoft.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Microsoft.Web.UnitTestUtil;

    [TestClass]
    public class MailToExtensionsTest {
        [TestMethod]
        public void MailToWithoutEmailThrowsArgumentNullException() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            ExceptionHelper.ExpectArgumentNullException(() => html.Mailto("link text", null), "emailAddress");
        }

        [TestMethod]
        public void MailToWithoutLinkTextThrowsArgumentNullException() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            ExceptionHelper.ExpectArgumentNullException(() => html.Mailto(null, "somebody@example.com"), "linkText");
        }

        [TestMethod]
        public void MailToWithLinkTextAndEmailRendersProperElement() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "test@example.com");
            Assert.AreEqual("<a href=\"mailto:test@example.com\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithLinkTextEmailAndHtmlAttributesRendersAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "test@example.com", new {title="this is a test"});
            Assert.AreEqual("<a href=\"mailto:test@example.com\" title=\"this is a test\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithLinkTextEmailAndHtmlAttributesDictionaryRendersAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "test@example.com", new RouteValueDictionary(new { title = "this is a test" }));
            Assert.AreEqual("<a href=\"mailto:test@example.com\" title=\"this is a test\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithSubjectAndHtmlAttributesRendersAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "test@example.com", "The subject", new { title = "this is a test" });
            Assert.AreEqual("<a href=\"mailto:test@example.com?subject=The subject\" title=\"this is a test\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithSubjectAndHtmlAttributesDictionaryRendersAttributes() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "test@example.com", "The subject", new RouteValueDictionary(new { title = "this is a test" }));
            Assert.AreEqual("<a href=\"mailto:test@example.com?subject=The subject\" title=\"this is a test\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToAttributeEncodesEmail() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "te\">st@example.com");
            Assert.AreEqual("<a href=\"mailto:te&quot;>st@example.com\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithMultipleRecipientsRendersWithCommas() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "te\">st@example.com,test2@example.com");
            Assert.AreEqual("<a href=\"mailto:te&quot;>st@example.com,test2@example.com\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithSubjectAppendsSubjectQuery() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "test@example.com", "This is the subject");
            Assert.AreEqual("<a href=\"mailto:test@example.com?subject=This is the subject\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithCopyOnlyAppendsCopyQuery() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            MvcHtmlString result = html.Mailto("This is a test", "test@example.com", null, null, "cctest@example.com", null, null);
            Assert.AreEqual("<a href=\"mailto:test@example.com?cc=cctest@example.com\">This is a test</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithMultipartBodyRendersProperMailtoEncoding() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            string body = @"Line one
Line two
Line three";
            MvcHtmlString result = html.Mailto("email me", "test@example.com", null, body, null, null, null);
            Assert.AreEqual("<a href=\"mailto:test@example.com?body=Line one%0ALine two%0ALine three\">email me</a>", result.ToHtmlString());
        }

        [TestMethod]
        public void MailToWithAllValuesProvidedRendersCorrectTag() {
            HtmlHelper html = MvcHelper.GetHtmlHelperWithPath(new ViewDataDictionary());
            string body = @"Line one
Line two
Line three";
            MvcHtmlString result = html.Mailto("email me", "test@example.com", "the subject", body, "cc@example.com", "bcc@example.com", new { title="email test" });
            string expected = @"<a href=""mailto:test@example.com?subject=the subject&amp;cc=cc@example.com&amp;bcc=bcc@example.com&amp;body=Line one%0ALine two%0ALine three"" title=""email test"">email me</a>";
            Assert.AreEqual(expected, result.ToHtmlString());
        }
    }
}
