﻿namespace Microsoft.Web.Mvc.AspNet4 {
    using System.Web.Mvc;

    public class DynamicViewPage<TModel> : ViewPage<TModel> {

        public new dynamic ViewData {
            get {
                return DynamicViewDataDictionary.Wrap(base.ViewData);
            }
        }

    }
}
