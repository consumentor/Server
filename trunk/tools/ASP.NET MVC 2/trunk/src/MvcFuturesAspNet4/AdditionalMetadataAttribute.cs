namespace Microsoft.Web.Mvc.AspNet4 {
    using System;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class AdditionalMetadataAttribute : Attribute, IMetadataAware {

        public AdditionalMetadataAttribute(string name, object value) {
            Name = name;
            Value = value;
        }

        public string Name {
            get;
            private set;
        }

        public override object TypeId {
            get {
                return new SimpleContainer(Name);
            }
        }

        public object Value {
            get;
            private set;
        }

        void IMetadataAware.OnMetadataCreated(ModelMetadata metadata) {
            metadata.AdditionalValues[Name] = Value;
        }

        private struct SimpleContainer {
            private readonly string _name;

            public SimpleContainer(string name) {
                _name = name;
            }

            public override bool Equals(object obj) {
                return (obj is SimpleContainer)
                    ? String.Equals(this._name, ((SimpleContainer)obj)._name, StringComparison.OrdinalIgnoreCase)
                    : false;
            }

            public override int GetHashCode() {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(this._name);
            }
        }

    }
}
