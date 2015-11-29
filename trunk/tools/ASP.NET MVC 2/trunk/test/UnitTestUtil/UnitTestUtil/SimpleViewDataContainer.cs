namespace Microsoft.Web.UnitTestUtil {
    using System.Web.Mvc;

    public class SimpleViewDataContainer : IViewDataContainer {
        public SimpleViewDataContainer(ViewDataDictionary viewData) {
            ViewData = viewData;
        }

        public ViewDataDictionary ViewData {
            get;
            set;
        }
    }
}
