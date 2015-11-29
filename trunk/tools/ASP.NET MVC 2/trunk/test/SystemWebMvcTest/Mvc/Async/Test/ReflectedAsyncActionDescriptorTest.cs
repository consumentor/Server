namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc.Async;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ReflectedAsyncActionDescriptorTest {

        private readonly MethodInfo _asyncMethod = typeof(ExecuteController).GetMethod("FooAsync");
        private readonly MethodInfo _completedMethod = typeof(ExecuteController).GetMethod("FooCompleted");

        [TestMethod]
        public void Constructor_SetsProperties() {
            // Arrange
            string actionName = "SomeAction";
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act
            ReflectedAsyncActionDescriptor ad = new ReflectedAsyncActionDescriptor(_asyncMethod, _completedMethod, actionName, cd);

            // Assert
            Assert.AreEqual(_asyncMethod, ad.AsyncMethodInfo);
            Assert.AreEqual(_completedMethod, ad.CompletedMethodInfo);
            Assert.AreEqual(actionName, ad.ActionName);
            Assert.AreEqual(cd, ad.ControllerDescriptor);
        }

        [TestMethod]
        public void Constructor_ThrowsIfActionNameIsEmpty() {
            // Arrange
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedAsyncActionDescriptor(_asyncMethod, _completedMethod, "", cd);
                }, "actionName");
        }

        [TestMethod]
        public void Constructor_ThrowsIfActionNameIsNull() {
            // Arrange
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    new ReflectedAsyncActionDescriptor(_asyncMethod, _completedMethod, null, cd);
                }, "actionName");
        }

        [TestMethod]
        public void Constructor_ThrowsIfAsyncMethodInfoIsInvalid() {
            // Arrange
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            MethodInfo getHashCodeMethod = typeof(object).GetMethod("GetHashCode");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedAsyncActionDescriptor(getHashCodeMethod, _completedMethod, "SomeAction", cd);
                },
                @"Cannot create a descriptor for instance method 'Int32 GetHashCode()' on type 'System.Object' because the type does not derive from ControllerBase.
Parameter name: asyncMethodInfo");
        }

        [TestMethod]
        public void Constructor_ThrowsIfAsyncMethodInfoIsNull() {
            // Arrange
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedAsyncActionDescriptor(null, _completedMethod, "SomeAction", cd);
                }, "asyncMethodInfo");
        }

        [TestMethod]
        public void Constructor_ThrowsIfCompletedMethodInfoIsInvalid() {
            // Arrange
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;
            MethodInfo getHashCodeMethod = typeof(object).GetMethod("GetHashCode");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    new ReflectedAsyncActionDescriptor(_asyncMethod, getHashCodeMethod, "SomeAction", cd);
                },
                @"Cannot create a descriptor for instance method 'Int32 GetHashCode()' on type 'System.Object' because the type does not derive from ControllerBase.
