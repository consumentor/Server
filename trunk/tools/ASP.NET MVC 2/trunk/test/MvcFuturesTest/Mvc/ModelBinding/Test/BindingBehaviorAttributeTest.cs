namespace Microsoft.Web.Mvc.ModelBinding.Test {
    using System;
    using Microsoft.Web.Mvc.ModelBinding;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BindingBehaviorAttributeTest {

        [TestMethod]
        public void Behavior_Property() {
            // Arrange
            BindingBehavior expectedBehavior = (BindingBehavior)(-20);

            // Act
            BindingBehaviorAttribute attr = new BindingBehaviorAttribute(expectedBehavior);

            // Assert
            Assert.AreEqual(expectedBehavior, attr.Behavior);
        }

        [TestMethod]
        public void TypeId_ReturnsSameValue() {
            // Arrange
            BindNeverAttribute neverAttr = new BindNeverAttribute();
            BindRequiredAttribute requiredAttr = new BindRequiredAttribute();

            // Act & assert
            Assert.AreSame(neverAttr.TypeId, requiredAttr.TypeId);
        }

        [TestMethod]
        public void BindNever_SetsBehavior() {
            // Act
            BindingBehaviorAttribute attr = new BindNeverAttribute();

            // Assert
            Assert.AreEqual(BindingBehavior.Never, attr.Behavior);
        }

        [TestMethod]
        public void BindRequired_SetsBehavior() {
            // Act
            BindingBehaviorAttribute attr = new BindRequiredAttribute();

            // Assert
            Assert.AreEqual(BindingBehavior.Required, attr.Behavior);
        }

    }
}
