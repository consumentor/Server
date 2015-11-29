namespace Sys.Mvc {
    using System;

    public sealed class NumberValidator {

        public NumberValidator() {
        }

        public static Validator Create(JsonValidationRule rule) {
            return new NumberValidator().Validate;
        }

        public object Validate(string value, ValidationContext context) {
            if (ValidationUtil.StringIsNullOrEmpty(value)) {
                return true; // let the RequiredValidator handle this case
            }

            Number n = Number.ParseLocale(value);
            return (!Number.IsNaN(n));
        }

    }
}
