namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AsyncControllerActionInvokerTest {

        [TestMethod]
        public void InvokeAction_ActionNotFound() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act
            IAsyncResult asyncResult = invoker.BeginInvokeAction(controllerContext, "ActionNotFound", null, null);
            bool retVal = invoker.EndInvokeAction(asyncResult);

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void InvokeAction_ActionThrowsException_Handled() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            IAsyncResult asyncResult = invoker.BeginInvokeAction(controllerContext, "ActionThrowsExceptionAndIsHandled", null, null);
            Assert.IsNull(((TestController)controllerContext.Controller).Log, "Result filter shouldn't have executed yet.");

            bool retVal = invoker.EndInvokeAction(asyncResult);
            Assert.IsTrue(retVal);
            Assert.AreEqual("From exception filter", ((TestController)controllerContext.Controller).Log);
        }

        [TestMethod]
        public void InvokeAction_ActionThrowsException_NotHandled() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    invoker.BeginInvokeAction(controllerContext, "ActionThrowsExceptionAndIsNotHandled", null, null);
                },
                @"Some exception text.");
        }

        [TestMethod]
        public void InvokeAction_ActionThrowsException_ThreadAbort() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    invoker.BeginInvokeAction(controllerContext, "ActionCallsThreadAbort", null, null);
                });
        }

        [TestMethod]
        public void InvokeAction_AuthorizationFilterShortCircuits() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act
            IAsyncResult asyncResult = invoker.BeginInvokeAction(controllerContext, "AuthorizationFilterShortCircuits", null, null);
            bool retVal = invoker.EndInvokeAction(asyncResult);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual("From authorization filter", ((TestController)controllerContext.Controller).Log);
        }

        [TestMethod]
        public void InvokeAction_NormalAction() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act
            IAsyncResult asyncResult = invoker.BeginInvokeAction(controllerContext, "NormalAction", null, null);
            bool retVal = invoker.EndInvokeAction(asyncResult);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual("From action", ((TestController)controllerContext.Controller).Log);
        }

        [TestMethod]
        public void InvokeAction_RequestValidationFails() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext(false /* passesRequestValidation */);
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            ExceptionHelper.ExpectException<HttpRequestValidationException>(
                delegate {
                    invoker.BeginInvokeAction(controllerContext, "NormalAction", null, null);
                });
        }

        [TestMethod]
        public void InvokeAction_ResultThrowsException_Handled() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            IAsyncResult asyncResult = invoker.BeginInvokeAction(controllerContext, "ResultThrowsExceptionAndIsHandled", null, null);
            bool retVal = invoker.EndInvokeAction(asyncResult);

            Assert.IsTrue(retVal);
            Assert.AreEqual("From exception filter", ((TestController)controllerContext.Controller).Log);
        }

        [TestMethod]
        public void InvokeAction_ResultThrowsException_NotHandled() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            IAsyncResult asyncResult = invoker.BeginInvokeAction(controllerContext, "ResultThrowsExceptionAndIsNotHandled", null, null);
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    invoker.EndInvokeAction(asyncResult);
                },
                @"Some exception text.");
        }

        [TestMethod]
        public void InvokeAction_ResultThrowsException_ThreadAbort() {
            // Arrange
            ControllerContext controllerContext = GetControllerContext();
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            IAsyncResult asyncResult = invoker.BeginInvokeAction(controllerContext, "ResultCallsThreadAbort", null, null);
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    invoker.EndInvokeAction(asyncResult);
                });
        }

        [TestMethod]
        public void InvokeAction_ThrowsIfActionNameIsEmpty() {
            // Arrange
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    invoker.BeginInvokeAction(new ControllerContext(), "", null, null);
                }, "actionName");
        }

        [TestMethod]
        public void InvokeAction_ThrowsIfActionNameIsNull() {
            // Arrange
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    invoker.BeginInvokeAction(new ControllerContext(), null, null, null);
                }, "actionName");
        }

        [TestMethod]
        public void InvokeAction_ThrowsIfControllerContextIsNull() {
            // Arrange
            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    invoker.BeginInvokeAction(null, "someAction", null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void InvokeActionMethod_AsynchronousDescriptor() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            IAsyncResult innerAsyncResult = new MockAsyncResult();
            ActionResult expectedResult = new ViewResult();

            Mock<AsyncActionDescriptor> mockActionDescriptor = new Mock<AsyncActionDescriptor>();
            mockActionDescriptor.Expect(d => d.BeginExecute(controllerContext, parameters, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(innerAsyncResult);
            mockActionDescriptor.Expect(d => d.EndExecute(innerAsyncResult)).Returns(expectedResult);

            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act
            IAsyncResult asyncResult = invoker.BeginInvokeActionMethod(controllerContext, mockActionDescriptor.Object, parameters, null, null);
            ActionResult returnedResult = invoker.EndInvokeActionMethod(asyncResult);

            // Assert
            Assert.AreEqual(expectedResult, returnedResult);
        }

        [TestMethod]
        public void InvokeActionMethod_SynchronousDescriptor() {
            // Arrange
            ControllerContext controllerContext = new ControllerContext();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionResult expectedResult = new ViewResult();

            Mock<ActionDescriptor> mockActionDescriptor = new Mock<ActionDescriptor>();
            mockActionDescriptor.Expect(d => d.Execute(controllerContext, parameters)).Returns(expectedResult);

            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();

            // Act
            IAsyncResult asyncResult = invoker.BeginInvokeActionMethod(controllerContext, mockActionDescriptor.Object, parameters, null, null);
            ActionResult returnedResult = invoker.EndInvokeActionMethod(asyncResult);

            // Assert
            Assert.AreEqual(expectedResult, returnedResult);
        }

        [TestMethod]
        public void InvokeActionMethodFilterAsynchronously_NextInChainThrowsOnActionExecutedException_Handled() {
            // Arrange
            ViewResult expectedResult = new ViewResult();

            bool nextInChainWasCalled = false;
            bool onActionExecutedWasCalled = false;

            ActionExecutingContext preContext = GetActionExecutingContext();
            ActionFilterImpl actionFilter = new ActionFilterImpl() {
                OnActionExecutedImpl = filterContext => {
                    onActionExecutedWasCalled = true;
                    Assert.IsNotNull(filterContext.Exception);
                    filterContext.ExceptionHandled = true;
                    filterContext.Result = expectedResult;
                }
            };

            // Act & assert pre-execution
            Func<ActionExecutedContext> continuation = AsyncControllerActionInvoker.InvokeActionMethodFilterAsynchronously(actionFilter, preContext,
                () => () => {
                    nextInChainWasCalled = true;
                    throw new Exception("Some exception text.");
                });

            Assert.IsFalse(onActionExecutedWasCalled, "OnActionExecuted() shouldn't have been called yet.");

            // Act & assert post-execution
            ActionExecutedContext postContext = continuation();

            Assert.IsTrue(nextInChainWasCalled, "Next in chain should've been called.");
            Assert.IsTrue(onActionExecutedWasCalled, "OnActionExecuted() should've been called.");
            Assert.AreEqual(expectedResult, postContext.Result);
        }

        [TestMethod]
        public void InvokeActionMethodFilterAsynchronously_NextInChainThrowsOnActionExecutedException_NotHandled() {
            // Arrange
            ViewResult expectedResult = new ViewResult();

            bool onActionExecutedWasCalled = false;

            ActionExecutingContext preContext = GetActionExecutingContext();
            ActionFilterImpl actionFilter = new ActionFilterImpl() {
                OnActionExecutedImpl = filterContext => {
                    onActionExecutedWasCalled = true;
                }
            };

            // Act & assert
            Func<ActionExecutedContext> continuation = AsyncControllerActionInvoker.InvokeActionMethodFilterAsynchronously(actionFilter, preContext,
                () => () => {
                    throw new Exception("Some exception text.");
                });

            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    continuation();
                },
                @"Some exception text.");

            // Assert
            Assert.IsTrue(onActionExecutedWasCalled, "OnActionExecuted() should've been called.");
        }

        [TestMethod]
        public void InvokeActionMethodFilterAsynchronously_NextInChainThrowsOnActionExecutedException_ThreadAbort() {
            // Arrange
            ViewResult expectedResult = new ViewResult();

            bool onActionExecutedWasCalled = false;

            ActionExecutingContext preContext = GetActionExecutingContext();
            ActionFilterImpl actionFilter = new ActionFilterImpl() {
                OnActionExecutedImpl = filterContext => {
                    onActionExecutedWasCalled = true;
                    Thread.ResetAbort();
                }
            };

            // Act & assert
            Func<ActionExecutedContext> continuation = AsyncControllerActionInvoker.InvokeActionMethodFilterAsynchronously(actionFilter, preContext,
                () => () => {
                    Thread.CurrentThread.Abort();
                    return null;
                });

            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    continuation();
                });

            // Assert
            Assert.IsTrue(onActionExecutedWasCalled, "OnActionExecuted() should've been called.");
        }

        [TestMethod]
        public void InvokeActionMethodFilterAsynchronously_NextInChainThrowsOnActionExecutingException_Handled() {
            // Arrange
            ViewResult expectedResult = new ViewResult();

            bool nextInChainWasCalled = false;
            bool onActionExecutingWasCalled = false;
            bool onActionExecutedWasCalled = false;

            ActionExecutingContext preContext = GetActionExecutingContext();
            ActionFilterImpl actionFilter = new ActionFilterImpl() {
                OnActionExecutingImpl = filterContext => {
                    onActionExecutingWasCalled = true;
                },
                OnActionExecutedImpl = filterContext => {
                    onActionExecutedWasCalled = true;
                    Assert.IsNotNull(filterContext.Exception);
                    filterContext.ExceptionHandled = true;
                    filterContext.Result = expectedResult;
                }
            };

            // Act
            Func<ActionExecutedContext> continuation = AsyncControllerActionInvoker.InvokeActionMethodFilterAsynchronously(actionFilter, preContext,
                () => {
                    nextInChainWasCalled = true;
                    throw new Exception("Some exception text.");
                });

            // Assert
            Assert.IsTrue(nextInChainWasCalled, "Next in chain should've been called.");
            Assert.IsTrue(onActionExecutingWasCalled, "OnActionExecuting() should've been called by the first invocation.");
            Assert.IsTrue(onActionExecutedWasCalled, "OnActionExecuted() should've been called due to exception handling.");

            ActionExecutedContext postContext = continuation();
            Assert.AreEqual(expectedResult, postContext.Result);
        }

        [TestMethod]
        public void InvokeActionMethodFilterAsynchronously_NextInChainThrowsOnActionExecutingException_NotHandled() {
            // Arrange
            ViewResult expectedResult = new ViewResult();

            bool nextInChainWasCalled = false;
            bool onActionExecutingWasCalled = false;
            bool onActionExecutedWasCalled = false;

            ActionExecutingContext preContext = GetActionExecutingContext();
            ActionFilterImpl actionFilter = new ActionFilterImpl() {
                OnActionExecutingImpl = filterContext => {
                    onActionExecutingWasCalled = true;
                },
                OnActionExecutedImpl = filterContext => {
                    onActionExecutedWasCalled = true;
                }
            };

            // Act & assert
            ExceptionHelper.ExpectException<Exception>(
                delegate {
                    AsyncControllerActionInvoker.InvokeActionMethodFilterAsynchronously(actionFilter, preContext,
                        () => {
                            nextInChainWasCalled = true;
                            throw new Exception("Some exception text.");
                        });
                },
                @"Some exception text.");

            // Assert
            Assert.IsTrue(nextInChainWasCalled, "Next in chain should've been called.");
            Assert.IsTrue(onActionExecutingWasCalled, "OnActionExecuting() should've been called by the first invocation.");
            Assert.IsTrue(onActionExecutedWasCalled, "OnActionExecuted() should've been called due to exception handling.");
        }

        [TestMethod]
        public void InvokeActionMethodFilterAsynchronously_NextInChainThrowsOnActionExecutingException_ThreadAbort() {
            // Arrange
            ViewResult expectedResult = new ViewResult();

            bool nextInChainWasCalled = false;
            bool onActionExecutingWasCalled = false;
            bool onActionExecutedWasCalled = false;

            ActionExecutingContext preContext = GetActionExecutingContext();
            ActionFilterImpl actionFilter = new ActionFilterImpl() {
                OnActionExecutingImpl = filterContext => {
                    onActionExecutingWasCalled = true;
                },
                OnActionExecutedImpl = filterContext => {
                    onActionExecutedWasCalled = true;
                    Thread.ResetAbort();
                }
            };

            // Act & assert
            ExceptionHelper.ExpectException<ThreadAbortException>(
                delegate {
                    AsyncControllerActionInvoker.InvokeActionMethodFilterAsynchronously(actionFilter, preContext,
                        () => {
                            nextInChainWasCalled = true;
                            Thread.CurrentThread.Abort();
                            return null;
                        });
                });

            // Assert
            Assert.IsTrue(nextInChainWasCalled, "Next in chain should've been called.");
            Assert.IsTrue(onActionExecutingWasCalled, "OnActionExecuting() should've been called by the first invocation.");
            Assert.IsTrue(onActionExecutedWasCalled, "OnActionExecuted() should've been called due to exception handling.");
        }

        [TestMethod]
        public void InvokeActionMethodFilterAsynchronously_NormalExecutionNotCanceled() {
            // Arrange
            bool nextInChainWasCalled = false;
            bool onActionExecutingWasCalled = false;
            bool onActionExecutedWasCalled = false;

            ActionExecutingContext preContext = GetActionExecutingContext();
            ActionFilterImpl actionFilter = new ActionFilterImpl() {
                OnActionExecutingImpl = _ => { onActionExecutingWasCalled = true; },
                OnActionExecutedImpl = _ => { onActionExecutedWasCalled = true; }
            };

            // Act
            Func<ActionExecutedContext> continuation = AsyncControllerActionInvoker.InvokeActionMethodFilterAsynchronously(actionFilter, preContext,
                () => {
                    nextInChainWasCalled = true;
                    return () => new ActionExecutedContext();
                });

            // Assert
            Assert.IsTrue(nextInChainWasCalled);
            Assert.IsTrue(onActionExecutingWasCalled, "OnActionExecuting() should've been called by the first invocation.");
            Assert.IsFalse(onActionExecutedWasCalled, "OnActionExecuted() shouldn't have been called by the first invocation.");

            continuation();
            Assert.IsTrue(onActionExecutedWasCalled, "OnActionExecuted() should've been called by the second invocation.");
        }

        [TestMethod]
        public void InvokeActionMethodFilterAsynchronously_OnActionExecutingSetsResult() {
            // Arrange
            ViewResult expectedResult = new ViewResult();

            bool nextInChainWasCalled = false;
            bool onActionExecutingWasCalled = false;
            bool onActionExecutedWasCalled = false;

            ActionExecutingContext preContext = GetActionExecutingContext();
            ActionFilterImpl actionFilter = new ActionFilterImpl() {
                OnActionExecutingImpl = filterContext => {
                    onActionExecutingWasCalled = true;
                    filterContext.Result = expectedResult;
                },
                OnActionExecutedImpl = _ => { onActionExecutedWasCalled = true; }
            };

            // Act
            Func<ActionExecutedContext> continuation = AsyncControllerActionInvoker.InvokeActionMethodFilterAsynchronously(actionFilter, preContext,
                () => {
                    nextInChainWasCalled = true;
                    return () => new ActionExecutedContext();
                });

            // Assert
            Assert.IsFalse(nextInChainWasCalled, "Next in chain shouldn't have been called due to short circuiting.");
            Assert.IsTrue(onActionExecutingWasCalled, "OnActionExecuting() should've been called by the first invocation.");
            Assert.IsFalse(onActionExecutedWasCalled, "OnActionExecuted() shouldn't have been called by the first invocation.");

            ActionExecutedContext postContext = continuation();
            Assert.IsFalse(onActionExecutedWasCalled, "OnActionExecuted() shouldn't have been called by the second invocation.");
            Assert.AreEqual(expectedResult, postContext.Result);
        }

        [TestMethod]
        public void InvokeActionMethodWithFilters() {
            // Arrange
            List<string> actionLog = new List<string>();
            ControllerContext controllerContext = new ControllerContext();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            MockAsyncResult innerAsyncResult = new MockAsyncResult();
            ActionResult actionResult = new ViewResult();

            ActionFilterImpl filter1 = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actionLog.Add("OnActionExecuting1");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actionLog.Add("OnActionExecuted1");
                }
            };
            ActionFilterImpl filter2 = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actionLog.Add("OnActionExecuting2");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actionLog.Add("OnActionExecuted2");
                }
            };

            Mock<AsyncActionDescriptor> mockActionDescriptor = new Mock<AsyncActionDescriptor>();
            mockActionDescriptor.Expect(d => d.BeginExecute(controllerContext, parameters, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Returns(innerAsyncResult);
            mockActionDescriptor.Expect(d => d.EndExecute(innerAsyncResult)).Returns(actionResult);

            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();
            IActionFilter[] filters = new IActionFilter[] { filter1, filter2 };

            // Act
            IAsyncResult outerAsyncResult = invoker.BeginInvokeActionMethodWithFilters(controllerContext, filters, mockActionDescriptor.Object, parameters, null, null);
            ActionExecutedContext postContext = invoker.EndInvokeActionMethodWithFilters(outerAsyncResult);

            // Assert
            CollectionAssert.AreEqual(
                new string[] { "OnActionExecuting1", "OnActionExecuting2", "OnActionExecuted2", "OnActionExecuted1" },
                actionLog);
            Assert.AreEqual(actionResult, postContext.Result);
        }

        [TestMethod]
        public void InvokeActionMethodWithFilters_ShortCircuited() {
            // Arrange
            List<string> actionLog = new List<string>();
            ControllerContext controllerContext = new ControllerContext();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            ActionResult actionResult = new ViewResult();

            ActionFilterImpl filter1 = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actionLog.Add("OnActionExecuting1");
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actionLog.Add("OnActionExecuted1");
                }
            };
            ActionFilterImpl filter2 = new ActionFilterImpl() {
                OnActionExecutingImpl = delegate(ActionExecutingContext filterContext) {
                    actionLog.Add("OnActionExecuting2");
                    filterContext.Result = actionResult;
                },
                OnActionExecutedImpl = delegate(ActionExecutedContext filterContext) {
                    actionLog.Add("OnActionExecuted2");
                }
            };

            Mock<AsyncActionDescriptor> mockActionDescriptor = new Mock<AsyncActionDescriptor>();
            mockActionDescriptor.Expect(d => d.BeginExecute(controllerContext, parameters, It.IsAny<AsyncCallback>(), It.IsAny<object>())).Throws(new Exception("I shouldn't have been called."));
            mockActionDescriptor.Expect(d => d.EndExecute(It.IsAny<IAsyncResult>())).Throws(new Exception("I shouldn't have been called."));

            AsyncControllerActionInvoker invoker = new AsyncControllerActionInvoker();
            IActionFilter[] filters = new IActionFilter[] { filter1, filter2 };

            // Act
            IAsyncResult outerAsyncResult = invoker.BeginInvokeActionMethodWithFilters(controllerContext, filters, mockActionDescriptor.Object, parameters, null, null);
            ActionExecutedContext postContext = invoker.EndInvokeActionMethodWithFilters(outerAsyncResult);

            // Assert
            CollectionAssert.AreEqual(
                new string[] { "OnActionExecuting1", "OnActionExecuting2", "OnActionExecuted1" },
                actionLog);
            Assert.AreEqual(actionResult, postContext.Result);
        }

        private static ActionExecutingContext GetActionExecutingContext() {
            return new ActionExecutingContext(new ControllerContext(), new Mock<ActionDescriptor>().Object, new Dictionary<string, object>());
        }

        private static ControllerContext GetControllerContext() {
            return GetControllerContext(true /* passesRequestValidation */);
        }

        private static ControllerContext GetControllerContext(bool passesRequestValidation) {
            Mock<HttpContextBase> mockHttpContext = new Mock<HttpContextBase>();
            if (passesRequestValidation) {
                mockHttpContext.Expect(o => o.Request.ValidateInput()).AtMostOnce();
            }
            else {
                mockHttpContext.Expect(o => o.Request.ValidateInput()).Throws(new HttpRequestValidationException());
            }

            return new ControllerContext() {
                Controller = new TestController(),
                HttpContext = mockHttpContext.Object
            };
        }

        private class ActionFilterImpl : IActionFilter, IResultFilter {

            public Action<ActionExecutingContext> OnActionExecutingImpl {
                get;
                set;
            }

            public void OnActionExecuting(ActionExecutingContext filterContext) {
                if (OnActionExecutingImpl != null) {
                    OnActionExecutingImpl(filterContext);
                }
            }

            public Action<ActionExecutedContext> OnActionExecutedImpl {
                get;
                set;
            }

            public void OnActionExecuted(ActionExecutedContext filterContext) {
                if (OnActionExecutedImpl != null) {
                    OnActionExecutedImpl(filterContext);
                }
            }

            public Action<ResultExecutingContext> OnResultExecutingImpl {
                get;
                set;
            }

            public void OnResultExecuting(ResultExecutingContext filterContext) {
                if (OnResultExecutingImpl != null) {
                    OnResultExecutingImpl(filterContext);
                }
            }

            public Action<ResultExecutedContext> OnResultExecutedImpl {
                get;
                set;
            }

            public void OnResultExecuted(ResultExecutedContext filterContext) {
                if (OnResultExecutedImpl != null) {
                    OnResultExecutedImpl(filterContext);
                }
            }

        }

        public class AsyncControllerActionInvokerHelper : AsyncControllerActionInvoker {

            public AsyncControllerActionInvokerHelper() {
                DescriptorCache = new ControllerDescriptorCache();
            }

            protected override ControllerDescriptor GetControllerDescriptor(ControllerContext controllerContext) {
                return PublicGetControllerDescriptor(controllerContext);
            }
            public virtual ControllerDescriptor PublicGetControllerDescriptor(ControllerContext controllerContext) {
                return base.GetControllerDescriptor(controllerContext);
            }
            protected override ExceptionContext InvokeExceptionFilters(ControllerContext controllerContext, IList<IExceptionFilter> filters, Exception exception) {
                return PublicInvokeExceptionFilters(controllerContext, filters, exception);
            }
            public virtual ExceptionContext PublicInvokeExceptionFilters(ControllerContext controllerContext, IList<IExceptionFilter> filters, Exception exception) {
                return base.InvokeExceptionFilters(controllerContext, filters, exception);
            }
            protected override void InvokeActionResult(ControllerContext controllerContext, ActionResult actionResult) {
                PublicInvokeActionResult(controllerContext, actionResult);
            }
            public virtual void PublicInvokeActionResult(ControllerContext controllerContext, ActionResult actionResult) {
                base.InvokeActionResult(controllerContext, actionResult);
            }
            protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return PublicGetFilters(controllerContext, actionDescriptor);
            }
            public virtual FilterInfo PublicGetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return base.GetFilters(controllerContext, actionDescriptor);
            }
            protected override AuthorizationContext InvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters, ActionDescriptor actionDescriptor) {
                return PublicInvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);
            }
            public virtual AuthorizationContext PublicInvokeAuthorizationFilters(ControllerContext controllerContext, IList<IAuthorizationFilter> filters, ActionDescriptor actionDescriptor) {
                return base.InvokeAuthorizationFilters(controllerContext, filters, actionDescriptor);
            }
            protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName) {
                return PublicFindAction(controllerContext, controllerDescriptor, actionName);
            }
            public virtual ActionDescriptor PublicFindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName) {
                return base.FindAction(controllerContext, controllerDescriptor, actionName);
            }
            protected override IDictionary<string, object> GetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return PublicGetParameterValues(controllerContext, actionDescriptor);
            }
            public virtual IDictionary<string, object> PublicGetParameterValues(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
                return base.GetParameterValues(controllerContext, actionDescriptor);
            }
        }

        [ResetThreadAbort]
        private class TestController : AsyncController {

            public string Log;

            public ActionResult ActionCallsThreadAbortAsync() {
                Thread.CurrentThread.Abort();
                return null;
            }

            public ActionResult ActionCallsThreadAbortCompleted() {
                return null;
            }

            public ActionResult ResultCallsThreadAbort() {
                return new ActionResultWhichCallsThreadAbort();
            }

            public ActionResult NormalAction() {
                return new LoggingActionResult("From action");
            }

            [AuthorizationFilterReturnsResult]
            public void AuthorizationFilterShortCircuits() {
            }

            [CustomExceptionFilterHandlesError]
            public void ActionThrowsExceptionAndIsHandledAsync() {
                throw new Exception("Some exception text.");
            }

            public void ActionThrowsExceptionAndIsHandledCompleted() {
            }

            [CustomExceptionFilterDoesNotHandleError]
            public void ActionThrowsExceptionAndIsNotHandledAsync() {
                throw new Exception("Some exception text.");
            }

            public void ActionThrowsExceptionAndIsNotHandledCompleted() {
            }

            [CustomExceptionFilterHandlesError]
            public ActionResult ResultThrowsExceptionAndIsHandled() {
                return new ActionResultWhichThrowsException();
            }

            [CustomExceptionFilterDoesNotHandleError]
            public ActionResult ResultThrowsExceptionAndIsNotHandled() {
                return new ActionResultWhichThrowsException();
            }

            private class AuthorizationFilterReturnsResultAttribute : FilterAttribute, IAuthorizationFilter {
                public void OnAuthorization(AuthorizationContext filterContext) {
                    filterContext.Result = new LoggingActionResult("From authorization filter");
                }
            }

            private class CustomExceptionFilterDoesNotHandleErrorAttribute : FilterAttribute, IExceptionFilter {
                public void OnException(ExceptionContext filterContext) {
                }
            }

            private class CustomExceptionFilterHandlesErrorAttribute : FilterAttribute, IExceptionFilter {
                public void OnException(ExceptionContext filterContext) {
                    filterContext.ExceptionHandled = true;
                    filterContext.Result = new LoggingActionResult("From exception filter");
                }
            }

            private class ActionResultWhichCallsThreadAbort : ActionResult {
                public override void ExecuteResult(ControllerContext context) {
                    Thread.CurrentThread.Abort();
                }
            }

            private class ActionResultWhichThrowsException : ActionResult {
                public override void ExecuteResult(ControllerContext context) {
                    throw new Exception("Some exception text.");
                }
            }
        }

        private class ResetThreadAbortAttribute : ActionFilterAttribute {
            public override void OnActionExecuted(ActionExecutedContext filterContext) {
                try {
                    Thread.ResetAbort();
                }
                catch (ThreadStateException) {
                    // thread wasn't being aborted
                }
            }
            public override void OnResultExecuted(ResultExecutedContext filterContext) {
                try {
                    Thread.ResetAbort();
                }
                catch (ThreadStateException) {
                    // thread wasn't being aborted
                }
            }
        }

        private class LoggingActionResult : ActionResult {
            private readonly string _logText;

            public LoggingActionResult(string logText) {
                _logText = logText;
            }

            public override void ExecuteResult(ControllerContext context) {
                ((TestController)context.Controller).Log = _logText;
            }
        }

    }
}
