namespace Microsoft.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelCopierTest {

        [TestMethod]
        public void CopyCollection_FromIsNull_DoesNothing() {
            // Arrange
            int[] from = null;
            List<int> to = new List<int>() { 1, 2, 3 };

            // Act
            ModelCopier.CopyCollection(from, to);

            // Assert
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, to);
        }

        [TestMethod]
        public void CopyCollection_ToIsImmutable_DoesNothing() {
            // Arrange
            List<int> from = new List<int>() { 1, 2, 3 };
            ICollection<int> to = new ReadOnlyCollection<int>(new int[] { 4, 5, 6 });

            // Act
            ModelCopier.CopyCollection(from, to);

            // Assert
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, from);
            CollectionAssert.AreEqual(new int[] { 4, 5, 6 }, to.ToArray());
        }

        [TestMethod]
        public void CopyCollection_ToIsMmutable_ClearsAndCopies() {
            // Arrange
            List<int> from = new List<int>() { 1, 2, 3 };
            ICollection<int> to = new List<int>() { 4, 5, 6 };

            // Act
            ModelCopier.CopyCollection(from, to);

            // Assert
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, from);
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, to.ToArray());
        }

        [TestMethod]
        public void CopyCollection_ToIsNull_DoesNothing() {
            // Arrange
            List<int> from = new List<int>() { 1, 2, 3 };
            List<int> to = null;

            // Act
            ModelCopier.CopyCollection(from, to);

            // Assert
            CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, from);
        }

        [TestMethod]
        public void CopyModel_ExactTypeMatch_Copies() {
            // Arrange
            GenericModel<int> from = new GenericModel<int>() { TheProperty = 21 };
            GenericModel<int> to = new GenericModel<int>() { TheProperty = 42 };

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.AreEqual(21, from.TheProperty);
            Assert.AreEqual(21, to.TheProperty);
        }

        [TestMethod]
        public void CopyModel_FromIsNull_DoesNothing() {
            // Arrange
            GenericModel<int> from = null;
            GenericModel<int> to = new GenericModel<int>() { TheProperty = 42 };

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.AreEqual(42, to.TheProperty);
        }

        [TestMethod]
        public void CopyModel_LiftedTypeMatch_ActualValueIsNotNull_Copies() {
            // Arrange
            GenericModel<int?> from = new GenericModel<int?>() { TheProperty = 21 };
            GenericModel<int> to = new GenericModel<int>() { TheProperty = 42 };

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.AreEqual(21, from.TheProperty);
            Assert.AreEqual(21, to.TheProperty);
        }

        [TestMethod]
        public void CopyModel_LiftedTypeMatch_ActualValueIsNull_DoesNothing() {
            // Arrange
            GenericModel<int?> from = new GenericModel<int?>() { TheProperty = null };
            GenericModel<int> to = new GenericModel<int>() { TheProperty = 42 };

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.IsNull(from.TheProperty);
            Assert.AreEqual(42, to.TheProperty);
        }

        [TestMethod]
        public void CopyModel_NoTypeMatch_DoesNothing() {
            // Arrange
            GenericModel<int> from = new GenericModel<int>() { TheProperty = 21 };
            GenericModel<long> to = new GenericModel<long>() { TheProperty = 42 };

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.AreEqual(21, from.TheProperty);
            Assert.AreEqual(42, to.TheProperty);
        }

        [TestMethod]
        public void CopyModel_SubclassedTypeMatch_Copies() {
            // Arrange
            string originalModel = "Hello, world!";

            GenericModel<string> from = new GenericModel<string>() { TheProperty = originalModel };
            GenericModel<object> to = new GenericModel<object>() { TheProperty = 42 };

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.AreSame(originalModel, from.TheProperty);
            Assert.AreSame(originalModel, to.TheProperty);
        }

        [TestMethod]
        public void CopyModel_ToDoesNotContainProperty_DoesNothing() {
            // Arrange
            GenericModel<int> from = new GenericModel<int>() { TheProperty = 21 };
            OtherGenericModel<int> to = new OtherGenericModel<int>() { SomeOtherProperty = 42 };

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.AreEqual(21, from.TheProperty);
            Assert.AreEqual(42, to.SomeOtherProperty);
        }

        [TestMethod]
        public void CopyModel_ToIsNull_DoesNothing() {
            // Arrange
            GenericModel<int> from = new GenericModel<int>() { TheProperty = 21 };
            GenericModel<int> to = null;

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.AreEqual(21, from.TheProperty);
        }

        [TestMethod]
        public void CopyModel_ToIsReadOnly_DoesNothing() {
            // Arrange
            GenericModel<int> from = new GenericModel<int>() { TheProperty = 21 };
            ReadOnlyGenericModel<int> to = new ReadOnlyGenericModel<int>(42);

            // Act
            ModelCopier.CopyModel(from, to);

            // Assert
            Assert.AreEqual(21, from.TheProperty);
            Assert.AreEqual(42, to.TheProperty);
        }

        private class GenericModel<T> {
            public T TheProperty { get; set; }
        }

        private class OtherGenericModel<T> {
            public T SomeOtherProperty { get; set; }
        }

        private class ReadOnlyGenericModel<T> {
            public ReadOnlyGenericModel(T propertyValue) {
                TheProperty = propertyValue;
            }

            public T TheProperty { get; private set; }
        }

    }
}
