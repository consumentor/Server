namespace Sys.Mvc {
    using System;

    public sealed class RegularExpressionValidator {

        private readonly string _pattern;

        public RegularExpressionValidator(string pattern) {
            _pattern = pattern;
        }

        public static Validator Create(JsonValidationRule rule) {
            string pattern = (string)rule.ValidationParameters["pattern"];
            return new RegularExpressionValidator(pattern).Validate;
        }

        public object Validate(string value, ValidationContext context) {
            if (ValidationUtil.StringIsNullOrEmpty(value)) {
                return true; // let the RequiredValidator handle this case
            }

            RegularExpression regExp = new RegularExpression(_pattern);
            string[] matches = regExp.Exec(value);
            return (!ValidationUtil.ArrayIsNullOrEmpty(matches) && matches[0].Length == value.Length);
        }

    }
}
