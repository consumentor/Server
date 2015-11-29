namespace System.Web.Mvc.Html.Test {
    using System.IO;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class RenderPartialExtensionsTest {
        [TestMethod]
        public void RenderPartialWithViewName() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();

            // Act
            helper.RenderPartial("partial-view");

            // Assert
            Assert.AreEqual("partial-view", helper.RenderPartialInternal_PartialViewName);
            Assert.AreSame(helper.ViewData, helper.RenderPartialInternal_ViewData);
            Assert.IsNull(helper.RenderPartialInternal_Model);
            Assert.AreSame(helper.ViewContext.Writer, helper.RenderPartialInternal_Writer);
            Assert.AreSame(ViewEngines.Engines, helper.RenderPartialInternal_ViewEngineCollection);
        }

        [TestMethod]
        public void RenderPartialWithViewNameAndViewData() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();

            // Act
            helper.RenderPartial("partial-view", viewData);

            // Assert
            Assert.AreEqual("partial-view", helper.RenderPartialInternal_PartialViewName);
            Assert.AreSame(viewData, helper.RenderPartialInternal_ViewData);
            Assert.IsNull(helper.RenderPartialInternal_Model);
            Assert.AreSame(helper.ViewContext.Writer, helper.RenderPartialInternal_Writer);
            Assert.AreSame(ViewEngines.Engines, helper.RenderPartialInternal_ViewEngineCollection);
        }

        [TestMethod]
        public void RenderPartialWithViewNameAndModel() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            object model = new object();

            // Act
            helper.RenderPartial("partial-view", model);

            // Assert
            Assert.AreEqual("partial-view", helper.RenderPartialInternal_PartialViewName);
            Assert.AreSame(helper.ViewData, helper.RenderPartialInternal_ViewData);
            Assert.AreSame(model, helper.RenderPartialInternal_Model);
            Assert.AreSame(helper.ViewContext.Writer, helper.RenderPartialInternal_Writer);
            Assert.AreSame(ViewEngines.Engines, helper.RenderPartialInternal_ViewEngineCollection);
        }

        [TestMethod]
        public void RenderPartialWithViewNameAndModelAndViewData() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            object model = new object();
            ViewDataDictionary viewData = new ViewDataDictionary();

            // Act
            helper.RenderPartial("partial-view", model, viewData);

            // Assert
            Assert.AreEqual("partial-view", helper.RenderPartialInternal_PartialViewName);
            Assert.AreSame(viewData, helper.RenderPartialInternal_ViewData);
            Assert.AreSame(model, helper.RenderPartialInternal_Model);
            Assert.AreSame(helper.ViewContext.Writer, helper.RenderPartialInternal_Writer);
            Assert.AreSame(ViewEngines.Engines, helper.RenderPartialInternal_ViewEngineCollection);
        }

        internal class SpyHtmlHelper : HtmlHelper {
            public string RenderPartialInternal_PartialViewName;
            public ViewDataDictionary RenderPartialInternal_ViewData;
            public object RenderPartialInternal_Model;
            public TextWriter RenderPartialInternal_Writer;
            public ViewEngineCollection RenderPartialInternal_ViewEngineCollection;

            SpyHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer)
                : base(viewContext, viewDataContainer) { }

            public static SpyHtmlHelper Create() {
                ViewDataDictionary viewData = new ViewDataDictionary();

                Mock<ViewContext> mockViewContext = new Mock<ViewContext>() { DefaultValue = DefaultValue.Mock };
                mockViewContext.Expect(c => c.HttpContext.Response.Output).Throws(new Exception("Response.Output should never be called."));
                mockViewContext.Expect(c => c.ViewData).Returns(viewData);
                mockViewContext.Expect(c => c.Writer).Returns(new StringWriter());

                Mock<IViewDataContainer> container = new Mock<IViewDataContainer>();
                container.Expect(c => c.ViewData).Returns(viewData);

                return new SpyHtmlHelper(mockViewContext.Object, container.Object);
            }

            internal override void RenderPartialInternal(string partialViewName, ViewDataDictionary viewData, object model,
                                                         TextWriter writer, ViewEngineCollection viewEngineCollection) {
                RenderPartialInternal_PartialViewName = partialViewName;
                RenderPartialInternal_ViewData = viewData;
                RenderPartialInternal_Model = model;
                RenderPartialInternal_Writer = writer;
                RenderPartialInternal_ViewEngineCollection = viewEngineCollection;

                writer.Write("This is the result of the view");
            }
        }
    }
}
