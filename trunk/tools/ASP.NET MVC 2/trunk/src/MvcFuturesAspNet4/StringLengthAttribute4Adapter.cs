namespace Microsoft.Web.Mvc.AspNet4 {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    // This version is different from the MVC 2 RTM version, because StringLengthAttribute
    // in DataAnnotations 4 added minimum string length support.

    public class StringLengthAttribute4Adapter : DataAnnotations4ModelValidator<StringLengthAttribute> {
        public StringLengthAttribute4Adapter(ModelMetadata metadata, ControllerContext context, StringLengthAttribute attribute)
            : base(metadata, context, attribute) {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            return new[] { new ModelClientValidationStringLengthRule(ErrorMessage, Attribute.MinimumLength, Attribute.MaximumLength) };
        }
    }
}
