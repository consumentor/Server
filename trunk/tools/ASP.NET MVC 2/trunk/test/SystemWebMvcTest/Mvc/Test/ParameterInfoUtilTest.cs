namespace System.Web.Mvc.Test {
    using System.ComponentModel;
    using System.Reflection;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ParameterInfoUtilTest {

        [TestMethod]
        public void TryGetDefaultValue_FirstChecksRawDefaultValue() {
            // Arrange
            Mock<ParameterInfo> mockPInfo = new Mock<ParameterInfo>() { DefaultValue = DefaultValue.Mock };
            mockPInfo.Expect(p => p.RawDefaultValue).Returns(42);
            mockPInfo.Expect(p => p.Name).Returns("someParameter");

            // Act
            object defaultValue;
            bool retVal = ParameterInfoUtil.TryGetDefaultValue(mockPInfo.Object, out defaultValue);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual(42, defaultValue);
        }

        [TestMethod]
        public void TryGetDefaultValue_SecondChecksDefaultValueAttribute() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("DefaultValues").GetParameters()[1]; // hasDefaultValue

            // Act
            object defaultValue;
            bool retVal = ParameterInfoUtil.TryGetDefaultValue(pInfo, out defaultValue);

            // Assert
            Assert.IsTrue(retVal);
            Assert.AreEqual("someValue", defaultValue);
        }

        [TestMethod]
        public void TryGetDefaultValue_RespectsNullRawDefaultValue() {
            // Arrange
            Mock<ParameterInfo> mockPInfo = new Mock<ParameterInfo>() { DefaultValue = DefaultValue.Mock };
            mockPInfo.Expect(p => p.RawDefaultValue).Returns(null);
            mockPInfo.Expect(p => p.Name).Returns("someParameter");
            mockPInfo
                .Expect(p => p.GetCustomAttributes(typeof(DefaultValueAttribute[]), false))
                .Returns(new DefaultValueAttribute[] { new DefaultValueAttribute(42) });

            // Act
            object defaultValue;
            bool retVal = ParameterInfoUtil.TryGetDefaultValue(mockPInfo.Object, out defaultValue);

            // Assert
            Assert.IsTrue(retVal);
            Assert.IsNull(defaultValue, "Shouldn't have looked at [DefaultValue] attribute.");
        }

        [TestMethod]
        public void TryGetDefaultValue_ReturnsFalseIfNoDefaultValue() {
            // Arrange
            ParameterInfo pInfo = typeof(MyController).GetMethod("DefaultValues").GetParameters()[0]; // noDefaultValue

            // Act
            object defaultValue;
            bool retVal = ParameterInfoUtil.TryGetDefaultValue(pInfo, out defaultValue);

            // Assert
            Assert.IsFalse(retVal);
            Assert.AreEqual(default(object), defaultValue);
        }

        private class MyController : Controller {
            public void DefaultValues(string noDefaultValue, [DefaultValue("someValue")] string hasDefaultValue) {
            }
        }

    }
}
