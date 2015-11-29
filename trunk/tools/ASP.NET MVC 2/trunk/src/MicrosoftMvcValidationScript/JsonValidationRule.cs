namespace Sys.Mvc {
    using System;

    [Record]
    public sealed class JsonValidationRule {
        [PreserveCase]
        public string ErrorMessage;

        [PreserveCase]
        public string ValidationType;

        [PreserveCase]
        public Dictionary ValidationParameters;
    }
}
