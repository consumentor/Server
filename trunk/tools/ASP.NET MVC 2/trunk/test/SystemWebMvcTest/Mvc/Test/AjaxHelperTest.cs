﻿namespace System.Web.Mvc.Test {
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AjaxHelperTest {
        [TestMethod]
        public void ConstructorWithNullViewContextThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AjaxHelper ajaxHelper = new AjaxHelper(null, new Mock<IViewDataContainer>().Object);
                },
                "viewContext");
        }

        [TestMethod]
        public void ConstructorWithNullViewDataContainerThrows() {
            // Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AjaxHelper ajaxHelper = new AjaxHelper(new Mock<ViewContext>().Object, null);
                },
                "viewDataContainer");
        }

        [TestMethod]
        public void ConstructorSetsProperties1() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            IViewDataContainer vdc = new Mock<IViewDataContainer>().Object;

            // Act
            AjaxHelper ajaxHelper = new AjaxHelper(viewContext, vdc);

            // Assert
            Assert.AreEqual(viewContext, ajaxHelper.ViewContext);
            Assert.AreEqual(vdc, ajaxHelper.ViewDataContainer);
            Assert.AreEqual(RouteTable.Routes, ajaxHelper.RouteCollection);
        }

        [TestMethod]
        public void ConstructorSetsProperties2() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            IViewDataContainer vdc = new Mock<IViewDataContainer>().Object;
            RouteCollection rc = new RouteCollection();

            // Act
            AjaxHelper ajaxHelper = new AjaxHelper(viewContext, vdc, rc);

            // Assert
            Assert.AreEqual(viewContext, ajaxHelper.ViewContext);
            Assert.AreEqual(vdc, ajaxHelper.ViewDataContainer);
            Assert.AreEqual(rc, ajaxHelper.RouteCollection);
        }

        [TestMethod]
        public void GenericHelperConstructorSetsProperties1() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            ViewDataDictionary<Controller> vdd = new ViewDataDictionary<Controller>(new Mock<Controller>().Object);
            Mock<IViewDataContainer> vdc = new Mock<IViewDataContainer>();
            vdc.Expect(v => v.ViewData).Returns(vdd);

            // Act
            AjaxHelper<Controller> ajaxHelper = new AjaxHelper<Controller>(viewContext, vdc.Object);

            // Assert
            Assert.AreEqual(viewContext, ajaxHelper.ViewContext);
            Assert.AreEqual(vdc.Object, ajaxHelper.ViewDataContainer);
            Assert.AreEqual(RouteTable.Routes, ajaxHelper.RouteCollection);
            Assert.AreEqual(vdd.Model, ajaxHelper.ViewData.Model);
        }

        [TestMethod]
        public void GenericHelperConstructorSetsProperties2() {
            // Arrange
            ViewContext viewContext = new Mock<ViewContext>().Object;
            ViewDataDictionary<Controller> vdd = new ViewDataDictionary<Controller>(new Mock<Controller>().Object);
            Mock<IViewDataContainer> vdc = new Mock<IViewDataContainer>();
            vdc.Expect(v => v.ViewData).Returns(vdd);
            RouteCollection rc = new RouteCollection();

            // Act
            AjaxHelper<Controller> ajaxHelper = new AjaxHelper<Controller>(viewContext, vdc.Object, rc);

            // Assert
            Assert.AreEqual(viewContext, ajaxHelper.ViewContext);
            Assert.AreEqual(vdc.Object, ajaxHelper.ViewDataContainer);
            Assert.AreEqual(rc, ajaxHelper.RouteCollection);
            Assert.AreEqual(vdd.Model, ajaxHelper.ViewData.Model);
        }

        [TestMethod]
        public void GlobalizationScriptPathPropertyDefault() {
            try {
                // Act
                AjaxHelper.GlobalizationScriptPath = null;

                // Assert
                Assert.AreEqual("~/Scripts/Globalization", AjaxHelper.GlobalizationScriptPath);
            }
            finally {
                AjaxHelper.GlobalizationScriptPath = null;
            }
        }

        [TestMethod]
        public void GlobalizationScriptPathPropertySet() {
            try {
                // Act
                AjaxHelper.GlobalizationScriptPath = "/Foo/Bar";

                // Assert
                Assert.AreEqual("/Foo/Bar", AjaxHelper.GlobalizationScriptPath);
            }
            finally {
                AjaxHelper.GlobalizationScriptPath = null;
            }
        }

        [TestMethod]
        public void JavaScriptStringEncodeReturnsEmptyStringIfMessageIsEmpty() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string encoded = ajaxHelper.JavaScriptStringEncode(String.Empty);

            // Assert
            Assert.AreEqual(String.Empty, encoded);
        }

        [TestMethod]
        public void JavaScriptStringEncodeReturnsEncodedMessage() {
            // Arrange
            string message = "I said, \"Hello, world!\"\nHow are you?";
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string encoded = ajaxHelper.JavaScriptStringEncode(message);

            // Assert
            Assert.AreEqual(@"I said, \""Hello, world!\""\nHow are you?", encoded);
        }

        [TestMethod]
        public void JavaScriptStringEncodeReturnsNullIfMessageIsNull() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            string encoded = ajaxHelper.JavaScriptStringEncode(null /* message */);

            // Assert
            Assert.IsNull(encoded);
        }

        private static AjaxHelper GetAjaxHelper() {
            ViewContext viewContext = new Mock<ViewContext>().Object;
            IViewDataContainer viewDataContainer = new Mock<IViewDataContainer>().Object;
            return new AjaxHelper(viewContext, viewDataContainer);
        }
    }
}
