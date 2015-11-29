namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Threading;
    using System.Web.Mvc.Async;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AsyncResultWrapperTest {

        [TestMethod]
        public void Begin_AsynchronousCompletion() {
            // Arrange
            AsyncCallback capturedCallback = null;
            IAsyncResult resultGivenToCallback = null;
            IAsyncResult innerResult = new MockAsyncResult();

            // Act
            IAsyncResult outerResult = AsyncResultWrapper.Begin(
                ar => { resultGivenToCallback = ar; },
                null,
                (callback, state) => {
                    capturedCallback = callback;
                    return innerResult;
                },
                ar => { });

            capturedCallback(innerResult);

            // Assert
            Assert.AreEqual(outerResult, resultGivenToCallback);
        }

        [TestMethod]
        public void Begin_ReturnsAsyncResultWhichWrapsInnerResult() {
            // Arrange
            IAsyncResult innerResult = new MockAsyncResult() {
                AsyncState = "inner state",
                CompletedSynchronously = true,
                IsCompleted = true
            };

            // Act
            IAsyncResult outerResult = AsyncResultWrapper.Begin(null, "outer state",
                (callback, state) => innerResult,
                ar => { });

            // Assert
            Assert.AreEqual(innerResult.AsyncState, outerResult.AsyncState);
            Assert.AreEqual(innerResult.AsyncWaitHandle, outerResult.AsyncWaitHandle);
            Assert.AreEqual(innerResult.CompletedSynchronously, outerResult.CompletedSynchronously);
            Assert.AreEqual(innerResult.IsCompleted, outerResult.IsCompleted);
        }

        [TestMethod]
        public void Begin_SynchronousCompletion() {
            // Arrange
            IAsyncResult resultGivenToCallback = null;
            IAsyncResult innerResult = new MockAsyncResult();

            // Act
            IAsyncResult outerResult = AsyncResultWrapper.Begin(
                ar => { resultGivenToCallback = ar; },
                null,
                (callback, state) => {
                    callback(innerResult);
                    return innerResult;
                },
                ar => { });

            // Assert
            Assert.AreEqual(outerResult, resultGivenToCallback);
        }

        [TestMethod]
        public void BeginSynchronous_Action() {
            // Arrange
            bool actionCalled = false;

            // Act
            IAsyncResult asyncResult = AsyncResultWrapper.BeginSynchronous(null, null, delegate { actionCalled = true; });
            AsyncResultWrapper.End(asyncResult);

            // Assert
            Assert.IsTrue(actionCalled);
            Assert.IsTrue(asyncResult.IsCompleted);
            Assert.IsTrue(asyncResult.CompletedSynchronously);
        }

        [TestMethod]
        public void BeginSynchronous_Func() {
            // Act
            IAsyncResult asyncResult = AsyncResultWrapper.BeginSynchronous(null, null, () => 42);
            int retVal = AsyncResultWrapper.End<int>(asyncResult);

            // Assert
            Assert.AreEqual(42, retVal);
            Assert.IsTrue(asyncResult.IsCompleted);
            Assert.IsTrue(asyncResult.CompletedSynchronously);
        }

        [TestMethod]
        public void End_ExecutesStoredDelegateAndReturnsValue() {
            // Arrange
            IAsyncResult asyncResult = AsyncResultWrapper.Begin(null, null,
                (callback, state) => new MockAsyncResult(),
                ar => 42);

            // Act
            int returned = AsyncResultWrapper.End<int>(asyncResult);

            // Assert
            Assert.AreEqual(42, returned);
        }

        [TestMethod]
        public void End_ThrowsIfAsyncResultIsIncorrectType() {
            // Arrange
            IAsyncResult asyncResult = AsyncResultWrapper.Begin(null, null,
                (callback, state) => new MockAsyncResult(),
                ar => { });

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    AsyncResultWrapper.End<int>(asyncResult);
                },
                @"The provided IAsyncResult is not valid for this method.
Parameter name: asyncResult");
        }

        [TestMethod]
        public void End_ThrowsIfAsyncResultIsNull() {
            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    AsyncResultWrapper.End(null);
                }, "asyncResult");
        }

        [TestMethod]
        public void End_ThrowsIfAsyncResultTagMismatch() {
            // Arrange
            IAsyncResult asyncResult = AsyncResultWrapper.Begin(null, null,
                (callback, state) => new MockAsyncResult(),
                ar => { },
                "some tag");

            // Act & assert
            ExceptionHelper.ExpectArgumentException(
                delegate {
                    AsyncResultWrapper.End(asyncResult, "some other tag");
                },
                @"The provided IAsyncResult is not valid for this method.
Parameter name: asyncResult");
        }

        [TestMethod]
        public void End_ThrowsIfCalledTwiceOnSameAsyncResult() {
            // Arrange
            IAsyncResult asyncResult = AsyncResultWrapper.Begin(null, null,
                (callback, state) => new MockAsyncResult(),
                ar => { });

            // Act & assert
            AsyncResultWrapper.End(asyncResult);
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    AsyncResultWrapper.End(asyncResult);
                },
                @"The provided IAsyncResult has already been consumed.");
        }

        [TestMethod]
        public void TimedOut() {
            // Arrange
            ManualResetEvent waitHandle = new ManualResetEvent(false /* initialState */);

            AsyncCallback callback = ar => {
                waitHandle.Set();
            };

            // Act & assert
            IAsyncResult asyncResult = AsyncResultWrapper.Begin(callback, null,
                (innerCallback, innerState) => new MockAsyncResult(),
                ar => {
                    Assert.Fail("This callback should never execute since we timed out.");
                },
                null, 0);

            // wait for the timeout
            waitHandle.WaitOne();

            ExceptionHelper.ExpectException<TimeoutException>(
                delegate {
                    AsyncResultWrapper.End(asyncResult);
                });
        }

    }
}
