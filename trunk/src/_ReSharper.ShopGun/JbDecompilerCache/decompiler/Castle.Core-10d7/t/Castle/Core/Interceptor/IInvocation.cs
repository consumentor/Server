// Type: Castle.Core.Interceptor.IInvocation
// Assembly: Castle.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc
// Assembly location: C:\Users\Simon\Consumentor\SVN\Shopgun\trunk\src\DomainService\bin\Debug\Castle.Core.dll

using System;
using System.Reflection;

namespace Castle.Core.Interceptor
{
  public interface IInvocation
  {
    object Proxy { get; }

    object InvocationTarget { get; }

    Type TargetType { get; }

    object[] Arguments { get; }

    Type[] GenericArguments { get; }

    MethodInfo Method { get; }

    MethodInfo MethodInvocationTarget { get; }

    object ReturnValue { get; set; }

    void SetArgumentValue(int index, object value);

    object GetArgumentValue(int index);

    MethodInfo GetConcreteMethod();

    MethodInfo GetConcreteMethodInvocationTarget();

    void Proceed();
  }
}
