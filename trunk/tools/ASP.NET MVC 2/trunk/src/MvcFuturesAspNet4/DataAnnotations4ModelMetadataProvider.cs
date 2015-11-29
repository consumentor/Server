namespace Microsoft.Web.Mvc.AspNet4 {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    public class DataAnnotations4ModelMetadataProvider : DataAnnotationsModelMetadataProvider {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType,
                                                        Func<object> modelAccessor, Type modelType, string propertyName) {
            ModelMetadata metadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            DisplayAttribute display = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if (display != null) {
                string name = display.GetName();
                if (name != null) {
                    metadata.DisplayName = name;
                }

                metadata.Description = display.GetDescription();
                metadata.ShortDisplayName = display.GetShortName();
                metadata.Watermark = display.GetPrompt();
            }

            EditableAttribute editable = attributes.OfType<EditableAttribute>().FirstOrDefault();
            if (editable != null) {
                metadata.IsReadOnly = !editable.AllowEdit;
            }

            DisplayFormatAttribute displayFormat = attributes.OfType<DisplayFormatAttribute>().FirstOrDefault();
            if (displayFormat != null && !displayFormat.HtmlEncode && String.IsNullOrWhiteSpace(metadata.DataTypeName)) {
                metadata.DataTypeName = DataType.Html.ToString();
            }

            foreach (IMetadataAware awareAttribute in attributes.OfType<IMetadataAware>()) {
                awareAttribute.OnMetadataCreated(metadata);
            }

            return metadata;
        }

        public static void RegisterProvider() {
            ModelMetadataProviders.Current = new DataAnnotations4ModelMetadataProvider();
        }
    }
}
