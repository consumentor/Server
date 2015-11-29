// Type: System.Web.Mvc.ActionFilterAttribute
// Assembly: System.Web.Mvc, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Simon\Consumentor\SVN\Shopgun\trunk\tools\ASP.NET MVC 2\trunk\bin\Debug\System.Web.Mvc.dll

using System;

namespace System.Web.Mvc
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public abstract class ActionFilterAttribute : FilterAttribute, IActionFilter, IResultFilter
  {
    public virtual void OnActionExecuting(ActionExecutingContext filterContext)
    {
    }

    public virtual void OnActionExecuted(ActionExecutedContext filterContext)
    {
    }

    public virtual void OnResultExecuting(ResultExecutingContext filterContext)
    {
    }

    public virtual void OnResultExecuted(ResultExecutedContext filterContext)
    {
    }
  }
}
