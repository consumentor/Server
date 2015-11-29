// Type: System.Web.Services.Protocols.SoapHttpClientProtocol
// Assembly: System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Web.Services.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Configuration;
using System.Web.Services.Description;
using System.Web.Services.Diagnostics;
using System.Web.Services.Discovery;
using System.Xml;
using System.Xml.Serialization;

namespace System.Web.Services.Protocols
{
  [ComVisible(true)]
  public class SoapHttpClientProtocol : HttpWebClientProtocol
  {
    private SoapClientType clientType;
    private SoapProtocolVersion version;

    [DefaultValue(SoapProtocolVersion.Default)]
    [WebServicesDescription("ClientProtocolSoapVersion")]
    [ComVisible(false)]
    public SoapProtocolVersion SoapVersion
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.version;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.version = value;
      }
    }

    string EnvelopeNs
    {
      private get
      {
        if (this.version != SoapProtocolVersion.Soap12)
          return "http://schemas.xmlsoap.org/soap/envelope/";
        else
          return "http://www.w3.org/2003/05/soap-envelope";
      }
    }

    string EncodingNs
    {
      private get
      {
        if (this.version != SoapProtocolVersion.Soap12)
          return "http://schemas.xmlsoap.org/soap/encoding/";
        else
          return "http://www.w3.org/2003/05/soap-encoding";
      }
    }

    string HttpContentType
    {
      private get
      {
        if (this.version != SoapProtocolVersion.Soap12)
          return "text/xml";
        else
          return "application/soap+xml";
      }
    }

    public SoapHttpClientProtocol()
    {
      Type type = this.GetType();
      this.clientType = (SoapClientType) WebClientProtocol.GetFromCache(type);
      if (this.clientType != null)
        return;
      lock (WebClientProtocol.InternalSyncObject)
      {
        this.clientType = (SoapClientType) WebClientProtocol.GetFromCache(type);
        if (this.clientType != null)
          return;
        this.clientType = new SoapClientType(type);
        WebClientProtocol.AddToCache(type, (object) this.clientType);
      }
    }

    public void Discover()
    {
      if (this.clientType.Binding == null)
      {
        throw new InvalidOperationException(System.Web.Services.Res.GetString("DiscoveryIsNotPossibleBecauseTypeIsMissing1", new object[1]
        {
          (object) this.GetType().FullName
        }));
      }
      else
      {
        foreach (object obj in (IEnumerable) new DiscoveryClientProtocol((HttpWebClientProtocol) this).Discover(this.Url).References)
        {
          System.Web.Services.Discovery.SoapBinding soapBinding = obj as System.Web.Services.Discovery.SoapBinding;
          if (soapBinding != null && this.clientType.Binding.Name == soapBinding.Binding.Name && this.clientType.Binding.Namespace == soapBinding.Binding.Namespace)
          {
            this.Url = soapBinding.Address;
            return;
          }
        }
        throw new InvalidOperationException(System.Web.Services.Res.GetString("TheBindingNamedFromNamespaceWasNotFoundIn3", (object) this.clientType.Binding.Name, (object) this.clientType.Binding.Namespace, (object) this.Url));
      }
    }

    protected override WebRequest GetWebRequest(Uri uri)
    {
      return base.GetWebRequest(uri);
    }

    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    protected virtual XmlWriter GetWriterForMessage(SoapClientMessage message, int bufferSize)
    {
      if (bufferSize < 512)
        bufferSize = 512;
      return (XmlWriter) new XmlTextWriter((TextWriter) new StreamWriter(message.Stream, this.RequestEncoding != null ? this.RequestEncoding : (Encoding) new UTF8Encoding(false), bufferSize));
    }

    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    protected virtual XmlReader GetReaderForMessage(SoapClientMessage message, int bufferSize)
    {
      Encoding encoding = message.SoapVersion == SoapProtocolVersion.Soap12 ? RequestResponseUtils.GetEncoding2(message.ContentType) : RequestResponseUtils.GetEncoding(message.ContentType);
      if (bufferSize < 512)
        bufferSize = 512;
      XmlTextReader xmlTextReader = encoding == null ? new XmlTextReader(message.Stream) : new XmlTextReader((TextReader) new StreamReader(message.Stream, encoding, true, bufferSize));
      xmlTextReader.DtdProcessing = DtdProcessing.Prohibit;
      xmlTextReader.Normalization = true;
      xmlTextReader.XmlResolver = (XmlResolver) null;
      return (XmlReader) xmlTextReader;
    }

    protected object[] Invoke(string methodName, object[] parameters)
    {
      WebRequest request = (WebRequest) null;
      try
      {
        request = this.GetWebRequest(this.Uri);
        this.NotifyClientCallOut(request);
        this.PendingSyncRequest = request;
        SoapClientMessage message = this.BeforeSerialize(request, methodName, parameters);
        Stream requestStream = request.GetRequestStream();
        try
        {
          message.SetStream(requestStream);
          this.Serialize(message);
        }
        finally
        {
          requestStream.Close();
        }
        WebResponse webResponse = this.GetWebResponse(request);
        Stream responseStream = (Stream) null;
        try
        {
          responseStream = webResponse.GetResponseStream();
          return this.ReadResponse(message, webResponse, responseStream, false);
        }
        catch (XmlException ex)
        {
          throw new InvalidOperationException(System.Web.Services.Res.GetString("WebResponseBadXml"), (Exception) ex);
        }
        finally
        {
          if (responseStream != null)
            responseStream.Close();
        }
      }
      finally
      {
        if (request == this.PendingSyncRequest)
          this.PendingSyncRequest = (WebRequest) null;
      }
    }

    protected IAsyncResult BeginInvoke(string methodName, object[] parameters, AsyncCallback callback, object asyncState)
    {
      return this.BeginSend(this.Uri, new WebClientAsyncResult((WebClientProtocol) this, (object) new SoapHttpClientProtocol.InvokeAsyncState(methodName, parameters), (WebRequest) null, callback, asyncState), true);
    }

    internal override void InitializeAsyncRequest(WebRequest request, object internalAsyncState)
    {
      SoapHttpClientProtocol.InvokeAsyncState invokeAsyncState = (SoapHttpClientProtocol.InvokeAsyncState) internalAsyncState;
      invokeAsyncState.Message = this.BeforeSerialize(request, invokeAsyncState.MethodName, invokeAsyncState.Parameters);
    }

    internal override void AsyncBufferedSerialize(WebRequest request, Stream requestStream, object internalAsyncState)
    {
      SoapHttpClientProtocol.InvokeAsyncState invokeAsyncState = (SoapHttpClientProtocol.InvokeAsyncState) internalAsyncState;
      invokeAsyncState.Message.SetStream(requestStream);
      this.Serialize(invokeAsyncState.Message);
    }

    protected object[] EndInvoke(IAsyncResult asyncResult)
    {
      object internalAsyncState = (object) null;
      Stream responseStream = (Stream) null;
      try
      {
        WebResponse response = this.EndSend(asyncResult, ref internalAsyncState, ref responseStream);
        return this.ReadResponse(((SoapHttpClientProtocol.InvokeAsyncState) internalAsyncState).Message, response, responseStream, true);
      }
      catch (XmlException ex)
      {
        throw new InvalidOperationException(System.Web.Services.Res.GetString("WebResponseBadXml"), (Exception) ex);
      }
      finally
      {
        if (responseStream != null)
          responseStream.Close();
      }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    protected void InvokeAsync(string methodName, object[] parameters, SendOrPostCallback callback)
    {
      this.InvokeAsync(methodName, parameters, callback, (object) null);
    }

    protected void InvokeAsync(string methodName, object[] parameters, SendOrPostCallback callback, object userState)
    {
      if (userState == null)
        userState = this.NullToken;
      SoapHttpClientProtocol.InvokeAsyncState invokeAsyncState = new SoapHttpClientProtocol.InvokeAsyncState(methodName, parameters);
      AsyncOperation operation = AsyncOperationManager.CreateOperation((object) new UserToken(callback, userState));
      WebClientAsyncResult asyncResult = new WebClientAsyncResult((WebClientProtocol) this, (object) invokeAsyncState, (WebRequest) null, new AsyncCallback(this.InvokeAsyncCallback), (object) operation);
      try
      {
        this.AsyncInvokes.Add(userState, (object) asyncResult);
      }
      catch (Exception ex)
      {
        if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
        {
          throw;
        }
        else
        {
          if (Tracing.On)
            Tracing.ExceptionCatch(TraceEventType.Warning, (object) this, "InvokeAsync", ex);
          InvokeCompletedEventArgs completedEventArgs = new InvokeCompletedEventArgs(new object[1], (Exception) new ArgumentException(System.Web.Services.Res.GetString("AsyncDuplicateUserState"), ex), false, userState);
          operation.PostOperationCompleted(callback, (object) completedEventArgs);
          return;
        }
      }
      try
      {
        this.BeginSend(this.Uri, asyncResult, true);
      }
      catch (Exception ex)
      {
        if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
        {
          throw;
        }
        else
        {
          if (Tracing.On)
            Tracing.ExceptionCatch(TraceEventType.Warning, (object) this, "InvokeAsync", ex);
          this.OperationCompleted(userState, new object[1], ex, false);
        }
      }
    }

    private void InvokeAsyncCallback(IAsyncResult result)
    {
      object[] parameters = (object[]) null;
      Exception e = (Exception) null;
      WebClientAsyncResult clientAsyncResult = (WebClientAsyncResult) result;
      if (clientAsyncResult.Request != null)
      {
        object internalAsyncState = (object) null;
        Stream responseStream = (Stream) null;
        try
        {
          WebResponse response = this.EndSend((IAsyncResult) clientAsyncResult, ref internalAsyncState, ref responseStream);
          parameters = this.ReadResponse(((SoapHttpClientProtocol.InvokeAsyncState) internalAsyncState).Message, response, responseStream, true);
        }
        catch (XmlException ex)
        {
          if (Tracing.On)
            Tracing.ExceptionCatch(TraceEventType.Warning, (object) this, "InvokeAsyncCallback", (Exception) ex);
          e = (Exception) new InvalidOperationException(System.Web.Services.Res.GetString("WebResponseBadXml"), (Exception) ex);
        }
        catch (Exception ex)
        {
          if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
          {
            throw;
          }
          else
          {
            if (Tracing.On)
              Tracing.ExceptionCatch(TraceEventType.Warning, (object) this, "InvokeAsyncCallback", ex);
            e = ex;
          }
        }
        finally
        {
          if (responseStream != null)
            responseStream.Close();
        }
      }
      this.OperationCompleted(((UserToken) ((AsyncOperation) result.AsyncState).UserSuppliedState).UserState, parameters, e, false);
    }

    private static Array CombineExtensionsHelper(Array array1, Array array2, Array array3, Type elementType)
    {
      int length = array1.Length + array2.Length + array3.Length;
      if (length == 0)
        return (Array) null;
      Array destinationArray;
      if (elementType == typeof (SoapReflectedExtension))
      {
        destinationArray = (Array) new SoapReflectedExtension[length];
      }
      else
      {
        if (!(elementType == typeof (object)))
          throw new ArgumentException(System.Web.Services.Res.GetString("ElementTypeMustBeObjectOrSoapReflectedException"), "elementType");
        destinationArray = (Array) new object[length];
      }
      int destinationIndex1 = 0;
      Array.Copy(array1, 0, destinationArray, destinationIndex1, array1.Length);
      int destinationIndex2 = destinationIndex1 + array1.Length;
      Array.Copy(array2, 0, destinationArray, destinationIndex2, array2.Length);
      int destinationIndex3 = destinationIndex2 + array2.Length;
      Array.Copy(array3, 0, destinationArray, destinationIndex3, array3.Length);
      return destinationArray;
    }

    private SoapClientMessage BeforeSerialize(WebRequest request, string methodName, object[] parameters)
    {
      if (parameters == null)
        throw new ArgumentNullException("parameters");
      SoapClientMethod method = this.clientType.GetMethod(methodName);
      if (method == null)
      {
        throw new ArgumentException(System.Web.Services.Res.GetString("WebInvalidMethodName", new object[1]
        {
          (object) methodName
        }));
      }
      else
      {
        SoapExtension[] soapExtensionArray = SoapMessage.InitializeExtensions((SoapReflectedExtension[]) SoapHttpClientProtocol.CombineExtensionsHelper((Array) this.clientType.HighPriExtensions, (Array) method.extensions, (Array) this.clientType.LowPriExtensions, typeof (SoapReflectedExtension)), (object[]) SoapHttpClientProtocol.CombineExtensionsHelper((Array) this.clientType.HighPriExtensionInitializers, (Array) method.extensionInitializers, (Array) this.clientType.LowPriExtensionInitializers, typeof (object)));
        SoapClientMessage soapClientMessage = new SoapClientMessage(this, method, this.Url);
        soapClientMessage.initializedExtensions = soapExtensionArray;
        if (soapExtensionArray != null)
          soapClientMessage.SetExtensionStream(new SoapExtensionStream());
        soapClientMessage.InitExtensionStreamChain(soapClientMessage.initializedExtensions);
        string action = UrlEncoder.EscapeString(method.action, Encoding.UTF8);
        soapClientMessage.SetStage(SoapMessageStage.BeforeSerialize);
        if (this.version == SoapProtocolVersion.Soap12)
          soapClientMessage.ContentType = ContentType.Compose("application/soap+xml", this.RequestEncoding != null ? this.RequestEncoding : Encoding.UTF8, action);
        else
          soapClientMessage.ContentType = ContentType.Compose("text/xml", this.RequestEncoding != null ? this.RequestEncoding : Encoding.UTF8);
        soapClientMessage.SetParameterValues(parameters);
        SoapHeaderHandling.GetHeaderMembers(soapClientMessage.Headers, (object) this, method.inHeaderMappings, SoapHeaderDirection.In, true);
        soapClientMessage.RunExtensions(soapClientMessage.initializedExtensions, true);
        request.ContentType = soapClientMessage.ContentType;
        if (soapClientMessage.ContentEncoding != null && soapClientMessage.ContentEncoding.Length > 0)
          ((NameValueCollection) request.Headers)["Content-Encoding"] = soapClientMessage.ContentEncoding;
        request.Method = "POST";
        if (this.version != SoapProtocolVersion.Soap12 && ((NameValueCollection) request.Headers)["SOAPAction"] == null)
        {
          StringBuilder stringBuilder = new StringBuilder(action.Length + 2);
          stringBuilder.Append('"');
          stringBuilder.Append(action);
          stringBuilder.Append('"');
          ((NameValueCollection) request.Headers).Add("SOAPAction", ((object) stringBuilder).ToString());
        }
        return soapClientMessage;
      }
    }

    private void Serialize(SoapClientMessage message)
    {
      Stream stream = message.Stream;
      SoapClientMethod method = message.Method;
      bool isEncoded = method.use == SoapBindingUse.Encoded;
      string envelopeNs = this.EnvelopeNs;
      string encodingNs = this.EncodingNs;
      XmlWriter writerForMessage = this.GetWriterForMessage(message, 1024);
      if (writerForMessage == null)
        throw new InvalidOperationException(System.Web.Services.Res.GetString("WebNullWriterForMessage"));
      writerForMessage.WriteStartDocument();
      writerForMessage.WriteStartElement("soap", "Envelope", envelopeNs);
      writerForMessage.WriteAttributeString("xmlns", "soap", (string) null, envelopeNs);
      if (isEncoded)
      {
        writerForMessage.WriteAttributeString("xmlns", "soapenc", (string) null, encodingNs);
        writerForMessage.WriteAttributeString("xmlns", "tns", (string) null, this.clientType.serviceNamespace);
        writerForMessage.WriteAttributeString("xmlns", "types", (string) null, SoapReflector.GetEncodedNamespace(this.clientType.serviceNamespace, this.clientType.serviceDefaultIsEncoded));
      }
      writerForMessage.WriteAttributeString("xmlns", "xsi", (string) null, "http://www.w3.org/2001/XMLSchema-instance");
      writerForMessage.WriteAttributeString("xmlns", "xsd", (string) null, "http://www.w3.org/2001/XMLSchema");
      SoapHeaderHandling.WriteHeaders(writerForMessage, method.inHeaderSerializer, message.Headers, method.inHeaderMappings, SoapHeaderDirection.In, isEncoded, this.clientType.serviceNamespace, this.clientType.serviceDefaultIsEncoded, envelopeNs);
      writerForMessage.WriteStartElement("Body", envelopeNs);
      if (isEncoded && this.version != SoapProtocolVersion.Soap12)
        writerForMessage.WriteAttributeString("soap", "encodingStyle", (string) null, encodingNs);
      object[] parameterValues = message.GetParameterValues();
      TraceMethod caller = Tracing.On ? new TraceMethod((object) this, "Serialize", new object[0]) : (TraceMethod) null;
      if (Tracing.On)
        Tracing.Enter(Tracing.TraceId("TraceWriteRequest"), caller, new TraceMethod((object) method.parameterSerializer, "Serialize", new object[4]
        {
          (object) writerForMessage,
          (object) parameterValues,
          null,
          isEncoded ? (object) encodingNs : (object) (string) null
        }));
      method.parameterSerializer.Serialize(writerForMessage, (object) parameterValues, (XmlSerializerNamespaces) null, isEncoded ? encodingNs : (string) null);
      if (Tracing.On)
        Tracing.Exit(Tracing.TraceId("TraceWriteRequest"), caller);
      writerForMessage.WriteEndElement();
      writerForMessage.WriteEndElement();
      writerForMessage.Flush();
      message.SetStage(SoapMessageStage.AfterSerialize);
      message.RunExtensions(message.initializedExtensions, true);
    }

    private object[] ReadResponse(SoapClientMessage message, WebResponse response, Stream responseStream, bool asyncCall)
    {
      SoapClientMethod method = message.Method;
      HttpWebResponse httpWebResponse = response as HttpWebResponse;
      int num1 = httpWebResponse != null ? (int) httpWebResponse.StatusCode : -1;
      if (num1 >= 300 && num1 != 500 && num1 != 400)
        throw new WebException(RequestResponseUtils.CreateResponseExceptionString((WebResponse) httpWebResponse, responseStream), (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) httpWebResponse);
      message.Headers.Clear();
      message.SetStream(responseStream);
      message.InitExtensionStreamChain(message.initializedExtensions);
      message.SetStage(SoapMessageStage.BeforeDeserialize);
      message.ContentType = response.ContentType;
      message.ContentEncoding = ((NameValueCollection) response.Headers)["Content-Encoding"];
      message.RunExtensions(message.initializedExtensions, false);
      if (method.oneWay && (httpWebResponse == null || httpWebResponse.StatusCode != HttpStatusCode.InternalServerError))
        return new object[0];
      bool flag1 = ContentType.IsSoap(message.ContentType);
      if (!flag1 || flag1 && httpWebResponse != null && httpWebResponse.ContentLength == 0L)
      {
        if (num1 == 400)
          throw new WebException(RequestResponseUtils.CreateResponseExceptionString((WebResponse) httpWebResponse, responseStream), (Exception) null, WebExceptionStatus.ProtocolError, (WebResponse) httpWebResponse);
        throw new InvalidOperationException(System.Web.Services.Res.GetString("WebResponseContent", (object) message.ContentType, (object) this.HttpContentType) + Environment.NewLine + RequestResponseUtils.CreateResponseExceptionString(response, responseStream));
      }
      else
      {
        if (message.Exception != null)
          throw message.Exception;
        int bufferSize = asyncCall || httpWebResponse == null ? 512 : RequestResponseUtils.GetBufferSize((int) httpWebResponse.ContentLength);
        XmlReader readerForMessage = this.GetReaderForMessage(message, bufferSize);
        if (readerForMessage == null)
          throw new InvalidOperationException(System.Web.Services.Res.GetString("WebNullReaderForMessage"));
        int num2 = (int) readerForMessage.MoveToContent();
        int depth = readerForMessage.Depth;
        string encodingNs = this.EncodingNs;
        string namespaceUri = readerForMessage.NamespaceURI;
        if (namespaceUri == null || namespaceUri.Length == 0)
          readerForMessage.ReadStartElement("Envelope");
        else if (readerForMessage.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/")
          readerForMessage.ReadStartElement("Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
        else if (readerForMessage.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope")
          readerForMessage.ReadStartElement("Envelope", "http://www.w3.org/2003/05/soap-envelope");
        else
          throw new SoapException(System.Web.Services.Res.GetString("WebInvalidEnvelopeNamespace", (object) namespaceUri, (object) this.EnvelopeNs), SoapException.VersionMismatchFaultCode);
        int num3 = (int) readerForMessage.MoveToContent();
        new SoapHeaderHandling().ReadHeaders(readerForMessage, method.outHeaderSerializer, message.Headers, method.outHeaderMappings, SoapHeaderDirection.Out | SoapHeaderDirection.Fault, namespaceUri, method.use == SoapBindingUse.Encoded ? encodingNs : (string) null, false);
        int num4 = (int) readerForMessage.MoveToContent();
        readerForMessage.ReadStartElement("Body", namespaceUri);
        int num5 = (int) readerForMessage.MoveToContent();
        if (readerForMessage.IsStartElement("Fault", namespaceUri))
          message.Exception = this.ReadSoapException(readerForMessage);
        else if (method.oneWay)
        {
          readerForMessage.Skip();
          message.SetParameterValues(new object[0]);
        }
        else
        {
          TraceMethod caller = Tracing.On ? new TraceMethod((object) this, "ReadResponse", new object[0]) : (TraceMethod) null;
          bool flag2 = method.use == SoapBindingUse.Encoded;
          if (Tracing.On)
            Tracing.Enter(Tracing.TraceId("TraceReadResponse"), caller, new TraceMethod((object) method.returnSerializer, "Deserialize", new object[2]
            {
              (object) readerForMessage,
              flag2 ? (object) encodingNs : (object) (string) null
            }));
          if (!flag2 && (WebServicesSection.Current.SoapEnvelopeProcessing.IsStrict || Tracing.On))
          {
            XmlDeserializationEvents events = Tracing.On ? Tracing.GetDeserializationEvents() : RuntimeUtils.GetDeserializationEvents();
            message.SetParameterValues((object[]) method.returnSerializer.Deserialize(readerForMessage, (string) null, events));
          }
          else
            message.SetParameterValues((object[]) method.returnSerializer.Deserialize(readerForMessage, flag2 ? encodingNs : (string) null));
          if (Tracing.On)
            Tracing.Exit(Tracing.TraceId("TraceReadResponse"), caller);
        }
        do
          ;
        while (depth < readerForMessage.Depth && readerForMessage.Read());
        if (readerForMessage.NodeType == XmlNodeType.EndElement)
          readerForMessage.Read();
        message.SetStage(SoapMessageStage.AfterDeserialize);
        message.RunExtensions(message.initializedExtensions, false);
        SoapHeaderHandling.SetHeaderMembers(message.Headers, (object) this, method.outHeaderMappings, SoapHeaderDirection.Out | SoapHeaderDirection.Fault, true);
        if (message.Exception != null)
          throw message.Exception;
        else
          return message.GetParameterValues();
      }
    }

    private SoapException ReadSoapException(XmlReader reader)
    {
      XmlQualifiedName code = XmlQualifiedName.Empty;
      string message = (string) null;
      string actor = (string) null;
      string role = (string) null;
      XmlNode detail = (XmlNode) null;
      SoapFaultSubCode subcode = (SoapFaultSubCode) null;
      string lang = (string) null;
      bool flag = reader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope";
      if (reader.IsEmptyElement)
      {
        reader.Skip();
      }
      else
      {
        reader.ReadStartElement();
        int num1 = (int) reader.MoveToContent();
        int depth = reader.Depth;
        while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
        {
          if (reader.NamespaceURI == "http://schemas.xmlsoap.org/soap/envelope/" || reader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope" || (reader.NamespaceURI == null || reader.NamespaceURI.Length == 0))
          {
            if (reader.LocalName == "faultcode" || reader.LocalName == "Code")
              code = !flag ? this.ReadFaultCode(reader) : this.ReadSoap12FaultCode(reader, out subcode);
            else if (reader.LocalName == "faultstring")
            {
              lang = reader.GetAttribute("lang", "http://www.w3.org/XML/1998/namespace");
              reader.MoveToElement();
              message = reader.ReadElementString();
            }
            else if (reader.LocalName == "Reason")
            {
              if (reader.IsEmptyElement)
              {
                reader.Skip();
              }
              else
              {
                reader.ReadStartElement();
                int num2 = (int) reader.MoveToContent();
                while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
                {
                  if (reader.LocalName == "Text" && reader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope")
                    message = reader.ReadElementString();
                  else
                    reader.Skip();
                  int num3 = (int) reader.MoveToContent();
                }
                while (reader.NodeType == XmlNodeType.Whitespace)
                  reader.Skip();
                if (reader.NodeType == XmlNodeType.None)
                  reader.Skip();
                else
                  reader.ReadEndElement();
              }
            }
            else if (reader.LocalName == "faultactor" || reader.LocalName == "Node")
              actor = reader.ReadElementString();
            else if (reader.LocalName == "detail" || reader.LocalName == "Detail")
              detail = new XmlDocument().ReadNode(reader);
            else if (reader.LocalName == "Role")
              role = reader.ReadElementString();
            else
              reader.Skip();
          }
          else
            reader.Skip();
          int num4 = (int) reader.MoveToContent();
        }
        do
          ;
        while (reader.Read() && depth < reader.Depth);
        if (reader.NodeType == XmlNodeType.EndElement)
          reader.Read();
      }
      if (detail != null || flag)
        return new SoapException(message, code, actor, role, lang, detail, subcode, (Exception) null);
      else
        return (SoapException) new SoapHeaderException(message, code, actor, role, lang, subcode, (Exception) null);
    }

    private XmlQualifiedName ReadSoap12FaultCode(XmlReader reader, out SoapFaultSubCode subcode)
    {
      SoapFaultSubCode soapFaultSubCode = this.ReadSoap12FaultCodesRecursive(reader, 0);
      if (soapFaultSubCode == null)
      {
        subcode = (SoapFaultSubCode) null;
        return (XmlQualifiedName) null;
      }
      else
      {
        subcode = soapFaultSubCode.SubCode;
        return soapFaultSubCode.Code;
      }
    }

    private SoapFaultSubCode ReadSoap12FaultCodesRecursive(XmlReader reader, int depth)
    {
      if (depth > 100)
        return (SoapFaultSubCode) null;
      if (reader.IsEmptyElement)
      {
        reader.Skip();
        return (SoapFaultSubCode) null;
      }
      else
      {
        XmlQualifiedName code = (XmlQualifiedName) null;
        SoapFaultSubCode subCode = (SoapFaultSubCode) null;
        int depth1 = reader.Depth;
        reader.ReadStartElement();
        int num1 = (int) reader.MoveToContent();
        while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType != XmlNodeType.None)
        {
          if (reader.NamespaceURI == "http://www.w3.org/2003/05/soap-envelope" || reader.NamespaceURI == null || reader.NamespaceURI.Length == 0)
          {
            if (reader.LocalName == "Value")
              code = this.ReadFaultCode(reader);
            else if (reader.LocalName == "Subcode")
              subCode = this.ReadSoap12FaultCodesRecursive(reader, depth + 1);
            else
              reader.Skip();
          }
          else
            reader.Skip();
          int num2 = (int) reader.MoveToContent();
        }
        do
          ;
        while (depth1 < reader.Depth && reader.Read());
        if (reader.NodeType == XmlNodeType.EndElement)
          reader.Read();
        return new SoapFaultSubCode(code, subCode);
      }
    }

    private XmlQualifiedName ReadFaultCode(XmlReader reader)
    {
      if (reader.IsEmptyElement)
      {
        reader.Skip();
        return (XmlQualifiedName) null;
      }
      else
      {
        reader.ReadStartElement();
        string str = reader.ReadString();
        int length = str.IndexOf(":", StringComparison.Ordinal);
        string ns = reader.NamespaceURI;
        if (length >= 0)
        {
          string prefix = str.Substring(0, length);
          ns = reader.LookupNamespace(prefix);
          if (ns == null)
            throw new InvalidOperationException(System.Web.Services.Res.GetString("WebQNamePrefixUndefined", new object[1]
            {
              (object) prefix
            }));
        }
        reader.ReadEndElement();
        return new XmlQualifiedName(str.Substring(length + 1), ns);
      }
    }

    private class InvokeAsyncState
    {
      public string MethodName;
      public object[] Parameters;
      public SoapClientMessage Message;

      public InvokeAsyncState(string methodName, object[] parameters)
      {
        this.MethodName = methodName;
        this.Parameters = parameters;
      }
    }
  }
}
