// Type: System.Web.Mvc.Controller
// Assembly: System.Web.Mvc, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Simon\Consumentor\SVN\Shopgun\trunk\tools\ASP.NET MVC 2\trunk\bin\Debug\System.Web.Mvc.dll

using System;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc.Resources;
using System.Web.Routing;

namespace System.Web.Mvc
{
  public abstract class Controller : ControllerBase, IActionFilter, IAuthorizationFilter, IDisposable, IExceptionFilter, IResultFilter
  {
    private IActionInvoker _actionInvoker;
    private ModelBinderDictionary _binders;
    private RouteCollection _routeCollection;
    private ITempDataProvider _tempDataProvider;

    public IActionInvoker ActionInvoker
    {
      get
      {
        if (this._actionInvoker == null)
          this._actionInvoker = this.CreateActionInvoker();
        return this._actionInvoker;
      }
      set
      {
        this._actionInvoker = value;
      }
    }

    protected internal ModelBinderDictionary Binders
    {
      get
      {
        if (this._binders == null)
          this._binders = ModelBinders.Binders;
        return this._binders;
      }
      set
      {
        this._binders = value;
      }
    }

    public HttpContextBase HttpContext
    {
      get
      {
        return this.ControllerContext == null ? (HttpContextBase) null : this.ControllerContext.HttpContext;
      }
    }

    public ModelStateDictionary ModelState
    {
      get
      {
        return this.ViewData.ModelState;
      }
    }

    public HttpRequestBase Request
    {
      get
      {
        return this.HttpContext == null ? (HttpRequestBase) null : this.HttpContext.Request;
      }
    }

    public HttpResponseBase Response
    {
      get
      {
        return this.HttpContext == null ? (HttpResponseBase) null : this.HttpContext.Response;
      }
    }

    internal RouteCollection RouteCollection
    {
      get
      {
        if (this._routeCollection == null)
          this._routeCollection = RouteTable.Routes;
        return this._routeCollection;
      }
      set
      {
        this._routeCollection = value;
      }
    }

    public RouteData RouteData
    {
      get
      {
        return this.ControllerContext == null ? (RouteData) null : this.ControllerContext.RouteData;
      }
    }

    public HttpServerUtilityBase Server
    {
      get
      {
        return this.HttpContext == null ? (HttpServerUtilityBase) null : this.HttpContext.Server;
      }
    }

    public HttpSessionStateBase Session
    {
      get
      {
        return this.HttpContext == null ? (HttpSessionStateBase) null : this.HttpContext.Session;
      }
    }

    public ITempDataProvider TempDataProvider
    {
      get
      {
        if (this._tempDataProvider == null)
          this._tempDataProvider = this.CreateTempDataProvider();
        return this._tempDataProvider;
      }
      set
      {
        this._tempDataProvider = value;
      }
    }

    public UrlHelper Url { get; set; }

    public IPrincipal User
    {
      get
      {
        return this.HttpContext == null ? (IPrincipal) null : this.HttpContext.User;
      }
    }

    protected internal ContentResult Content(string content)
    {
      return this.Content(content, (string) null);
    }

    protected internal ContentResult Content(string content, string contentType)
    {
      return this.Content(content, contentType, (Encoding) null);
    }

    protected internal virtual ContentResult Content(string content, string contentType, Encoding contentEncoding)
    {
      return new ContentResult()
      {
        Content = content,
        ContentType = contentType,
        ContentEncoding = contentEncoding
      };
    }

    protected virtual IActionInvoker CreateActionInvoker()
    {
      return (IActionInvoker) new ControllerActionInvoker();
    }

