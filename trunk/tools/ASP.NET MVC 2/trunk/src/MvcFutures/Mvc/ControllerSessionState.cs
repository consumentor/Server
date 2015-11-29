namespace Microsoft.Web.Mvc {
    using System;

    // Essentially a clone of the .NET 4 SessionStateBehavior enum.
    public enum ControllerSessionState {
        Default = 0,
        Required = 1,
        ReadOnly = 2,
        Disabled = 3
    }
}
