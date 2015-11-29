namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;

    public sealed class ModelValidatingEventArgs : CancelEventArgs {

        public ModelValidatingEventArgs(ControllerContext controllerContext, ModelValidationNode parentNode) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }

            ControllerContext = controllerContext;
            ParentNode = parentNode;
        }

        public ControllerContext ControllerContext {
            get;
            private set;
        }

        public ModelValidationNode ParentNode {
            get;
            private set;
        }

    }
}
