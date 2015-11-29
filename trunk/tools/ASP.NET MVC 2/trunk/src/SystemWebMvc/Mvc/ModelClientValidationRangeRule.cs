namespace System.Web.Mvc {
    public class ModelClientValidationRangeRule : ModelClientValidationRule {
        public ModelClientValidationRangeRule(string errorMessage, object minValue, object maxValue) {
            ErrorMessage = errorMessage;
            ValidationType = "range";
            ValidationParameters["minimum"] = minValue;
            ValidationParameters["maximum"] = maxValue;
        }
    }
}