Parameter name: completedMethodInfo");
        }

        [TestMethod]
        public void Constructor_ThrowsIfCompletedMethodInfoIsNull() {
            // Arrange
            ControllerDescriptor cd = new Mock<ControllerDescriptor>().Object;

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedAsyncActionDescriptor(_asyncMethod, null, "SomeAction", cd);
                }, "completedMethodInfo");
        }

        [TestMethod]
        public void Constructor_ThrowsIfControllerDescriptorIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new ReflectedAsyncActionDescriptor(_asyncMethod, _completedMethod, "SomeAction", null);
                }, "controllerDescriptor");
        }

        [TestMethod]
        public void Execute() {
            // Arrange
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.Controller).Returns(new ExecuteController());
            ControllerContext controllerContext = mockControllerContext.Object;

            Dictionary<string, object> parameters = new Dictionary<string, object>() {
                { "id1", 42 }
            };

            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);

            SignalContainer<object> resultContainer = new SignalContainer<object>();
            AsyncCallback callback = ar => {
                object o = ad.EndExecute(ar);
                resultContainer.Signal(o);
            };

            // Act
            ad.BeginExecute(controllerContext, parameters, callback, null);
            object retVal = resultContainer.Wait();

            // Assert
            Assert.AreEqual("Hello world: 42", retVal);
        }

        [TestMethod]
        public void Execute_ThrowsIfControllerContextIsNull() {
            // Arrange
            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.BeginExecute(null, new Dictionary<string, object>(), null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void Execute_ThrowsIfControllerIsNotAsyncManagerContainer() {
            // Arrange
            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);
            ControllerContext controllerContext = new ControllerContext() {
                Controller = new RegularSyncController()
            };

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    ad.BeginExecute(controllerContext, new Dictionary<string, object>(), null, null);
                },
                @"The controller of type 'System.Web.Mvc.Async.Test.ReflectedAsyncActionDescriptorTest+RegularSyncController' must subclass AsyncController or implement the IAsyncManagerContainer interface.");
        }

        [TestMethod]
        public void Execute_ThrowsIfParametersIsNull() {
            // Arrange
            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    ad.BeginExecute(new ControllerContext(), null, null, null);
                }, "parameters");
        }

        [TestMethod]
        public void GetCustomAttributes() {
            // Arrange
            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);

            // Act
            object[] attributes = ad.GetCustomAttributes(true /* inherit */);

            // Assert
            Assert.AreEqual(1, attributes.Length);
            Assert.AreEqual(typeof(AuthorizeAttribute), attributes[0].GetType());
        }

        [TestMethod]
        public void GetCustomAttributes_FilterByType() {
            // Shouldn't match attributes on the Completed() method, only the Async() method

            // Arrange
            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);

            // Act
            object[] attributes = ad.GetCustomAttributes(typeof(OutputCacheAttribute), true /* inherit */);

            // Assert
            Assert.AreEqual(0, attributes.Length);
        }

        [TestMethod]
        public void GetFilters_ConsidersOnlyAsyncMethod() {
            // Arrange
            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);

            // Act
            FilterInfo filterInfo = ad.GetFilters();

            // Assert
            Assert.AreEqual(0, filterInfo.ActionFilters.Count, "Filters on Completed() method should be excluded from consideration.");
        }

        [TestMethod]
        public void GetParameters() {
            // Arrange
            ParameterInfo pInfo = _asyncMethod.GetParameters()[0];
            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);

            // Act
            ParameterDescriptor[] pDescsFirstCall = ad.GetParameters();
            ParameterDescriptor[] pDescsSecondCall = ad.GetParameters();

            // Assert
            Assert.AreNotSame(pDescsFirstCall, pDescsSecondCall, "GetParameters() should return a new array on each invocation.");
            CollectionAssert.AreEqual(pDescsFirstCall, pDescsSecondCall, "Array elements were not equal.");
            Assert.AreEqual(1, pDescsFirstCall.Length);

            ReflectedParameterDescriptor pDesc = pDescsFirstCall[0] as ReflectedParameterDescriptor;

            Assert.IsNotNull(pDesc, "Parameter 0 should have been of type ReflectedParameterDescriptor.");
            Assert.AreSame(ad, pDesc.ActionDescriptor, "Parameter 0 Action did not match.");
            Assert.AreSame(pInfo, pDesc.ParameterInfo, "Parameter 0 ParameterInfo did not match.");
        }

        [TestMethod]
        public void GetSelectors() {
            // Arrange
            ControllerContext controllerContext = new Mock<ControllerContext>().Object;
            Mock<MethodInfo> mockMethod = new Mock<MethodInfo>();

            Mock<ActionMethodSelectorAttribute> mockAttr = new Mock<ActionMethodSelectorAttribute>();
            mockAttr.Expect(attr => attr.IsValidForRequest(controllerContext, mockMethod.Object)).Returns(true).Verifiable();
            mockMethod.Expect(m => m.GetCustomAttributes(typeof(ActionMethodSelectorAttribute), true)).Returns(new ActionMethodSelectorAttribute[] { mockAttr.Object });

            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(mockMethod.Object, _completedMethod);

            // Act
            ICollection<ActionSelector> selectors = ad.GetSelectors();
            bool executedSuccessfully = selectors.All(s => s(controllerContext));

            // Assert
            Assert.AreEqual(1, selectors.Count);
            Assert.IsTrue(executedSuccessfully);
            mockAttr.Verify();
        }

        [TestMethod]
        public void IsDefined() {
            // Arrange
            ReflectedAsyncActionDescriptor ad = GetActionDescriptor(_asyncMethod, _completedMethod);

            // Act
            bool isDefined = ad.IsDefined(typeof(AuthorizeAttribute), true /* inherit */);

            // Assert
            Assert.IsTrue(isDefined);
        }

        private static ReflectedAsyncActionDescriptor GetActionDescriptor(MethodInfo asyncMethod, MethodInfo completedMethod) {
            return new ReflectedAsyncActionDescriptor(asyncMethod, completedMethod, "someName", new Mock<ControllerDescriptor>().Object, false /* validateMethod */) {
                DispatcherCache = new ActionMethodDispatcherCache()
            };
        }

        private class ExecuteController : AsyncController {
            private Func<object, string> _func;

            [Authorize]
            public void FooAsync(int id1) {
                _func = o => Convert.ToString(o, CultureInfo.InvariantCulture) + id1.ToString(CultureInfo.InvariantCulture);
                AsyncManager.Parameters["id2"] = "Hello world: ";
                AsyncManager.Finish();
            }

            [OutputCache]
            public string FooCompleted(string id2) {
                return _func(id2);
            }

            public string FooWithBool(bool id2) {
                return _func(id2);
            }

            public string FooWithException(Exception id2) {
                return _func(id2);
            }
        }

        private class RegularSyncController : Controller {
        }

    }
}
