namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Web.Mvc.Async;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AsyncActionDescriptorTest {

        [TestMethod]
        public void Execute_CalledSynchronously() {
            // Arrange
            HttpApplication app = new HttpApplication();
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.ApplicationInstance).Returns(app);

            AsyncActionDescriptor actionDescriptor = new SynchronouslyCalledAsyncActionDescriptor();

            // Act
            object retVal;
            lock (app) {
                retVal = actionDescriptor.Execute(mockControllerContext.Object, null);
            }

            // Assert
            Assert.AreEqual(retVal, "sample value");
        }

        private class SynchronouslyCalledAsyncActionDescriptor : AsyncActionDescriptor {
            private Timer _timer;

            public override IAsyncResult BeginExecute(ControllerContext controllerContext, IDictionary<string, object> parameters, AsyncCallback callback, object state) {
                MockAsyncResult asyncResult = new MockAsyncResult() {
                    IsCompleted = false,
                    CompletedSynchronously = false
                };

                _timer = new Timer(_ => {
                    lock (controllerContext.HttpContext.ApplicationInstance) {
                        asyncResult.IsCompleted = true;
                        asyncResult.AsyncWaitHandle.Set();
                    }
                }, null, 1000, Timeout.Infinite);

                return asyncResult;
            }

            public override object EndExecute(IAsyncResult asyncResult) {
                return "sample value";
            }

            public override string ActionName {
                get { throw new NotImplementedException(); }
            }

            public override ControllerDescriptor ControllerDescriptor {
                get { throw new NotImplementedException(); }
            }

            public override ParameterDescriptor[] GetParameters() {
                throw new NotImplementedException();
            }
        }

    }
}
