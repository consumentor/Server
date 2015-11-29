// Type: System.ServiceModel.Web.WebGetAttribute
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.ServiceModel.Web.dll

using System;
using System.ServiceModel;
using System.ServiceModel.Administration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Web
{
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class WebGetAttribute : Attribute, IOperationContractAttributeProvider, IOperationBehavior, IWmiInstanceProvider
  {
    private WebMessageBodyStyle bodyStyle;
    private bool isBodyStyleDefined;
    private bool isRequestMessageFormatSet;
    private bool isResponseMessageFormatSet;
    private WebMessageFormat requestMessageFormat;
    private WebMessageFormat responseMessageFormat;
    private string uriTemplate;

    public WebMessageBodyStyle BodyStyle
    {
      get
      {
        return this.bodyStyle;
      }
      set
      {
        if (!WebMessageBodyStyleHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.bodyStyle = value;
        this.isBodyStyleDefined = true;
      }
    }

    public bool IsBodyStyleSetExplicitly
    {
      get
      {
        return this.isBodyStyleDefined;
      }
    }

    public bool IsRequestFormatSetExplicitly
    {
      get
      {
        return this.isRequestMessageFormatSet;
      }
    }

    public bool IsResponseFormatSetExplicitly
    {
      get
      {
        return this.isResponseMessageFormatSet;
      }
    }

    public WebMessageFormat RequestFormat
    {
      get
      {
        return this.requestMessageFormat;
      }
      set
      {
        if (!WebMessageFormatHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.requestMessageFormat = value;
        this.isRequestMessageFormatSet = true;
      }
    }

    public WebMessageFormat ResponseFormat
    {
      get
      {
        return this.responseMessageFormat;
      }
      set
      {
        if (!WebMessageFormatHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.responseMessageFormat = value;
        this.isResponseMessageFormatSet = true;
      }
    }

    public string UriTemplate
    {
      get
      {
        return this.uriTemplate;
      }
      set
      {
        this.uriTemplate = value;
      }
    }

    void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    {
    }

    void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
    }

    void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
    }

    void IOperationBehavior.Validate(OperationDescription operationDescription)
    {
    }

    void IWmiInstanceProvider.FillInstance(IWmiInstance wmiInstance)
    {
      if (wmiInstance == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("wmiInstance");
      wmiInstance.SetProperty("BodyStyle", (object) ((object) this.BodyStyle).ToString());
      wmiInstance.SetProperty("IsBodyStyleSetExplicitly", (object) this.IsBodyStyleSetExplicitly.ToString());
      wmiInstance.SetProperty("RequestFormat", (object) ((object) this.RequestFormat).ToString());
      wmiInstance.SetProperty("IsRequestFormatSetExplicitly", (object) this.IsRequestFormatSetExplicitly.ToString());
      wmiInstance.SetProperty("ResponseFormat", (object) ((object) this.ResponseFormat).ToString());
      wmiInstance.SetProperty("IsResponseFormatSetExplicitly", (object) this.IsResponseFormatSetExplicitly.ToString());
      wmiInstance.SetProperty("UriTemplate", (object) this.UriTemplate);
    }

    string IWmiInstanceProvider.GetInstanceType()
    {
      return "WebGetAttribute";
    }

    internal WebMessageBodyStyle GetBodyStyleOrDefault(WebMessageBodyStyle defaultStyle)
    {
      if (this.IsBodyStyleSetExplicitly)
        return this.BodyStyle;
      else
        return defaultStyle;
    }

    OperationContractAttribute IOperationContractAttributeProvider.GetOperationContractAttribute()
    {
      return new OperationContractAttribute();
    }
  }
}
