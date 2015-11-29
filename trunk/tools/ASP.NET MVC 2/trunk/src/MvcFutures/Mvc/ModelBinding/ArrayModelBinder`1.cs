namespace Microsoft.Web.Mvc.ModelBinding {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class ArrayModelBinder<TElement> : CollectionModelBinder<TElement> {

        protected override bool CreateOrReplaceCollection(ControllerContext controllerContext, ExtensibleModelBindingContext bindingContext, IList<TElement> newCollection) {
            bindingContext.Model = newCollection.ToArray();
            return true;
        }

    }
}
