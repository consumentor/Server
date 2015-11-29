﻿namespace System.Web.Mvc {
    using System;
    using System.Reflection;

    public abstract class ControllerDescriptor : ICustomAttributeProvider {

        public virtual string ControllerName {
            get {
                string typeName = ControllerType.Name;
                if (typeName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)) {
                    return typeName.Substring(0, typeName.Length - "Controller".Length);
                }

                return typeName;
            }
        }

        public abstract Type ControllerType {
            get;
        }

        public abstract ActionDescriptor FindAction(ControllerContext controllerContext, string actionName);

        public abstract ActionDescriptor[] GetCanonicalActions();

        public virtual object[] GetCustomAttributes(bool inherit) {
            return GetCustomAttributes(typeof(object), inherit);
        }

        public virtual object[] GetCustomAttributes(Type attributeType, bool inherit) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }

            return (object[])Array.CreateInstance(attributeType, 0);
        }

        public virtual bool IsDefined(Type attributeType, bool inherit) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }

            return false;
        }

    }
}
