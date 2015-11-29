namespace Sys.Mvc {
    using System;

    public sealed class RangeValidator {

        private readonly Number _minimum;
        private readonly Number _maximum;

        public RangeValidator(Number minimum, Number maximum) {
            _minimum = minimum;
            _maximum = maximum;
        }

        public static Validator Create(JsonValidationRule rule) {
            Number min = (Number)rule.ValidationParameters["minimum"];
            Number max = (Number)rule.ValidationParameters["maximum"];
            return new RangeValidator(min, max).Validate;
        }

        public object Validate(string value, ValidationContext context) {
            if (ValidationUtil.StringIsNullOrEmpty(value)) {
                return true; // let the RequiredValidator handle this case
            }

            Number n = Number.ParseLocale(value);
            return (!Number.IsNaN(n) && _minimum <= n && n <= _maximum);
        }

    }
}
