namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HttpPutAttributeTest {

        [TestMethod]
        public void IsValidForRequestReturnsFalseIfHttpVerbIsNotPost() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeWithInvalidVerb<HttpPutAttribute>("GET");
        }

        [TestMethod]
        public void IsValidForRequestReturnsTrueIfHttpVerbIsPost() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeWithValidVerb<HttpPutAttribute>("PUT");
        }

        [TestMethod]
        public void IsValidForRequestThrowsIfControllerContextIsNull() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeNullControllerContext<HttpPutAttribute>();
        }
    }
}
