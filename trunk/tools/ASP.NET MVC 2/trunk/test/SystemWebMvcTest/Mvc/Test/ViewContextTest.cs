namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ViewContextTest {

        [TestMethod]
        public void ConstructorThrowsIfTempDataIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            IView view = new Mock<IView>().Object;
            ViewDataDictionary viewData = new ViewDataDictionary();
            TempDataDictionary tempData = null;
            StringWriter writer = new StringWriter();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ViewContext(controllerContext, view, viewData, tempData, writer);
                }, "tempData");
        }

        [TestMethod]
        public void ConstructorThrowsIfViewDataIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            IView view = new Mock<IView>().Object;
            ViewDataDictionary viewData = null;
            TempDataDictionary tempData = new TempDataDictionary();
            StringWriter writer = new StringWriter();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ViewContext(controllerContext, view, viewData, tempData, writer);
                }, "viewData");
        }

        [TestMethod]
        public void ConstructorThrowsIfViewIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            IView view = null;
            ViewDataDictionary viewData = new ViewDataDictionary();
            TempDataDictionary tempData = new TempDataDictionary();
            StringWriter writer = new StringWriter();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ViewContext(controllerContext, view, viewData, tempData, writer);
                }, "view");
        }

        [TestMethod]
        public void ConstructorThrowsIfWriterIsNull() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            IView view = new Mock<IView>().Object;
            ViewDataDictionary viewData = new ViewDataDictionary();
            TempDataDictionary tempData = new TempDataDictionary();
            StringWriter writer = null;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ViewContext(controllerContext, view, viewData, tempData, writer);
                }, "writer");
        }

        [TestMethod]
        public void FormIdGeneratorProperty() {
            // Arrange
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(o => o.Items).Returns(new Hashtable());

            ViewContext viewContext = new ViewContext() {
                HttpContext = mockHttpContext.Object
            };

            // Act
            string form0Name = viewContext.FormIdGenerator();
            string form1Name = viewContext.FormIdGenerator();
            string form2Name = viewContext.FormIdGenerator();

            // Assert
            Assert.AreEqual("form0", form0Name);
            Assert.AreEqual("form1", form1Name);
            Assert.AreEqual("form2", form2Name);
        }

        [TestMethod]
        public void PropertiesAreSet() {
            // Arrange
            Mock<ControllerContext> mockCc = new Mock<ControllerContext>();
            mockCc.Expect(o => o.HttpContext.Items).Returns(new Hashtable());
            IView view = new Mock<IView>().Object;
            ViewDataDictionary viewData = new ViewDataDictionary();
            TempDataDictionary tempData = new TempDataDictionary();
            StringWriter writer = new StringWriter();

            // Act
            ViewContext viewContext = new ViewContext(mockCc.Object, view, viewData, tempData, writer);

            // Assert
            Assert.AreEqual(view, viewContext.View);
            Assert.AreEqual(viewData, viewContext.ViewData);
            Assert.AreEqual(tempData, viewContext.TempData);
            Assert.AreEqual(writer, viewContext.Writer);
            Assert.IsNull(viewContext.FormContext, "FormContext shouldn't be set unless Html.BeginForm() has been called.");
        }

    }
}
