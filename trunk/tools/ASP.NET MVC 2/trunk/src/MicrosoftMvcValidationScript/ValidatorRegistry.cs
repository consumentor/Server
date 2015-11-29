namespace Sys.Mvc {
    using System;

    public static class ValidatorRegistry {

        public static Dictionary Validators = GetDefaultValidators();

        public static Validator GetValidator(JsonValidationRule rule) {
            ValidatorCreator creator = (ValidatorCreator)Validators[rule.ValidationType];
            return (creator != null) ? creator(rule) : null;
        }

        private static Dictionary GetDefaultValidators() {
            return new Dictionary(
                "required", (ValidatorCreator)RequiredValidator.Create,
                "stringLength", (ValidatorCreator)StringLengthValidator.Create,
                "regularExpression", (ValidatorCreator)RegularExpressionValidator.Create,
                "range", (ValidatorCreator)RangeValidator.Create,
                "number", (ValidatorCreator)NumberValidator.Create
                );
        }

    }
}
