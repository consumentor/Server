namespace Microsoft.Web.Mvc.AspNet4 {
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class DataAnnotations4ModelValidator<TAttribute> : DataAnnotations4ModelValidator where TAttribute : ValidationAttribute {
        public DataAnnotations4ModelValidator(ModelMetadata metadata, ControllerContext context, TAttribute attribute)
            : base(metadata, context, attribute) { }

        protected new TAttribute Attribute {
            get {
                return (TAttribute)base.Attribute;
            }
        }
    }
}
