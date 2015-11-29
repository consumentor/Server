// Type: Castle.DynamicProxy.AbstractInvocation
// Assembly: Castle.DynamicProxy2, Version=2.0.3.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc
// Assembly location: C:\Users\Simon\Consumentor\SVN\Shopgun\trunk\tools\Castle\Castle.DynamicProxy2.dll

using Castle.Core.Interceptor;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace Castle.DynamicProxy
{
  [Serializable]
  public abstract class AbstractInvocation : IInvocation, ISerializable
  {
    private int execIndex = -1;
    private Type[] genericMethodArguments = (Type[]) null;
    private readonly object proxy;
    private readonly object target;
    private readonly IInterceptor[] interceptors;
    private readonly Type targetType;
    private readonly MethodInfo targetMethod;
    private readonly MethodInfo interfMethod;
    private object returnValue;
    private object[] arguments;

    public Type[] GenericArguments
    {
      get
      {
        return this.genericMethodArguments;
      }
    }

    public object Proxy
    {
      get
      {
        return this.proxy;
      }
    }

    public object InvocationTarget
    {
      get
      {
        return this.target;
      }
    }

    public Type TargetType
    {
      get
      {
        return this.targetType;
      }
    }

    public MethodInfo Method
    {
      get
      {
        if (this.interfMethod == null)
          return this.targetMethod;
        else
          return this.interfMethod;
      }
    }

    public MethodInfo MethodInvocationTarget
    {
      get
      {
        return this.targetMethod;
      }
    }

    public object ReturnValue
    {
      get
      {
        return this.returnValue;
      }
      set
      {
        this.returnValue = value;
      }
    }

    public object[] Arguments
    {
      get
      {
        return this.arguments;
      }
    }

    protected AbstractInvocation(object target, object proxy, IInterceptor[] interceptors, Type targetType, MethodInfo targetMethod, object[] arguments)
    {
      this.proxy = proxy;
      this.target = target;
      this.interceptors = interceptors;
      this.targetType = targetType;
      this.targetMethod = targetMethod;
      this.arguments = arguments;
    }

    protected AbstractInvocation(object target, object proxy, IInterceptor[] interceptors, Type targetType, MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments)
      : this(target, proxy, interceptors, targetType, targetMethod, arguments)
    {
      this.interfMethod = interfMethod;
    }

    public void SetGenericMethodArguments(Type[] arguments)
    {
      this.genericMethodArguments = arguments;
    }

    public MethodInfo GetConcreteMethod()
    {
      return this.EnsureClosedMethod(this.Method);
    }

    public MethodInfo GetConcreteMethodInvocationTarget()
    {
      return this.EnsureClosedMethod(this.MethodInvocationTarget);
    }

    public void SetArgumentValue(int index, object value)
    {
      this.arguments[index] = value;
    }

    public object GetArgumentValue(int index)
    {
      return this.arguments[index];
    }

    public void Proceed()
    {
      if (this.interceptors == null)
      {
        this.InvokeMethodOnTarget();
      }
      else
      {
        ++this.execIndex;
        if (this.execIndex == this.interceptors.Length)
        {
          this.InvokeMethodOnTarget();
        }
        else
        {
          if (this.execIndex > this.interceptors.Length)
            throw new InvalidOperationException("Proceed() cannot delegate to another interceptor. This usually signify a bug in the calling code");
          this.interceptors[this.execIndex].Intercept((IInvocation) this);
        }
      }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.SetType(typeof (RemotableInvocation));
      info.AddValue("invocation", (object) new RemotableInvocation((IInvocation) this));
    }

    protected abstract void InvokeMethodOnTarget();

    private MethodInfo EnsureClosedMethod(MethodInfo method)
    {
      if (!method.ContainsGenericParameters)
        return method;
      Debug.Assert(this.genericMethodArguments != null);
      return method.GetGenericMethodDefinition().MakeGenericMethod(this.genericMethodArguments);
    }
  }
}
