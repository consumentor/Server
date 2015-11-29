namespace Microsoft.Web.Mvc.Test {
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ValueProviderUtilTest {

        [TestMethod]
        public void IsPrefixMatch() {
            // Arrange
            var tests = new[] {
                new { Prefix = "Prefix", TestString = (string)null, ExpectedResult = false, Reason = "Null test string shouldn't match anything." },
                new { Prefix = "", TestString = "SomeTestString", ExpectedResult = true, Reason = "Empty prefix should match any non-null test string." },
                new { Prefix = "SomeString", TestString = "SomeString", ExpectedResult = true, Reason = "This was an exact match." },
                new { Prefix = "Foo", TestString = "NotFoo", ExpectedResult = false, Reason = "Prefix 'foo' doesn't match 'notfoo'." },
                new { Prefix = "Foo", TestString = "foo.bar", ExpectedResult = true, Reason = "Prefix 'foo' matched." },
                new { Prefix = "Foo", TestString = "foo[bar]", ExpectedResult = true, Reason = "Prefix 'foo' matched." },
                new { Prefix = "Foo", TestString = "FooBar", ExpectedResult = false, Reason = "Prefix 'foo' was not followed by a delimiter in the test string." }
            };

            // Act & assert
            foreach (var test in tests) {
                bool retVal = ValueProviderUtil.IsPrefixMatch(test.Prefix, test.TestString);
                Assert.AreEqual(test.ExpectedResult, retVal, test.Reason);
            }
        }

    }
}
