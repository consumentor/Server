namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SimpleAsyncResultTest {

        [TestMethod]
        public void AsyncStateProperty() {
            // Arrange
            string expected = "Hello!";
            SimpleAsyncResult asyncResult = new SimpleAsyncResult(expected);

            // Act
            object asyncState = asyncResult.AsyncState;

            // Assert
            Assert.AreEqual(expected, asyncState);
        }

        [TestMethod]
        public void AsyncWaitHandleProperty() {
            // Arrange
            SimpleAsyncResult asyncResult = new SimpleAsyncResult(null);

            // Act
            WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;

            // Assert
            Assert.IsNull(asyncWaitHandle);
        }

        [TestMethod]
        public void CompletedSynchronouslyProperty() {
            // Arrange
            SimpleAsyncResult asyncResult = new SimpleAsyncResult(null);

            // Act
            bool completedSynchronously = asyncResult.CompletedSynchronously;

            // Assert
            Assert.IsFalse(completedSynchronously, "CompletedSynchronously should default to false.");
        }

        [TestMethod]
        public void IsCompletedProperty() {
            // Arrange
            SimpleAsyncResult asyncResult = new SimpleAsyncResult(null);

            // Act
            bool isCompleted = asyncResult.IsCompleted;

            // Assert
            Assert.IsFalse(isCompleted, "IsCompleted should default to false.");
        }

        [TestMethod]
        public void MarkCompleted_AsynchronousCompletion() {
            // Arrange
            SimpleAsyncResult asyncResult = new SimpleAsyncResult(null);

            bool callbackWasCalled = false;
            AsyncCallback callback = ar => {
                callbackWasCalled = true;
                Assert.AreEqual(asyncResult, ar, "Wrong IAsyncResult passed to callback.");
                Assert.IsTrue(ar.IsCompleted, "IsCompleted property should have been true.");
                Assert.IsFalse(ar.CompletedSynchronously, "CompletedSynchronously property should have been false.");
            };

            // Act & assert
            asyncResult.MarkCompleted(false, callback);
            Assert.IsTrue(callbackWasCalled);
        }

        [TestMethod]
        public void MarkCompleted_SynchronousCompletion() {
            // Arrange
            SimpleAsyncResult asyncResult = new SimpleAsyncResult(null);

            bool callbackWasCalled = false;
            AsyncCallback callback = ar => {
                callbackWasCalled = true;
                Assert.AreEqual(asyncResult, ar, "Wrong IAsyncResult passed to callback.");
                Assert.IsTrue(ar.IsCompleted, "IsCompleted property should have been true.");
                Assert.IsTrue(ar.CompletedSynchronously, "CompletedSynchronously property should have been true.");
            };

            // Act & assert
            asyncResult.MarkCompleted(true, callback);
            Assert.IsTrue(callbackWasCalled);
        }

    }
}
