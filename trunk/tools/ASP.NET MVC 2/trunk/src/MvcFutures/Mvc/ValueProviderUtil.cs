﻿namespace Microsoft.Web.Mvc {
    using System;

    internal static class ValueProviderUtil {

        public static bool IsPrefixMatch(string prefix, string testString) {
            if (testString == null) {
                return false;
            }

            if (prefix.Length == 0) {
                return true; // shortcut - non-null testString matches empty prefix
            }

            if (!testString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) {
                return false; // prefix doesn't match
            }

            if (testString.Length == prefix.Length) {
                return true; // exact match
            }

            // invariant: testString.Length > prefix.Length
            switch (testString[prefix.Length]) {
                case '.':
                case '[':
                    return true; // known delimiters

                default:
                    return false; // not known delimiter
            }
        }

    }
}
