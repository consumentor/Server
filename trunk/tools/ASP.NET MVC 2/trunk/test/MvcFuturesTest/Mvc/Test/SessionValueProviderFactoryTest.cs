namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Web.Mvc;
    using Moq;

    [TestClass]
    public class SessionValueProviderFactoryTest {

        [TestMethod]
        public void GetValueProvider() {
            // Arrange
            Dictionary<string, object> backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) {
                { "foo", "fooValue" },
                { "bar.baz", "barBazValue" }
            };
            MockSessionState session = new MockSessionState(backingStore);

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Session).Returns(session);

            SessionValueProviderFactory factory = new SessionValueProviderFactory();

            // Act
            IValueProvider provider = factory.GetValueProvider(mockControllerContext.Object);

            // Assert
            Assert.IsTrue(provider.ContainsPrefix("bar"));
            Assert.AreEqual("fooValue", provider.GetValue("foo").AttemptedValue);
            Assert.AreEqual(CultureInfo.InvariantCulture, provider.GetValue("foo").Culture);
        }

        private sealed class MockSessionState : HttpSessionStateBase {
            private readonly IDictionary<string, object> _backingStore;

            public MockSessionState(IDictionary<string, object> backingStore) {
                _backingStore = backingStore;
            }

            public override object this[string name] {
                get {
                    return _backingStore[name];
                }
                set {
                    _backingStore[name] = value;
                }
            }

            public override IEnumerator GetEnumerator() {
                return _backingStore.Keys.GetEnumerator();
            }
        }

    }
}
