namespace Microsoft.Web.Mvc.Resources {
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Web;
    using System.Web.Routing;

    /// <summary>
    /// Base class for content negotiation support
    /// </summary>
    public abstract class FormatHelper {
        /// <summary>
        /// Returns the ContentType of a given request.
        /// </summary>
        /// <param name="requestContext">The request.</param>
        /// <returns>The format of the request.</returns>
        /// <exception cref="HttpException">If the format is unrecognized or not supported.</exception>
        public abstract ContentType GetRequestFormat(RequestContext requestContext);

        /// <summary>
        /// Returns a collection of ContentType instances that can be used to render a response to a given request, sorted in priority order.
        /// </summary>
        /// <param name="requestContext">The request.</param>
        /// <returns>The formats to use for rendering a response.</returns>
        public abstract List<ContentType> GetResponseFormats(RequestContext requestContext);

        /// <summary>
        /// <summary>
        /// Determines whether the specified HTTP request was sent by a Browser.
        /// </summary>
        /// <param name="requestContext">The request.</param>
        /// <returns></returns>
        public abstract bool IsBrowserRequest(RequestContext requestContext);
    }
}
