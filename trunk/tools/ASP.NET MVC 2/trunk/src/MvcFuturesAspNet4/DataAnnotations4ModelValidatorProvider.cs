namespace Microsoft.Web.Mvc.AspNet4 {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web.Mvc;
    using Microsoft.Web.Mvc.AspNet4.Resources;

    public delegate ModelValidator DataAnnotations4ValidatableObjectAdapterFactory(ModelMetadata metadata, ControllerContext context);

    /// <summary>
    /// An implementation of <see cref="ModelValidatorProvider"/> which providers validators
    /// for attributes which derive from <see cref="ValidationAttribute"/>. It also provides
    /// a validator for types which implement <see cref="IValidatableObject"/>. To support
    /// client side validation, you can either register adapters through the static methods
    /// on this class, or by having your validation attributes implement
    /// <see cref="IClientValidatable"/>. The logic to support IClientValidatable
    /// is implemented in <see cref="DataAnnotations4ModelValidator"/>.
    /// </summary>
    public class DataAnnotations4ModelValidatorProvider : AssociatedValidatorProvider {
        private static ReaderWriterLockSlim _lockObject = new ReaderWriterLockSlim();

        // Factories for validation attributes

        internal static DataAnnotationsModelValidationFactory DefaultAttributeFactory =
            (metadata, context, attribute) => new DataAnnotations4ModelValidator(metadata, context, attribute);

        internal static Dictionary<Type, DataAnnotationsModelValidationFactory> AttributeFactories = new Dictionary<Type, DataAnnotationsModelValidationFactory>() {
            {
                typeof(RangeAttribute),
                (metadata, context, attribute) => new RangeAttribute4Adapter(metadata, context, (RangeAttribute)attribute)
            },
            {
                typeof(RegularExpressionAttribute),
                (metadata, context, attribute) => new RegularExpressionAttribute4Adapter(metadata, context, (RegularExpressionAttribute)attribute)
            },
            {
                typeof(RequiredAttribute),
                (metadata, context, attribute) => new RequiredAttribute4Adapter(metadata, context, (RequiredAttribute)attribute)
            },
            {
                typeof(StringLengthAttribute),
                (metadata, context, attribute) => new StringLengthAttribute4Adapter(metadata, context, (StringLengthAttribute)attribute)
            },
        };

        // Factories for IValidatableObject models

        internal static DataAnnotations4ValidatableObjectAdapterFactory DefaultValidatableFactory =
            (metadata, context) => new ValidatableObjectAdapter(metadata, context);

        internal static Dictionary<Type, DataAnnotations4ValidatableObjectAdapterFactory> ValidatableFactories = new Dictionary<Type, DataAnnotations4ValidatableObjectAdapterFactory>();

        public DataAnnotations4ModelValidatorProvider() {
            AddImpliedRequiredAttributes = true;
        }

        /// <summary>
        /// A flag which indicates whether the validator provider should automatically
        /// generate <see cref="RequiredAttribute"/> instances for non-nullable value
        /// types when there isn't one already present. In
        /// <see cref="DataAnnotationsModelValidatorProvider"/>, this was always true.
        /// </summary>
        public bool AddImpliedRequiredAttributes { get; set; }

        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes) {
            _lockObject.EnterReadLock();

            try {
                List<ModelValidator> results = new List<ModelValidator>();

                // Add an implied [Required] attribute for any non-nullable value type,
                // unless they've configured us not to do that.
                if (AddImpliedRequiredAttributes
                        && metadata.IsRequired
                        && !attributes.Any(a => a is RequiredAttribute)) {
                    attributes = attributes.Concat(new[] { new RequiredAttribute() });
                }

                // Produce a validator for each validation attribute we find
                foreach (ValidationAttribute attribute in attributes.OfType<ValidationAttribute>()) {
                        DataAnnotationsModelValidationFactory factory;
                        if (!AttributeFactories.TryGetValue(attribute.GetType(), out factory)) {
                            factory = DefaultAttributeFactory;
                        }
                        results.Add(factory(metadata, context, attribute));
                }

                // Produce a validator if the type supports IValidatableObject
                if (typeof(IValidatableObject).IsAssignableFrom(metadata.ModelType)) {
                    DataAnnotations4ValidatableObjectAdapterFactory factory;
                    if (!ValidatableFactories.TryGetValue(metadata.ModelType, out factory)) {
                        factory = DefaultValidatableFactory;
                    }
                    results.Add(factory(metadata, context));
                }

                return results;
            }
            finally {
                _lockObject.ExitReadLock();
            }
        }

        #region Validation attribute adapter registration

        public static void RegisterAdapter(Type attributeType, Type adapterType) {
            GuardAttributeType(attributeType);
            GuardAttributeAdapterType(adapterType);
            ConstructorInfo constructor = GetAttributeAdapterConstructor(attributeType, adapterType);

            _lockObject.EnterWriteLock();

            try {
                AttributeFactories[attributeType] = (metadata, context, attribute) => (ModelValidator)constructor.Invoke(new object[] { metadata, context, attribute });
            }
            finally {
                _lockObject.ExitWriteLock();
            }
        }

        public static void RegisterAdapterFactory(Type attributeType, DataAnnotationsModelValidationFactory factory) {
            GuardAttributeType(attributeType);
            GuardAttributeFactory(factory);

            _lockObject.EnterWriteLock();

            try {
                AttributeFactories[attributeType] = factory;
            }
            finally {
                _lockObject.ExitWriteLock();
            }
        }

        public static void RegisterDefaultAdapter(Type adapterType) {
            GuardAttributeAdapterType(adapterType);
            ConstructorInfo constructor = GetAttributeAdapterConstructor(typeof(ValidationAttribute), adapterType);

            DefaultAttributeFactory = (metadata, context, attribute) => (ModelValidator)constructor.Invoke(new object[] { metadata, context, attribute });
        }

        public static void RegisterDefaultAdapterFactory(DataAnnotationsModelValidationFactory factory) {
            GuardAttributeFactory(factory);

            DefaultAttributeFactory = factory;
        }

        // Helpers 

        private static ConstructorInfo GetAttributeAdapterConstructor(Type attributeType, Type adapterType) {
            ConstructorInfo constructor = adapterType.GetConstructor(new[] { typeof(ModelMetadata), typeof(ControllerContext), attributeType });
            if (constructor == null) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.DataAnnotations4ModelValidatorProvider_AttributeConstructorRequirements,
                        adapterType.FullName,
                        typeof(ModelMetadata).FullName,
                        typeof(ControllerContext).FullName,
                        attributeType.FullName
                    ),
                    "adapterType"
                );
            }

            return constructor;
        }

        private static void GuardAttributeAdapterType(Type adapterType) {
            if (adapterType == null) {
                throw new ArgumentNullException("adapterType");
            }
            if (!typeof(ModelValidator).IsAssignableFrom(adapterType)) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.Common_TypeMustDeriveFromType,
                        adapterType.FullName,
                        typeof(ModelValidator).FullName
                    ),
                    "adapterType"
                );
            }
        }

        private static void GuardAttributeType(Type attributeType) {
            if (attributeType == null) {
                throw new ArgumentNullException("attributeType");
            }
            if (!typeof(ValidationAttribute).IsAssignableFrom(attributeType)) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.Common_TypeMustDeriveFromType,
                        attributeType.FullName,
                        typeof(ValidationAttribute).FullName
                    ),
                    "attributeType");
            }
        }

        private static void GuardAttributeFactory(DataAnnotationsModelValidationFactory factory) {
            if (factory == null) {
                throw new ArgumentNullException("factory");
            }
        }

        #endregion

        #region IValidatableObject adapter registration

        /// <summary>
        /// Registers an adapter type for the given <see cref="modelType"/>, which must
        /// implement <see cref="IValidatableObject"/>. The adapter type must derive from
        /// <see cref="ModelValidator"/> and it must contain a public constructor
        /// which takes two parameters of types <see cref="ModelMetadata"/> and
        /// <see cref="ControllerContext"/>.
        /// </summary>
        public static void RegisterValidatableObjectAdapter(Type modelType, Type adapterType) {
            GuardValidatableModelType(modelType);
            GuardValidatableAdapterType(adapterType);
            ConstructorInfo constructor = GetValidatableAdapterConstructor(adapterType);

            _lockObject.EnterWriteLock();

            try {
                ValidatableFactories[modelType] = (metadata, context) => (ModelValidator)constructor.Invoke(new object[] { metadata, context });
            }
            finally {
                _lockObject.ExitWriteLock();
            }
        }

        /// <summary>
        /// Registers an adapter factory for the given <see cref="modelType"/>, which must
        /// implement <see cref="IValidatableObject"/>.
        /// </summary>
        public static void RegisterValidatableObjectAdapterFactory(Type modelType, DataAnnotations4ValidatableObjectAdapterFactory factory) {
            GuardValidatableModelType(modelType);
            GuardValidatableFactory(factory);

            _lockObject.EnterWriteLock();

            try {
                ValidatableFactories[modelType] = factory;
            }
            finally {
                _lockObject.ExitWriteLock();
            }
        }

        /// <summary>
        /// Registers the default adapter type for objects which implement
        /// <see cref="IValidatableObject"/>. The adapter type must derive from
        /// <see cref="ModelValidator"/> and it must contain a public constructor
        /// which takes two parameters of types <see cref="ModelMetadata"/> and
        /// <see cref="ControllerContext"/>.
        /// </summary>
        public static void RegisterDefaultValidatableObjectAdapter(Type adapterType) {
            GuardValidatableAdapterType(adapterType);
            ConstructorInfo constructor = GetValidatableAdapterConstructor(adapterType);

            DefaultValidatableFactory = (metadata, context) => (ModelValidator)constructor.Invoke(new object[] { metadata, context });
        }

        /// <summary>
        /// Registers the default adapter factory for objects which implement
        /// <see cref="IValidatableObject"/>.
        /// </summary>
        public static void RegisterDefaultValidatableObjectAdapterFactory(DataAnnotations4ValidatableObjectAdapterFactory factory) {
            GuardValidatableFactory(factory);

            DefaultValidatableFactory = factory;
        }

        // Helpers 

        private static ConstructorInfo GetValidatableAdapterConstructor(Type adapterType) {
            ConstructorInfo constructor = adapterType.GetConstructor(new[] { typeof(ModelMetadata), typeof(ControllerContext) });
            if (constructor == null) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.DataAnnotations4ModelValidatorProvider_ValidatableConstructorRequirements,
                        adapterType.FullName,
                        typeof(ModelMetadata).FullName,
                        typeof(ControllerContext).FullName
                    ),
                    "adapterType"
                );
            }

            return constructor;
        }

        private static void GuardValidatableAdapterType(Type adapterType) {
            if (adapterType == null) {
                throw new ArgumentNullException("adapterType");
            }
            if (!typeof(ModelValidator).IsAssignableFrom(adapterType)) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.Common_TypeMustDeriveFromType,
                        adapterType.FullName,
                        typeof(ModelValidator).FullName
                    ),
                    "adapterType");
            }
        }

        private static void GuardValidatableModelType(Type modelType) {
            if (modelType == null) {
                throw new ArgumentNullException("modelType");
            }
            if (!typeof(IValidatableObject).IsAssignableFrom(modelType)) {
                throw new ArgumentException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources.Common_TypeMustDeriveFromType,
                        modelType.FullName,
                        typeof(IValidatableObject).FullName
                    ),
                    "modelType"
                );
            }
        }

        private static void GuardValidatableFactory(DataAnnotations4ValidatableObjectAdapterFactory factory) {
            if (factory == null) {
                throw new ArgumentNullException("factory");
            }
        }

        #endregion

        /// <summary>
        /// Provider registration helper, which replaces the existing
        /// <see cref="DataAnnotationsModelValidatorProvider"/> with the new one that
        /// supports .NET 4. If the old provider isn't registered, then the new one
        /// is added at the end of the list.
        /// </summary>
        /// <param name="addImpliedRequiredAttributes">A flag which indicates whether
        /// the validator provider should automatically generate <see cref="RequiredAttribute"/>
        /// instances for non-nullable value types when there isn't one already
        /// present. In <see cref="DataAnnotationsModelValidatorProvider"/>, this
        /// was always true.</param>
        public static void RegisterProvider(bool addImpliedRequiredAttributes = true) {
            // Start assuming we'll go at the end
            int index = ModelValidatorProviders.Providers.Count;

            // We need to remove the old DA provider, and record its place in the provider list
            // so we can preserve the original order
            var oldProvider = ModelValidatorProviders.Providers.FirstOrDefault(p => p is DataAnnotationsModelValidatorProvider);
            if (oldProvider != null) {
                index = ModelValidatorProviders.Providers.IndexOf(oldProvider);
                ModelValidatorProviders.Providers.Remove(oldProvider);
            }

            var newProvider = new DataAnnotations4ModelValidatorProvider { AddImpliedRequiredAttributes = addImpliedRequiredAttributes };
            ModelValidatorProviders.Providers.Insert(index, newProvider);
        }
    }
}
