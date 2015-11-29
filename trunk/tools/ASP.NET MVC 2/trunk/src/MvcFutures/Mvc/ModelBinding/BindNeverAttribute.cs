namespace Microsoft.Web.Mvc.ModelBinding {
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class BindNeverAttribute : BindingBehaviorAttribute {

        public BindNeverAttribute()
            : base(BindingBehavior.Never) {
        }

    }
}
