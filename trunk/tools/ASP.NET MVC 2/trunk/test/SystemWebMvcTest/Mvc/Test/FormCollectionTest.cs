namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class FormCollectionTest {

        [TestMethod]
        public void ConstructorCopiesProvidedCollection() {
            // Arrange
            NameValueCollection nvc = new NameValueCollection() {
                { "foo", "fooValue" },
                { "bar", "barValue" }
            };

            // Act
            FormCollection formCollection = new FormCollection(nvc);

            // Assert
            Assert.AreEqual(2, formCollection.Count);
            Assert.AreEqual("fooValue", formCollection["foo"]);
            Assert.AreEqual("barValue", formCollection["bar"]);
        }

        [TestMethod]
        public void ConstructorThrowsIfCollectionIsNull() {
            // Act & Assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    new FormCollection(null);
                }, "collection");
        }

        [TestMethod]
        public void CustomBinderBindModelReturnsFormCollection() {
            // Arrange
            NameValueCollection nvc = new NameValueCollection() { { "foo", "fooValue" }, { "bar", "barValue" } };
            IModelBinder binder = ModelBinders.Binders.GetBinder(typeof(FormCollection));

            Mock<ControllerContext> mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Expect(c => c.HttpContext.Request.Form).Returns(nvc);

            // Act
            FormCollection formCollection = (FormCollection)binder.BindModel(mockControllerContext.Object, null);

            // Assert
            Assert.IsNotNull(formCollection, "BindModel() should have returned a FormCollection.");
            Assert.AreEqual(2, formCollection.Count);
            Assert.AreEqual("fooValue", nvc["foo"]);
            Assert.AreEqual("barValue", nvc["bar"]);
        }

        [TestMethod]
        public void CustomBinderBindModelThrowsIfControllerContextIsNull() {
            // Arrange
            IModelBinder binder = ModelBinders.Binders.GetBinder(typeof(FormCollection));

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    binder.BindModel(null, null);
                }, "controllerContext");
        }

        [TestMethod]
        public void GetValue_ThrowsIfNameIsNull() {
            // Arrange
            FormCollection formCollection = new FormCollection();

            // Act & assert
            ExceptionHelper.ExpectArgumentNullException(
                delegate {
                    formCollection.GetValue(null);
                }, "name");
        }

        [TestMethod]
        public void GetValue_KeyDoesNotExist_ReturnsNull() {
            // Arrange
            FormCollection formCollection = new FormCollection();

            // Act
            ValueProviderResult vpResult = formCollection.GetValue("");

            // Assert
            Assert.IsNull(vpResult);
        }

        [TestMethod]
        public void GetValue_KeyExists_ReturnsResult() {
            // Arrange
            FormCollection formCollection = new FormCollection() {
                { "foo", "1" },
                { "foo", "2" }
            };

            // Act
            ValueProviderResult vpResult = formCollection.GetValue("foo");

            // Assert
            Assert.IsNotNull(vpResult);
            CollectionAssert.AreEqual(new string[] { "1", "2" }, (string[])vpResult.RawValue);
            Assert.AreEqual("1,2", vpResult.AttemptedValue);
            Assert.AreEqual(CultureInfo.CurrentCulture, vpResult.Culture);
        }

    }
}
