namespace System.Web.Mvc {
    public class ModelClientValidationStringLengthRule : ModelClientValidationRule {
        public ModelClientValidationStringLengthRule(string errorMessage, int minimumLength, int maximumLength) {
            ErrorMessage = errorMessage;
            ValidationType = "stringLength";
            ValidationParameters["minimumLength"] = minimumLength;
            ValidationParameters["maximumLength"] = maximumLength;
        }
    }
}
