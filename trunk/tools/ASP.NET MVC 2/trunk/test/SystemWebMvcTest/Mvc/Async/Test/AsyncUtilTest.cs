namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AsyncUtilTest {

        [TestMethod]
        public void WaitForAsyncResultCompletion_DoesNothingIfResultAlreadyCompleted() {
            // Arrange
            MockAsyncResult asyncResult = new MockAsyncResult() {
                IsCompleted = true
            };

            // Act
            AsyncUtil.WaitForAsyncResultCompletion(asyncResult, null /* app */);

            // Assert
            // If we reached this point of execution, the operation completed
        }

        [TestMethod]
        public void WaitForAsyncResultCompletion_SpinsThreadIfNoWaitHandleAvailable() {
            // Arrange
            MockAsyncResult asyncResult = new MockAsyncResult() {
                AsyncWaitHandle = null,
                IsCompleted = false
            };
            HttpApplication app = new HttpApplication();

            Timer timer = new Timer(_ => {
                lock (app) {
                    asyncResult.IsCompleted = true;
                }
            }, null, 1000, Timeout.Infinite);

            // Act
            lock (app) {
                AsyncUtil.WaitForAsyncResultCompletion(asyncResult, app);
            }
        }

        [TestMethod]
        public void WaitForAsyncResultCompletion_WaitsOnWaitHandleIfAvailable() {
            // Arrange
            MockAsyncResult asyncResult = new MockAsyncResult() {
                IsCompleted = false
            };
            HttpApplication app = new HttpApplication();

            Timer timer = new Timer(_ => {
                lock (app) {
                    asyncResult.IsCompleted = true;
                    asyncResult.AsyncWaitHandle.Set();
                }
            }, null, 1000, Timeout.Infinite);

            // Act
            lock (app) {
                AsyncUtil.WaitForAsyncResultCompletion(asyncResult, app);
            }
        }

        [TestMethod]
        public void WrapCallbackForSynchronizedExecution_CallsSyncIfOperationCompletedAsynchronously() {
            // Arrange
            MockAsyncResult asyncResult = new MockAsyncResult() {
                CompletedSynchronously = false,
                IsCompleted = true
            };

            bool originalCallbackCalled = false;
            AsyncCallback originalCallback = ar => {
                Assert.AreEqual(asyncResult, ar);
                originalCallbackCalled = true;
            };

            DummySynchronizationContext syncContext = new DummySynchronizationContext();

            // Act
            AsyncCallback retVal = AsyncUtil.WrapCallbackForSynchronizedExecution(originalCallback, syncContext);
            retVal(asyncResult);

            // Assert
            Assert.IsTrue(originalCallbackCalled);
            Assert.IsTrue(syncContext.SendCalled);
        }

        [TestMethod]
        public void WrapCallbackForSynchronizedExecution_DoesNotCallSyncIfOperationCompletedSynchronously() {
            // Arrange
            MockAsyncResult asyncResult = new MockAsyncResult() {
                CompletedSynchronously = true,
                IsCompleted = true
            };

            bool originalCallbackCalled = false;
            AsyncCallback originalCallback = ar => {
                Assert.AreEqual(asyncResult, ar);
                originalCallbackCalled = true;
            };

            DummySynchronizationContext syncContext = new DummySynchronizationContext();

            // Act
            AsyncCallback retVal = AsyncUtil.WrapCallbackForSynchronizedExecution(originalCallback, syncContext);
            retVal(asyncResult);

            // Assert
            Assert.IsTrue(originalCallbackCalled);
            Assert.IsFalse(syncContext.SendCalled);
        }

        [TestMethod]
        public void WrapCallbackForSynchronizedExecution_ReturnsNullIfCallbackIsNull() {
            // Act
            AsyncCallback retVal = AsyncUtil.WrapCallbackForSynchronizedExecution(null, new SynchronizationContext());

            // Assert
            Assert.IsNull(retVal);
        }

        [TestMethod]
        public void WrapCallbackForSynchronizedExecution_ReturnsOriginalCallbackIfSyncContextIsNull() {
            // Arrange
            AsyncCallback originalCallback = _ => { };

            // Act
            AsyncCallback retVal = AsyncUtil.WrapCallbackForSynchronizedExecution(originalCallback, null);

            // Assert
            Assert.AreSame(originalCallback, retVal);
        }

        private class DummySynchronizationContext : SynchronizationContext {
            public bool SendCalled { get; private set; }

            public override void Send(SendOrPostCallback d, object state) {
                SendCalled = true;
                base.Send(d, state);
            }
        }

    }
}
