namespace Sys.Mvc {
    using System;
    using System.DHTML;
    using Sys.UI;

    public class FormContext {

        private const string _validationSummaryErrorCss = "validation-summary-errors";
        private const string _validationSummaryValidCss = "validation-summary-valid";

        private const string _formValidationTag = "__MVC_FormValidation";

        private readonly DomEventHandler _onClickHandler;
        private readonly DomEventHandler _onSubmitHandler;

        private ArrayList _errors = new ArrayList();
        private InputElement _submitButtonClicked;
        private readonly DOMElement _validationSummaryElement;
        private readonly DOMElement _validationSummaryULElement;

        public FieldContext[] Fields = new FieldContext[0];
        private FormElement FormElement;

        public bool ReplaceValidationSummary;

        public FormContext(FormElement formElement, DOMElement validationSummaryElement) {
            FormElement = formElement;
            _validationSummaryElement = validationSummaryElement;

            Type.SetField(formElement, _formValidationTag, this);

            // need to retrieve the actual <ul> element, since that will be dynamically modified
            if (validationSummaryElement != null) {
                DOMElementCollection ulElements = validationSummaryElement.GetElementsByTagName("ul");
                if (ulElements.Length > 0) {
                    _validationSummaryULElement = ulElements[0];
                }
            }

            _onClickHandler = Form_OnClick;
            _onSubmitHandler = Form_OnSubmit;
        }

        // not meant for public use, but must be marked public so that it's callable by the .jst
        public static void _Application_Load() {
            ArrayList allFormOptions = (ArrayList)Type.GetField(typeof(Window), "mvcClientValidationMetadata");
            if (allFormOptions != null) {
                while (allFormOptions.Length > 0) {
                    JsonValidationOptions thisFormOptions = (JsonValidationOptions)Type.InvokeMethod(allFormOptions, "pop");
                    ParseJsonOptions(thisFormOptions);
                }
            }
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
            if (_validationSummaryElement != null) {
                if (_validationSummaryULElement != null) {
                    // replace all children with the current error list
                    ValidationUtil.RemoveAllChildren(_validationSummaryULElement);
                    for (int i = 0; i < _errors.Length; i++) {
                        DOMElement liElement = Document.CreateElement("li");
                        ValidationUtil.SetInnerText(liElement, (string)_errors[i]);
                        _validationSummaryULElement.AppendChild(liElement);
                    }
                }

                DomElement.RemoveCssClass(_validationSummaryElement, _validationSummaryValidCss);
                DomElement.AddCssClass(_validationSummaryElement, _validationSummaryErrorCss);
            }
        }

        private void DisplaySuccess() {
            DOMElement validationSummaryElement = _validationSummaryElement;

            if (validationSummaryElement != null) {
                DOMElement validationSummaryULElement = _validationSummaryULElement;
                if (validationSummaryULElement != null) {
                    // delete all children
                    validationSummaryULElement.InnerHTML = "";
                }

                DomElement.RemoveCssClass(validationSummaryElement, _validationSummaryErrorCss);
                DomElement.AddCssClass(validationSummaryElement, _validationSummaryValidCss);
            }
        }

        public void EnableDynamicValidation() {
            DomEvent.AddHandler(FormElement, "click", _onClickHandler);
            DomEvent.AddHandler(FormElement, "submit", _onSubmitHandler);
        }

        private InputElement FindSubmitButton(DOMElement element) {
            // element: the form element that is active

            if (element.Disabled) {
                return null;
            }

            string tagName = element.TagName.ToUpperCase();
            InputElement inputElement = (InputElement)element;
            if (tagName == "INPUT") {
                string type = inputElement.Type;
                if (type == "submit" || type == "image") {
                    return inputElement;
                }
            }
            else if ((tagName == "BUTTON") && (inputElement.Type == "submit")) {
                return inputElement;
            }

            return null;
        }

        // hooking the click event allows us to find the submit button that started this whole process
        private void Form_OnClick(DomEvent e) {
            _submitButtonClicked = FindSubmitButton(e.Target);
        }

        private void Form_OnSubmit(DomEvent e) {
            FormElement form = (FormElement)e.Target;

            InputElement submitButton = _submitButtonClicked;
            if (submitButton != null && (bool)Type.GetField(submitButton, "disableValidation")) {
                return; // don't perform validation
            }

            string[] errorMessages = Validate("submit");
            if (!ValidationUtil.ArrayIsNullOrEmpty(errorMessages)) {
                e.PreventDefault(); // there was an error
            }
        }

