// Type: System.Web.Mvc.IActionFilter
// Assembly: System.Web.Mvc, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Simon\Consumentor\SVN\Shopgun\trunk\tools\ASP.NET MVC 2\trunk\bin\Debug\System.Web.Mvc.dll

namespace System.Web.Mvc
{
  public interface IActionFilter
  {
    void OnActionExecuting(ActionExecutingContext filterContext);

    void OnActionExecuted(ActionExecutedContext filterContext);
  }
}
