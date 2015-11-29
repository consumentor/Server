namespace System.Web.Mvc.Html.Test {
    using System;
    using System.Collections;
    using System.IO;
    using System.Web.Mvc.Html;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MvcFormTest {
        [TestMethod]
        public void ConstructorWithNullViewContextThrows() {
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new MvcForm((ViewContext)null);
                },
                "viewContext");
        }

        [TestMethod]
        public void DisposeRendersCloseFormTag() {
            // Arrange
            StringWriter writer = new StringWriter();
            ViewContext viewContext = GetViewContext(writer);

            MvcForm form = new MvcForm(viewContext);

            // Act
            form.Dispose();

            // Assert
            Assert.AreEqual("</form>", writer.ToString());
        }

        [TestMethod]
        public void EndFormRendersCloseFormTag() {
            // Arrange
            StringWriter writer = new StringWriter();
            ViewContext viewContext = GetViewContext(writer);

            MvcForm form = new MvcForm(viewContext);

            // Act
            form.EndForm();

            // Assert
            Assert.AreEqual("</form>", writer.ToString());
        }

        [TestMethod]
        public void DisposeTwiceRendersCloseFormTagOnce() {
            // Arrange
            StringWriter writer = new StringWriter();
            ViewContext viewContext = GetViewContext(writer);

            MvcForm form = new MvcForm(viewContext);

            // Act
            form.Dispose();
            form.Dispose();

            // Assert
            Assert.AreEqual("</form>", writer.ToString());
        }

        [TestMethod]
        public void EndFormTwiceRendersCloseFormTagOnce() {
            // Arrange
            StringWriter writer = new StringWriter();
            ViewContext viewContext = GetViewContext(writer);

            MvcForm form = new MvcForm(viewContext);

            // Act
            form.EndForm();
            form.EndForm();

            // Assert
            Assert.AreEqual("</form>", writer.ToString());
        }

        private static ViewContext GetViewContext(TextWriter writer) {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Expect(o => o.Items).Returns(new Hashtable());

            return new ViewContext() {
                HttpContext = mockHttpContext.Object,
                Writer = writer
            };
        }

    }
}
