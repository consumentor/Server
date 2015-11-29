namespace Microsoft.Web.Mvc.Test {
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;

    [TestClass]
    public class LazyTest {

        [TestMethod]
        public void EvalExecutesDelegateOnlyOnce() {
            // Arrange
            int numTimesCalled = 0;
            Lazy<string> lazy = new Lazy<string>(delegate {
                numTimesCalled++;
                return "Hello!";
            });

            // Act
            string retVal1 = lazy.Eval();
            string retVal2 = lazy.Eval();

            // Assert
            Assert.AreEqual("Hello!", retVal1);
            Assert.AreEqual("Hello!", retVal2);
            Assert.AreEqual(1, numTimesCalled, "Delegate should not have been called multiple times.");
        }

    }
}
