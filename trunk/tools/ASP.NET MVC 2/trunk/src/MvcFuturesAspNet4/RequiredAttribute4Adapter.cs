namespace Microsoft.Web.Mvc.AspNet4 {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    // TODO: Should be able to remove this for RTM, since this class was extracted and
    // made public, and this version is identical to the new public version.

    public class RequiredAttribute4Adapter : DataAnnotations4ModelValidator<RequiredAttribute> {
        public RequiredAttribute4Adapter(ModelMetadata metadata, ControllerContext context, RequiredAttribute attribute)
            : base(metadata, context, attribute) {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            return new[] { new ModelClientValidationRequiredRule(ErrorMessage) };
        }
    }
}
