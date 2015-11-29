// Type: System.Web.Services.Protocols.SoapHeaderAttribute
// Assembly: System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Web.Services.dll

using System;
using System.Runtime;

namespace System.Web.Services.Protocols
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public sealed class SoapHeaderAttribute : Attribute
  {
    private SoapHeaderDirection direction = SoapHeaderDirection.In;
    private bool required = true;
    private string memberName;

    public string MemberName
    {
      get
      {
        if (this.memberName != null)
          return this.memberName;
        else
          return string.Empty;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.memberName = value;
      }
    }

    public SoapHeaderDirection Direction
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.direction;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.direction = value;
      }
    }

    [Obsolete("This property will be removed from a future version. The presence of a particular header in a SOAP message is no longer enforced", false)]
    public bool Required
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.required;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.required = value;
      }
    }

    public SoapHeaderAttribute(string memberName)
    {
      this.memberName = memberName;
    }
  }
}
