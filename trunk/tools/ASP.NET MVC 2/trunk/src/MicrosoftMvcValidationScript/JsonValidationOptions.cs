namespace Sys.Mvc {
    using System;

    [Record]
    public sealed class JsonValidationOptions {
        [PreserveCase]
        public JsonValidationField[] Fields;

        [PreserveCase]
        public string FormId;

        [PreserveCase]
        public bool ReplaceValidationSummary;

        [PreserveCase]
        public string ValidationSummaryId;
    }
}
