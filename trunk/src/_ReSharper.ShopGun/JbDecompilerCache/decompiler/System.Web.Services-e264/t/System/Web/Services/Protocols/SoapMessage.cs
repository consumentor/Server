// Type: System.Web.Services.Protocols.SoapMessage
// Assembly: System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Web.Services.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Web.Services;
using System.Web.Services.Diagnostics;

namespace System.Web.Services.Protocols
{
  [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
  public abstract class SoapMessage
  {
    private SoapHeaderCollection headers = new SoapHeaderCollection();
    private SoapMessageStage stage;
    private Stream stream;
    private SoapExtensionStream extensionStream;
    private string contentType;
    private string contentEncoding;
    private object[] parameterValues;
    private SoapException exception;

    public abstract bool OneWay { get; }

    public SoapException Exception
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.exception;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.exception = value;
      }
    }

    public abstract LogicalMethodInfo MethodInfo { get; }

    public SoapHeaderCollection Headers
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.headers;
      }
    }

    public Stream Stream
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.stream;
      }
    }

    public string ContentType
    {
      get
      {
        this.EnsureStage((SoapMessageStage) 5);
        return this.contentType;
      }
      set
      {
        this.EnsureStage((SoapMessageStage) 5);
        this.contentType = value;
      }
    }

    public string ContentEncoding
    {
      get
      {
        this.EnsureStage((SoapMessageStage) 5);
        return this.contentEncoding;
      }
      set
      {
        this.EnsureStage((SoapMessageStage) 5);
        this.contentEncoding = value;
      }
    }

    public SoapMessageStage Stage
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.stage;
      }
    }

    public abstract string Url { get; }

    public abstract string Action { get; }

    [ComVisible(false)]
    [DefaultValue(SoapProtocolVersion.Default)]
    public virtual SoapProtocolVersion SoapVersion
    {
      get
      {
        return SoapProtocolVersion.Default;
      }
    }

    internal SoapMessage()
    {
    }

    public object GetInParameterValue(int index)
    {
      this.EnsureInStage();
      this.EnsureNoException();
      if (index >= 0 && index < this.parameterValues.Length)
        return this.parameterValues[index];
      throw new IndexOutOfRangeException(Res.GetString("indexMustBeBetweenAnd0Inclusive", new object[1]
      {
        (object) this.parameterValues.Length
      }));
    }

    public object GetOutParameterValue(int index)
    {
      this.EnsureOutStage();
      this.EnsureNoException();
      if (!this.MethodInfo.IsVoid)
      {
        if (index == int.MaxValue)
          throw new IndexOutOfRangeException(Res.GetString("indexMustBeBetweenAnd0Inclusive", new object[1]
          {
            (object) this.parameterValues.Length
          }));
        else
          ++index;
      }
      if (index >= 0 && index < this.parameterValues.Length)
        return this.parameterValues[index];
      throw new IndexOutOfRangeException(Res.GetString("indexMustBeBetweenAnd0Inclusive", new object[1]
      {
        (object) this.parameterValues.Length
      }));
    }

    public object GetReturnValue()
    {
      this.EnsureOutStage();
      this.EnsureNoException();
      if (this.MethodInfo.IsVoid)
        throw new InvalidOperationException(Res.GetString("WebNoReturnValue"));
      else
        return this.parameterValues[0];
    }

    protected abstract void EnsureOutStage();

    protected abstract void EnsureInStage();

    protected void EnsureStage(SoapMessageStage stage)
    {
      if ((this.stage & stage) != (SoapMessageStage) 0)
        return;
      throw new InvalidOperationException(Res.GetString("WebCannotAccessValueStage", new object[1]
      {
        (object) ((object) this.stage).ToString()
      }));
    }

    internal void SetParameterValues(object[] parameterValues)
    {
      this.parameterValues = parameterValues;
    }

    internal object[] GetParameterValues()
    {
      return this.parameterValues;
    }

    private void EnsureNoException()
    {
      if (this.exception != null)
        throw new InvalidOperationException(Res.GetString("WebCannotAccessValue"), (Exception) this.exception);
    }

    internal void SetStream(Stream stream)
    {
      if (this.extensionStream != null)
      {
        this.extensionStream.SetInnerStream(stream);
        this.extensionStream.SetStreamReady();
        this.extensionStream = (SoapExtensionStream) null;
      }
      else
        this.stream = stream;
    }

    internal void SetExtensionStream(SoapExtensionStream extensionStream)
    {
      this.extensionStream = extensionStream;
      this.stream = (Stream) extensionStream;
    }

    internal void SetStage(SoapMessageStage stage)
    {
      this.stage = stage;
    }

    internal static SoapExtension[] InitializeExtensions(SoapReflectedExtension[] reflectedExtensions, object[] extensionInitializers)
    {
      if (reflectedExtensions == null)
        return (SoapExtension[]) null;
      SoapExtension[] soapExtensionArray = new SoapExtension[reflectedExtensions.Length];
      for (int index = 0; index < soapExtensionArray.Length; ++index)
        soapExtensionArray[index] = reflectedExtensions[index].CreateInstance(extensionInitializers[index]);
      return soapExtensionArray;
    }

    internal void InitExtensionStreamChain(SoapExtension[] extensions)
    {
      if (extensions == null)
        return;
      for (int index = 0; index < extensions.Length; ++index)
        this.stream = extensions[index].ChainStream(this.stream);
    }

    internal void RunExtensions(SoapExtension[] extensions, bool throwOnException)
    {
      if (extensions == null)
        return;
      TraceMethod traceMethod1;
      if (!Tracing.On)
        traceMethod1 = (TraceMethod) null;
      else
        traceMethod1 = new TraceMethod((object) this, "RunExtensions", new object[2]
        {
          (object) extensions,
          (object) (bool) (throwOnException ? 1 : 0)
        });
      TraceMethod traceMethod2 = traceMethod1;
      if ((this.stage & (SoapMessageStage) 12) != (SoapMessageStage) 0)
      {
        for (int index = 0; index < extensions.Length; ++index)
        {
          if (Tracing.On)
            Tracing.Enter("SoapExtension", traceMethod2, new TraceMethod((object) extensions[index], "ProcessMessage", new object[1]
            {
              (object) this.stage
            }));
          extensions[index].ProcessMessage(this);
          if (Tracing.On)
            Tracing.Exit("SoapExtension", traceMethod2);
          if (this.Exception != null)
          {
            if (throwOnException)
              throw this.Exception;
            if (Tracing.On)
              Tracing.ExceptionIgnore(TraceEventType.Warning, traceMethod2, (Exception) this.Exception);
          }
        }
      }
      else
      {
        for (int index = extensions.Length - 1; index >= 0; --index)
        {
          if (Tracing.On)
            Tracing.Enter("SoapExtension", traceMethod2, new TraceMethod((object) extensions[index], "ProcessMessage", new object[1]
            {
              (object) this.stage
            }));
          extensions[index].ProcessMessage(this);
          if (Tracing.On)
            Tracing.Exit("SoapExtension", traceMethod2);
          if (this.Exception != null)
          {
            if (throwOnException)
              throw this.Exception;
            if (Tracing.On)
              Tracing.ExceptionIgnore(TraceEventType.Warning, traceMethod2, (Exception) this.Exception);
          }
        }
      }
    }
  }
}
