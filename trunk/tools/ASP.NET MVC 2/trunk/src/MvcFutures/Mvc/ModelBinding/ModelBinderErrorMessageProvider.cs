namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Web.Mvc;

    public delegate string ModelBinderErrorMessageProvider(ControllerContext controllerContext, ModelMetadata modelMetadata, object incomingValue);

}
