namespace Microsoft.Web.Mvc.ModelBinding {
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class BindRequiredAttribute : BindingBehaviorAttribute {

        public BindRequiredAttribute()
            : base(BindingBehavior.Required) {
        }

    }
}
