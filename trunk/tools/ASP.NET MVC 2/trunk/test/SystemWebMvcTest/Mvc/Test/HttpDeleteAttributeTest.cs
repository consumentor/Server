namespace System.Web.Mvc.Test {
    using System;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HttpDeleteAttributeTest {

        [TestMethod]
        public void IsValidForRequestReturnsFalseIfHttpVerbIsNotPost() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeWithInvalidVerb<HttpDeleteAttribute>("POST");
        }

        [TestMethod]
        public void IsValidForRequestReturnsTrueIfHttpVerbIsPost() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeWithValidVerb<HttpDeleteAttribute>("DELETE");
        }

        [TestMethod]
        public void IsValidForRequestThrowsIfControllerContextIsNull() {
            HttpVerbAttributeHelper.TestHttpVerbAttributeNullControllerContext<HttpDeleteAttribute>();
        }
    }
}
