namespace Microsoft.Web.Mvc.ModelBinding {
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BindingBehaviorAttribute : Attribute {

        private static readonly object _typeId = new object();

        public BindingBehaviorAttribute(BindingBehavior behavior) {
            Behavior = behavior;
        }

        public BindingBehavior Behavior {
            get;
            private set;
        }

        public override object TypeId {
            get {
                return _typeId;
            }
        }

    }
}
