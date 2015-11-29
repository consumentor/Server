namespace Sys.Mvc {
    using System;
    using System.DHTML;

    public sealed class RequiredValidator {

        public static Validator Create(JsonValidationRule rule) {
            return new RequiredValidator().Validate;
        }

        public object Validate(string value, ValidationContext context) {
            // ignore the value - we actually need to look at the DOM Elements

            DOMElement[] elements = context.FieldContext.Elements;
            if (elements.Length == 0) {
                // nothing to validate, so return success
                return true;
            }

            DOMElement sampleElement = elements[0];
            if (IsTextualInputElement(sampleElement)) {
                return ValidateTextualInput((InputElement)sampleElement);
            }

            if (IsRadioInputElement(sampleElement)) {
                return ValidateRadioInput(elements);
            }

            if (IsSelectInputElement(sampleElement)) {
                return ValidateSelectInput(((SelectElement)sampleElement).Options);
            }

            // can't validate, so just report success
            return true;
        }

        private static bool IsRadioInputElement(DOMElement element) {
            if (element.TagName.ToUpperCase() == "INPUT") {
                string inputType = ((string)Type.GetField(element, "type")).ToUpperCase();
                if (inputType == "RADIO") {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSelectInputElement(DOMElement element) {
            if (element.TagName.ToUpperCase() == "SELECT") {
                return true;
            }

            return false;
        }

        private static bool IsTextualInputElement(DOMElement element) {
            if (element.TagName.ToUpperCase() == "INPUT") {
                string inputType = ((string)Type.GetField(element, "type")).ToUpperCase();
                switch (inputType) {
                    case "TEXT":
                    case "PASSWORD":
                    case "FILE":
                        return true;
                }
            }

            if (element.TagName.ToUpperCase() == "TEXTAREA") {
                return true;
            }

            return false;
        }

        private static object ValidateRadioInput(DOMElement[] elements) {
            for (int i = 0; i < elements.Length; i++) {
                DOMElement element = elements[i];
                if ((bool)Type.GetField(element, "checked")) {
                    return true; // at least one is selected, so OK
                }
            }

            return false; // failure
        }

        private static object ValidateSelectInput(DOMElementCollection optionElements) {
            for (int i = 0; i < optionElements.Length; i++) {
                DOMElement element = optionElements[i];
                if ((bool)Type.GetField(element, "selected")) {
                    if (!ValidationUtil.StringIsNullOrEmpty((string)Type.GetField(element, "value"))) {
                        return true; // at least one is selected, so OK
                    }
                }
            }

            return false; // failure
        }

        private static object ValidateTextualInput(InputElement element) {
            return (!ValidationUtil.StringIsNullOrEmpty(element.Value));
        }

    }
}
