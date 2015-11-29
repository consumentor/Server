// Type: Castle.Core.InterceptorAttribute
// Assembly: Castle.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc
// Assembly location: C:\Users\Simon\Consumentor\SVN\Shopgun\trunk\src\DomainService\bin\Debug\Castle.Core.dll

using System;

namespace Castle.Core
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class InterceptorAttribute : Attribute
  {
    private readonly InterceptorReference interceptorRef;

    public InterceptorReference Interceptor
    {
      get
      {
        return this.interceptorRef;
      }
    }

    public InterceptorAttribute(string componentKey)
    {
      this.interceptorRef = new InterceptorReference(componentKey);
    }

    public InterceptorAttribute(Type interceptorType)
    {
      this.interceptorRef = new InterceptorReference(interceptorType);
    }
  }
}
