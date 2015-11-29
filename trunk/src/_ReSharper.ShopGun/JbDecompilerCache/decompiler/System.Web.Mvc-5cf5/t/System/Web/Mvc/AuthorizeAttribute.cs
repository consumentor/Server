// Type: System.Web.Mvc.AuthorizeAttribute
// Assembly: System.Web.Mvc, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Simon\Consumentor\SVN\Shopgun\trunk\tools\ASP.NET MVC 2\trunk\bin\Debug\System.Web.Mvc.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace System.Web.Mvc
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
  public class AuthorizeAttribute : FilterAttribute, IAuthorizationFilter
  {
    private readonly object _typeId = new object();
    private string[] _rolesSplit = new string[0];
    private string[] _usersSplit = new string[0];
    private string _roles;
    private string _users;

    public string Roles
    {
      get
      {
        return this._roles ?? string.Empty;
      }
      set
      {
        this._roles = value;
        this._rolesSplit = AuthorizeAttribute.SplitString(value);
      }
    }

    public override object TypeId
    {
      get
      {
        return this._typeId;
      }
    }

    public string Users
    {
      get
      {
        return this._users ?? string.Empty;
      }
      set
      {
        this._users = value;
        this._usersSplit = AuthorizeAttribute.SplitString(value);
      }
    }

    protected virtual bool AuthorizeCore(HttpContextBase httpContext)
    {
      if (httpContext == null)
        throw new ArgumentNullException("httpContext");
      IPrincipal user = httpContext.User;
      if (!user.Identity.IsAuthenticated || this._usersSplit.Length > 0 && !Enumerable.Contains<string>((IEnumerable<string>) this._usersSplit, user.Identity.Name, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) || this._rolesSplit.Length > 0 && !Enumerable.Any<string>((IEnumerable<string>) this._rolesSplit, new Func<string, bool>(user.IsInRole)))
        return false;
      else
        return true;
    }

    private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
    {
      validationStatus = this.OnCacheAuthorization((HttpContextBase) new HttpContextWrapper(context));
    }

    public virtual void OnAuthorization(AuthorizationContext filterContext)
    {
      if (filterContext == null)
        throw new ArgumentNullException("filterContext");
      if (this.AuthorizeCore(filterContext.HttpContext))
      {
        HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
        cache.SetProxyMaxAge(new TimeSpan(0L));
        cache.AddValidationCallback(new HttpCacheValidateHandler(this.CacheValidateHandler), (object) null);
      }
      else
        this.HandleUnauthorizedRequest(filterContext);
    }

    protected virtual void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
      filterContext.Result = (ActionResult) new HttpUnauthorizedResult();
    }

    protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
    {
      if (httpContext == null)
        throw new ArgumentNullException("httpContext");
      else
        return this.AuthorizeCore(httpContext) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
    }

    internal static string[] SplitString(string original)
    {
      if (string.IsNullOrEmpty(original))
        return new string[0];
      return Enumerable.ToArray<string>(Enumerable.Select(Enumerable.Where(Enumerable.Select((IEnumerable<string>) original.Split(new char[1]
      {
        ','
      }), piece =>
      {
        var local_0 = new
        {
          piece = piece,
          trimmed = piece.Trim()
        };
        return local_0;
      }), param0 => !string.IsNullOrEmpty(param0.trimmed)), param0 => param0.trimmed));
    }
  }
}