        private static DOMElement[] GetFormElementsWithName(FormElement formElement, string name) {
            ArrayList allElementsWithNameInForm = new ArrayList();

            DOMElementCollection allElementsWithName = Document.GetElementsByName(name);
            for (int i = 0; i < allElementsWithName.Length; i++) {
                DOMElement thisElement = allElementsWithName[i];
                if (IsElementInHierarchy(formElement, thisElement)) {
                    ArrayList.Add(allElementsWithNameInForm, thisElement);
                }
            }

            return (DOMElement[])allElementsWithNameInForm;
        }

        public static FormContext GetValidationForForm(FormElement formElement) {
            return (FormContext)Type.GetField(formElement, _formValidationTag);
        }

        private static bool IsElementInHierarchy(DOMElement parent, DOMElement child) {
            while (child != null) {
                if (parent == child) {
                    return true;
                }
                child = child.ParentNode;
            }
            return false;
        }

        private void OnErrorCountChanged() {
            if (_errors.Length == 0) {
                DisplaySuccess();
            }
            else {
                DisplayError();
            }
        }

        internal static FormContext ParseJsonOptions(JsonValidationOptions options) {
            // First hook up the form logic
            FormElement formElement = (FormElement)Document.GetElementById(options.FormId);
            DOMElement validationSummaryElement = (!ValidationUtil.StringIsNullOrEmpty(options.ValidationSummaryId))
                ? Document.GetElementById(options.ValidationSummaryId)
                : null;

            FormContext formContext = new FormContext(formElement, validationSummaryElement);
            formContext.EnableDynamicValidation();
            formContext.ReplaceValidationSummary = options.ReplaceValidationSummary;

            // Then hook up the field logic
            for (int i = 0; i < options.Fields.Length; i++) {
                JsonValidationField field = options.Fields[i];
                DOMElement[] fieldElements = GetFormElementsWithName(formElement, field.FieldName);
                DOMElement validationMessageElement = (!ValidationUtil.StringIsNullOrEmpty(field.ValidationMessageId))
                    ? Document.GetElementById(field.ValidationMessageId)
                    : null;

                FieldContext fieldContext = new FieldContext(formContext);
                ArrayList.AddRange((ArrayList)(object)fieldContext.Elements, fieldElements);
                fieldContext.ValidationMessageElement = validationMessageElement;
                fieldContext.ReplaceValidationMessageContents = field.ReplaceValidationMessageContents;

                // Hook up rules
                for (int j = 0; j < field.ValidationRules.Length; j++) {
                    JsonValidationRule rule = field.ValidationRules[j];
                    Validator validator = ValidatorRegistry.GetValidator(rule);
                    if (validator != null) {
                        Validation validation = new Validation();
                        validation.FieldErrorMessage = rule.ErrorMessage;
                        validation.Validator = validator;
                        ArrayList.Add((ArrayList)(object)fieldContext.Validations, validation);
                    }
                }

                fieldContext.EnableDynamicValidation();
                ArrayList.Add((ArrayList)(object)formContext.Fields, fieldContext);
            }

            // hook up callback so that it can be executed by the AJAX code
            ArrayList registeredValidatorCallbacks = (ArrayList)Type.GetField(formElement, "validationCallbacks");
            if (registeredValidatorCallbacks == null) {
                registeredValidatorCallbacks = new ArrayList();
                Type.SetField(formElement, "validationCallbacks", registeredValidatorCallbacks);
            }
            Type.InvokeMethod(registeredValidatorCallbacks, "push", (ValidationCallback)delegate() {
                return ValidationUtil.ArrayIsNullOrEmpty(formContext.Validate("submit"));
            });

            return formContext;
        }

        // returns an array of validation error messages
        public string[] Validate(string eventName) {
            FieldContext[] fields = Fields;
            ArrayList errors = new ArrayList();

            for (int i = 0; i < fields.Length; i++) {
                FieldContext field = fields[i];
                string[] thisErrors = field.Validate(eventName);
                if (thisErrors != null) {
                    ArrayList.AddRange(errors, thisErrors);
                }
            }

            if (ReplaceValidationSummary) {
                ClearErrors();
                AddErrors((string[])errors);
            }

            return (string[])errors;
        }

    }

    internal delegate bool ValidationCallback();
}
