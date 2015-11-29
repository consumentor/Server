namespace Microsoft.Web.Mvc.AspNet4 {
    using System.Web.Mvc;

    /// <summary>
    /// This interface is implemented by attributes which wish to contribute to
    /// the <see cref="ModelMetadata"/> creation process without needing to write
    /// a custom metadata provider. It is currently consumed by
    /// <see cref="DataAnnotations4ModelMetadataProvider"/>, but if put into the
    /// main code, would eventually be consumed by
    /// <see cref="AssociatedModelMetadataProvider"/>.
    /// </summary>
    public interface IMetadataAware {
        void OnMetadataCreated(ModelMetadata metadata);
    }
}
