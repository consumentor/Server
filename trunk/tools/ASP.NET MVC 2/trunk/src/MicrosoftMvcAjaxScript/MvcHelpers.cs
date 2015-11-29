namespace Sys.Mvc {
    using System;
    using System.DHTML;
    using Sys.Net;

    public static class MvcHelpers {

        // MVC implementation of PageRequestManager.js / _onFormElementActive()
        internal static string SerializeSubmitButton(DOMElement element, int offsetX, int offsetY) {
            // element: the form element that is active
            // offsetX/Y: if the element is an image button, the coordinates of the click

            if (element.Disabled) {
                return null;
            }

            string name = (string)Type.GetField(element, "name");
            if (name != null) {
                string tagName = element.TagName.ToUpperCase();
                string encodedName = name.EncodeURIComponent();
                InputElement inputElement = (InputElement)element;
                if (tagName == "INPUT") {
                    string type = inputElement.Type;
                    if (type == "submit") {
                        return encodedName + "=" + inputElement.Value.EncodeURIComponent();
                    }
                    else if (type == "image") {
                        return encodedName + ".x=" + offsetX + "&" + encodedName + ".y=" + offsetY;
                    }
                }
                else if ((tagName == "BUTTON") && (name.Length != 0) && (inputElement.Type == "submit")) {
                    return encodedName + "=" + inputElement.Value.EncodeURIComponent();
                }
            }

            return null;
        }

        internal static string SerializeForm(FormElement form) {
            DOMElement[] formElements = form.Elements;
            StringBuilder formBody = new StringBuilder();

            int count = formElements.Length;
            for (int i = 0; i < count; i++) {
                DOMElement element = formElements[i];
                string name = (string)Type.GetField(element, "name");
                if (name == null || name.Length == 0) {
                    continue;
                }

                string tagName = element.TagName.ToUpperCase();

                if (tagName == "INPUT") {
                    InputElement inputElement = (InputElement)element;
                    string type = inputElement.Type;
                    if ((type == "text") ||
                        (type == "password") ||
                        (type == "hidden") ||
                        (((type == "checkbox") || (type == "radio")) && (bool)Type.GetField(element, "checked"))) {

                        formBody.Append(name.EncodeURIComponent());
                        formBody.Append("=");
                        formBody.Append(inputElement.Value.EncodeURIComponent());
                        formBody.Append("&");
                    }
                }
                else if (tagName == "SELECT") {
                    SelectElement selectElement = (SelectElement)element;
                    int optionCount = selectElement.Options.Length;
                    for (int j = 0; j < optionCount; j++) {
                        OptionElement optionElement = (OptionElement)selectElement.Options[j];
                        if (optionElement.Selected) {
                            formBody.Append(name.EncodeURIComponent());
                            formBody.Append("=");
                            formBody.Append(optionElement.Value.EncodeURIComponent());
                            formBody.Append("&");
                        }
                    }
                }
                else if (tagName == "TEXTAREA") {
                    formBody.Append(name.EncodeURIComponent());
                    formBody.Append("=");
                    formBody.Append(((string)Type.GetField(element, "value")).EncodeURIComponent());
                    formBody.Append("&");
                }
            }

            // additional input represents the submit button or image that was clicked
            string additionalInput = (string)Type.GetField(form, "_additionalInput");
            if (additionalInput != null) {
                formBody.Append(additionalInput);
                formBody.Append("&");
            }

            return formBody.ToString();
        }

        internal static void AsyncRequest(string url, string verb, string body, DOMElement triggerElement, AjaxOptions ajaxOptions) {
            // Run the confirm popup, if specified
            if (ajaxOptions.Confirm != null) {
                if (!Script.Confirm(ajaxOptions.Confirm)) {
                    return;
                }
            }

            // Override the url if specified in AjaxOptions
            if (ajaxOptions.Url != null) {
                url = ajaxOptions.Url;
            }

            // Override the verb if specified in AjaxOptions
            if (ajaxOptions.HttpMethod != null) {
                verb = ajaxOptions.HttpMethod;
            }

            // Add the special hidden fields to the body
            if (body.Length > 0 && !body.EndsWith('&')) {
                body += "&";
            }
            body += "X-Requested-With=XMLHttpRequest";

            string upperCaseVerb = verb.ToUpperCase();
            bool isGetOrPost = (upperCaseVerb == "GET" || upperCaseVerb == "POST");
            if (!isGetOrPost) {
                body += "&";
                body += "X-HTTP-Method-Override=" + upperCaseVerb;
            }

            // Determine where to place the body
            string requestBody = "";
            if (upperCaseVerb == "GET" || upperCaseVerb == "DELETE") {
                if (url.IndexOf('?') > -1) {
                    // Case 1: http://foo.bar/baz?abc=123
                    if (!url.EndsWith('&')) {
                        url += "&";
                    }
                    url += body;
                }
                else {
                    // Case 2: http://foo.bar/baz
                    url += "?";
                    url += body;
                }
            }
            else {
                requestBody = body;
            }

            // Create the request
            WebRequest request = new WebRequest();

            request.Url = url;
            // Some browsers only support XMLHttpRequest with GET and POST. Just to be
            // safe we restrict out requests to use only those two methods and use a
            // header as well as a form post field to override the verb. On the server side
            // the header and form post field are supported using the AcceptVerbs attribute.
            if (isGetOrPost) {
                request.HttpVerb = verb;
            }
            else {
                request.HttpVerb = "POST";
                request.Headers["X-HTTP-Method-Override"] = upperCaseVerb;
            }
            request.Body = requestBody;
            if (verb.ToUpperCase() == "PUT") {
                request.Headers["Content-Type"] = "application/x-www-form-urlencoded;";
            }
            request.Headers["X-Requested-With"] = "XMLHttpRequest";

            DOMElement updateElement = null;
            if (ajaxOptions.UpdateTargetId != null) {
                updateElement = Document.GetElementById(ajaxOptions.UpdateTargetId);
            }

            DOMElement loadingElement = null;
            if (ajaxOptions.LoadingElementId != null) {
                loadingElement = Document.GetElementById(ajaxOptions.LoadingElementId);
            }

            // Create the AjaxContext for the request
            AjaxContext ajaxContext = new AjaxContext(request, updateElement, loadingElement, ajaxOptions.InsertionMode);

            // Run onBegin and check for cancellation
            bool continueRequest = true;
            if (ajaxOptions.OnBegin != null) {
                // Have to convert to objects to force the "!== false" to be emitted.
                // We want no return value to be treated as returning true, so we only want to cancel the request if the result is exactly "false"
                continueRequest = (object)ajaxOptions.OnBegin(ajaxContext) != (object)false;
            }

            // Display the loading element, if specified
            if (loadingElement != null) {
                Type.InvokeMethod(typeof(Sys.UI.DomElement), "setVisible", ajaxContext.LoadingElement, true);
            }

            if (continueRequest) {
                // Setup the callback
                request.Completed += delegate(WebRequestExecutor executor) {
                    MvcHelpers.OnComplete(request, ajaxOptions, ajaxContext);
                };

                request.Invoke();
            }
        }

        internal static void OnComplete(WebRequest request, AjaxOptions ajaxOptions, AjaxContext ajaxContext) {
            // Update the AjaxContext
            ajaxContext.Response = request.Executor;

            // Run onComplete and check for cancellation
            // Have to convert to objects to force the "=== false" to be emitted.
            // We want no return value to be treated as returning true, so we only want to cancel the request if the result is exactly "false"
            if (ajaxOptions.OnComplete != null && (object)ajaxOptions.OnComplete(ajaxContext) == (object)false) {
                return;
            }

            // If the status code was successful...
            int statusCode = ajaxContext.Response.StatusCode;
            if ((statusCode >= 200 && statusCode < 300) || statusCode == 304 || statusCode == 1223) {
                // If the status code is one of 204 (No Content), 304 (Not Modified), or 1223 (IE-specific code caused by 204), don't do the injection
                if (statusCode != 204 && statusCode != 304 && statusCode != 1223) {
                    string contentType = ajaxContext.Response.GetResponseHeader("Content-Type");
                    if ((contentType != null) && (contentType.IndexOf("application/x-javascript") != -1)) {
                        Script.Eval(ajaxContext.Data);
                    }
                    else {
                        UpdateDomElement(ajaxContext.UpdateTarget, ajaxContext.InsertionMode, ajaxContext.Data);
                    }
                }

                if (ajaxOptions.OnSuccess != null) {
                    ajaxOptions.OnSuccess(ajaxContext);
                }
            }
            else {
                if (ajaxOptions.OnFailure != null) {
                    ajaxOptions.OnFailure(ajaxContext);
                }
            }

            // Hide the loading panel, if there is one
            if (ajaxContext.LoadingElement != null) {
                Type.InvokeMethod(typeof(Sys.UI.DomElement), "setVisible", ajaxContext.LoadingElement, false);
            }
        }

        public static void UpdateDomElement(DOMElement target, InsertionMode insertionMode, string content) {
            if (target != null) {
                switch (insertionMode) {
                    case InsertionMode.Replace:
                        target.InnerHTML = content;
                        break;
                    case InsertionMode.InsertBefore:
                        if (content != null && content.Length > 0) {
                            // Trimming here may be a bit too aggressive.  It is done to make behavior consistent across
                            // browsers (since IE normalizes whitespace BEFORE building its internal data structures).
                            // However, it may end up trimming whitespace left intentionally by the developer.
                            target.InnerHTML = content + target.InnerHTML.TrimStart();
                        }
                        break;
                    case InsertionMode.InsertAfter:
                        if (content != null && content.Length > 0) {
                            // See comment for InsertBefore re: Trimming
                            target.InnerHTML = target.InnerHTML.TrimEnd() + content;
                        }
                        break;
                }
            }
        }
    }


}
