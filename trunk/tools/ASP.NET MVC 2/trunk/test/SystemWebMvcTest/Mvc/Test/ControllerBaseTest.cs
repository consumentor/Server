namespace System.Web.Mvc.Test {
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.UnitTestUtil;
    using Moq;

    [TestClass]
    public class ControllerBaseTest {

        [TestMethod]
        public void ExecuteCallsControllerBaseExecute() {
            // Arrange
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());

            Mock<ControllerBaseHelper> mockController = new Mock<ControllerBaseHelper>() { CallBase = true };
            mockController.Expect(c => c.PublicInitialize(requestContext)).Verifiable();
            mockController.Expect(c => c.PublicExecuteCore()).Verifiable();
            IController controller = mockController.Object;

            // Act
            controller.Execute(requestContext);

            // Assert
            mockController.Verify();
        }

        [TestMethod]
        public void ExecuteThrowsIfCalledTwice() {
            // Arrange
            EmptyControllerBase controller = new EmptyControllerBase();
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());

            // Act
            ((IController)controller).Execute(requestContext); // first call
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    ((IController)controller).Execute(requestContext); // second call
                },
                @"A single instance of controller 'System.Web.Mvc.Test.ControllerBaseTest+EmptyControllerBase' cannot be used to handle multiple requests. If a custom controller factory is in use, make sure that it creates a new instance of the controller for each request.");

            // Assert
            Assert.AreEqual(1, controller.NumTimesExecuteCoreCalled);
        }

        [TestMethod]
        public void ExecuteThrowsIfRequestContextIsNull() {
            // Arrange
            IController controller = new ControllerBaseHelper();

            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    controller.Execute(null);
                }, "requestContext");
        }

        [TestMethod]
        public void InitializeSetsControllerContext() {
            // Arrange
            ControllerBaseHelper helper = new ControllerBaseHelper();
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, new RouteData());

            // Act
            helper.PublicInitialize(requestContext);

            // Assert
            Assert.AreSame(requestContext.HttpContext, helper.ControllerContext.HttpContext);
            Assert.AreSame(requestContext.RouteData, helper.ControllerContext.RouteData);
            Assert.AreSame(helper, helper.ControllerContext.Controller);
        }

        [TestMethod]
        public void TempDataProperty() {
            // Arrange
            ControllerBase controller = new ControllerBaseHelper();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(controller, "TempData", new TempDataDictionary());
        }

        [TestMethod]
        public void TempDataReturnsParentTempDataWhenInChildRequest() {
            // Arrange
            TempDataDictionary tempData = new TempDataDictionary();
            ViewContext viewContext = new ViewContext { TempData = tempData };
            RouteData routeData = new RouteData();
            routeData.DataTokens[ControllerContext.PARENT_ACTION_VIEWCONTEXT] = viewContext;
            RequestContext requestContext = new RequestContext(new Mock<HttpContextBase>().Object, routeData);
            ControllerBaseHelper controller = new ControllerBaseHelper();
            controller.PublicInitialize(requestContext);

            // Act
            TempDataDictionary result = controller.TempData;

            // Assert
            Assert.AreSame(result, tempData);
        }

        [TestMethod]
        public void ValidateRequestProperty() {
            // Arrange
            ControllerBase controller = new ControllerBaseHelper();

            // Act & assert
            MemberHelper.TestBooleanProperty(controller, "ValidateRequest", true /* initialValue */, false /* testDefaultValue */);
        }

        [TestMethod]
        public void ValueProviderProperty() {
            // Arrange
            ControllerBase controller = new ControllerBaseHelper();
            IValueProvider valueProvider = new SimpleValueProvider();

            // Act & assert
            ValueProviderFactory[] originalFactories = ValueProviderFactories.Factories.ToArray();
            try {
                ValueProviderFactories.Factories.Clear();
                MemberHelper.TestPropertyWithDefaultInstance(controller, "ValueProvider", valueProvider);
            }
            finally {
                foreach (ValueProviderFactory factory in originalFactories) {
                    ValueProviderFactories.Factories.Add(factory);
                }
            }
        }

        [TestMethod]
        public void ViewDataProperty() {
            // Arrange
            ControllerBase controller = new ControllerBaseHelper();

            // Act & Assert
            MemberHelper.TestPropertyWithDefaultInstance(controller, "ViewData", new ViewDataDictionary());
        }

        public class ControllerBaseHelper : ControllerBase {
            protected override void Initialize(RequestContext requestContext) {
                PublicInitialize(requestContext);
            }
            public virtual void PublicInitialize(RequestContext requestContext) {
                base.Initialize(requestContext);
            }
            protected override void ExecuteCore() {
                PublicExecuteCore();
            }
            public virtual void PublicExecuteCore() {
                throw new NotImplementedException();
            }
        }

        private class EmptyControllerBase : ControllerBase {
            public int NumTimesExecuteCoreCalled = 0;
            protected override void ExecuteCore() {
                NumTimesExecuteCoreCalled++;
            }
        }

    }
}
