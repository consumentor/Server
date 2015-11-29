﻿namespace Microsoft.Web.Mvc {
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ActionLinkAreaAttribute : Attribute {

        public ActionLinkAreaAttribute(string area) {
            if (area == null) {
                throw new ArgumentNullException("area");
            }

            Area = area;
        }

        public string Area {
            get;
            private set;
        }

    }
}
