namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class HttpFileCollectionValueProviderTest {

        private static readonly KeyValuePair<string, HttpPostedFileBase>[] _allFiles = new KeyValuePair<string, HttpPostedFileBase>[] {
            new KeyValuePair<string, HttpPostedFileBase>("foo", new MockHttpPostedFile(42, "fooFile1")),
            new KeyValuePair<string, HttpPostedFileBase>("foo", null),
            new KeyValuePair<string, HttpPostedFileBase>("foo", new MockHttpPostedFile(0, "") /* empty */),
            new KeyValuePair<string, HttpPostedFileBase>("foo", new MockHttpPostedFile(100, "fooFile2")),
            new KeyValuePair<string, HttpPostedFileBase>("bar.baz", new MockHttpPostedFile(200, "barBazFile"))
        };

        [TestMethod]
        public void ContainsPrefix() {
            // Arrange
            HttpFileCollectionValueProvider valueProvider = GetValueProvider();

            // Act
            bool result = valueProvider.ContainsPrefix("bar");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ContainsPrefix_DoesNotContainEmptyPrefixIfBackingStoreIsEmpty() {
            // Arrange
            HttpFileCollectionValueProvider valueProvider = GetEmptyValueProvider();

            // Act
            bool result = valueProvider.ContainsPrefix("");

            // Assert
            Assert.IsFalse(result, "The '' prefix shouldn't have been present.");
        }

        [TestMethod]
        public void ContainsPrefix_ThrowsIfPrefixIsNull() {
            // Arrange
            HttpFileCollectionValueProvider valueProvider = GetValueProvider();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    valueProvider.ContainsPrefix(null);
                }, "prefix");
        }

        [TestMethod]
        public void GetValue() {
            // Arrange
            HttpFileCollectionValueProvider valueProvider = GetValueProvider();

            // Act
            ValueProviderResult vpResult = valueProvider.GetValue("foo");

            // Assert
            Assert.IsNotNull(vpResult);

            HttpPostedFileBase[] expectedRawValues = (from el in _allFiles
                                                      where el.Key == "foo"
                                                      let file = el.Value
                                                      let hasContent = (file != null && file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                                                      select (hasContent) ? file : null).ToArray();
            CollectionAssert.AreEqual(expectedRawValues, (HttpPostedFileBase[])vpResult.RawValue);
            Assert.AreEqual("System.Web.HttpPostedFileBase[]", vpResult.AttemptedValue);
            Assert.AreEqual(CultureInfo.InvariantCulture, vpResult.Culture);
        }

        [TestMethod]
        public void GetValue_ReturnsNullIfKeyNotFound() {
            // Arrange
            HttpFileCollectionValueProvider valueProvider = GetValueProvider();

            // Act
            ValueProviderResult vpResult = valueProvider.GetValue("bar");

            // Assert
            Assert.IsNull(vpResult);
        }

        [TestMethod]
        public void GetValue_ThrowsIfKeyIsNull() {
            // Arrange
            HttpFileCollectionValueProvider valueProvider = GetValueProvider();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    valueProvider.GetValue(null);
                }, "key");
        }

        private static HttpFileCollectionValueProvider GetEmptyValueProvider() {
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.Files.Count).Returns(0);
            return new HttpFileCollectionValueProvider(mockControllerContext.Object);
        }

        private static HttpFileCollectionValueProvider GetValueProvider() {
            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(o => o.HttpContext.Request.Files.Count).Returns(_allFiles.Length);
            mockControllerContext.Expect(o => o.HttpContext.Request.Files.AllKeys).Returns(_allFiles.Select(f => f.Key).ToArray());
            for (int i = 0; i < _allFiles.Length; i++) {
                int j = i;
                mockControllerContext.Expect(o => o.HttpContext.Request.Files[j]).Returns(_allFiles[j].Value);
            }

            return new HttpFileCollectionValueProvider(mockControllerContext.Object);
        }

        private sealed class MockHttpPostedFile : HttpPostedFileBase {
            private readonly int _contentLength;
            private readonly string _fileName;

            public MockHttpPostedFile(int contentLength, string fileName) {
                _contentLength = contentLength;
                _fileName = fileName;
            }

            public override int ContentLength {
                get {
                    return _contentLength;
                }
            }

            public override string FileName {
                get {
                    return _fileName;
                }
            }
        }

    }
}
