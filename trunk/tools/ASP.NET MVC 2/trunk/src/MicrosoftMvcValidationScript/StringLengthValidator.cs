namespace Sys.Mvc {
    using System;

    public sealed class StringLengthValidator {

        private readonly int _maxLength;
        private readonly int _minLength;

        public StringLengthValidator(int minLength, int maxLength) {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public static Validator Create(JsonValidationRule rule) {
            int minLength = (int)rule.ValidationParameters["minimumLength"];
            int maxLength = (int)rule.ValidationParameters["maximumLength"];
            return new StringLengthValidator(minLength, maxLength).Validate;
        }

        public object Validate(string value, ValidationContext context) {
            if (ValidationUtil.StringIsNullOrEmpty(value)) {
                return true; // let the RequiredValidator handle this case
            }

            return (_minLength <= value.Length && value.Length <= _maxLength);
        }

    }
}
