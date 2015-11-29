namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Threading;
    using System.Web.Mvc.Async;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class AsyncManagerExtensionsTest {

        [TestMethod]
        public void RegisterTask_AsynchronousCompletion() {
            // Arrange
            SimpleSynchronizationContext syncContext = new SimpleSynchronizationContext();
            AsyncManager asyncManager = new AsyncManager(syncContext);
            bool endDelegateWasCalled = false;

            ManualResetEvent waitHandle = new ManualResetEvent(false /* initialState */);

            Func<AsyncCallback, IAsyncResult> beginDelegate = callback => {
                Assert.AreEqual(1, asyncManager.OutstandingOperations.Count, "Counter was not incremented properly.");
                MockAsyncResult asyncResult = new MockAsyncResult(false /* completedSynchronously */);
                ThreadPool.QueueUserWorkItem(_ => {
                    Assert.AreEqual(1, asyncManager.OutstandingOperations.Count, "Counter shouldn't have been decremented yet.");
                    callback(asyncResult);
                    waitHandle.Set();
                });
                return asyncResult;
            };
            Action<IAsyncResult> endDelegate = delegate { endDelegateWasCalled = true; };

            // Act
            asyncManager.RegisterTask(beginDelegate, endDelegate);
            waitHandle.WaitOne();

            // Assert
            Assert.IsTrue(endDelegateWasCalled);
            Assert.IsTrue(syncContext.SendWasCalled, "Asynchronous call to End() should have been routed through SynchronizationContext.Send()");
            Assert.AreEqual(0, asyncManager.OutstandingOperations.Count, "Counter was not decremented properly.");
        }

        [TestMethod]
        public void RegisterTask_AsynchronousCompletion_SwallowsExceptionsThrownByEndDelegate() {
            // Arrange
            SimpleSynchronizationContext syncContext = new SimpleSynchronizationContext();
            AsyncManager asyncManager = new AsyncManager(syncContext);
            bool endDelegateWasCalled = false;

            ManualResetEvent waitHandle = new ManualResetEvent(false /* initialState */);

            Func<AsyncCallback, IAsyncResult> beginDelegate = callback => {
                MockAsyncResult asyncResult = new MockAsyncResult(false /* completedSynchronously */);
                ThreadPool.QueueUserWorkItem(_ => {
                    callback(asyncResult);
                    waitHandle.Set();
                });
                return asyncResult;
            };
            Action<IAsyncResult> endDelegate = delegate {
                endDelegateWasCalled = true;
                throw new Exception("This is a sample exception.");
            };

            // Act
            asyncManager.RegisterTask(beginDelegate, endDelegate);
            waitHandle.WaitOne();

            // Assert
            Assert.IsTrue(endDelegateWasCalled);
            Assert.AreEqual(0, asyncManager.OutstandingOperations.Count, "Counter was not decremented properly.");
        }

        [TestMethod]
        public void RegisterTask_ResetsOutstandingOperationCountIfBeginMethodThrows() {
            // Arrange
            SimpleSynchronizationContext syncContext = new SimpleSynchronizationContext();
            AsyncManager asyncManager = new AsyncManager(syncContext);

            Func<AsyncCallback, IAsyncResult> beginDelegate = cb => {
                throw new InvalidOperationException("BeginDelegate throws.");
            };
            Action<IAsyncResult> endDelegate = ar => {
                Assert.Fail("This should never be called.");
            };

            // Act & assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    asyncManager.RegisterTask(beginDelegate, endDelegate);
                }, "BeginDelegate throws.");

            Assert.AreEqual(0, asyncManager.OutstandingOperations.Count);
        }

        [TestMethod]
        public void RegisterTask_SynchronousCompletion() {
            // Arrange
            SimpleSynchronizationContext syncContext = new SimpleSynchronizationContext();
            AsyncManager asyncManager = new AsyncManager(syncContext);
            bool endDelegateWasCalled = false;

            Func<AsyncCallback, IAsyncResult> beginDelegate = callback => {
                Assert.AreEqual(1, asyncManager.OutstandingOperations.Count, "Counter was not incremented properly.");
                MockAsyncResult asyncResult = new MockAsyncResult(true /* completedSynchronously */);
                callback(asyncResult);
                return asyncResult;
            };
            Action<IAsyncResult> endDelegate = delegate { endDelegateWasCalled = true; };

            // Act
            asyncManager.RegisterTask(beginDelegate, endDelegate);

            // Assert
            Assert.IsTrue(endDelegateWasCalled);
            Assert.IsFalse(syncContext.SendWasCalled, "Synchronous call to End() should not have been routed through SynchronizationContext.Send()");
            Assert.AreEqual(0, asyncManager.OutstandingOperations.Count, "Counter was not decremented properly.");
        }

        [TestMethod]
        public void RegisterTask_ThrowsIfAsyncManagerIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AsyncManagerExtensions.RegisterTask(null, _ => null, _ => { });
                }, "asyncManager");
        }

        [TestMethod]
        public void RegisterTask_ThrowsIfBeginDelegateIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AsyncManagerExtensions.RegisterTask(new AsyncManager(), null, _ => { });
                }, "beginDelegate");
        }

        [TestMethod]
        public void RegisterTask_ThrowsIfEndDelegateIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AsyncManagerExtensions.RegisterTask(new AsyncManager(), _ => null, null);
                }, "endDelegate");
        }

        private class SimpleSynchronizationContext : SynchronizationContext {
            public bool SendWasCalled;

            public override void Send(SendOrPostCallback d, object state) {
                SendWasCalled = true;
                d(state);
            }
        }

        private class MockAsyncResult : IAsyncResult {
            private readonly bool _completedSynchronously;

            public MockAsyncResult(bool completedSynchronously) {
                _completedSynchronously = completedSynchronously;
            }

            public object AsyncState {
                get { throw new NotImplementedException(); }
            }

            public WaitHandle AsyncWaitHandle {
                get { throw new NotImplementedException(); }
            }

            public bool CompletedSynchronously {
                get { return _completedSynchronously; }
            }

            public bool IsCompleted {
                get { throw new NotImplementedException(); }
            }
        }

    }
}
