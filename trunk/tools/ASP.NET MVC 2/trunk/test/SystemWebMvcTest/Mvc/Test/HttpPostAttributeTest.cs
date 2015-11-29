namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HttpPostAttributeTest {

        [TestMethod]
        public void IsValidForRequestReturnsFalseIfHttpVerbIsNotPost() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeWithInvalidVerb<HttpPostAttribute>("DELETE");
        }

        [TestMethod]
        public void IsValidForRequestReturnsTrueIfHttpVerbIsPost() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeWithValidVerb<HttpPostAttribute>("POST");
        }

        [TestMethod]
        public void IsValidForRequestThrowsIfControllerContextIsNull() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeNullControllerContext<HttpPostAttribute>();
        }
    }
}
