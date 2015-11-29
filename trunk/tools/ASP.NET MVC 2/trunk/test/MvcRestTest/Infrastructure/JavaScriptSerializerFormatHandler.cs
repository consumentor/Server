using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Microsoft.Web.Mvc.Resources;

namespace MovieApp.Infrastructure
{
    // use json2 rather than json so this runs SxS with the OOB json
    public class JavaScriptSerializerFormatHandler : IRequestFormatHandler, IResponseFormatHandler
    {
        public bool CanDeserialize(ContentType requestFormat)
        {
            return requestFormat != null && IsCompatibleMediaType(requestFormat.MediaType);
        }

        public object Deserialize(ControllerContext controllerContext, ModelBindingContext bindingContext, ContentType requestFormat)
        {
            string input = new StreamReader(controllerContext.HttpContext.Request.InputStream).ReadToEnd();
            MethodInfo deserialize = typeof(JavaScriptSerializer).GetMethod("Deserialize", new Type[] { typeof(string) });
            MethodInfo deserializeForType = deserialize.MakeGenericMethod(bindingContext.ModelType);
            return deserializeForType.Invoke(new JavaScriptSerializer(), new object[] { input });
        }

        public bool CanSerialize(ContentType responseFormat)
        {
            return responseFormat != null && IsCompatibleMediaType(responseFormat.MediaType);
        }

        public void Serialize(ControllerContext context, object model, ContentType responseFormat)
        {
            JavaScriptSerializerActionResult json = new JavaScriptSerializerActionResult(model, responseFormat);
            json.ExecuteResult(context);
        }

        protected virtual bool IsCompatibleMediaType(string mediaType)
        {
            return (mediaType == "text/json2" || mediaType == "application/json2");
        }

        public bool TryMapFormatFriendlyName(string friendlyName, out ContentType contentType)
        {
            if (string.Equals(friendlyName, this.FriendlyName, StringComparison.OrdinalIgnoreCase))
            {
                contentType = new ContentType("application/json2");
                return true;
            }
            contentType = null;
            return false;
        }

        public string FriendlyName { get { return "Json2"; } }

        class JavaScriptSerializerActionResult : DataContractJsonActionResult
        {
            public JavaScriptSerializerActionResult(object data, ContentType contentType)
                : base(data, contentType)
            {
            }

            public override void ExecuteResult(ControllerContext context)
            {
                Encoding encoding = Encoding.UTF8;
                if (!string.IsNullOrEmpty(this.ContentType.CharSet))
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(this.ContentType.CharSet);
                    }
                    catch (ArgumentException)
                    {
                        throw new HttpException((int)HttpStatusCode.NotAcceptable, string.Format(CultureInfo.CurrentCulture, "Format {0} not supported", this.ContentType));
                    }
                }
                JavaScriptSerializer json = new JavaScriptSerializer();
                this.ContentType.CharSet = encoding.HeaderName;
                context.HttpContext.Response.ContentType = this.ContentType.ToString();
                StringBuilder sb = new StringBuilder();
                json.Serialize(this.Data, sb);
                byte[] bytes = encoding.GetBytes(sb.ToString());
                context.HttpContext.Response.OutputStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
