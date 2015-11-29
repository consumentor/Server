// Type: System.Web.Mvc.FilterAttribute
// Assembly: System.Web.Mvc, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Simon\Consumentor\SVN\Shopgun\trunk\tools\ASP.NET MVC 2\trunk\bin\Debug\System.Web.Mvc.dll

using System;
using System.Web.Mvc.Resources;

namespace System.Web.Mvc
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public abstract class FilterAttribute : Attribute
  {
    private int _order = -1;

    public int Order
    {
      get
      {
        return this._order;
      }
      set
      {
        if (value < -1)
          throw new ArgumentOutOfRangeException("value", MvcResources.FilterAttribute_OrderOutOfRange);
        this._order = value;
      }
    }
  }
}
