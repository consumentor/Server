namespace Sys.Mvc {
    using System;

    [Record]
    public sealed class JsonValidationField {
        [PreserveCase]
        public string FieldName;

        [PreserveCase]
        public bool ReplaceValidationMessageContents;

        [PreserveCase]
        public string ValidationMessageId;

        [PreserveCase]
        public JsonValidationRule[] ValidationRules;
    }
}
