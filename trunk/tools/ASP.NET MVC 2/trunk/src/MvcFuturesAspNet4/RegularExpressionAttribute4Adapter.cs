namespace Microsoft.Web.Mvc.AspNet4 {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    // TODO: Should be able to remove this for RTM, since this class was extracted and
    // made public, and this version is identical to the new public version.

    public class RegularExpressionAttribute4Adapter : DataAnnotations4ModelValidator<RegularExpressionAttribute> {
        public RegularExpressionAttribute4Adapter(ModelMetadata metadata, ControllerContext context, RegularExpressionAttribute attribute)
            : base(metadata, context, attribute) {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            return new[] { new ModelClientValidationRegexRule(ErrorMessage, Attribute.Pattern) };
        }
    }
}
