namespace System.Web.Mvc.Html.Test {
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using SpyHtmlHelper = RenderPartialExtensionsTest.SpyHtmlHelper;

    [TestClass]
    public class PartialExtensionsTest {
        [TestMethod]
        public void PartialWithViewName() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();

            // Act
            MvcHtmlString result = helper.Partial("partial-view");

            // Assert
            Assert.AreEqual("partial-view", helper.RenderPartialInternal_PartialViewName);
            Assert.AreSame(helper.ViewData, helper.RenderPartialInternal_ViewData);
            Assert.IsNull(helper.RenderPartialInternal_Model);
            Assert.IsInstanceOfType(helper.RenderPartialInternal_Writer, typeof(StringWriter));
            Assert.AreSame(ViewEngines.Engines, helper.RenderPartialInternal_ViewEngineCollection);
            Assert.AreEqual("This is the result of the view", result.ToHtmlString());
        }

        [TestMethod]
        public void PartialWithViewNameAndViewData() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            ViewDataDictionary viewData = new ViewDataDictionary();

            // Act
            MvcHtmlString result = helper.Partial("partial-view", viewData);

            // Assert
            Assert.AreEqual("partial-view", helper.RenderPartialInternal_PartialViewName);
            Assert.AreSame(viewData, helper.RenderPartialInternal_ViewData);
            Assert.IsNull(helper.RenderPartialInternal_Model);
            Assert.IsInstanceOfType(helper.RenderPartialInternal_Writer, typeof(StringWriter));
            Assert.AreSame(ViewEngines.Engines, helper.RenderPartialInternal_ViewEngineCollection);
            Assert.AreEqual("This is the result of the view", result.ToHtmlString());
        }

        [TestMethod]
        public void PartialWithViewNameAndModel() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            object model = new object();

            // Act
            MvcHtmlString result = helper.Partial("partial-view", model);

            // Assert
            Assert.AreEqual("partial-view", helper.RenderPartialInternal_PartialViewName);
            Assert.AreSame(helper.ViewData, helper.RenderPartialInternal_ViewData);
            Assert.AreSame(model, helper.RenderPartialInternal_Model);
            Assert.IsInstanceOfType(helper.RenderPartialInternal_Writer, typeof(StringWriter));
            Assert.AreSame(ViewEngines.Engines, helper.RenderPartialInternal_ViewEngineCollection);
            Assert.AreEqual("This is the result of the view", result.ToHtmlString());
        }

        [TestMethod]
        public void PartialWithViewNameAndModelAndViewData() {
            // Arrange
            SpyHtmlHelper helper = SpyHtmlHelper.Create();
            object model = new object();
            ViewDataDictionary viewData = new ViewDataDictionary();

            // Act
            MvcHtmlString result = helper.Partial("partial-view", model, viewData);

            // Assert
            Assert.AreEqual("partial-view", helper.RenderPartialInternal_PartialViewName);
            Assert.AreSame(viewData, helper.RenderPartialInternal_ViewData);
            Assert.AreSame(model, helper.RenderPartialInternal_Model);
            Assert.IsInstanceOfType(helper.RenderPartialInternal_Writer, typeof(StringWriter));
            Assert.AreSame(ViewEngines.Engines, helper.RenderPartialInternal_ViewEngineCollection);
            Assert.AreEqual("This is the result of the view", result.ToHtmlString());
        }
    }
}
