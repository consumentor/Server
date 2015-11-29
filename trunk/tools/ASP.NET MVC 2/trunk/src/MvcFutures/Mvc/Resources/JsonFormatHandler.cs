namespace Microsoft.Web.Mvc.Resources {
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization.Json;
    using System.Web.Mvc;

    public class JsonFormatHandler : IRequestFormatHandler, IResponseFormatHandler {
        public bool CanDeserialize(ContentType requestFormat) {
            return requestFormat != null && IsCompatibleMediaType(requestFormat.MediaType);
        }

        public object Deserialize(ControllerContext controllerContext, ModelBindingContext bindingContext, ContentType requestFormat) {
            DataContractJsonSerializer json = new DataContractJsonSerializer(bindingContext.ModelType);
            return json.ReadObject(controllerContext.HttpContext.Request.InputStream);
        }

        public bool CanSerialize(ContentType responseFormat) {
            return responseFormat != null && IsCompatibleMediaType(responseFormat.MediaType);
        }

        public void Serialize(ControllerContext context, object model, ContentType responseFormat) {
            DataContractJsonActionResult json = new DataContractJsonActionResult(model, responseFormat);
            json.ExecuteResult(context);
        }

        protected virtual bool IsCompatibleMediaType(string mediaType) {
            return (mediaType == "text/json" || mediaType == "application/json");
        }

        public bool TryMapFormatFriendlyName(string friendlyName, out ContentType contentType) {
            if (string.Equals(friendlyName, this.FriendlyName, StringComparison.OrdinalIgnoreCase)) {
                contentType = new ContentType("application/json");
                return true;
            }
            contentType = null;
            return false;
        }

        public string FriendlyName { get { return "Json"; } }
    }
}
