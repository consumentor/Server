namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ExpressionHelper = Microsoft.Web.Mvc.Internal.ExpressionHelper;

    [TestClass]
    public class ExpressionHelperTest {
        [TestMethod]
        public void BuildRouteValueDictionary_TargetsAsynchronousAsyncMethod_StripsSuffix() {
            // Arrange
            Expression<Action<TestAsyncController>> expr = (c => c.AsynchronousAsync());

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(expr);

            // Assert
            Assert.AreEqual("Asynchronous", rvd["action"]);
            Assert.AreEqual("TestAsync", rvd["controller"]);
            Assert.IsFalse(rvd.ContainsKey("area"));
        }

        [TestMethod]
        public void BuildRouteValueDictionary_TargetsAsynchronousCompletedMethod_Throws() {
            // Arrange
            Expression<Action<TestAsyncController>> expr = (c => c.AsynchronousCompleted());

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    ExpressionHelper.GetRouteValuesFromExpression(expr);
                },
                @"The method 'AsynchronousCompleted' is an asynchronous completion method and cannot be called directly.");
        }

        [TestMethod]
        public void BuildRouteValueDictionary_TargetsControllerWithAreaAttribute_AddsAreaName() {
            // Arrange
            Expression<Action<ControllerWithAreaController>> expr = c => c.Index();

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(expr);

            // Assert
            Assert.AreEqual("Index", rvd["action"]);
            Assert.AreEqual("ControllerWithArea", rvd["controller"]);
            Assert.AreEqual("the area name", rvd["area"]);
        }

        [TestMethod]
        public void BuildRouteValueDictionary_TargetsNonActionMethod_Throws() {
            // Arrange
            Expression<Action<TestController>> expr = (c => c.NotAnAction());

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    ExpressionHelper.GetRouteValuesFromExpression(expr);
                },
                @"The method 'NotAnAction' is marked [NonAction] and cannot be called directly.");
        }

        [TestMethod]
        public void BuildRouteValueDictionary_TargetsRenamedMethod_UsesNewName() {
            // Arrange
            Expression<Action<TestController>> expr = (c => c.Renamed());

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(expr);

            // Assert
            Assert.AreEqual("NewName", rvd["action"]);
            Assert.AreEqual("Test", rvd["controller"]);
            Assert.IsFalse(rvd.ContainsKey("area"));
        }

        [TestMethod]
        public void BuildRouteValueDictionary_TargetsSynchronousMethodOnAsyncController_ReturnsOriginalName() {
            // Arrange
            Expression<Action<TestAsyncController>> expr = (c => c.Synchronous());

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(expr);

            // Assert
            Assert.AreEqual("Synchronous", rvd["action"]);
            Assert.AreEqual("TestAsync", rvd["controller"]);
            Assert.IsFalse(rvd.ContainsKey("area"));
        }

        [TestMethod]
        public void BuildRouteValueDictionaryWithNullExpressionThrowsArgumentNullException() {
            ExceptionHelper.ExpectArgumentNullException(
                () => ExpressionHelper.GetRouteValuesFromExpression<TestController>(null),
                "action");
        }

        [TestMethod]
        public void BuildRouteValueDictionaryWithNonMethodExpressionThrowsInvalidOperationException() {
            // Arrange
            Expression<Action<TestController>> expression = c => new TestController();

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => ExpressionHelper.GetRouteValuesFromExpression<TestController>(expression),
                "Expression must be a method call." + Environment.NewLine + "Parameter name: action");
        }

        [TestMethod]
        public void BuildRouteValueDictionaryWithoutControllerSuffixThrowsInvalidOperationException() {
            // Arrange
            Expression<Action<TestControllerNot>> index = (c => c.Index(123));

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => ExpressionHelper.GetRouteValuesFromExpression(index),
                "Controller name must end in 'Controller'." + Environment.NewLine + "Parameter name: action");
        }

        [TestMethod]
        public void BuildRouteValueDictionaryWithControllerBaseClassThrowsInvalidOperationException() {
            // Arrange
            Expression<Action<Controller>> index = (c => c.Dispose());

            // Act & Assert
            ExceptionHelper.ExpectArgumentException(
                () => ExpressionHelper.GetRouteValuesFromExpression(index),
                "Cannot route to class named 'Controller'." + Environment.NewLine + "Parameter name: action");
        }

        [TestMethod]
        public void BuildRouteValueDictionaryAddsControllerNameToDictionary() {
            // Arrange
            Expression<Action<TestController>> index = (c => c.Index(123));

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(index);

            // Assert
            Assert.AreEqual("Test", rvd["Controller"]);
        }

        [TestMethod]
        public void BuildRouteValueDictionaryFromExpressionReturnsCorrectDictionary() {
            // Arrange
            Expression<Action<TestController>> index = (c => c.Index(123));

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(index);

            // Assert
            Assert.AreEqual("Test", rvd["Controller"]);
            Assert.AreEqual("Index", rvd["Action"]);
            Assert.AreEqual(123, rvd["page"]);
        }

        [TestMethod]
        public void BuildRouteValueDictionaryFromNonConstantExpressionReturnsCorrectDictionary() {
            // Arrange
            Expression<Action<TestController>> index = (c => c.About(Foo));

            // Act
            RouteValueDictionary rvd = ExpressionHelper.GetRouteValuesFromExpression(index);

            // Assert
            Assert.AreEqual("Test", rvd["Controller"]);
            Assert.AreEqual("About", rvd["Action"]);
            Assert.AreEqual("FooValue", rvd["s"]);
        }

        [TestMethod]
        public void GetInputNameFromPropertyExpressionReturnsPropertyName() {
            // Arrange
            Expression<Func<TestModel, int>> expression = m => m.IntProperty;

            // Act
            string name = ExpressionHelper.GetInputName(expression);

            // Assert
            Assert.AreEqual("IntProperty", name);
        }

        [TestMethod]
        public void GetInputNameFromPropertyWithMethodCallExpressionReturnsPropertyName() {
            // Arrange
            Expression<Func<TestModel, string>> expression = m => m.IntProperty.ToString();

            // Act
            string name = ExpressionHelper.GetInputName(expression);

            // Assert
            Assert.AreEqual("IntProperty", name);
        }

        [TestMethod]
        public void GetInputNameFromPropertyWithTwoMethodCallExpressionReturnsPropertyName() {
            // Arrange
            Expression<Func<TestModel, string>> expression = m => m.IntProperty.ToString().ToUpper();

            // Act
            string name = ExpressionHelper.GetInputName(expression);

            // Assert
            Assert.AreEqual("IntProperty", name);
        }

        [TestMethod]
        public void GetInputNameFromExpressionWithTwoPropertiesUsesWholeExpression() {
            // Arrange
            Expression<Func<TestModel, int>> expression = m => m.StringProperty.Length;

            // Act
            string name = ExpressionHelper.GetInputName(expression);

            // Assert
            Assert.AreEqual("StringProperty.Length", name);
        }

        public class TestController : Controller {
            public ActionResult Index(int page) {
                return null;
            }

            public string About(string s) {
                return "The value is " + s;
            }

            [ActionName("NewName")]
            public void Renamed() {
            }

            [NonAction]
            public void NotAnAction() {
            }
        }

        public class TestAsyncController : AsyncController {
            public void Synchronous() {
            }

            public void AsynchronousAsync() {
            }

            public void AsynchronousCompleted() {
            }
        }

        public string Foo {
            get {
                return "FooValue";
            }
        }

        public class TestControllerNot : Controller {
            public ActionResult Index(int page) {
                return null;
            }
        }

        [ActionLinkArea("the area name")]
        public class ControllerWithAreaController : Controller {
            public ActionResult Index() {
                return null;
            }
        }

        public class TestModel {
            public int IntProperty {
                get;
                set;
            }
            public string StringProperty {
                get;
                set;
            }
        }
    }
}
