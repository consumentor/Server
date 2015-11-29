﻿namespace Microsoft.Web.Mvc {
    using System;
    using System.Web.Mvc;
    using Microsoft.Web.Resources;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ContentTypeAttribute : ActionFilterAttribute {
        public ContentTypeAttribute(string contentType) {
            if (String.IsNullOrEmpty(contentType)) {
                throw new ArgumentException(MvcResources.Common_NullOrEmpty, "contentType");
            }
            
            ContentType = contentType;
        }

        public string ContentType {
            get;
            private set;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext) {
            filterContext.HttpContext.Response.ContentType = ContentType;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext) {
            filterContext.HttpContext.Response.ContentType = ContentType;
        }
    }
}

