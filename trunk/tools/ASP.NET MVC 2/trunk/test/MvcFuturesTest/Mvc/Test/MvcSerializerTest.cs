namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Runtime.Serialization;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class MvcSerializerTest {

        [TestMethod]
        public void DeserializeThrowsIfModeIsOutOfRange() {
            // Arrange
            MvcSerializer serializer = new MvcSerializer();

            // Act & assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    serializer.Serialize("blah", (SerializationMode)(-1));
                },
                "mode",
                @"The provided SerializationMode is invalid.
Parameter name: mode");
        }

        [TestMethod]
        public void DeserializeThrowsIfSerializedValueIsCorrupt() {
            // Arrange
            MvcSerializer serializer = new MvcSerializer();

            // Act & assert
            Exception exception = ExceptionHelper.ExpectException<SerializationException>(
                delegate {
                    serializer.Deserialize("This is a corrupted value.", SerializationMode.Plaintext);
                },
                @"Deserialization failed. Verify that the data is being deserialized using the same SerializationMode with which it was serialized. Otherwise see the inner exception.");

            Assert.IsNotNull(exception.InnerException, "Inner exception was not propagated correctly.");
        }

        [TestMethod]
        public void DeserializeThrowsIfSerializedValueIsEmpty() {
            // Arrange
            MvcSerializer serializer = new MvcSerializer();

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    serializer.Deserialize("");
                }, "serializedValue");
        }

        [TestMethod]
        public void DeserializeThrowsIfSerializedValueIsNull() {
            // Arrange
            MvcSerializer serializer = new MvcSerializer();

            // Act & assert
            ExceptionHelper.ExpectArgumentExceptionNullOrEmpty(
                delegate {
                    serializer.Deserialize(null);
                }, "serializedValue");
        }

        [TestMethod]
        public void SerializeAllowsNullValues() {
            // Arrange
            MvcSerializer serializer = new MvcSerializer();

            // Act
            string serializedValue = serializer.Serialize(null, SerializationMode.Plaintext);

            // Assert
            Assert.AreEqual(@"/wFk", serializedValue);
        }

        [TestMethod]
        public void SerializeAndDeserializeRoundTripsValue() {
            // Arrange
            MvcSerializer serializer = new MvcSerializer();

            // Act
            string serializedValue = serializer.Serialize(42, SerializationMode.Plaintext);
            object deserializedValue = serializer.Deserialize(serializedValue, SerializationMode.Plaintext);

            // Assert
            Assert.AreEqual(42, deserializedValue, "Value was not round-tripped properly.");
        }

        [TestMethod]
        public void SerializeThrowsIfModeIsOutOfRange() {
            // Arrange
            MvcSerializer serializer = new MvcSerializer();

            // Act & assert
            ExceptionHelper.ExpectArgumentOutOfRangeException(
                delegate {
                    serializer.Serialize(null, (SerializationMode)(-1));
                },
                "mode",
                @"The provided SerializationMode is invalid.
Parameter name: mode");
        }

    }
}
