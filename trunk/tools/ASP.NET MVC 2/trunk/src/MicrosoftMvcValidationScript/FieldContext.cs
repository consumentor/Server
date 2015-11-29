namespace Sys.Mvc {
    using System;
    using System.DHTML;
    using Sys.UI;

    public class FieldContext {

        private const string _hasTextChangedTag = "__MVC_HasTextChanged";
        private const string _hasValidationFiredTag = "__MVC_HasValidationFired";

        private const string _inputElementErrorCss = "input-validation-error";
        private const string _inputElementValidCss = "input-validation-valid";
        private const string _validationMessageErrorCss = "field-validation-error";
        private const string _validationMessageValidCss = "field-validation-valid";

        private readonly DomEventHandler _onBlurHandler;
        private readonly DomEventHandler _onChangeHandler;
        private readonly DomEventHandler _onInputHandler;
        private readonly DomEventHandler _onPropertyChangeHandler;

        private ArrayList _errors = new ArrayList();

        public string DefaultErrorMessage;
        public DOMElement[] Elements = new DOMElement[0];
        public readonly FormContext FormContext;
        public bool ReplaceValidationMessageContents;
        public DOMElement ValidationMessageElement;
        public Validation[] Validations = new Validation[0];

        public FieldContext(FormContext formContext) {
            FormContext = formContext;

            _onBlurHandler = Element_OnBlur;
            _onChangeHandler = Element_OnChange;
            _onInputHandler = Element_OnInput;
            _onPropertyChangeHandler = Element_OnPropertyChange;
        }

        public void AddError(string message) {
            AddErrors(new string[] { message });
        }

        public void AddErrors(string[] messages) {
            if (!ValidationUtil.ArrayIsNullOrEmpty(messages)) {
                ArrayList.AddRange(_errors, messages);
                OnErrorCountChanged();
            }
        }

        public void ClearErrors() {
            ArrayList.Clear(_errors);
            OnErrorCountChanged();
        }

        private void DisplayError() {
            DOMElement validationMessageElement = ValidationMessageElement;
            if (validationMessageElement != null) {
                if (ReplaceValidationMessageContents) {
                    ValidationUtil.SetInnerText(validationMessageElement, (string)_errors[0]);
                }
                DomElement.RemoveCssClass(validationMessageElement, _validationMessageValidCss);
                DomElement.AddCssClass(validationMessageElement, _validationMessageErrorCss);
            }

            // update actual input elements
            DOMElement[] elements = Elements;
            for (int i = 0; i < elements.Length; i++) {
                DOMElement element = elements[i];
                DomElement.RemoveCssClass(element, _inputElementValidCss);
                DomElement.AddCssClass(element, _inputElementErrorCss);
            }
        }

        private void DisplaySuccess() {
            DOMElement validationMessageElement = ValidationMessageElement;
            if (validationMessageElement != null) {
                if (ReplaceValidationMessageContents) {
                    ValidationUtil.SetInnerText(validationMessageElement, "");
                }
                DomElement.RemoveCssClass(validationMessageElement, _validationMessageErrorCss);
                DomElement.AddCssClass(validationMessageElement, _validationMessageValidCss);
            }

            // update actual input elements
            DOMElement[] elements = Elements;
            for (int i = 0; i < elements.Length; i++) {
                DOMElement element = elements[i];
                DomElement.RemoveCssClass(element, _inputElementErrorCss);
                DomElement.AddCssClass(element, _inputElementValidCss);
            }
        }

        private void Element_OnBlur(DomEvent e) {
            if ((bool)Type.GetField(e.Target, _hasTextChangedTag) || (bool)Type.GetField(e.Target, _hasValidationFiredTag)) {
                Validate("blur");
            }
        }

        private void Element_OnChange(DomEvent e) {
            Type.SetField(e.Target, _hasTextChangedTag, true);
        }

        // Firefox, Opera, Safari, Chrome - contents of an input element changed
        private void Element_OnInput(DomEvent e) {
            Type.SetField(e.Target, _hasTextChangedTag, true);
            if ((bool)Type.GetField(e.Target, _hasValidationFiredTag)) {
                Validate("input");
            }
        }

        // IE - contents of an input element changed
        private void Element_OnPropertyChange(DomEvent e) {
            if ((string)Type.GetField(e.RawEvent, "propertyName") == "value") {
                Type.SetField(e.Target, _hasTextChangedTag, true);
                if ((bool)Type.GetField(e.Target, _hasValidationFiredTag)) {
                    Validate("input");
                }
            }
        }

        public void EnableDynamicValidation() {
            DOMElement[] elements = Elements;
            for (int i = 0; i < elements.Length; i++) {
                DOMElement element = elements[i];

                if (ValidationUtil.ElementSupportsEvent(element, "onpropertychange")) {
                    // IE

                    // DDB #227842: IE (before version 8) sometimes incorrectly fires the OnPropertyChange event
                    // for 'value' asynchronously when the element's CSS class is changed, which throws our
                    // validation library into an infinite loop and hangs the user's browser. We disable real-
                    // time validation in downlevel versions of IE to prevent this situation.
                    // 'documentMode' documented at http://msdn.microsoft.com/en-us/library/cc196988(VS.85).aspx
                    object compatMode = Script.Literal("document.documentMode");
                    if (compatMode != null && (int)compatMode >= 8) {
                        DomEvent.AddHandler(element, "propertychange", _onPropertyChangeHandler);
                    }
                }
                else {
                    // Firefox, Safari, Opera, Chrome
                    DomEvent.AddHandler(element, "input", _onInputHandler);
                }

                // Everybody else
                DomEvent.AddHandler(element, "change", _onChangeHandler);
                DomEvent.AddHandler(element, "blur", _onBlurHandler);
            }
        }

        private string GetErrorString(object validatorReturnValue, string fieldErrorMessage) {
            string fallbackErrorMessage = fieldErrorMessage ?? DefaultErrorMessage;

            // overload return value as Boolean
            if (validatorReturnValue is bool) {
                return ((bool)validatorReturnValue) ? null /* success */ : fallbackErrorMessage /* failure */;
            }

            // overload return value as String
            if (validatorReturnValue is string) {
                return (((string)validatorReturnValue).Length != 0) ? (string)validatorReturnValue : fallbackErrorMessage;
            }

            // no error
            return null;
        }

        private string GetStringValue() {
            DOMElement[] elements = Elements;
            return (elements.Length > 0) ? (string)Type.GetField(elements[0], "value") : null;
        }

        private void MarkValidationFired() {
            DOMElement[] elements = Elements;
            for (int i = 0; i < elements.Length; i++) {
                DOMElement element = elements[i];
                Type.SetField(element, _hasValidationFiredTag, true); // mark all as validation fired
            }
        }

        private void OnErrorCountChanged() {
            if (_errors.Length == 0) {
                DisplaySuccess();
            }
            else {
                DisplayError();
            }
        }

        public string[] Validate(string eventName) {
            Validation[] validations = Validations;
            ArrayList errors = new ArrayList();
            string value = GetStringValue();

            // build up the list of all errors
            for (int i = 0; i < validations.Length; i++) {
                Validation validation = validations[i];
                ValidationContext context = new ValidationContext();
                context.EventName = eventName;
                context.FieldContext = this;
                context.Validation = validation;

                object retVal = validation.Validator(value, context);

                string errorMessage = GetErrorString(retVal, validation.FieldErrorMessage);
                if (!ValidationUtil.StringIsNullOrEmpty(errorMessage)) {
                    ArrayList.Add(errors, errorMessage);
                }
            }

            MarkValidationFired();
            ClearErrors();
            AddErrors((string[])errors);
            return (string[])errors;
        }

    }
}
