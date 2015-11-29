namespace System.Web.Mvc {
    using System;

    public sealed class FormValueProviderFactory : ValueProviderFactory {

        public override IValueProvider GetValueProvider(ControllerContext controllerContext) {
            if (controllerContext == null) {
                throw new ArgumentNullException("controllerContext");
            }

            return new FormValueProvider(controllerContext);
        }

    }
}
