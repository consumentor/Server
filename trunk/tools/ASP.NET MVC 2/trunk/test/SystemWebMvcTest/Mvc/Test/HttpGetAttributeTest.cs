namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HttpGetAttributeTest {

        [TestMethod]
        public void IsValidForRequestReturnsFalseIfHttpVerbIsNotPost() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeWithInvalidVerb<HttpGetAttribute>("DELETE");
        }

        [TestMethod]
        public void IsValidForRequestReturnsTrueIfHttpVerbIsPost() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeWithValidVerb<HttpGetAttribute>("GET");
        }

        [TestMethod]
        public void IsValidForRequestThrowsIfControllerContextIsNull() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeNullControllerContext<HttpGetAttribute>();
        }
    }
}