    protected virtual ITempDataProvider CreateTempDataProvider()
    {
      return (ITempDataProvider) new SessionStateTempDataProvider();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected override void ExecuteCore()
    {
      this.PossiblyLoadTempData();
      try
      {
        string requiredString = this.RouteData.GetRequiredString("action");
        if (this.ActionInvoker.InvokeAction(this.ControllerContext, requiredString))
          return;
        this.HandleUnknownAction(requiredString);
      }
      finally
      {
        this.PossiblySaveTempData();
      }
    }

    protected internal FileContentResult File(byte[] fileContents, string contentType)
    {
      return this.File(fileContents, contentType, (string) null);
    }

    protected internal virtual FileContentResult File(byte[] fileContents, string contentType, string fileDownloadName)
    {
      FileContentResult fileContentResult = new FileContentResult(fileContents, contentType);
      fileContentResult.FileDownloadName = fileDownloadName;
      return fileContentResult;
    }

    protected internal FileStreamResult File(Stream fileStream, string contentType)
    {
      return this.File(fileStream, contentType, (string) null);
    }

    protected internal virtual FileStreamResult File(Stream fileStream, string contentType, string fileDownloadName)
    {
      FileStreamResult fileStreamResult = new FileStreamResult(fileStream, contentType);
      fileStreamResult.FileDownloadName = fileDownloadName;
      return fileStreamResult;
    }

    protected internal FilePathResult File(string fileName, string contentType)
    {
      return this.File(fileName, contentType, (string) null);
    }

    protected internal virtual FilePathResult File(string fileName, string contentType, string fileDownloadName)
    {
      FilePathResult filePathResult = new FilePathResult(fileName, contentType);
      filePathResult.FileDownloadName = fileDownloadName;
      return filePathResult;
    }

    protected virtual void HandleUnknownAction(string actionName)
    {
      throw new HttpException(404, string.Format((IFormatProvider) CultureInfo.CurrentUICulture, MvcResources.Controller_UnknownAction, new object[2]
      {
        (object) actionName,
        (object) this.GetType().FullName
      }));
    }

    protected internal virtual JavaScriptResult JavaScript(string script)
    {
      return new JavaScriptResult()
      {
        Script = script
      };
    }

    protected internal JsonResult Json(object data)
    {
      return this.Json(data, (string) null, (Encoding) null, JsonRequestBehavior.DenyGet);
    }

    protected internal JsonResult Json(object data, string contentType)
    {
      return this.Json(data, contentType, (Encoding) null, JsonRequestBehavior.DenyGet);
    }

    protected internal virtual JsonResult Json(object data, string contentType, Encoding contentEncoding)
    {
      return this.Json(data, contentType, contentEncoding, JsonRequestBehavior.DenyGet);
    }

    protected internal JsonResult Json(object data, JsonRequestBehavior behavior)
    {
      return this.Json(data, (string) null, (Encoding) null, behavior);
    }

    protected internal JsonResult Json(object data, string contentType, JsonRequestBehavior behavior)
    {
      return this.Json(data, contentType, (Encoding) null, behavior);
    }

    protected internal virtual JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
    {
      return new JsonResult()
      {
        Data = data,
        ContentType = contentType,
        ContentEncoding = contentEncoding,
        JsonRequestBehavior = behavior
      };
    }

    protected override void Initialize(RequestContext requestContext)
    {
      base.Initialize(requestContext);
      this.Url = new UrlHelper(requestContext);
    }

    protected virtual void OnActionExecuting(ActionExecutingContext filterContext)
    {
    }

    protected virtual void OnActionExecuted(ActionExecutedContext filterContext)
    {
    }

    protected virtual void OnAuthorization(AuthorizationContext filterContext)
    {
    }

    protected virtual void OnException(ExceptionContext filterContext)
    {
    }

    protected virtual void OnResultExecuted(ResultExecutedContext filterContext)
    {
    }

    protected virtual void OnResultExecuting(ResultExecutingContext filterContext)
    {
    }

    protected internal PartialViewResult PartialView()
    {
      return this.PartialView((string) null, (object) null);
    }

    protected internal PartialViewResult PartialView(object model)
    {
      return this.PartialView((string) null, model);
    }

    protected internal PartialViewResult PartialView(string viewName)
    {
      return this.PartialView(viewName, (object) null);
    }

    protected internal virtual PartialViewResult PartialView(string viewName, object model)
    {
      if (model != null)
        this.ViewData.Model = model;
      PartialViewResult partialViewResult = new PartialViewResult();
      partialViewResult.ViewName = viewName;
      partialViewResult.ViewData = this.ViewData;
      partialViewResult.TempData = this.TempData;
      return partialViewResult;
    }

    internal void PossiblyLoadTempData()
    {
      if (this.ControllerContext.IsChildAction)
        return;
      this.TempData.Load(this.ControllerContext, this.TempDataProvider);
    }

    internal void PossiblySaveTempData()
    {
      if (this.ControllerContext.IsChildAction)
        return;
      this.TempData.Save(this.ControllerContext, this.TempDataProvider);
    }

    protected internal virtual RedirectResult Redirect(string url)
    {
      if (string.IsNullOrEmpty(url))
        throw new ArgumentException(MvcResources.Common_NullOrEmpty, "url");
      else
        return new RedirectResult(url);
    }

    protected internal RedirectToRouteResult RedirectToAction(string actionName)
    {
      return this.RedirectToAction(actionName, (RouteValueDictionary) null);
    }

    protected internal RedirectToRouteResult RedirectToAction(string actionName, object routeValues)
    {
      return this.RedirectToAction(actionName, new RouteValueDictionary(routeValues));
    }

    protected internal RedirectToRouteResult RedirectToAction(string actionName, RouteValueDictionary routeValues)
    {
      return this.RedirectToAction(actionName, (string) null, routeValues);
    }

    protected internal RedirectToRouteResult RedirectToAction(string actionName, string controllerName)
    {
      return this.RedirectToAction(actionName, controllerName, (RouteValueDictionary) null);
    }

    protected internal RedirectToRouteResult RedirectToAction(string actionName, string controllerName, object routeValues)
    {
      return this.RedirectToAction(actionName, controllerName, new RouteValueDictionary(routeValues));
    }

    protected internal virtual RedirectToRouteResult RedirectToAction(string actionName, string controllerName, RouteValueDictionary routeValues)
    {
      return new RedirectToRouteResult(this.RouteData != null ? RouteValuesHelpers.MergeRouteValues(actionName, controllerName, this.RouteData.Values, routeValues, true) : RouteValuesHelpers.MergeRouteValues(actionName, controllerName, (RouteValueDictionary) null, routeValues, true));
    }

    protected internal RedirectToRouteResult RedirectToRoute(object routeValues)
    {
      return this.RedirectToRoute(new RouteValueDictionary(routeValues));
    }

    protected internal RedirectToRouteResult RedirectToRoute(RouteValueDictionary routeValues)
    {
      return this.RedirectToRoute((string) null, routeValues);
    }

    protected internal RedirectToRouteResult RedirectToRoute(string routeName)
    {
      return this.RedirectToRoute(routeName, (RouteValueDictionary) null);
    }

    protected internal RedirectToRouteResult RedirectToRoute(string routeName, object routeValues)
    {
      return this.RedirectToRoute(routeName, new RouteValueDictionary(routeValues));
    }

    protected internal virtual RedirectToRouteResult RedirectToRoute(string routeName, RouteValueDictionary routeValues)
    {
      return new RedirectToRouteResult(routeName, RouteValuesHelpers.GetRouteValues(routeValues));
    }

    protected internal bool TryUpdateModel<TModel>(TModel model) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, (string) null, (string[]) null, (string[]) null, this.ValueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, string prefix) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, prefix, (string[]) null, (string[]) null, this.ValueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, string[] includeProperties) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, (string) null, includeProperties, (string[]) null, this.ValueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, prefix, includeProperties, (string[]) null, this.ValueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, prefix, includeProperties, excludeProperties, this.ValueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, IValueProvider valueProvider) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, (string) null, (string[]) null, (string[]) null, valueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, string prefix, IValueProvider valueProvider) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, prefix, (string[]) null, (string[]) null, valueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, string[] includeProperties, IValueProvider valueProvider) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, (string) null, includeProperties, (string[]) null, valueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, IValueProvider valueProvider) where TModel : class
    {
      return this.TryUpdateModel<TModel>(model, prefix, includeProperties, (string[]) null, valueProvider);
    }

    protected internal bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider) where TModel : class
    {
      if ((object) model == null)
        throw new ArgumentNullException("model");
      if (valueProvider == null)
        throw new ArgumentNullException("valueProvider");
      Predicate<string> predicate = (Predicate<string>) (propertyName => BindAttribute.IsPropertyAllowed(propertyName, includeProperties, excludeProperties));
      this.Binders.GetBinder(typeof (TModel)).BindModel(this.ControllerContext, new ModelBindingContext()
      {
        ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType((Func<object>) (() => (object) (TModel) model), typeof (TModel)),
        ModelName = prefix,
        ModelState = this.ModelState,
        PropertyFilter = predicate,
        ValueProvider = valueProvider
      });
      return this.ModelState.IsValid;
    }

    protected internal bool TryValidateModel(object model)
    {
      return this.TryValidateModel(model, (string) null);
    }

    protected internal bool TryValidateModel(object model, string prefix)
    {
      if (model == null)
        throw new ArgumentNullException("model");
      foreach (ModelValidationResult validationResult in ModelValidator.GetModelValidator(ModelMetadataProviders.Current.GetMetadataForType((Func<object>) (() => model), model.GetType()), this.ControllerContext).Validate((object) null))
        this.ModelState.AddModelError(DefaultModelBinder.CreateSubPropertyName(prefix, validationResult.MemberName), validationResult.Message);
      return this.ModelState.IsValid;
    }

    protected internal void UpdateModel<TModel>(TModel model) where TModel : class
    {
      this.UpdateModel<TModel>(model, (string) null, (string[]) null, (string[]) null, this.ValueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, string prefix) where TModel : class
    {
      this.UpdateModel<TModel>(model, prefix, (string[]) null, (string[]) null, this.ValueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, string[] includeProperties) where TModel : class
    {
      this.UpdateModel<TModel>(model, (string) null, includeProperties, (string[]) null, this.ValueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, string prefix, string[] includeProperties) where TModel : class
    {
      this.UpdateModel<TModel>(model, prefix, includeProperties, (string[]) null, this.ValueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) where TModel : class
    {
      this.UpdateModel<TModel>(model, prefix, includeProperties, excludeProperties, this.ValueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, IValueProvider valueProvider) where TModel : class
    {
      this.UpdateModel<TModel>(model, (string) null, (string[]) null, (string[]) null, valueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, string prefix, IValueProvider valueProvider) where TModel : class
    {
      this.UpdateModel<TModel>(model, prefix, (string[]) null, (string[]) null, valueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, string[] includeProperties, IValueProvider valueProvider) where TModel : class
    {
      this.UpdateModel<TModel>(model, (string) null, includeProperties, (string[]) null, valueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, IValueProvider valueProvider) where TModel : class
    {
      this.UpdateModel<TModel>(model, prefix, includeProperties, (string[]) null, valueProvider);
    }

    protected internal void UpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider) where TModel : class
    {
      if (this.TryUpdateModel<TModel>(model, prefix, includeProperties, excludeProperties, valueProvider))
        return;
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, MvcResources.Controller_UpdateModel_UpdateUnsuccessful, new object[1]
      {
        (object) typeof (TModel).FullName
      }));
    }

    protected internal void ValidateModel(object model)
    {
      this.ValidateModel(model, (string) null);
    }

    protected internal void ValidateModel(object model, string prefix)
    {
      if (this.TryValidateModel(model, prefix))
        return;
      throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, MvcResources.Controller_Validate_ValidationFailed, new object[1]
      {
        (object) model.GetType().FullName
      }));
    }

    protected internal ViewResult View()
    {
      return this.View((string) null, (string) null, (object) null);
    }

    protected internal ViewResult View(object model)
    {
      return this.View((string) null, (string) null, model);
    }

    protected internal ViewResult View(string viewName)
    {
      return this.View(viewName, (string) null, (object) null);
    }

    protected internal ViewResult View(string viewName, string masterName)
    {
      return this.View(viewName, masterName, (object) null);
    }

    protected internal ViewResult View(string viewName, object model)
    {
      return this.View(viewName, (string) null, model);
    }

    protected internal virtual ViewResult View(string viewName, string masterName, object model)
    {
      if (model != null)
        this.ViewData.Model = model;
      ViewResult viewResult = new ViewResult();
      viewResult.ViewName = viewName;
      viewResult.MasterName = masterName;
      viewResult.ViewData = this.ViewData;
      viewResult.TempData = this.TempData;
      return viewResult;
    }

    protected internal ViewResult View(IView view)
    {
      return this.View(view, (object) null);
    }

    protected internal virtual ViewResult View(IView view, object model)
    {
      if (model != null)
        this.ViewData.Model = model;
      ViewResult viewResult = new ViewResult();
      viewResult.View = view;
      viewResult.ViewData = this.ViewData;
      viewResult.TempData = this.TempData;
      return viewResult;
    }

    void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
    {
      this.OnActionExecuting(filterContext);
    }

    void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
    {
      this.OnActionExecuted(filterContext);
    }

    void IAuthorizationFilter.OnAuthorization(AuthorizationContext filterContext)
    {
      this.OnAuthorization(filterContext);
    }

    void IExceptionFilter.OnException(ExceptionContext filterContext)
    {
      this.OnException(filterContext);
    }

    void IResultFilter.OnResultExecuting(ResultExecutingContext filterContext)
    {
      this.OnResultExecuting(filterContext);
    }

    void IResultFilter.OnResultExecuted(ResultExecutedContext filterContext)
    {
      this.OnResultExecuted(filterContext);
    }
  }
}
