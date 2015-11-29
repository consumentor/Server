namespace Sys.Mvc {
    using System;

    [Record]
    public sealed class Validation {

        public string FieldErrorMessage;
        public Validator Validator;

    }
}
