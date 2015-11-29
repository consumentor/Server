namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ValueProviderUtilTest {

        [TestMethod]
        public void CollectionContainsPrefix_EmptyCollectionReturnsFalse() {
            // Arrange
            string[] collection = new string[0];

            // Act
            bool retVal = ValueProviderUtil.CollectionContainsPrefix(collection, "");

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void CollectionContainsPrefix_ExactMatch() {
            // Arrange
            string[] collection = new string[] { "Hello" };

            // Act
            bool retVal = ValueProviderUtil.CollectionContainsPrefix(collection, "Hello");

            // Assert
            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void CollectionContainsPrefix_MatchIsCaseInsensitive() {
            // Arrange
            string[] collection = new string[] { "Hello" };

            // Act
            bool retVal = ValueProviderUtil.CollectionContainsPrefix(collection, "hello");

            // Assert
            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void CollectionContainsPrefix_MatchIsNotSimpleSubstringMatch() {
            // Arrange
            string[] collection = new string[] { "Hello" };

            // Act
            bool retVal = ValueProviderUtil.CollectionContainsPrefix(collection, "He");

            // Assert
            Assert.IsFalse(retVal);
        }

        [TestMethod]
        public void CollectionContainsPrefix_NonEmptyCollectionReturnsTrueIfPrefixIsEmptyString() {
            // Arrange
            string[] collection = new string[] { "Hello" };

            // Act
            bool retVal = ValueProviderUtil.CollectionContainsPrefix(collection, "");

            // Assert
            Assert.IsTrue(retVal);
        }

        [TestMethod]
        public void CollectionContainsPrefix_PrefixBoundaries() {
            // Arrange
            string[] collection = new string[] { "Hello.There[0]" };

            // Act
            bool retVal1 = ValueProviderUtil.CollectionContainsPrefix(collection, "hello");
            bool retVal2 = ValueProviderUtil.CollectionContainsPrefix(collection, "hello.there");

            // Assert
            Assert.IsTrue(retVal1);
            Assert.IsTrue(retVal2);
        }

        [TestMethod]
        public void GetPrefixes() {
            // Arrange
            string key = "foo.bar[baz].quux";
            string[] expected = new string[] {
                "foo.bar[baz].quux",
                "foo.bar[baz]",
                "foo.bar",
                "foo"
            };

            // Act
            List<string> result = ValueProviderUtil.GetPrefixes(key).ToList();

            // Assert
            CollectionAssert.AreEquivalent(expected, result);
        }

    }
}
