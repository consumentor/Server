namespace System.Web.Mvc.Async.Test {
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SynchronousOperationExceptionTest {

        [TestMethod]
        public void ConstructorWithMessageAndInnerExceptionParameter() {
            // Arrange
            Exception innerException = new Exception();

            // Act
            SynchronousOperationException ex = new SynchronousOperationException("the message", innerException);

            // Assert
            Assert.AreEqual("the message", ex.Message);
            Assert.AreEqual(innerException, ex.InnerException);
        }

        [TestMethod]
        public void ConstructorWithMessageParameter() {
            // Act
            SynchronousOperationException ex = new SynchronousOperationException("the message");

            // Assert
            Assert.AreEqual("the message", ex.Message);
        }

        [TestMethod]
        public void ConstructorWithoutParameters() {
            // Act & assert
            ExceptionHelper.ExpectException<SynchronousOperationException>(
                delegate {
                    throw new SynchronousOperationException();
                });
        }

        [TestMethod]
        public void TypeIsSerializable() {
            // Arrange
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            SynchronousOperationException ex = new SynchronousOperationException("the message", new Exception("inner exception"));

            // Act
            formatter.Serialize(ms, ex);
            ms.Position = 0;
            SynchronousOperationException deserialized = formatter.Deserialize(ms) as SynchronousOperationException;

            // Assert
            Assert.IsNotNull(deserialized, "Deserialization process did not return the exception.");
            Assert.AreEqual("the message", deserialized.Message);
            Assert.IsNotNull(deserialized.InnerException);
            Assert.AreEqual("inner exception", deserialized.InnerException.Message);
        }

    }
}
