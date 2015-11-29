namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Net.Mime;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Web.Resources;

    /// <summary>
    /// Default implementation of FormatHelper
    /// The results for GetRequestFormat() and GetResponseFormats() are cached on the HttpContext.Items dictionary:
    /// HttpContext.Items["requestFormat"]
    /// HttpContext.Items["responseFormat"]
    /// </summary>
    public class DefaultFormatHelper : FormatHelper {
        const string formatVariableName = "format";
        const string qualityFactor = "q";

        public string requestFormatKey = "requestFormat";
        public const string responseFormatKey = "responseFormat";

        static FormatHelper current = new DefaultFormatHelper();

        /// <summary>
        /// Returns the format of a given request, according to the following
        /// rules:
        /// 1. If a Content-Type header exists it returns a ContentType for it or fails if one can't be created
        /// 2. Otherwie, if a Content-Type header does not exists it provides the default ContentType of "application/octet-stream" (per RFC 2616 7.2.1)
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The format of the request.</returns>
        /// <exception cref="HttpException">If the format is unrecognized or not supported.</exception>
        public override ContentType GetRequestFormat(RequestContext requestContext) {
            ContentType result;
            if (!requestContext.HttpContext.Items.Contains(requestFormatKey)) {
                result = DefaultFormatHelper.GetRequestFormat(requestContext.HttpContext.Request, true);
                requestContext.HttpContext.Items.Add(requestFormatKey, result);
            }
            else {
                result = (ContentType)requestContext.HttpContext.Items[requestFormatKey];
            }
            return result;
        }

        internal static ContentType GetRequestFormat(HttpRequestBase request, bool throwOnError) {
            if (!string.IsNullOrEmpty(request.ContentType)) {
                ContentType contentType = ParseContentType(request.ContentType);
                if (contentType != null) {
                    return contentType;
                }
                if (throwOnError) {
                    throw new HttpException((int)HttpStatusCode.UnsupportedMediaType, string.Format(CultureInfo.CurrentUICulture, MvcResources.Resources_UnsupportedMediaType, request.ContentType));
                }
                return null;
            }
            return new ContentType();
        }

        /// <summary>
        /// Returns the preferred content type to use for the response, based on the request, according to the following
        /// rules:
        /// 1. If the RouteData contains a value for a key called "format", its value is returned as the content type
        /// 2. Otherwise, if the query string contains a key called "format", its value is returned as the content type
        /// 3. Otherwise, if the request has an Accepts header, the list of content types in order of preference is returned
        /// 4. Otherwise, if the request has a content type, its value is returned
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The formats to use for rendering a response.</returns>
        public override List<ContentType> GetResponseFormats(RequestContext requestContext) {
            List<ContentType> result;
            if (!requestContext.HttpContext.Items.Contains(responseFormatKey)) {
                result = DefaultFormatHelper.GetResponseFormatsRouteAware(requestContext);
                requestContext.HttpContext.Items.Add(responseFormatKey, result);
            }
            else {
                result = (List<ContentType>)requestContext.HttpContext.Items[responseFormatKey];
            }
            return result;
        }

        static List<ContentType> GetResponseFormatsRouteAware(RequestContext requestContext) {
            List<ContentType> result = DefaultFormatHelper.GetResponseFormatsCore(requestContext.HttpContext.Request);
            ContentType contentType;
            if (result == null) {
                contentType = FormatManager.Current.FormatHelper.GetRequestFormat(requestContext);
                result = new List<ContentType>(new ContentType[] { contentType });
            }
            if (TryGetFromRouteData(requestContext.RouteData, out contentType)) {
                result.Insert(0, contentType);
            }
            return result;
        }

        /// <summary>
        /// Returns the preferred content type to use for the response, based on the request, according to the following
        /// rules:
        /// 1. If the query string contains a key called "format", its value is returned as the content type
        /// 2. Otherwise, if the request has an Accepts header, the list of content types in order of preference is returned
        /// 3. Otherwise, if the request has a content type, its value is returned
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static List<ContentType> GetResponseFormats(HttpRequestBase request) {
            List<ContentType> result = DefaultFormatHelper.GetResponseFormatsCore(request);
            if (result == null) {
                ContentType contentType = DefaultFormatHelper.GetRequestFormat(request, true);
                result = new List<ContentType>(new ContentType[] { contentType });
            }
            return result;
        }

        static List<ContentType> GetResponseFormatsCore(HttpRequestBase request) {
            ContentType contentType;
            if (DefaultFormatHelper.TryGetFromUri(request, out contentType)) {
                return new List<ContentType>(new ContentType[] { contentType });
            }
            string[] accepts = request.AcceptTypes;
            if (accepts != null && accepts.Length > 0) {
                return DefaultFormatHelper.GetAcceptHeaderElements(accepts);
            }
            return null;
        }

        // CONSIDER: we currently don't process the Accept-Charset header, need to take it into account, EG:
        // Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
        static List<ContentType> GetAcceptHeaderElements(string[] acceptHeaderElements) {
            List<ContentType> contentTypeList = new List<ContentType>(acceptHeaderElements.Length);
            foreach (string acceptHeaderElement in acceptHeaderElements) {
                if (acceptHeaderElement != null) {
                    ContentType contentType = ParseContentType(acceptHeaderElement);
                    // ignore unknown formats to allow fallback
                    if (contentType != null) {
                        contentTypeList.Add(contentType);
                    }
                }
            }
            contentTypeList.Sort(new AcceptHeaderElementComparer());
            // CONSIDER: we used the "q" parameter for sorting, so now we strip it
            // it might be ebtter to strip it later in case someone needs to access it
            foreach (ContentType ct in contentTypeList) {
                if (ct.Parameters.ContainsKey(DefaultFormatHelper.qualityFactor)) {
                    ct.Parameters.Remove(DefaultFormatHelper.qualityFactor);
                }
            }
            return contentTypeList;
        }

        public override bool IsBrowserRequest(RequestContext requestContext) {
            return DefaultFormatHelper.IsBrowserRequest(requestContext.HttpContext.Request);
        }

        // Parses a string into a ContentType instance, supports
        // friendly names and enforces a charset (which defaults to utf-8)
        internal static ContentType ParseContentType(string contentTypeString) {
            ContentType contentType = null;
            try {
                contentType = new ContentType(contentTypeString);
            }
            catch (FormatException) {
                // This may be a friendly name (for example, "xml" instead of "text/xml").
                // if so, try mapping to a content type
                if (!FormatManager.Current.TryMapFormatFriendlyName(contentTypeString, out contentType)) {
                    return null;
                }
            }
            Encoding encoding = Encoding.UTF8;
            if (!string.IsNullOrEmpty(contentType.CharSet)) {
                try {
                    encoding = Encoding.GetEncoding(contentType.CharSet);
                }
                catch (ArgumentException) {
                    return null;
                }
            }
            contentType.CharSet = encoding.HeaderName;
            return contentType;
        }

        // Route-based format override so clients can use a route variable
        static bool TryGetFromRouteData(RouteData routeData, out ContentType contentType) {
            contentType = null;
            if (routeData != null) {
                string fromRouteData = routeData.Values[DefaultFormatHelper.formatVariableName] as string;
                if (!string.IsNullOrEmpty(fromRouteData)) {
                    contentType = DefaultFormatHelper.ParseContentType(fromRouteData);
                }
            }
            return contentType != null;
        }

        // Uri-based format override so clients can use a query string
        // also useful when using the browser where you can't set headerss
        static bool TryGetFromUri(HttpRequestBase request, out ContentType contentType) {
            string fromParams = request.QueryString[DefaultFormatHelper.formatVariableName];
            if (fromParams != null) {
                contentType = ParseContentType(fromParams);
                if (contentType != null) {
                    return true;
                }
            }
            contentType = null;
            return false;
        }

        class AcceptHeaderElementComparer : IComparer<ContentType> {
            public int Compare(ContentType x, ContentType y) {
                string[] xTypeSubType = x.MediaType.Split('/');
                string[] yTypeSubType = y.MediaType.Split('/');

                if (string.Equals(xTypeSubType[0], yTypeSubType[0], StringComparison.OrdinalIgnoreCase)) {
                    if (string.Equals(xTypeSubType[1], yTypeSubType[1], StringComparison.OrdinalIgnoreCase)) {
                        // need to check the number of parameters to determine which is more specific
                        bool xHasParam = AcceptHeaderElementComparer.HasParameters(x);
                        bool yHasParam = AcceptHeaderElementComparer.HasParameters(y);
                        if (xHasParam && !yHasParam) {
                            return 1;
                        }
                        else if (!xHasParam && yHasParam) {
                            return -1;
                        }
                    }
                    else {
                        if (xTypeSubType[1][0] == '*' && xTypeSubType[1].Length == 1) {
                            return 1;
                        }
                        if (yTypeSubType[1][0] == '*' && yTypeSubType[1].Length == 1) {
                            return -1;
                        }
                    }
                }
                else if (xTypeSubType[0][0] == '*' && xTypeSubType[0].Length == 1) {
                    return 1;
                }
                else if (yTypeSubType[0][0] == '*' && yTypeSubType[0].Length == 1) {
                    return -1;
                }

                decimal qualityDifference = AcceptHeaderElementComparer.GetQualityFactor(x) - AcceptHeaderElementComparer.GetQualityFactor(y);
                if (qualityDifference < 0) {
                    return 1;
                }
                else if (qualityDifference > 0) {
                    return -1;
                }
                return 0;
            }

            static decimal GetQualityFactor(ContentType contentType) {
                decimal result;
                foreach (string key in contentType.Parameters.Keys) {
                    if (string.Equals(DefaultFormatHelper.qualityFactor, key, StringComparison.OrdinalIgnoreCase)) {
                        if (decimal.TryParse(contentType.Parameters[key], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result) &&
                            (result <= (decimal)1.0)) {
                            return result;
                        }
                    }
                }

                return (decimal)1.0;
            }

            static bool HasParameters(ContentType contentType) {
                int number = 0;
                foreach (string param in contentType.Parameters.Keys) {
                    if (!string.Equals(DefaultFormatHelper.qualityFactor, param, StringComparison.OrdinalIgnoreCase)) {
                        number++;
                    }
                }

                return (number > 0);
            }
        }

        /// <summary>
        /// Determines whether the specified HTTP request was sent by a Browser.
        /// A request is considered to be from the browser if:
        /// it's a GET or POST
        /// and does not have a non-HTML entity format (XML/JSON)
        /// and has a known User-Agent header (as determined by the request's BrowserCapabilities property),
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>true if the specified HTTP request is a Browser request; otherwise, false.</returns>
        internal static bool IsBrowserRequest(HttpRequestBase request) {
            if (!request.IsHttpMethod(HttpVerbs.Get) && !request.IsHttpMethod(HttpVerbs.Post)) {
                return false;
            }
            ContentType requestFormat = DefaultFormatHelper.GetRequestFormat(request, false);
            if (requestFormat == null || string.Compare(requestFormat.MediaType, FormatManager.UrlEncoded, StringComparison.OrdinalIgnoreCase) != 0) {
                if (FormatManager.Current.CanDeserialize(requestFormat)) {
                    return false;
                }
            }
            HttpBrowserCapabilitiesBase browserCapabilities = request.Browser;
            if (browserCapabilities != null && !string.IsNullOrEmpty(request.Browser.Browser) && request.Browser.Browser != "Unknown") {
                return true;
            }
            return false;
        }

    }
}
