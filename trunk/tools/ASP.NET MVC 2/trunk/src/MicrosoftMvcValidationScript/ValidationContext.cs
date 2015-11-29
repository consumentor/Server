namespace Sys.Mvc {
    using System;
    using System.DHTML;

    [Record]
    public sealed class ValidationContext {

        public string EventName;
        public FieldContext FieldContext;
        public Validation Validation;

    }
}
