namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    internal interface IDynamicSessionStateConfigurator {

        void ConfigureSessionState(ControllerSessionState mode);

    }
}
