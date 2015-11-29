namespace System.Web.Mvc.Async.Test {
    using System;
    using System.Threading;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class AsyncManagerTest {

        [TestMethod]
        public void FinishEvent_ExplicitCallToFinishMethod() {
            // Arrange
            AsyncManager helper = new AsyncManager();

            bool delegateCalled = false;
            helper.Finished += delegate { delegateCalled = true; };

            // Act
            helper.Finish();

            // Assert
            Assert.IsTrue(delegateCalled);
        }

        [TestMethod]
        public void FinishEvent_LinkedToOutstandingOperationsCompletedEvent() {
            // Arrange
            AsyncManager helper = new AsyncManager();

            bool delegateCalled = false;
            helper.Finished += delegate { delegateCalled = true; };

            // Act
            helper.OutstandingOperations.Increment();
            helper.OutstandingOperations.Decrement();

            // Assert
            Assert.IsTrue(delegateCalled);
        }

        [TestMethod]
        public void OutstandingOperationsProperty() {
            // Act
            AsyncManager helper = new AsyncManager();

            // Assert
            Assert.IsNotNull(helper.OutstandingOperations);
        }

        [TestMethod]
        public void ParametersProperty() {
            // Act
            AsyncManager helper = new AsyncManager();

            // Assert
            Assert.IsNotNull(helper.Parameters);
        }

        [TestMethod]
        public void Sync() {
            // Arrange
            Mock<SynchronizationContext> mockSyncContext = new Mock<SynchronizationContext>();
            mockSyncContext
                .Expect(c => c.Send(It.IsAny<SendOrPostCallback>(), null))
                .Callback(
                    delegate(SendOrPostCallback d, object state) {
                        d(state);
                    });

            AsyncManager helper = new AsyncManager(mockSyncContext.Object);
            bool wasCalled = false;

            // Act
            helper.Sync(() => { wasCalled = true; });

            // Assert
            Assert.IsTrue(wasCalled);
        }

        [TestMethod]
        public void TimeoutProperty() {
            // Arrange
            int setValue = 50;
            AsyncManager helper = new AsyncManager();

            // Act
            int defaultTimeout = helper.Timeout;
            helper.Timeout = setValue;
            int newTimeout = helper.Timeout;

            // Assert
            Assert.AreEqual(45000, defaultTimeout);
            Assert.AreEqual(setValue, newTimeout);
        }

        [TestMethod]
        public void TimeoutPropertyThrowsIfDurationIsOutOfRange() {
            // Arrange
            int timeout = -30;
            AsyncManager helper = new AsyncManager();

            // Act & assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    helper.Timeout = timeout;
                }, "value",
                @"The timeout value must be non-negative or Timeout.Infinite.
Parameter name: value");
        }

    }
}
