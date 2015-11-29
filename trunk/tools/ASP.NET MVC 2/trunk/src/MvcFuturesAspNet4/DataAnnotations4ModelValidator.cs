namespace Microsoft.Web.Mvc.AspNet4 {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class DataAnnotations4ModelValidator : ModelValidator {
        public DataAnnotations4ModelValidator(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute)
            : base(metadata, context) {

            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }

            Attribute = attribute;
        }

        protected internal ValidationAttribute Attribute { get; private set; }

        protected internal string ErrorMessage {
            get {
                return Attribute.FormatErrorMessage(Metadata.GetDisplayName());
            }
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            IClientValidatable clientValidatable = Attribute as IClientValidatable;
            if (clientValidatable != null) {
                return clientValidatable.GetClientValidationRules(Metadata, ControllerContext);
            }

            return base.GetClientValidationRules();
        }

        public override IEnumerable<ModelValidationResult> Validate(object container) {
            // Per the RIA Services team, instance can never be null (if you have no parent,
            // you pass yourself for the "instance" parameter).
            object instance = container ?? Metadata.Model;
            ValidationContext context = new ValidationContext(instance, null, null);
            context.DisplayName = Metadata.GetDisplayName();

            ValidationResult result = Attribute.GetValidationResult(Metadata.Model, context);
            if (result != ValidationResult.Success) {
                yield return new ModelValidationResult {
                    Message = result.ErrorMessage
                };
            }
        }
    }
}
