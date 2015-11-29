﻿namespace Microsoft.Web.UnitTestUtil {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AnonymousObject {
        public static string Inspect(object obj) {
            if (obj == null) {
                return "(null)";
            }

            object[] args = Enumerable.Empty<Object>().ToArray();
            IEnumerable<string> values = obj.GetType()
                                            .GetProperties()
                                            .Select(prop => String.Format("{0}: {1}", prop.Name, prop.GetValue(obj, args)));

            if (!values.Any()) {
                return "(no properties)";
            }

            return "{ " + values.Aggregate((left, right) => left + ", " + right) + " }";
        }
    }
}
