namespace Microsoft.Web.Mvc.AspNet4 {
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// A generic interface which represents something which supports client-side
    /// validation.
    /// </summary>
    /// <remarks>
    /// The purpose of this interface is to make something as supporting client-side
    /// validation, which could be discovered at runtime by whatever validation
    /// framework you're using. Because this interface is designed to be independent
    /// of underlying implementation details, where you apply this interface will
    /// depend on your specific validation framework.
    /// For <see cref="DataAnnotations4ModelValidatorProvider"/>, you apply this
    /// interface either custom validation attributes (which derive from 
    /// <see cref="ValidationAttribute"/>).
    /// </remarks>
    public interface IClientValidatable {
        IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context);
    }
}
