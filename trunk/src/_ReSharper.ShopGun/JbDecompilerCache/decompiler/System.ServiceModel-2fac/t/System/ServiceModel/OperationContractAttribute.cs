// Type: System.ServiceModel.OperationContractAttribute
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.ServiceModel.dll

using System;
using System.Net.Security;
using System.ServiceModel.Security;

namespace System.ServiceModel
{
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class OperationContractAttribute : Attribute
  {
    private bool isInitiating = true;
    internal const string ActionPropertyName = "Action";
    internal const string ProtectionLevelPropertyName = "ProtectionLevel";
    internal const string ReplyActionPropertyName = "ReplyAction";
    private string name;
    private string action;
    private string replyAction;
    private bool asyncPattern;
    private bool isTerminating;
    private bool isOneWay;
    private ProtectionLevel protectionLevel;
    private bool hasProtectionLevel;

    public string Name
    {
      get
      {
        return this.name;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        if (value == "")
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", System.ServiceModel.SR.GetString("SFxNameCannotBeEmpty")));
        this.name = value;
      }
    }

    public string Action
    {
      get
      {
        return this.action;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        this.action = value;
      }
    }

    public ProtectionLevel ProtectionLevel
    {
      get
      {
        return this.protectionLevel;
      }
      set
      {
        if (!ProtectionLevelHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.protectionLevel = value;
        this.hasProtectionLevel = true;
      }
    }

    public bool HasProtectionLevel
    {
      get
      {
        return this.hasProtectionLevel;
      }
    }

    public string ReplyAction
    {
      get
      {
        return this.replyAction;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        this.replyAction = value;
      }
    }

    public bool AsyncPattern
    {
      get
      {
        return this.asyncPattern;
      }
      set
      {
        this.asyncPattern = value;
      }
    }

    public bool IsOneWay
    {
      get
      {
        return this.isOneWay;
      }
      set
      {
        this.isOneWay = value;
      }
    }

    public bool IsInitiating
    {
      get
      {
        return this.isInitiating;
      }
      set
      {
        this.isInitiating = value;
      }
    }

    public bool IsTerminating
    {
      get
      {
        return this.isTerminating;
      }
      set
      {
        this.isTerminating = value;
      }
    }
  }
}
