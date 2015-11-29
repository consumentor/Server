namespace Microsoft.Web.Mvc.AspNet4 {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Mvc;
    using Microsoft.Web.Mvc.AspNet4.Resources;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ChildActionCacheAttribute : ActionFilterAttribute {

        private const string _cacheKeyPrefix = "__MVC_ChildActionCache_Entry_";
        private static readonly object _typeId = new object();

        // Non-static, as we want this to be unique per filter declaration. Ideally this would be unique per filter
        // invocation, but MVC 2 makes no guarantees about whether filters are reused between invocations.
        // We really need a mechanism so that we know which particular invocation is currently running.
        private readonly object _itemsCacheKey = new object();

        private string _key;

        // measures in seconds
        public int Duration {
            get;
            set;
        }

        // Most users won't need this, but in certain circumstances it could be necessary to avoid collisions,
        // e.g. if two actions on a controller have the same name and same signature.
        public string Key {
            get {
                return _key ?? String.Empty;
            }
            set {
                _key = value;
            }
        }

        public override object TypeId {
            get {
                return _typeId;
            }
        }

        private string GenerateCacheKey(IDictionary<string, object> parameters, params object[] extraInput) {
            // generate a cache key for just the parameters
            List<object> parameterCacheKeyInputs = new List<object>(parameters.Count * 2);
            foreach (var entry in parameters.OrderBy(o => o.Key, StringComparer.OrdinalIgnoreCase)) {
                parameterCacheKeyInputs.Add(entry.Key);
                parameterCacheKeyInputs.Add(entry.Value);
            }
            string parametersKey = CacheKeyUtil.GenerateCacheKey(parameterCacheKeyInputs.ToArray());

            object[] combinedInputs =
                new object[] { Duration, Key, parametersKey }
                .Concat(extraInput).ToArray();

            return _cacheKeyPrefix + CacheKeyUtil.GenerateCacheKey(combinedInputs);
        }

        internal virtual object GetCacheItem(HttpContextBase httpContext, string cacheKey) {
            return httpContext.Cache[cacheKey];
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            ValidateAttribute();

            if (!filterContext.IsChildAction) {
                return; // do nothing for non-child actions
            }

            string cacheKey = GenerateCacheKey(filterContext.ActionParameters,
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName,
                filterContext.HttpContext.Response.ContentEncoding.CodePage);

            // Does this already exist in the cache?
            string cachedOutput = GetCacheItem(filterContext.HttpContext, cacheKey) as string;
            if (cachedOutput != null) {
                filterContext.Result = new ContentResult() { Content = cachedOutput };
                return;
            }

            // Tell the result filter to hook the response
            filterContext.HttpContext.Items[_itemsCacheKey] = cacheKey;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext) {
            // Should we hook the response?
            string cacheKey = filterContext.HttpContext.Items[_itemsCacheKey] as string;
            if (cacheKey != null) {
                TextWriter originalWriter = filterContext.HttpContext.Response.Output;
                WrappedStringWriter newWriter = new WrappedStringWriter(originalWriter);
                filterContext.HttpContext.Response.Output = newWriter;
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext) {
            string cacheKey = filterContext.HttpContext.Items[_itemsCacheKey] as string;
            if (cacheKey == null) {
                return; // nothing to do
            }

            // We know that the current writer is our special writer, so pop it
            WrappedStringWriter newWriter = (WrappedStringWriter)filterContext.HttpContext.Response.Output;
            filterContext.HttpContext.Response.Output = newWriter.OriginalWriter;
            string capturedText = newWriter.ToString();

            // dump the text to the output
            filterContext.HttpContext.Response.Write(capturedText);

            // save to cache
            if (filterContext.Exception == null) {
                SetCacheItem(filterContext.HttpContext, cacheKey, capturedText);
            }
        }

        internal virtual void SetCacheItem(HttpContextBase httpContext, string cacheKey, object value) {
            httpContext.Cache.Insert(cacheKey, value, null /* dependencies */, DateTime.UtcNow.AddSeconds(Duration), Cache.NoSlidingExpiration);
        }

        private void ValidateAttribute() {
            if (Duration <= 0) {
                throw new InvalidOperationException(MvcResources.ChildActionCacheAttribute_DurationMustBePositive);
            }
        }

        private static class CacheKeyUtil {
            public static string GenerateCacheKey(params object[] input) {
                string[] inputAsString = Array.ConvertAll(input, element => Convert.ToString(element, CultureInfo.InvariantCulture));

                // First append the lengths of all the strings

                int totalInputLength = 0;
                StringBuilder keyBuilder = new StringBuilder();
                keyBuilder.Append(inputAsString.Length.ToString(CultureInfo.InvariantCulture));
                keyBuilder.Append('_');
                foreach (string s in inputAsString) {
                    if (s == null) {
                        keyBuilder.Append("null_");
                    }
                    else {
                        totalInputLength += s.Length;
                        keyBuilder.Append(s.Length.ToString(CultureInfo.InvariantCulture));
                        keyBuilder.Append('_');
                    }
                    totalInputLength++;
                }

                // Then append the strings themselves

                keyBuilder.EnsureCapacity(keyBuilder.Length + totalInputLength);
                foreach (string s in inputAsString) {
                    if (s != null) {
                        keyBuilder.Append(s);
                    }
                    keyBuilder.Append('_');
                }
                return keyBuilder.ToString(0, keyBuilder.Length - 1);
            }
        }

        // This is a special StringWriter that overrides certain properties necessary to
        // mimic an underlying TextWriter.
        // See http://msdn.microsoft.com/en-us/library/system.io.stringwriter_properties(v=VS.100).aspx.
        internal sealed class WrappedStringWriter : StringWriter {
            public WrappedStringWriter(TextWriter originalWriter) {
                OriginalWriter = originalWriter;
            }

            public override Encoding Encoding {
                get {
                    return OriginalWriter.Encoding;
                }
            }

            public override IFormatProvider FormatProvider {
                get {
                    return OriginalWriter.FormatProvider;
                }
            }

            public override string NewLine {
                get {
                    return OriginalWriter.NewLine;
                }
                set {
                    OriginalWriter.NewLine = value;
                }
            }

            public TextWriter OriginalWriter {
                get;
                private set;
            }
        }

    }
}
