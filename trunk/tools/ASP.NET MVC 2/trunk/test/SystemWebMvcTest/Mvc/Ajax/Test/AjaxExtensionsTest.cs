namespace System.Web.Mvc.Ajax.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class AjaxExtensionsTest {
        private const string AjaxForm = @"<form action=""/rawUrl"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithDefaultAction = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/home/oldaction"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithDefaultController = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/home/Action"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithId = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action/5"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithIdAndHtmlAttributes = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action/5"" method=""get"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithEmptyOptions = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxFormWithTargetId = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">";
        private const string AjaxFormWithHtmlAttributes = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" method=""get"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">";
        private const string AjaxFormClose = "</form>";
        private const string AjaxRouteFormWithNamedRoute = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/named/home/oldaction"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxRouteFormWithNamedRouteNoDefaults = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/any/url"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxRouteFormWithEmptyOptions = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/named/home/oldaction"" method=""post"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">";
        private const string AjaxRouteFormWithHtmlAttributes = @"<form action=""" + MvcHelper.AppPathModifier + @"/app/named/home/oldaction"" method=""get"" onclick=""Sys.Mvc.AsyncForm.handleClick(this, new Sys.UI.DomEvent(event));"" onsubmit=""Sys.Mvc.AsyncForm.handleSubmit(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">";

        private static readonly object _valuesObjectDictionary = new { id = 5 };
        private static readonly object _attributesObjectDictionary = new { method = "post" };

        [TestMethod]
        public void ActionLinkWithNullActionName() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString actionLink = ajaxHelper.ActionLink("linkText", null, GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkWithNullActionNameAndNullOptions() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString actionLink = ajaxHelper.ActionLink("linkText", null, null);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkWithNullOrEmptyLinkTextThrows() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    MvcHtmlString actionLink = ajaxHelper.ActionLink(String.Empty, String.Empty, null, null, null, null);
                },
                "linkText");
        }

        [TestMethod]
        public void ActionLink() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString actionLink = ajaxHelper.ActionLink("linkText", "Action", GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/home/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkAnonymousValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object values = new { controller = "Controller" };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.ActionLink("Some Text", "Action", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object htmlAttributes = new { foo = "bar", baz = "quux" };
            object values = new { controller = "Controller" };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.ActionLink("Some Text", "Action", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkTypedValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" }
            };

            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.ActionLink("Some Text", "Action", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" }
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "foo", "bar" },
                { "baz", "quux" }
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.ActionLink("Some Text", "Action", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkController() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString actionLink = ajaxHelper.ActionLink("linkText", "Action", "Controller", GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkControllerAnonymousValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object values = new { id = 5 };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.ActionLink("Some Text", "Action", "Controller", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action/5"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkControllerAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object htmlAttributes = new { foo = "bar", baz = "quux" };
            object values = new { id = 5 };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.ActionLink("Some Text", "Action", "Controller", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action/5"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkControllerTypedValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "id", 5 }
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.ActionLink("Some Text", "Action", "Controller", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action/5"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkControllerTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "id",5}
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "foo", "bar" },
                { "baz", "quux" }
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.ActionLink("Some Text", "Action", "Controller", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action/5"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkWithOptions() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString actionLink = ajaxHelper.ActionLink("linkText", "Action", "Controller", new AjaxOptions { UpdateTargetId = "some-id" });

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkWithNullHostName() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString actionLink = ajaxHelper.ActionLink("linkText", "Action", "Controller",
                null, null, null, null, new AjaxOptions { UpdateTargetId = "some-id" }, null);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void ActionLinkWithProtocol() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString actionLink = ajaxHelper.ActionLink("linkText", "Action", "Controller", "https", null, null, null, new AjaxOptions { UpdateTargetId = "some-id" }, null);

            // Assert
            Assert.AreEqual(@"<a href=""https://foo.bar.baz" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'some-id' });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void BeginFormSetsAndRestoresFormContext() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            FormContext originalContext = new FormContext();
            ajaxHelper.ViewContext.FormContext = originalContext;

            // Act & assert - push
            MvcForm theForm = ajaxHelper.BeginForm(new AjaxOptions());
            Assert.IsNotNull(ajaxHelper.ViewContext.FormContext);
            Assert.AreNotEqual(originalContext, ajaxHelper.ViewContext.FormContext, "FormContext should have been set to a new instance.");

            // Act & assert - pop
            theForm.Dispose();
            Assert.AreEqual(originalContext, ajaxHelper.ViewContext.FormContext, "FormContext was not properly restored.");
        }

        [TestMethod]
        public void GlobalizationScriptWithNullCultureInfoThrows() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ajaxHelper.GlobalizationScript(null);
                },
                "cultureInfo");
        }

        [TestMethod]
        public void GlobalizationScriptUsesCurrentCultureAsDefault() {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            try {
                // Arrange
                AjaxHelper ajaxHelper = GetAjaxHelper();
                AjaxHelper.GlobalizationScriptPath = null;
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

                // Act
                MvcHtmlString globalizationScript = ajaxHelper.GlobalizationScript();

                // Assert
                Assert.AreEqual(@"<script type=""text/javascript"" src=""~/Scripts/Globalization/en-GB.js""></script>", globalizationScript.ToHtmlString());
            }
            finally {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        [TestMethod]
        public void GlobalizationScriptWithCultureInfo() {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            try {
                // Arrange
                AjaxHelper ajaxHelper = GetAjaxHelper();
                AjaxHelper.GlobalizationScriptPath = null;
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

                // Act
                MvcHtmlString globalizationScript = ajaxHelper.GlobalizationScript(CultureInfo.GetCultureInfo("en-CA"));

                // Assert
                Assert.AreEqual(@"<script type=""text/javascript"" src=""~/Scripts/Globalization/en-CA.js""></script>", globalizationScript.ToHtmlString());
            }
            finally {
                Thread.CurrentThread.CurrentCulture = currentCulture;
            }
        }

        [TestMethod]
        public void RouteLinkWithNullOrEmptyLinkTextThrows() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    MvcHtmlString actionLink = ajaxHelper.RouteLink(String.Empty, String.Empty, null, null, null);
                },
                "linkText");
        }

        [TestMethod]
        public void RouteLinkWithNullOptions() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString routeLink = ajaxHelper.RouteLink("Some Text", new RouteValueDictionary(), null);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">Some Text</a>", routeLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkAnonymousValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object values = new {
                action = "Action",
                controller = "Controller"
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString routeLink = helper.RouteLink("Some Text", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", routeLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            object htmlAttributes = new {
                foo = "bar",
                baz = "quux"
            };
            object values = new {
                action = "Action",
                controller = "Controller"
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.RouteLink("Some Text", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkTypedValues() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" },
                { "action", "Action" }
            };

            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.RouteLink("Some Text", values, options);

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper helper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" },
                { "action", "Action" }
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "foo", "bar" },
                { "baz", "quux" }
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = helper.RouteLink("Some Text", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkNamedRoute() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("linkText", "namedroute", GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkNamedRouteAnonymousAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            object htmlAttributes = new {
                foo = "bar",
                baz = "quux"
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkNamedRouteTypedAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "foo", "bar" }, { "baz", "quux" } };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkNamedRouteWithAnonymousValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            object values = new {
                action = "Action",
                controller = "Controller"
            };

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("linkText", "namedroute", values, GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/named/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkNamedRouteAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            object values = new {
                action = "Action",
                controller = "Controller"
            };

            object htmlAttributes = new {
                foo = "bar",
                baz = "quux"
            };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/named/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkNamedRouteWithTypedValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" },
                { "action", "Action" }
            };

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("linkText", "namedroute", values, GetEmptyOptions());

            // Assert
            Assert.AreEqual(@"<a href=""" + MvcHelper.AppPathModifier + @"/app/named/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace });"">linkText</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkNamedRouteTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" },
                { "action", "Action" }
            };

            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "foo", "bar" }, { "baz", "quux" } };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", values, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/named/Controller/Action"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkNamedRouteNullValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "foo", "bar" }, { "baz", "quux" } };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", null, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""" + MvcHelper.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void RouteLinkWithHostName() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "foo", "bar" }, { "baz", "quux" } };
            AjaxOptions options = new AjaxOptions { UpdateTargetId = "update-div" };

            // Act
            MvcHtmlString actionLink = ajaxHelper.RouteLink("Some Text", "namedroute", null, "baz.bar.foo", null, null, options, htmlAttributes);

            // Assert
            Assert.AreEqual(@"<a baz=""quux"" foo=""bar"" href=""http://baz.bar.foo" + MvcHelper.AppPathModifier + @"/app/named/home/oldaction"" onclick=""Sys.Mvc.AsyncHyperlink.handleClick(this, new Sys.UI.DomEvent(event), { insertionMode: Sys.Mvc.InsertionMode.replace, updateTargetId: 'update-div' });"">Some Text</a>", actionLink.ToHtmlString());
        }

        [TestMethod]
        public void FormOnlyWithNullOptions() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm(null);

            // Assert
            Assert.AreEqual(AjaxForm, writer.ToString());
        }

        [TestMethod]
        public void FormWithNullActionName() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm(null, ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxFormWithDefaultAction, writer.ToString());
        }

        [TestMethod]
        public void FormWithNullOptions() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", null);

            // Assert
            Assert.AreEqual(AjaxFormWithEmptyOptions, writer.ToString());
        }

        [TestMethod]
        public void Form() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm(ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxForm, writer.ToString());

        }

        [TestMethod]
        public void FormAction() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxFormWithDefaultController, writer.ToString());
        }

        [TestMethod]
        public void FormAnonymousValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            object values = new { controller = "Controller" };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", values, ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxFormWithEmptyOptions, writer.ToString());
        }

        [TestMethod]
        public void FormAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            object values = new { controller = "Controller" };
            object htmlAttributes = new { method = "get" };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", values, ajaxOptions, htmlAttributes);

            // Assert
            Assert.AreEqual(AjaxFormWithHtmlAttributes, writer.ToString());
        }

        [TestMethod]
        public void FormTypedValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" }
            };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", values, ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxFormWithEmptyOptions, writer.ToString());
        }

        [TestMethod]
        public void FormTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            RouteValueDictionary values = new RouteValueDictionary {
                { "controller", "Controller" }
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "method", "get" }
            };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", values, ajaxOptions, htmlAttributes);

            // Assert
            Assert.AreEqual(AjaxFormWithHtmlAttributes, writer.ToString());
        }

        [TestMethod]
        public void FormController() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxFormWithEmptyOptions, writer.ToString());
        }

        [TestMethod]
        public void FormControllerAnonymousValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            object values = new { id = 5 };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", values, ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxFormWithId, writer.ToString());
        }

        [TestMethod]
        public void FormControllerAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            object values = new { id = 5 };
            object htmlAttributes = new { method = "get" };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", values, ajaxOptions, htmlAttributes);

            // Assert
            Assert.AreEqual(AjaxFormWithIdAndHtmlAttributes, writer.ToString());
        }

        [TestMethod]
        public void FormControllerTypedValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            RouteValueDictionary values = new RouteValueDictionary {
                { "id", 5 }
            };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", values, ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxFormWithId, writer.ToString());
        }

        [TestMethod]
        public void FormControllerTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            RouteValueDictionary values = new RouteValueDictionary {
                { "id", 5 }
            };
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> {
                { "method", "get" }
            };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", values, ajaxOptions, htmlAttributes);

            // Assert
            Assert.AreEqual(AjaxFormWithIdAndHtmlAttributes, writer.ToString());
        }

        [TestMethod]
        public void FormWithTargetId() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxFormWithTargetId, writer.ToString());
        }

        [TestMethod]
        public void DisposeWritesClosingFormTag() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginForm("Action", "Controller", ajaxOptions);
            form.Dispose();

            // Assert
            Assert.AreEqual(AjaxFormWithTargetId + AjaxFormClose, writer.ToString());
        }

        [TestMethod]
        public void InsertionModeToString() {
            // Act & Assert
            Assert.AreEqual(AjaxExtensions.InsertionModeToString(InsertionMode.Replace), "Sys.Mvc.InsertionMode.replace");
            Assert.AreEqual(AjaxExtensions.InsertionModeToString(InsertionMode.InsertAfter), "Sys.Mvc.InsertionMode.insertAfter");
            Assert.AreEqual(AjaxExtensions.InsertionModeToString(InsertionMode.InsertBefore), "Sys.Mvc.InsertionMode.insertBefore");
            Assert.AreEqual(AjaxExtensions.InsertionModeToString((InsertionMode)4), "4");
        }

        [TestMethod]
        public void RouteForm() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxRouteFormWithNamedRoute, writer.ToString());
        }

        [TestMethod]
        public void RouteFormAnonymousValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            AjaxHelper poes = GetAjaxHelper();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", null, ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxRouteFormWithEmptyOptions, writer.ToString());
        }

        [TestMethod]
        public void RouteFormAnonymousValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            object htmlAttributes = new { method = "get" };
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", null, ajaxOptions, htmlAttributes);

            // Assert
            Assert.AreEqual(AjaxRouteFormWithHtmlAttributes, writer.ToString());
        }

        [TestMethod]
        public void RouteFormCanUseNamedRouteWithoutSpecifyingDefaults() {
            // DevDiv 217072: Non-mvc specific helpers should not give default values for controller and action

            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            ajaxHelper.RouteCollection.MapRoute("MyRouteName", "any/url", new { controller = "Charlie" });
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("MyRouteName", new AjaxOptions());

            // Assert
            Assert.AreEqual(AjaxRouteFormWithNamedRouteNoDefaults, writer.ToString());
        }

        [TestMethod]
        public void RouteFormTypedValues() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            AjaxOptions ajaxOptions = GetEmptyOptions();
            RouteValueDictionary values = new RouteValueDictionary();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", values, ajaxOptions);

            // Assert
            Assert.AreEqual(AjaxRouteFormWithEmptyOptions, writer.ToString());
        }

        [TestMethod]
        public void RouteFormTypedValuesAndAttributes() {
            // Arrange
            AjaxHelper ajaxHelper = GetAjaxHelper();
            Dictionary<string, object> htmlAttributes = new Dictionary<string, object> { { "method", "get" } };
            AjaxOptions ajaxOptions = new AjaxOptions { UpdateTargetId = "some-id" };
            RouteValueDictionary values = new RouteValueDictionary();
            StringWriter writer = new StringWriter();
            ajaxHelper.ViewContext.Writer = writer;

            // Act
            IDisposable form = ajaxHelper.BeginRouteForm("namedroute", values, ajaxOptions, htmlAttributes);

            // Assert
            Assert.AreEqual(AjaxRouteFormWithHtmlAttributes, writer.ToString());
        }

        private static AjaxHelper GetAjaxHelper() {
            return GetAjaxHelper(new Mock<HttpResponseBase>());
        }

        private static AjaxHelper GetAjaxHelper(Mock<HttpResponseBase> mockResponse) {
            HttpContextBase httpcontext = GetHttpContext("/app/", mockResponse);
            RouteCollection rt = new RouteCollection();
            rt.Add(new Route("{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            rt.Add("namedroute", new Route("named/{controller}/{action}/{id}", null) { Defaults = new RouteValueDictionary(new { id = "defaultid" }) });
            RouteData rd = new RouteData();
            rd.Values.Add("controller", "home");
            rd.Values.Add("action", "oldaction");

            ViewContext viewContext = new ViewContext() {
                HttpContext = httpcontext,
                RouteData = rd,
                Writer = TextWriter.Null
            };
            AjaxHelper ajaxHelper = new AjaxHelper(viewContext, new Mock<IViewDataContainer>().Object, rt);
            return ajaxHelper;
        }

        private static AjaxOptions GetEmptyOptions() {
            return new AjaxOptions();
        }

        private static HttpContextBase GetHttpContext(string appPath, Mock<HttpResponseBase> mockResponse) {
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            if (!String.IsNullOrEmpty(appPath)) {
                mockRequest.Expect(o => o.ApplicationPath).Returns(appPath);
            }
            mockRequest.Expect(o => o.Url).Returns(new Uri("http://foo.bar.baz"));
            mockRequest.Expect(o => o.RawUrl).Returns("/rawUrl");
            mockRequest.Expect(o => o.PathInfo).Returns(String.Empty);
            mockContext.Expect(o => o.Request).Returns(mockRequest.Object);
            mockContext.Expect(o => o.Session).Returns((HttpSessionStateBase)null);
            mockContext.Expect(o => o.Items).Returns(new Hashtable());

            mockResponse.Expect(o => o.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(r => MvcHelper.AppPathModifier + r);
            mockContext.Expect(o => o.Response).Returns(mockResponse.Object);

            return mockContext.Object;
        }
    }
}
