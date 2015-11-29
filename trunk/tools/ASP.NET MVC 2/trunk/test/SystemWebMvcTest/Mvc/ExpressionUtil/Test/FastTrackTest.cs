namespace System.Web.Mvc.ExpressionUtil.Test {
    using System;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FastTrackTest {

        [TestMethod]
        public void GetFunc_Constant() {
            // Arrange
            Expression<Func<string, int>> expr = model => 2;

            // Act
            Func<string, int> func = FastTrack<string, int>.GetFunc(expr.Parameters[0], expr.Body);
            int result = func("anything");

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetFunc_Identity() {
            // Arrange
            Expression<Func<string, object>> expr = model => model;

            // Act
            Func<string, object> func = FastTrack<string, object>.GetFunc(expr.Parameters[0], expr.Body);
            object result = func("test phrase");


            // Assert
            Assert.AreEqual("test phrase", result);
        }

        [TestMethod]
        public void GetFunc_InstanceMemberOnConst() {
            // Arrange
            Expression<Func<string, int>> expr = model => SomeInstanceProperty;

            // Act
            Func<string, int> func = FastTrack<string, int>.GetFunc(expr.Parameters[0], expr.Body);
            int result = func("anything");

            // Assert
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void GetFunc_InstanceMemberOnModel() {
            // Arrange
            Expression<Func<string, int>> expr = model => model.Length;
            int expected = "Hello, world".Length;

            // Act
            Func<string, int> func = FastTrack<string, int>.GetFunc(expr.Parameters[0], expr.Body);
            int result = func("Hello, world");

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetFunc_StaticMember() {
            // Arrange
            Expression<Func<string, int>> expr = model => SomeStaticProperty;

            // Act
            Func<string, int> func = FastTrack<string, int>.GetFunc(expr.Parameters[0], expr.Body);
            int result = func("anything");

            // Assert
            Assert.AreEqual(40, result);
        }

        [TestMethod]
        public void GetFuncReturnsNullIfUnknownPattern() {
            // Arrange
            Expression<Func<string[], int>> expr = model => model[0].Length; // array indexing is unknown to the fast-tracker

            // Act
            Func<string[], int> func = FastTrack<string[], int>.GetFunc(expr.Parameters[0], expr.Body);

            // Assert
            Assert.IsNull(func, "Unknown pattern should result in null return value.");
        }

        private int SomeInstanceProperty {
            get {
                return 20;
            }
        }

        private static int SomeStaticProperty {
            get {
                return 40;
            }
        }

    }
}
