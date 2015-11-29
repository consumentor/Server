namespace System.Web.Mvc.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ViewDataInfoTest {
        [TestMethod]
        public void ViewDataInfoDoesNotCallAccessorUntilValuePropertyAccessed() {
            // Arrange
            bool called = false;
            ViewDataInfo vdi = new ViewDataInfo(() => { called = true; return 21; });

            // Act & Assert
            Assert.IsFalse(called);
            object result = vdi.Value;
            Assert.IsTrue(called);
            Assert.AreEqual(21, result);
        }

        [TestMethod]
        public void AccessorIsOnlyCalledOnce() {
            // Arrange
            int callCount = 0;
            ViewDataInfo vdi = new ViewDataInfo(() => { ++callCount; return null; });

            // Act & Assert
            Assert.AreEqual(0, callCount);
            object unused;
            unused = vdi.Value;
            unused = vdi.Value;
            unused = vdi.Value;
            Assert.AreEqual(1, callCount);
        }

        [TestMethod]
        public void SettingExplicitValueOverridesAccessorMethod() {
            // Arrange
            bool called = false;
            ViewDataInfo vdi = new ViewDataInfo(() => { called = true; return null; });

            // Act & Assert
            Assert.IsFalse(called);
            vdi.Value = 42;
            object result = vdi.Value;
            Assert.IsFalse(called);
            Assert.AreEqual(42, result);
        }
    }
}
