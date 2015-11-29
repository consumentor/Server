// Type: System.Web.HttpRuntime
// Assembly: System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Web.dll

using System;
using System.Collections;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Caching;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Management;
using System.Web.UI;
using System.Web.Util;
using System.Xml;

namespace System.Web
{
  public sealed class HttpRuntime
  {
    internal static byte[] s_autogenKeys = new byte[1024];
    private static string DirectorySeparatorString = new string(Path.DirectorySeparatorChar, 1);
    private static string DoubleDirectorySeparatorString = new string(Path.DirectorySeparatorChar, 2);
    private static char[] s_InvalidPhysicalPathChars = new char[7]
    {
      '/',
      '?',
      '*',
      '<',
      '>',
      '|',
      '"'
    };
    private static bool s_initialized = false;
    private static bool s_isEngineLoaded = false;
    private static object s_factoryLock = new object();
    private bool _beforeFirstRequest = true;
    internal const string codegenDirName = "Temporary ASP.NET Files";
    internal const string profileFileName = "profileoptimization.prof";
    internal const string BinDirectoryName = "bin";
    internal const string CodeDirectoryName = "App_Code";
    internal const string WebRefDirectoryName = "App_WebReferences";
    internal const string ResourcesDirectoryName = "App_GlobalResources";
    internal const string LocalResourcesDirectoryName = "App_LocalResources";
    internal const string DataDirectoryName = "App_Data";
    internal const string ThemesDirectoryName = "App_Themes";
    internal const string GlobalThemesDirectoryName = "Themes";
    internal const string BrowsersDirectoryName = "App_Browsers";
    private static HttpRuntime _theRuntime;
    private static string s_installDirectory;
    private NamedPermissionSet _namedPermissionSet;
    private PolicyLevel _policyLevel;
    private string _hostSecurityPolicyResolverType;
    private FileChangesMonitor _fcm;
    private CacheInternal _cacheInternal;
    private Cache _cachePublic;
    private bool _isOnUNCShare;
    private Profiler _profiler;
    private RequestTimeoutManager _timeoutManager;
    private RequestQueue _requestQueue;
    private bool _apartmentThreading;
    private bool _processRequestInApplicationTrust;
    private bool _disableProcessRequestInApplicationTrust;
    private bool _isLegacyCas;
    private DateTime _firstRequestStartTime;
    private bool _firstRequestCompleted;
    private bool _userForcedShutdown;
    private bool _configInited;
    private bool _fusionInited;
    private int _activeRequestCount;
    private volatile bool _disposingHttpRuntime;
    private DateTime _lastShutdownAttemptTime;
    private bool _shutdownInProgress;
    private string _shutDownStack;
    private string _shutDownMessage;
    private ApplicationShutdownReason _shutdownReason;
    private string _trustLevel;
    private string _wpUserId;
    private bool _shutdownWebEventRaised;
    private bool _enableHeaderChecking;
    private AsyncCallback _requestNotificationCompletionCallback;
    private AsyncCallback _handlerCompletionCallback;
    private HttpWorkerRequest.EndOfSendNotification _asyncEndOfSendCallback;
    private WaitCallback _appDomainUnloadallback;
    private Exception _initializationError;
    private bool _hostingInitFailed;
    private System.Threading.Timer _appDomainShutdownTimer;
    private string _tempDir;
    private string _codegenDir;
    private string _appDomainAppId;
    private string _appDomainAppPath;
    private VirtualPath _appDomainAppVPath;
    private string _appDomainId;
    private bool _debuggingEnabled;
    private byte[] _appOfflineMessage;
    private string _clientScriptVirtualPath;
    private string _clientScriptPhysicalPath;
    private static Version _iisVersion;
    private static bool _useIntegratedPipeline;
    private static bool _enablePrefetchOptimization;
    private static BuildManagerHostUnloadEventHandler AppDomainShutdown;
    private static FactoryGenerator s_factoryGenerator;
    private static Hashtable s_factoryCache;
    private static bool s_initializedFactory;
    private static string _DefaultPhysicalPathOnMapPathFailure;
    private const string AppOfflineFileName = "App_Offline.htm";
    private const long MaxAppOfflineFileLength = 1048576L;
    private const string AspNetClientFilesSubDirectory = "asp.netclientfiles";
    private const string AspNetClientFilesParentVirtualPath = "/aspnet_client/system_web/";

    internal static Exception InitializationException
    {
      get
      {
        return HttpRuntime._theRuntime._initializationError;
      }
      set
      {
        HttpRuntime._theRuntime._initializationError = value;
        if (HttpRuntime.HostingInitFailed)
          return;
        HttpRuntime._theRuntime.StartAppDomainShutdownTimer();
      }
    }

    internal static bool HostingInitFailed
    {
      get
      {
        return HttpRuntime._theRuntime._hostingInitFailed;
      }
    }

    internal static bool EnableHeaderChecking
    {
      get
      {
        return HttpRuntime._theRuntime._enableHeaderChecking;
      }
    }

    internal static bool ProcessRequestInApplicationTrust
    {
      get
      {
        return HttpRuntime._theRuntime._processRequestInApplicationTrust;
      }
    }

    internal static bool DisableProcessRequestInApplicationTrust
    {
      get
      {
        return HttpRuntime._theRuntime._disableProcessRequestInApplicationTrust;
      }
    }

    internal static bool IsLegacyCas
    {
      get
      {
        return HttpRuntime._theRuntime._isLegacyCas;
      }
    }

    internal static byte[] AppOfflineMessage
    {
      get
      {
        return HttpRuntime._theRuntime._appOfflineMessage;
      }
    }

    public static Version IISVersion
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return HttpRuntime._iisVersion;
      }
    }

    public static bool UsingIntegratedPipeline
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return HttpRuntime.UseIntegratedPipeline;
      }
    }

    internal static bool UseIntegratedPipeline
    {
      [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")] get
      {
        return HttpRuntime._useIntegratedPipeline;
      }
    }

    internal static bool EnablePrefetchOptimization
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return HttpRuntime._enablePrefetchOptimization;
      }
    }

    internal static ApplicationShutdownReason ShutdownReason
    {
      get
      {
        return HttpRuntime._theRuntime._shutdownReason;
      }
    }

    private DateTime LastShutdownAttemptTime
    {
      get
      {
        lock (this)
          return this._lastShutdownAttemptTime;
      }
      set
      {
        lock (this)
          this._lastShutdownAttemptTime = value;
      }
    }

    internal static Profiler Profile
    {
      get
      {
        return HttpRuntime._theRuntime._profiler;
      }
    }

    internal static bool IsTrustLevelInitialized
    {
      get
      {
        if (HostingEnvironment.IsHosted)
          return HttpRuntime.TrustLevel != null;
        else
          return true;
      }
    }

    internal static NamedPermissionSet NamedPermissionSet
    {
      get
      {
        return HttpRuntime._theRuntime._namedPermissionSet;
      }
    }

    internal static PolicyLevel PolicyLevel
    {
      get
      {
        return HttpRuntime._theRuntime._policyLevel;
      }
    }

    internal static string HostSecurityPolicyResolverType
    {
      get
      {
        return HttpRuntime._theRuntime._hostSecurityPolicyResolverType;
      }
    }

    internal static bool IsFullTrust
    {
      get
      {
        return HttpRuntime._theRuntime._namedPermissionSet == null;
      }
    }

    internal static FileChangesMonitor FileChangesMonitor
    {
      get
      {
        return HttpRuntime._theRuntime._fcm;
      }
    }

    internal static RequestTimeoutManager RequestTimeoutManager
    {
      get
      {
        return HttpRuntime._theRuntime._timeoutManager;
      }
    }

    public static Cache Cache
    {
      get
      {
        if (HttpRuntime.AspInstallDirectoryInternal == null)
        {
          throw new HttpException(System.Web.SR.GetString("Aspnet_not_installed", new object[1]
          {
            (object) VersionInfo.SystemWebVersion
          }));
        }
        else
        {
          Cache cache1 = HttpRuntime._theRuntime._cachePublic;
          if (cache1 == null)
          {
            CacheInternal cacheInternal = HttpRuntime.CacheInternal;
            CacheSection cache2 = RuntimeConfig.GetAppConfig().Cache;
            cacheInternal.ReadCacheInternalConfig(cache2);
            HttpRuntime._theRuntime._cachePublic = cacheInternal.CachePublic;
            cache1 = HttpRuntime._theRuntime._cachePublic;
          }
          return cache1;
        }
      }
    }

    internal static CacheInternal CacheInternal
    {
      get
      {
        CacheInternal cacheInternal = HttpRuntime._theRuntime._cacheInternal;
        if (cacheInternal == null)
        {
          HttpRuntime._theRuntime.CreateCache();
          cacheInternal = HttpRuntime._theRuntime._cacheInternal;
        }
        return cacheInternal;
      }
    }

    public static string AspInstallDirectory
    {
      get
      {
        string directoryInternal = HttpRuntime.AspInstallDirectoryInternal;
        if (directoryInternal == null)
        {
          throw new HttpException(System.Web.SR.GetString("Aspnet_not_installed", new object[1]
          {
            (object) VersionInfo.SystemWebVersion
          }));
        }
        else
        {
          InternalSecurityPermissions.PathDiscovery(directoryInternal).Demand();
          return directoryInternal;
        }
      }
    }

    internal static string AspInstallDirectoryInternal
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return HttpRuntime.s_installDirectory;
      }
    }

    public static string AspClientScriptVirtualPath
    {
      get
      {
        if (HttpRuntime._theRuntime._clientScriptVirtualPath == null)
        {
          string systemWebVersion = VersionInfo.SystemWebVersion;
          string str = "/aspnet_client/system_web/" + systemWebVersion.Substring(0, systemWebVersion.LastIndexOf('.')).Replace('.', '_');
          HttpRuntime._theRuntime._clientScriptVirtualPath = str;
        }
        return HttpRuntime._theRuntime._clientScriptVirtualPath;
      }
    }

    public static string AspClientScriptPhysicalPath
    {
      get
      {
        string physicalPathInternal = HttpRuntime.AspClientScriptPhysicalPathInternal;
        if (physicalPathInternal != null)
          return physicalPathInternal;
        throw new HttpException(System.Web.SR.GetString("Aspnet_not_installed", new object[1]
        {
          (object) VersionInfo.SystemWebVersion
        }));
      }
    }

    internal static string AspClientScriptPhysicalPathInternal
    {
      get
      {
        if (HttpRuntime._theRuntime._clientScriptPhysicalPath == null)
        {
          string str = Path.Combine(HttpRuntime.AspInstallDirectoryInternal, "asp.netclientfiles");
          HttpRuntime._theRuntime._clientScriptPhysicalPath = str;
        }
        return HttpRuntime._theRuntime._clientScriptPhysicalPath;
      }
    }

    public static string ClrInstallDirectory
    {
      get
      {
        string directoryInternal = HttpRuntime.ClrInstallDirectoryInternal;
        InternalSecurityPermissions.PathDiscovery(directoryInternal).Demand();
        return directoryInternal;
      }
    }

    internal static string ClrInstallDirectoryInternal
    {
      get
      {
        return HttpConfigurationSystem.MsCorLibDirectory;
      }
    }

    public static string MachineConfigurationDirectory
    {
      get
      {
        string directoryInternal = HttpRuntime.MachineConfigurationDirectoryInternal;
        InternalSecurityPermissions.PathDiscovery(directoryInternal).Demand();
        return directoryInternal;
      }
    }

    internal static string MachineConfigurationDirectoryInternal
    {
      get
      {
        return HttpConfigurationSystem.MachineConfigurationDirectory;
      }
    }

    internal static bool IsEngineLoaded
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return HttpRuntime.s_isEngineLoaded;
      }
    }

    public static string CodegenDir
    {
      get
      {
        string codegenDirInternal = HttpRuntime.CodegenDirInternal;
        InternalSecurityPermissions.PathDiscovery(codegenDirInternal).Demand();
        return codegenDirInternal;
      }
    }

    internal static string CodegenDirInternal
    {
      get
      {
        return HttpRuntime._theRuntime._codegenDir;
      }
    }

    internal static string TempDirInternal
    {
      get
      {
        return HttpRuntime._theRuntime._tempDir;
      }
    }

    public static string AppDomainAppId
    {
      get
      {
        return HttpRuntime._theRuntime._appDomainAppId;
      }
    }

    internal static bool IsAspNetAppDomain
    {
      get
      {
        return HttpRuntime.AppDomainAppId != null;
      }
    }

    public static string AppDomainAppPath
    {
      get
      {
        InternalSecurityPermissions.AppPathDiscovery.Demand();
        return HttpRuntime.AppDomainAppPathInternal;
      }
    }

    internal static string AppDomainAppPathInternal
    {
      get
      {
        return HttpRuntime._theRuntime._appDomainAppPath;
      }
    }

    public static string AppDomainAppVirtualPath
    {
      get
      {
        return VirtualPath.GetVirtualPathStringNoTrailingSlash(HttpRuntime._theRuntime._appDomainAppVPath);
      }
    }

    internal static string AppDomainAppVirtualPathString
    {
      get
      {
        return VirtualPath.GetVirtualPathString(HttpRuntime._theRuntime._appDomainAppVPath);
      }
    }

    internal static VirtualPath AppDomainAppVirtualPathObject
    {
      get
      {
        return HttpRuntime._theRuntime._appDomainAppVPath;
      }
    }

    public static string AppDomainId
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries"), AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.High)] get
      {
        return HttpRuntime.AppDomainIdInternal;
      }
    }

    internal static string AppDomainIdInternal
    {
      get
      {
        return HttpRuntime._theRuntime._appDomainId;
      }
    }

    public static string BinDirectory
    {
      get
      {
        string directoryInternal = HttpRuntime.BinDirectoryInternal;
        InternalSecurityPermissions.PathDiscovery(directoryInternal).Demand();
        return directoryInternal;
      }
    }

    internal static string BinDirectoryInternal
    {
      get
      {
        return Path.Combine(HttpRuntime._theRuntime._appDomainAppPath, "bin") + (object) Path.DirectorySeparatorChar;
      }
    }

    internal static VirtualPath CodeDirectoryVirtualPath
    {
      get
      {
        return HttpRuntime._theRuntime._appDomainAppVPath.SimpleCombineWithDir("App_Code");
      }
    }

    internal static VirtualPath ResourcesDirectoryVirtualPath
    {
      get
      {
        return HttpRuntime._theRuntime._appDomainAppVPath.SimpleCombineWithDir("App_GlobalResources");
      }
    }

    internal static VirtualPath WebRefDirectoryVirtualPath
    {
      get
      {
        return HttpRuntime._theRuntime._appDomainAppVPath.SimpleCombineWithDir("App_WebReferences");
      }
    }

    public static bool IsOnUNCShare
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries"), AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Low)] get
      {
        return HttpRuntime.IsOnUNCShareInternal;
      }
    }

    internal static bool IsOnUNCShareInternal
    {
      get
      {
        return HttpRuntime._theRuntime._isOnUNCShare;
      }
    }

    public static Version TargetFramework
    {
      get
      {
        return BinaryCompatibility.Current.TargetFramework;
      }
    }

    internal static bool DebuggingEnabled
    {
      get
      {
        return HttpRuntime._theRuntime._debuggingEnabled;
      }
    }

    internal static bool ConfigInited
    {
      get
      {
        return HttpRuntime._theRuntime._configInited;
      }
    }

    internal static bool FusionInited
    {
      get
      {
        return HttpRuntime._theRuntime._fusionInited;
      }
    }

    internal static bool ApartmentThreading
    {
      get
      {
        return HttpRuntime._theRuntime._apartmentThreading;
      }
    }

    internal static bool ShutdownInProgress
    {
      get
      {
        return HttpRuntime._theRuntime._shutdownInProgress;
      }
    }

    internal static string TrustLevel
    {
      get
      {
        return HttpRuntime._theRuntime._trustLevel;
      }
    }

    internal static string WpUserId
    {
      get
      {
        return HttpRuntime._theRuntime._wpUserId;
      }
    }

    internal static bool IsMapPathRelaxed
    {
      get
      {
        return HttpRuntime._DefaultPhysicalPathOnMapPathFailure != null;
      }
    }

    internal static event BuildManagerHostUnloadEventHandler AppDomainShutdown
    {
      add
      {
        BuildManagerHostUnloadEventHandler unloadEventHandler1 = HttpRuntime.AppDomainShutdown;
        BuildManagerHostUnloadEventHandler comparand;
        do
        {
          comparand = unloadEventHandler1;
          BuildManagerHostUnloadEventHandler unloadEventHandler2 = comparand + value;
          unloadEventHandler1 = Interlocked.CompareExchange<BuildManagerHostUnloadEventHandler>(ref HttpRuntime.AppDomainShutdown, unloadEventHandler2, comparand);
        }
        while (unloadEventHandler1 != comparand);
      }
      remove
      {
        BuildManagerHostUnloadEventHandler unloadEventHandler1 = HttpRuntime.AppDomainShutdown;
        BuildManagerHostUnloadEventHandler comparand;
        do
        {
          comparand = unloadEventHandler1;
          BuildManagerHostUnloadEventHandler unloadEventHandler2 = comparand - value;
          unloadEventHandler1 = Interlocked.CompareExchange<BuildManagerHostUnloadEventHandler>(ref HttpRuntime.AppDomainShutdown, unloadEventHandler2, comparand);
        }
        while (unloadEventHandler1 != comparand);
      }
    }

    static HttpRuntime()
    {
      HttpRuntime.AddAppDomainTraceMessage("*HttpRuntime::cctor");
      HttpRuntime.StaticInit();
      HttpRuntime._theRuntime = new HttpRuntime();
      HttpRuntime._theRuntime.Init();
      HttpRuntime.AddAppDomainTraceMessage("HttpRuntime::cctor*");
    }

    internal static void ForceStaticInit()
    {
    }

    internal static void InitializeHostingFeatures(HostingEnvironmentFlags hostingFlags, PolicyLevel policyLevel, Exception appDomainCreationException)
    {
      HttpRuntime._theRuntime.HostingInit(hostingFlags, policyLevel, appDomainCreationException);
    }

    [SecurityPermission(SecurityAction.Assert, ControlThread = true)]
    internal static void SetCurrentThreadCultureWithAssert(CultureInfo cultureInfo)
    {
      Thread.CurrentThread.CurrentCulture = cultureInfo;
    }

    internal static void StartListeningToLocalResourcesDirectory(VirtualPath virtualDir)
    {
      HttpRuntime._theRuntime._fcm.StartListeningToLocalResourcesDirectory(virtualDir);
    }

    internal static void CheckApplicationEnabled()
    {
      string str = Path.Combine(HttpRuntime._theRuntime._appDomainAppPath, "App_Offline.htm");
      bool flag = false;
      HttpRuntime._theRuntime._fcm.StartMonitoringFile(str, new FileChangeEventHandler(HttpRuntime._theRuntime.OnAppOfflineFileChange));
      try
      {
        if (System.IO.File.Exists(str))
        {
          using (FileStream fileStream = new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            if (fileStream.Length <= 1048576L)
            {
              int count = (int) fileStream.Length;
              if (count > 0)
              {
                byte[] buffer = new byte[count];
                if (fileStream.Read(buffer, 0, count) == count)
                {
                  HttpRuntime._theRuntime._appOfflineMessage = buffer;
                  flag = true;
                }
              }
              else
              {
                flag = true;
                HttpRuntime._theRuntime._appOfflineMessage = new byte[0];
              }
            }
          }
        }
      }
      catch
      {
      }
      if (flag)
        throw new HttpException(503, string.Empty);
      if (!RuntimeConfig.GetAppConfig().HttpRuntime.Enable)
        throw new HttpException(404, string.Empty);
    }

    internal static void IncrementActivePipelineCount()
    {
      Interlocked.Increment(ref HttpRuntime._theRuntime._activeRequestCount);
      HostingEnvironment.IncrementBusyCount();
    }

    internal static void DecrementActivePipelineCount()
    {
      HostingEnvironment.DecrementBusyCount();
      Interlocked.Decrement(ref HttpRuntime._theRuntime._activeRequestCount);
    }

    internal static void PopulateIISVersionInformation()
    {
      if (!HttpRuntime.IsEngineLoaded)
        return;
      uint pdwVersion;
      bool pfIsIntegratedMode;
      UnsafeIISMethods.MgdGetIISVersionInformation(out pdwVersion, out pfIsIntegratedMode);
      if ((int) pdwVersion == 0)
        return;
      HttpRuntime._iisVersion = new Version((int) (pdwVersion >> 16), (int) pdwVersion & (int) ushort.MaxValue);
      HttpRuntime._useIntegratedPipeline = pfIsIntegratedMode;
    }

    internal static RequestNotificationStatus ProcessRequestNotification(IIS7WorkerRequest wr, HttpContext context)
    {
      return HttpRuntime._theRuntime.ProcessRequestNotificationPrivate(wr, context);
    }

    internal static void FinishPipelineRequest(HttpContext context)
    {
      HttpRuntime._theRuntime._firstRequestCompleted = true;
      context.RaiseOnRequestCompleted();
      context.Request.Dispose();
      context.Response.Dispose();
      HttpApplication applicationInstance = context.ApplicationInstance;
      if (applicationInstance != null)
      {
        System.Web.ThreadContext threadContext = context.IndicateCompletionContext;
        if (threadContext != null && !threadContext.HasBeenDisassociatedFromThread)
        {
          lock (threadContext)
          {
            if (!threadContext.HasBeenDisassociatedFromThread)
            {
              threadContext.DisassociateFromCurrentThread();
              context.IndicateCompletionContext = (System.Web.ThreadContext) null;
              context.InIndicateCompletion = false;
            }
          }
        }
        applicationInstance.ReleaseAppInstance();
      }
      HttpRuntime.SetExecutionTimePerformanceCounter(context);
      HttpRuntime.UpdatePerfCounters(context.Response.StatusCode);
      if (EtwTrace.IsTraceEnabled(5, 1))
        EtwTrace.Trace(EtwTraceType.ETW_TYPE_END_HANDLER, context.WorkerRequest);
      if (!HttpRuntime.HostingInitFailed)
        return;
      HttpRuntime.ShutdownAppDomain(ApplicationShutdownReason.HostingEnvironment, "HostingInit error");
    }

    internal static void ReportAppOfflineErrorMessage(HttpResponse response, byte[] appOfflineMessage)
    {
      response.StatusCode = 503;
      response.ContentType = "text/html";
      response.AddHeader("Retry-After", "3600");
      response.OutputStream.Write(appOfflineMessage, 0, appOfflineMessage.Length);
    }

    internal static void CoalesceNotifications()
    {
      int num1 = 0;
      int num2 = 0;
      try
      {
        HttpRuntimeSection httpRuntime = RuntimeConfig.GetAppLKGConfig().HttpRuntime;
        if (httpRuntime != null)
        {
          num1 = httpRuntime.WaitChangeNotification;
          num2 = httpRuntime.MaxWaitChangeNotification;
        }
      }
      catch
      {
      }
      if (num1 == 0 || num2 == 0)
        return;
      DateTime dateTime = DateTime.UtcNow.AddSeconds((double) num2);
      try
      {
        while (DateTime.UtcNow < dateTime && !(DateTime.UtcNow > HttpRuntime._theRuntime.LastShutdownAttemptTime.AddSeconds((double) num1)))
          Thread.Sleep(250);
      }
      catch
      {
      }
    }

    internal static void OnAppDomainShutdown(BuildManagerHostUnloadEventArgs e)
    {
      if (HttpRuntime.AppDomainShutdown == null)
        return;
      HttpRuntime.AppDomainShutdown((object) HttpRuntime._theRuntime, e);
    }

    internal static void SetUserForcedShutdown()
    {
      HttpRuntime._theRuntime._userForcedShutdown = true;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal static bool ShutdownAppDomain(ApplicationShutdownReason reason, string message)
    {
      return HttpRuntime.ShutdownAppDomainWithStackTrace(reason, message, (string) null);
    }

    internal static bool ShutdownAppDomainWithStackTrace(ApplicationShutdownReason reason, string message, string stackTrace)
    {
      HttpRuntime.SetShutdownReason(reason, message);
      return HttpRuntime.ShutdownAppDomain(stackTrace);
    }

    internal static void RecoverFromUnexceptedAppDomainUnload()
    {
      if (HttpRuntime._theRuntime._shutdownInProgress)
        return;
      HttpRuntime._theRuntime._shutdownInProgress = true;
      try
      {
        ISAPIRuntime.RemoveThisAppDomainFromUnmanagedTable();
        PipelineRuntime.RemoveThisAppDomainFromUnmanagedTable();
        HttpRuntime.AddAppDomainTraceMessage("AppDomainRestart");
      }
      finally
      {
        HttpRuntime._theRuntime.Dispose();
      }
    }

    internal static void OnConfigChange(string message)
    {
      HttpRuntime.ShutdownAppDomain(ApplicationShutdownReason.ConfigurationChange, message != null ? message : "CONFIG change");
    }

    internal static void SetShutdownReason(ApplicationShutdownReason reason, string message)
    {
      if (HttpRuntime._theRuntime._shutdownReason == ApplicationShutdownReason.None)
        HttpRuntime._theRuntime._shutdownReason = reason;
      HttpRuntime.SetShutdownMessage(message);
    }

    internal static void SetShutdownMessage(string message)
    {
      if (message == null)
        return;
      if (HttpRuntime._theRuntime._shutDownMessage == null)
      {
        HttpRuntime._theRuntime._shutDownMessage = message;
      }
      else
      {
        HttpRuntime httpRuntime = HttpRuntime._theRuntime;
        string str = httpRuntime._shutDownMessage + "\r\n" + message;
        httpRuntime._shutDownMessage = str;
      }
    }

    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Medium)]
    public static void ProcessRequest(HttpWorkerRequest wr)
    {
      if (wr == null)
        throw new ArgumentNullException("wr");
      if (HttpRuntime.UseIntegratedPipeline)
        throw new PlatformNotSupportedException(System.Web.SR.GetString("Method_Not_Supported_By_Iis_Integrated_Mode", new object[1]
        {
          (object) "HttpRuntime.ProcessRequest"
        }));
      else
        HttpRuntime.ProcessRequestNoDemand(wr);
    }

    internal static void ProcessRequestNoDemand(HttpWorkerRequest wr)
    {
      RequestQueue requestQueue = HttpRuntime._theRuntime._requestQueue;
      wr.UpdateInitialCounters();
      if (requestQueue != null)
        wr = requestQueue.GetRequestToExecute(wr);
      if (wr == null)
        return;
      HttpRuntime.CalculateWaitTimeAndUpdatePerfCounter(wr);
      wr.ResetStartTime();
      HttpRuntime.ProcessRequestNow(wr);
    }

    internal static void ProcessRequestNow(HttpWorkerRequest wr)
    {
      HttpRuntime._theRuntime.ProcessRequestInternal(wr);
    }

    internal static void RejectRequestNow(HttpWorkerRequest wr, bool silent)
    {
      HttpRuntime._theRuntime.RejectRequestInternal(wr, silent);
    }

    [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
    public static void Close()
    {
      if (!HttpRuntime._theRuntime.InitiateShutdownOnce())
        return;
      HttpRuntime.SetShutdownReason(ApplicationShutdownReason.HttpRuntimeClose, "HttpRuntime.Close is called");
      if (HostingEnvironment.IsHosted)
        HostingEnvironment.InitiateShutdownWithoutDemand();
      else
        HttpRuntime._theRuntime.Dispose();
    }

    public static void UnloadAppDomain()
    {
      HttpRuntime._theRuntime._userForcedShutdown = true;
      HttpRuntime.ShutdownAppDomain(ApplicationShutdownReason.UnloadAppDomainCalled, "User code called UnloadAppDomain");
    }

    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Unrestricted)]
    public static NamedPermissionSet GetNamedPermissionSet()
    {
      NamedPermissionSet permSet = HttpRuntime._theRuntime._namedPermissionSet;
      if (permSet == null)
        return (NamedPermissionSet) null;
      else
        return new NamedPermissionSet(permSet);
    }

    internal static void CheckVirtualFilePermission(string virtualPath)
    {
      HttpRuntime.CheckFilePermission(HostingEnvironment.MapPath(virtualPath));
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal static void CheckFilePermission(string path)
    {
      HttpRuntime.CheckFilePermission(path, false);
    }

    internal static void CheckFilePermission(string path, bool writePermissions)
    {
      if (HttpRuntime.HasFilePermission(path, writePermissions))
        return;
      throw new HttpException(System.Web.SR.GetString("Access_denied_to_path", new object[1]
      {
        (object) HttpRuntime.GetSafePath(path)
      }));
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal static bool HasFilePermission(string path)
    {
      return HttpRuntime.HasFilePermission(path, false);
    }

    internal static bool HasFilePermission(string path, bool writePermissions)
    {
      if (HttpRuntime.TrustLevel == null && HttpRuntime.InitializationException != null || HttpRuntime.NamedPermissionSet == null)
        return true;
      bool flag = false;
      IPermission permission1 = HttpRuntime.NamedPermissionSet.GetPermission(typeof (FileIOPermission));
      if (permission1 != null)
      {
        IPermission permission2;
        try
        {
          permission2 = writePermissions ? (IPermission) new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write | FileIOPermissionAccess.Append | FileIOPermissionAccess.PathDiscovery, path) : (IPermission) new FileIOPermission(FileIOPermissionAccess.Read, path);
        }
        catch
        {
          return false;
        }
        flag = permission2.IsSubsetOf(permission1);
      }
      return flag;
    }

    internal static bool HasWebPermission(Uri uri)
    {
      if (HttpRuntime.NamedPermissionSet == null)
        return true;
      bool flag = false;
      IPermission permission1 = HttpRuntime.NamedPermissionSet.GetPermission(typeof (WebPermission));
      if (permission1 != null)
      {
        IPermission permission2;
        try
        {
          permission2 = (IPermission) new WebPermission(NetworkAccess.Connect, uri.ToString());
        }
        catch
        {
          return false;
        }
        flag = permission2.IsSubsetOf(permission1);
      }
      return flag;
    }

    internal static bool HasDbPermission(DbProviderFactory factory)
    {
      if (HttpRuntime.NamedPermissionSet == null)
        return true;
      bool flag = false;
      CodeAccessPermission permission1 = factory.CreatePermission(PermissionState.Unrestricted);
      if (permission1 != null)
      {
        IPermission permission2 = HttpRuntime.NamedPermissionSet.GetPermission(permission1.GetType());
        if (permission2 != null)
          flag = permission1.IsSubsetOf(permission2);
      }
      return flag;
    }

    internal static bool HasPathDiscoveryPermission(string path)
    {
      if (HttpRuntime.TrustLevel == null && HttpRuntime.InitializationException != null || HttpRuntime.NamedPermissionSet == null)
        return true;
      bool flag = false;
      IPermission permission = HttpRuntime.NamedPermissionSet.GetPermission(typeof (FileIOPermission));
      if (permission != null)
        flag = new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).IsSubsetOf(permission);
      return flag;
    }

    internal static bool HasAppPathDiscoveryPermission()
    {
      return HttpRuntime.HasPathDiscoveryPermission(HttpRuntime.AppDomainAppPathInternal);
    }

    internal static string GetSafePath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      try
      {
        if (HttpRuntime.HasPathDiscoveryPermission(path))
          return path;
      }
      catch
      {
      }
      return Path.GetFileName(path);
    }

    internal static bool HasUnmanagedPermission()
    {
      if (HttpRuntime.NamedPermissionSet == null)
        return true;
      SecurityPermission securityPermission = (SecurityPermission) HttpRuntime.NamedPermissionSet.GetPermission(typeof (SecurityPermission));
      if (securityPermission == null)
        return false;
      else
        return (securityPermission.Flags & SecurityPermissionFlag.UnmanagedCode) != SecurityPermissionFlag.NoFlags;
    }

    internal static bool HasAspNetHostingPermission(AspNetHostingPermissionLevel level)
    {
      if (HttpRuntime.NamedPermissionSet == null)
        return true;
      AspNetHostingPermission hostingPermission = (AspNetHostingPermission) HttpRuntime.NamedPermissionSet.GetPermission(typeof (AspNetHostingPermission));
      if (hostingPermission == null)
        return false;
      else
        return hostingPermission.Level >= level;
    }

    internal static void CheckAspNetHostingPermission(AspNetHostingPermissionLevel level, string errorMessageId)
    {
      if (!HttpRuntime.HasAspNetHostingPermission(level))
        throw new HttpException(System.Web.SR.GetString(errorMessageId));
    }

    internal static void FailIfNoAPTCABit(Type t, ElementInformation elemInfo, string propertyName)
    {
      if (HttpRuntime.IsTypeAllowedInConfig(t))
        return;
      if (elemInfo != null)
      {
        PropertyInformation propertyInformation = elemInfo.Properties[propertyName];
        throw new ConfigurationErrorsException(System.Web.SR.GetString("Type_from_untrusted_assembly", new object[1]
        {
          (object) t.FullName
        }), propertyInformation.Source, propertyInformation.LineNumber);
      }
      else
        throw new ConfigurationErrorsException(System.Web.SR.GetString("Type_from_untrusted_assembly", new object[1]
        {
          (object) t.FullName
        }));
    }

    internal static void FailIfNoAPTCABit(Type t, XmlNode node)
    {
      if (HttpRuntime.IsTypeAllowedInConfig(t))
        return;
      throw new ConfigurationErrorsException(System.Web.SR.GetString("Type_from_untrusted_assembly", new object[1]
      {
        (object) t.FullName
      }), node);
    }

    internal static bool IsTypeAllowedInConfig(Type t)
    {
      if (HttpRuntime.HasAspNetHostingPermission(AspNetHostingPermissionLevel.Unrestricted))
        return true;
      else
        return HttpRuntime.IsTypeAccessibleFromPartialTrust(t);
    }

    internal static bool IsTypeAccessibleFromPartialTrust(Type t)
    {
      Assembly assembly = t.Assembly;
      if (assembly.SecurityRuleSet == SecurityRuleSet.Level1)
      {
        if (assembly.IsFullyTrusted)
          return HttpRuntime.HasAPTCABit(assembly);
        else
          return true;
      }
      else if (HttpRuntime.HasAPTCABit(assembly) || t.IsSecurityTransparent)
        return true;
      else
        return t.IsSecuritySafeCritical;
    }

    internal static bool IsPathWithinAppRoot(string path)
    {
      if (HttpRuntime.AppDomainIdInternal == null)
        return true;
      else
        return System.Web.Util.UrlPath.IsEqualOrSubpath(HttpRuntime.AppDomainAppVirtualPathString, path);
    }

    internal static void AddAppDomainTraceMessage(string message)
    {
      AppDomain domain = Thread.GetDomain();
      string str = domain.GetData("ASP.NET Domain Trace") as string;
      domain.SetData("ASP.NET Domain Trace", str != null ? (object) (str + " ... " + message) : (object) message);
    }

    internal static string MakeFileUrl(string path)
    {
      return new Uri(path).ToString();
    }

    internal static string GetGacLocation()
    {
      StringBuilder pwzCachePath = new StringBuilder(262);
      int pcchPath = 260;
      if (System.Web.UnsafeNativeMethods.GetCachePath(2, pwzCachePath, ref pcchPath) >= 0)
        return ((object) pwzCachePath).ToString();
      else
        throw new HttpException(System.Web.SR.GetString("GetGacLocaltion_failed"));
    }

    internal static void RestrictIISFolders(HttpContext context)
    {
      HttpWorkerRequest workerRequest = context.WorkerRequest;
      if (workerRequest == null || !(workerRequest is ISAPIWorkerRequest) || workerRequest is ISAPIWorkerRequestInProcForIIS6)
        return;
      byte[] bufOut = new byte[1];
      byte[] bytes = BitConverter.GetBytes(1);
      context.CallISAPI(System.Web.UnsafeNativeMethods.CallISAPIFunc.RestrictIISFolders, bytes, bufOut);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal static object CreateNonPublicInstance(Type type)
    {
      return HttpRuntime.CreateNonPublicInstance(type, (object[]) null);
    }

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    internal static object CreateNonPublicInstance(Type type, object[] args)
    {
      return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, args, (CultureInfo) null);
    }

    internal static object CreatePublicInstance(Type type)
    {
      return Activator.CreateInstance(type);
    }

    internal static object FastCreatePublicInstance(Type type)
    {
      if (!type.Assembly.GlobalAssemblyCache)
        return HttpRuntime.CreatePublicInstance(type);
      if (!HttpRuntime.s_initializedFactory)
      {
        lock (HttpRuntime.s_factoryLock)
        {
          if (!HttpRuntime.s_initializedFactory)
          {
            HttpRuntime.s_factoryGenerator = new FactoryGenerator();
            HttpRuntime.s_factoryCache = Hashtable.Synchronized(new Hashtable());
            HttpRuntime.s_initializedFactory = true;
          }
        }
      }
      IWebObjectFactory webObjectFactory = (IWebObjectFactory) HttpRuntime.s_factoryCache[(object) type];
      if (webObjectFactory == null)
      {
        webObjectFactory = HttpRuntime.s_factoryGenerator.CreateFactory(type);
        HttpRuntime.s_factoryCache[(object) type] = (object) webObjectFactory;
      }
      return webObjectFactory.CreateInstance();
    }

    internal static object CreatePublicInstance(Type type, object[] args)
    {
      if (args == null)
        return Activator.CreateInstance(type);
      else
        return Activator.CreateInstance(type, args);
    }

    internal static string GetRelaxedMapPathResult(string originalResult)
    {
      if (!HttpRuntime.IsMapPathRelaxed)
        return originalResult;
      if (originalResult == null)
        return HttpRuntime._DefaultPhysicalPathOnMapPathFailure;
      if (originalResult.IndexOfAny(HttpRuntime.s_InvalidPhysicalPathChars) >= 0)
        return HttpRuntime._DefaultPhysicalPathOnMapPathFailure;
      try
      {
        bool pathTooLong;
        if (!System.Web.Util.FileUtil.IsSuspiciousPhysicalPath(originalResult, out pathTooLong))
        {
          if (!pathTooLong)
            goto label_10;
        }
        return HttpRuntime._DefaultPhysicalPathOnMapPathFailure;
      }
      catch
      {
        return HttpRuntime._DefaultPhysicalPathOnMapPathFailure;
      }
label_10:
      return originalResult;
    }

    private static void StaticInit()
    {
      if (HttpRuntime.s_initialized)
        return;
      bool flag1 = false;
      bool flag2 = false;
      string runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
      if (System.Web.UnsafeNativeMethods.GetModuleHandle("webengine4.dll") != IntPtr.Zero)
        flag1 = true;
      if (!flag1 && System.Web.UnsafeNativeMethods.LoadLibrary(runtimeDirectory + (object) Path.DirectorySeparatorChar + "webengine4.dll") != IntPtr.Zero)
      {
        flag1 = true;
        flag2 = true;
      }
      if (flag1)
      {
        System.Web.UnsafeNativeMethods.InitializeLibrary(false);
        if (flag2)
          System.Web.UnsafeNativeMethods.PerfCounterInitialize();
      }
      HttpRuntime.s_installDirectory = runtimeDirectory;
      HttpRuntime.s_isEngineLoaded = flag1;
      HttpRuntime.s_initialized = true;
      HttpRuntime.PopulateIISVersionInformation();
      HttpRuntime.AddAppDomainTraceMessage("Initialize");
    }

    private void Init()
    {
      try
      {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
          throw new PlatformNotSupportedException(System.Web.SR.GetString("RequiresNT"));
        this._profiler = new Profiler();
        this._timeoutManager = new RequestTimeoutManager();
        this._wpUserId = HttpRuntime.GetCurrentUserName();
        this._requestNotificationCompletionCallback = new AsyncCallback(this.OnRequestNotificationCompletion);
        this._handlerCompletionCallback = new AsyncCallback(this.OnHandlerCompletion);
        this._asyncEndOfSendCallback = new HttpWorkerRequest.EndOfSendNotification(this.EndOfSendCallback);
        this._appDomainUnloadallback = new WaitCallback(this.ReleaseResourcesAndUnloadAppDomain);
        if (HttpRuntime.GetAppDomainString(".appDomain") != null)
        {
          this._appDomainAppId = HttpRuntime.GetAppDomainString(".appId");
          this._appDomainAppPath = HttpRuntime.GetAppDomainString(".appPath");
          this._appDomainAppVPath = VirtualPath.CreateNonRelativeTrailingSlash(HttpRuntime.GetAppDomainString(".appVPath"));
          this._appDomainId = HttpRuntime.GetAppDomainString(".domainId");
          this._isOnUNCShare = System.Web.Util.StringUtil.StringStartsWith(this._appDomainAppPath, "\\\\");
          PerfCounters.Open(this._appDomainAppId);
        }
        this._fcm = new FileChangesMonitor(HostingEnvironment.FcnMode);
      }
      catch (Exception ex)
      {
        HttpRuntime.InitializationException = ex;
      }
    }

    private void SetUpDataDirectory()
    {
      string path = Path.Combine(this._appDomainAppPath, "App_Data");
      AppDomain.CurrentDomain.SetData("DataDirectory", (object) path, (IPermission) new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path));
    }

    private void DisposeAppDomainShutdownTimer()
    {
      System.Threading.Timer comparand = this._appDomainShutdownTimer;
      if (comparand == null || Interlocked.CompareExchange<System.Threading.Timer>(ref this._appDomainShutdownTimer, (System.Threading.Timer) null, comparand) != comparand)
        return;
      comparand.Dispose();
    }

    private void AppDomainShutdownTimerCallback(object state)
    {
      try
      {
        this.DisposeAppDomainShutdownTimer();
        HttpRuntime.ShutdownAppDomain(ApplicationShutdownReason.InitializationError, "Initialization Error");
      }
      catch
      {
      }
    }

    private void StartAppDomainShutdownTimer()
    {
      if (this._appDomainShutdownTimer != null || this._shutdownInProgress)
        return;
      lock (this)
      {
        if (this._appDomainShutdownTimer != null || this._shutdownInProgress)
          return;
        this._appDomainShutdownTimer = new System.Threading.Timer(new TimerCallback(this.AppDomainShutdownTimerCallback), (object) null, 10000, 0);
      }
    }

    private void HostingInit(HostingEnvironmentFlags hostingFlags, PolicyLevel policyLevel, Exception appDomainCreationException)
    {
      using (new ApplicationImpersonationContext())
      {
        try
        {
          this._firstRequestStartTime = DateTime.UtcNow;
          this.SetUpDataDirectory();
          this.EnsureAccessToApplicationDirectory();
          this.StartMonitoringDirectoryRenamesAndBinDirectory();
          if (HttpRuntime.InitializationException == null)
            HostingEnvironment.InitializeObjectCacheHost();
          CacheSection cacheSection;
          TrustSection trustSection;
          SecurityPolicySection securityPolicySection;
          CompilationSection compilationSection;
          HostingEnvironmentSection hostingEnvironmentSection;
          Exception initException;
          this.GetInitConfigSections(out cacheSection, out trustSection, out securityPolicySection, out compilationSection, out hostingEnvironmentSection, out initException);
          HttpRuntime.CacheInternal.ReadCacheInternalConfig(cacheSection);
          this.SetUpCodegenDirectory(compilationSection);
          if (compilationSection != null)
          {
            HttpRuntime._enablePrefetchOptimization = compilationSection.EnablePrefetchOptimization;
            if (HttpRuntime._enablePrefetchOptimization)
              System.Web.UnsafeNativeMethods.StartPrefetchActivity((uint) System.Web.Util.StringUtil.GetStringHashCode(this._appDomainAppId));
          }
          if (appDomainCreationException != null)
            throw appDomainCreationException;
          if (trustSection == null || string.IsNullOrEmpty(trustSection.Level))
          {
            throw new ConfigurationErrorsException(System.Web.SR.GetString("Config_section_not_present", new object[1]
            {
              (object) "trust"
            }));
          }
          else
          {
            if (trustSection.LegacyCasModel)
            {
              try
              {
                this._disableProcessRequestInApplicationTrust = false;
                this._isLegacyCas = true;
                this.SetTrustLevel(trustSection, securityPolicySection);
              }
              catch
              {
                if (initException != null)
                  throw initException;
                throw;
              }
            }
            else if ((hostingFlags & HostingEnvironmentFlags.ClientBuildManager) != HostingEnvironmentFlags.Default)
            {
              this._trustLevel = "Full";
            }
            else
            {
              this._disableProcessRequestInApplicationTrust = true;
              this.SetTrustParameters(trustSection, securityPolicySection, policyLevel);
            }
            this.InitFusion(hostingEnvironmentSection);
            CachedPathData.InitializeUrlMetadataSlidingExpiration(hostingEnvironmentSection);
            HttpConfigurationSystem.CompleteInit();
            if (initException != null)
              throw initException;
            this.SetThreadPoolLimits();
            HttpRuntime.SetAutogenKeys();
            BuildManager.InitializeBuildManager();
            if (compilationSection != null && compilationSection.ProfileGuidedOptimizations == ProfileGuidedOptimizationsFlags.All)
            {
              ProfileOptimization.SetProfileRoot(this._codegenDir);
              ProfileOptimization.StartProfile("profileoptimization.prof");
            }
            this.InitApartmentThreading();
            this.InitDebuggingSupport();
            this._processRequestInApplicationTrust = trustSection.ProcessRequestInApplicationTrust;
            AppDomainResourcePerfCounters.Init();
            this.RelaxMapPathIfRequired();
          }
        }
        catch (Exception ex)
        {
          this._hostingInitFailed = true;
          HttpRuntime.InitializationException = ex;
          if ((hostingFlags & HostingEnvironmentFlags.ThrowHostingInitErrors) == HostingEnvironmentFlags.Default)
            return;
          throw;
        }
      }
    }

    private void FirstRequestInit(HttpContext context)
    {
      Exception exception = (Exception) null;
      if (HttpRuntime.InitializationException == null)
      {
        if (this._appDomainId != null)
        {
          try
          {
            using (new ApplicationImpersonationContext())
            {
              CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
              CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
              try
              {
                HttpRuntime.InitHttpConfiguration();
                HttpRuntime.CheckApplicationEnabled();
                this.CheckAccessToTempDirectory();
                this.InitializeHealthMonitoring();
                this.InitRequestQueue();
                this.InitTrace(context);
                HealthMonitoringManager.StartHealthMonitoringHeartbeat();
                HttpRuntime.RestrictIISFolders(context);
                this.PreloadAssembliesFromBin();
                this.InitHeaderEncoding();
                HttpEncoder.InitializeOnFirstRequest();
                RequestValidator.InitializeOnFirstRequest();
                if (context.WorkerRequest is ISAPIWorkerRequestOutOfProc)
                {
                  ProcessModelSection processModel = RuntimeConfig.GetMachineConfig().ProcessModel;
                }
              }
              finally
              {
                Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                HttpRuntime.SetCurrentThreadCultureWithAssert(currentCulture);
              }
            }
          }
          catch (ConfigurationException ex)
          {
            exception = (Exception) ex;
          }
          catch (Exception ex)
          {
            exception = (Exception) new HttpException(System.Web.SR.GetString("XSP_init_error", new object[1]
            {
              (object) ex.Message
            }), ex);
          }
        }
      }
      if (HttpRuntime.InitializationException != null)
        throw new HttpException(HttpRuntime.InitializationException.Message, HttpRuntime.InitializationException);
      if (exception != null)
      {
        HttpRuntime.InitializationException = exception;
        throw exception;
      }
      else
        HttpRuntime.AddAppDomainTraceMessage("FirstRequestInit");
    }

    private void EnsureFirstRequestInit(HttpContext context)
    {
      if (!this._beforeFirstRequest)
        return;
      lock (this)
      {
        if (!this._beforeFirstRequest)
          return;
        this._firstRequestStartTime = DateTime.UtcNow;
        this.FirstRequestInit(context);
        this._beforeFirstRequest = false;
        context.FirstRequest = true;
      }
    }

    private void EnsureAccessToApplicationDirectory()
    {
      if (System.Web.Util.FileUtil.DirectoryAccessible(this._appDomainAppPath))
        return;
      if (this._appDomainAppPath.IndexOf('?') >= 0)
        throw new HttpException(System.Web.SR.GetString("Access_denied_to_unicode_app_dir", new object[1]
        {
          (object) this._appDomainAppPath
        }));
      else
        throw new HttpException(System.Web.SR.GetString("Access_denied_to_app_dir", new object[1]
        {
          (object) this._appDomainAppPath
        }));
    }

    private void StartMonitoringDirectoryRenamesAndBinDirectory()
    {
      this._fcm.StartMonitoringDirectoryRenamesAndBinDirectory(HttpRuntime.AppDomainAppPathInternal, new FileChangeEventHandler(this.OnCriticalDirectoryChange));
    }

    private void GetInitConfigSections(out CacheSection cacheSection, out TrustSection trustSection, out SecurityPolicySection securityPolicySection, out CompilationSection compilationSection, out HostingEnvironmentSection hostingEnvironmentSection, out Exception initException)
    {
      cacheSection = (CacheSection) null;
      trustSection = (TrustSection) null;
      securityPolicySection = (SecurityPolicySection) null;
      compilationSection = (CompilationSection) null;
      hostingEnvironmentSection = (HostingEnvironmentSection) null;
      initException = (Exception) null;
      RuntimeConfig appLkgConfig = RuntimeConfig.GetAppLKGConfig();
      RuntimeConfig runtimeConfig = (RuntimeConfig) null;
      try
      {
        runtimeConfig = RuntimeConfig.GetAppConfig();
      }
      catch (Exception ex)
      {
        initException = ex;
      }
      if (runtimeConfig != null)
      {
        try
        {
          cacheSection = runtimeConfig.Cache;
        }
        catch (Exception ex)
        {
          if (initException == null)
            initException = ex;
        }
      }
      if (cacheSection == null)
        cacheSection = appLkgConfig.Cache;
      if (runtimeConfig != null)
      {
        try
        {
          trustSection = runtimeConfig.Trust;
        }
        catch (Exception ex)
        {
          if (initException == null)
            initException = ex;
        }
      }
      if (trustSection == null)
        trustSection = appLkgConfig.Trust;
      if (runtimeConfig != null)
      {
        try
        {
          securityPolicySection = runtimeConfig.SecurityPolicy;
        }
        catch (Exception ex)
        {
          if (initException == null)
            initException = ex;
        }
      }
      if (securityPolicySection == null)
        securityPolicySection = appLkgConfig.SecurityPolicy;
      if (runtimeConfig != null)
      {
        try
        {
          compilationSection = runtimeConfig.Compilation;
        }
        catch (Exception ex)
        {
          if (initException == null)
            initException = ex;
        }
      }
      if (compilationSection == null)
        compilationSection = appLkgConfig.Compilation;
      if (runtimeConfig != null)
      {
        try
        {
          hostingEnvironmentSection = runtimeConfig.HostingEnvironment;
        }
        catch (Exception ex)
        {
          if (initException == null)
            initException = ex;
        }
      }
      if (hostingEnvironmentSection != null)
        return;
      hostingEnvironmentSection = appLkgConfig.HostingEnvironment;
    }

    private void SetUpCodegenDirectory(CompilationSection compilationSection)
    {
      AppDomain domain = Thread.GetDomain();
      string path2 = AppManagerAppDomainFactory.ConstructSimpleAppName(HttpRuntime.AppDomainAppVirtualPath);
      string str1 = (string) null;
      string tempDirAttribName = (string) null;
      string configFileName = (string) null;
      int configLineNumber = 0;
      if (compilationSection != null && !string.IsNullOrEmpty(compilationSection.TempDirectory))
      {
        str1 = compilationSection.TempDirectory;
        compilationSection.GetTempDirectoryErrorInfo(out tempDirAttribName, out configFileName, out configLineNumber);
      }
      string str2;
      if (str1 != null)
      {
        string path = str1.Trim();
        if (!Path.IsPathRooted(path))
        {
          str2 = (string) null;
        }
        else
        {
          try
          {
            str2 = new DirectoryInfo(path).FullName;
          }
          catch
          {
            str2 = (string) null;
          }
        }
        if (str2 == null)
        {
          throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_temp_directory", new object[1]
          {
            (object) tempDirAttribName
          }), configFileName, configLineNumber);
        }
        else
        {
          try
          {
            Directory.CreateDirectory(str2);
          }
          catch (Exception ex)
          {
            throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_temp_directory", new object[1]
            {
              (object) tempDirAttribName
            }), ex, configFileName, configLineNumber);
          }
        }
      }
      else
        str2 = Path.Combine(HttpRuntime.s_installDirectory, "Temporary ASP.NET Files");
      if (!Util.HasWriteAccessToDirectory(str2))
      {
        if (!Environment.UserInteractive)
          throw new HttpException(System.Web.SR.GetString("No_codegen_access", (object) Util.GetCurrentAccountName(), (object) str2));
        else
          str2 = Path.Combine(Path.GetTempPath(), "Temporary ASP.NET Files");
      }
      this._tempDir = str2;
      string path1 = Path.Combine(str2, path2);
      domain.SetDynamicBase(path1);
      this._codegenDir = Thread.GetDomain().DynamicDirectory;
      Directory.CreateDirectory(this._codegenDir);
    }

    private void InitFusion(HostingEnvironmentSection hostingEnvironmentSection)
    {
      AppDomain domain = Thread.GetDomain();
      string str = this._appDomainAppPath;
      if (str.IndexOf(HttpRuntime.DoubleDirectorySeparatorString, 1, StringComparison.Ordinal) >= 1)
        str = (string) (object) str[0] + (object) str.Substring(1).Replace(HttpRuntime.DoubleDirectorySeparatorString, HttpRuntime.DirectorySeparatorString);
      domain.AppendPrivatePath(str + "bin");
      if (hostingEnvironmentSection != null && !hostingEnvironmentSection.ShadowCopyBinAssemblies)
        domain.ClearShadowCopyPath();
      else
        domain.SetShadowCopyPath(str + "bin");
      string fullName = Directory.GetParent(this._codegenDir).FullName;
      domain.SetCachePath(fullName);
      this._fusionInited = true;
    }

    private void InitRequestQueue()
    {
      RuntimeConfig appConfig = RuntimeConfig.GetAppConfig();
      HttpRuntimeSection httpRuntime = appConfig.HttpRuntime;
      ProcessModelSection processModel = appConfig.ProcessModel;
      if (processModel.AutoConfig)
      {
        this._requestQueue = new RequestQueue(88 * processModel.CpuCount, 76 * processModel.CpuCount, httpRuntime.AppRequestQueueLimit, processModel.ClientConnectedCheck);
      }
      else
      {
        int num = processModel.MaxWorkerThreadsTimesCpuCount < processModel.MaxIoThreadsTimesCpuCount ? processModel.MaxWorkerThreadsTimesCpuCount : processModel.MaxIoThreadsTimesCpuCount;
        if (httpRuntime.MinFreeThreads >= num)
        {
          if (httpRuntime.ElementInformation.Properties["minFreeThreads"].LineNumber == 0)
          {
            if (processModel.ElementInformation.Properties["maxWorkerThreads"].LineNumber != 0)
              throw new ConfigurationErrorsException(System.Web.SR.GetString("Thread_pool_limit_must_be_greater_than_minFreeThreads", new object[1]
              {
                (object) httpRuntime.MinFreeThreads.ToString((IFormatProvider) CultureInfo.InvariantCulture)
              }), processModel.ElementInformation.Properties["maxWorkerThreads"].Source, processModel.ElementInformation.Properties["maxWorkerThreads"].LineNumber);
            else
              throw new ConfigurationErrorsException(System.Web.SR.GetString("Thread_pool_limit_must_be_greater_than_minFreeThreads", new object[1]
              {
                (object) httpRuntime.MinFreeThreads.ToString((IFormatProvider) CultureInfo.InvariantCulture)
              }), processModel.ElementInformation.Properties["maxIoThreads"].Source, processModel.ElementInformation.Properties["maxIoThreads"].LineNumber);
          }
          else
            throw new ConfigurationErrorsException(System.Web.SR.GetString("Min_free_threads_must_be_under_thread_pool_limits", new object[1]
            {
              (object) num.ToString((IFormatProvider) CultureInfo.InvariantCulture)
            }), httpRuntime.ElementInformation.Properties["minFreeThreads"].Source, httpRuntime.ElementInformation.Properties["minFreeThreads"].LineNumber);
        }
        else if (httpRuntime.MinLocalRequestFreeThreads > httpRuntime.MinFreeThreads)
        {
          if (httpRuntime.ElementInformation.Properties["minLocalRequestFreeThreads"].LineNumber == 0)
            throw new ConfigurationErrorsException(System.Web.SR.GetString("Local_free_threads_cannot_exceed_free_threads"), processModel.ElementInformation.Properties["minFreeThreads"].Source, processModel.ElementInformation.Properties["minFreeThreads"].LineNumber);
          else
            throw new ConfigurationErrorsException(System.Web.SR.GetString("Local_free_threads_cannot_exceed_free_threads"), httpRuntime.ElementInformation.Properties["minLocalRequestFreeThreads"].Source, httpRuntime.ElementInformation.Properties["minLocalRequestFreeThreads"].LineNumber);
        }
        else
          this._requestQueue = new RequestQueue(httpRuntime.MinFreeThreads, httpRuntime.MinLocalRequestFreeThreads, httpRuntime.AppRequestQueueLimit, processModel.ClientConnectedCheck);
      }
    }

    private void InitApartmentThreading()
    {
      HttpRuntimeSection httpRuntime = RuntimeConfig.GetAppConfig().HttpRuntime;
      if (httpRuntime != null)
        this._apartmentThreading = httpRuntime.ApartmentThreading;
      else
        this._apartmentThreading = false;
    }

    private void InitTrace(HttpContext context)
    {
      System.Web.Configuration.TraceSection trace = RuntimeConfig.GetAppConfig().Trace;
      HttpRuntime.Profile.RequestsToProfile = trace.RequestLimit;
      HttpRuntime.Profile.PageOutput = trace.PageOutput;
      HttpRuntime.Profile.OutputMode = TraceMode.SortByTime;
      if (trace.TraceMode == TraceDisplayMode.SortByCategory)
        HttpRuntime.Profile.OutputMode = TraceMode.SortByCategory;
      HttpRuntime.Profile.LocalOnly = trace.LocalOnly;
      HttpRuntime.Profile.IsEnabled = trace.Enabled;
      HttpRuntime.Profile.MostRecent = trace.MostRecent;
      HttpRuntime.Profile.Reset();
      context.TraceIsEnabled = trace.Enabled;
      TraceContext.SetWriteToDiagnosticsTrace(trace.WriteToDiagnosticsTrace);
    }

    private void InitDebuggingSupport()
    {
      this._debuggingEnabled = RuntimeConfig.GetAppConfig().Compilation.Debug;
    }

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    private void PreloadAssembliesFromBin()
    {
      bool flag = false;
      if (!this._isOnUNCShare)
      {
        IdentitySection identity = RuntimeConfig.GetAppConfig().Identity;
        if (identity.Impersonate && identity.ImpersonateToken == IntPtr.Zero)
          flag = true;
      }
      if (!flag)
        return;
      DirectoryInfo dirInfo = new DirectoryInfo(HttpRuntime.BinDirectoryInternal);
      if (!dirInfo.Exists)
        return;
      this.PreloadAssembliesFromBinRecursive(dirInfo);
    }

    private void PreloadAssembliesFromBinRecursive(DirectoryInfo dirInfo)
    {
      foreach (FileInfo fileInfo in dirInfo.GetFiles("*.dll"))
      {
        try
        {
          Assembly.Load(Util.GetAssemblyNameFromFileName(fileInfo.Name));
        }
        catch (FileNotFoundException ex)
        {
          try
          {
            Assembly.LoadFrom(fileInfo.FullName);
          }
          catch
          {
          }
        }
        catch
        {
        }
      }
      foreach (DirectoryInfo dirInfo1 in dirInfo.GetDirectories())
        this.PreloadAssembliesFromBinRecursive(dirInfo1);
    }

    private void SetAutoConfigLimits(ProcessModelSection pmConfig)
    {
      int workerThreads;
      int completionPortThreads;
      ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
      if (pmConfig.DefaultMaxWorkerThreadsForAutoConfig != workerThreads || pmConfig.DefaultMaxIoThreadsForAutoConfig != completionPortThreads)
        System.Web.UnsafeNativeMethods.SetClrThreadPoolLimits(pmConfig.DefaultMaxWorkerThreadsForAutoConfig, pmConfig.DefaultMaxIoThreadsForAutoConfig, true);
      ServicePointManager.DefaultConnectionLimit = int.MaxValue;
    }

    private void SetThreadPoolLimits()
    {
      try
      {
        ProcessModelSection processModel = RuntimeConfig.GetMachineConfig().ProcessModel;
        if (processModel.AutoConfig)
          this.SetAutoConfigLimits(processModel);
        else if (processModel.MaxWorkerThreadsTimesCpuCount > 0 && processModel.MaxIoThreadsTimesCpuCount > 0)
        {
          int workerThreads;
          int completionPortThreads;
          ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
          if (processModel.MaxWorkerThreadsTimesCpuCount != workerThreads || processModel.MaxIoThreadsTimesCpuCount != completionPortThreads)
            System.Web.UnsafeNativeMethods.SetClrThreadPoolLimits(processModel.MaxWorkerThreadsTimesCpuCount, processModel.MaxIoThreadsTimesCpuCount, false);
        }
        if (processModel.MinWorkerThreadsTimesCpuCount <= 0 && processModel.MinIoThreadsTimesCpuCount <= 0)
          return;
        int workerThreads1;
        int completionPortThreads1;
        ThreadPool.GetMinThreads(out workerThreads1, out completionPortThreads1);
        int workerThreads2 = processModel.MinWorkerThreadsTimesCpuCount > 0 ? processModel.MinWorkerThreadsTimesCpuCount : workerThreads1;
        int completionPortThreads2 = processModel.MinIoThreadsTimesCpuCount > 0 ? processModel.MinIoThreadsTimesCpuCount : completionPortThreads1;
        if (workerThreads2 <= 0 || completionPortThreads2 <= 0 || workerThreads2 == workerThreads1 && completionPortThreads2 == completionPortThreads1)
          return;
        ThreadPool.SetMinThreads(workerThreads2, completionPortThreads2);
      }
      catch
      {
      }
    }

    [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
    private void CheckAccessToTempDirectory()
    {
      if (!HostingEnvironment.HasHostingIdentity)
        return;
      using (new ApplicationImpersonationContext())
      {
        if (Util.HasWriteAccessToDirectory(this._tempDir))
          return;
        throw new HttpException(System.Web.SR.GetString("No_codegen_access", (object) Util.GetCurrentAccountName(), (object) this._tempDir));
      }
    }

    private void InitializeHealthMonitoring()
    {
      ProcessModelSection processModel = RuntimeConfig.GetMachineConfig().ProcessModel;
      System.Web.UnsafeNativeMethods.InitializeHealthMonitor((int) processModel.ResponseDeadlockInterval.TotalSeconds, processModel.RequestQueueLimit);
    }

    private static void InitHttpConfiguration()
    {
      if (HttpRuntime._theRuntime._configInited)
        return;
      HttpRuntime._theRuntime._configInited = true;
      HttpConfigurationSystem.EnsureInit((IConfigMapPath) null, true, true);
      GlobalizationSection globalization = RuntimeConfig.GetAppLKGConfig().Globalization;
      if (globalization != null)
      {
        if (!string.IsNullOrEmpty(globalization.Culture) && !System.Web.Util.StringUtil.StringStartsWithIgnoreCase(globalization.Culture, "auto"))
          HttpRuntime.SetCurrentThreadCultureWithAssert(HttpServerUtility.CreateReadOnlyCultureInfo(globalization.Culture));
        if (!string.IsNullOrEmpty(globalization.UICulture) && !System.Web.Util.StringUtil.StringStartsWithIgnoreCase(globalization.UICulture, "auto"))
          Thread.CurrentThread.CurrentUICulture = HttpServerUtility.CreateReadOnlyCultureInfo(globalization.UICulture);
      }
      RuntimeConfig appConfig = RuntimeConfig.GetAppConfig();
      ProcessModelSection processModel = appConfig.ProcessModel;
      HostingEnvironmentSection hostingEnvironment = appConfig.HostingEnvironment;
    }

    private void InitHeaderEncoding()
    {
      this._enableHeaderChecking = RuntimeConfig.GetAppConfig().HttpRuntime.EnableHeaderChecking;
    }

    private static void SetAutogenKeys()
    {
      byte[] numArray = new byte[HttpRuntime.s_autogenKeys.Length];
      byte[] bufferOut = new byte[HttpRuntime.s_autogenKeys.Length];
      bool flag = false;
      new RNGCryptoServiceProvider().GetBytes(numArray);
      if (!flag)
        flag = System.Web.UnsafeNativeMethods.EcbCallISAPI(IntPtr.Zero, System.Web.UnsafeNativeMethods.CallISAPIFunc.GetAutogenKeys, numArray, numArray.Length, bufferOut, bufferOut.Length) == 1;
      if (flag)
        Buffer.BlockCopy((Array) bufferOut, 0, (Array) HttpRuntime.s_autogenKeys, 0, HttpRuntime.s_autogenKeys.Length);
      else
        Buffer.BlockCopy((Array) numArray, 0, (Array) HttpRuntime.s_autogenKeys, 0, HttpRuntime.s_autogenKeys.Length);
    }

    private RequestNotificationStatus ProcessRequestNotificationPrivate(IIS7WorkerRequest wr, HttpContext context)
    {
      RequestNotificationStatus status = RequestNotificationStatus.Pending;
      try
      {
        int currentModuleIndex;
        bool isPostNotification;
        int currentNotification;
        UnsafeIISMethods.MgdGetCurrentNotificationInfo(wr.RequestContext, out currentModuleIndex, out isPostNotification, out currentNotification);
        context.CurrentModuleIndex = currentModuleIndex;
        context.IsPostNotification = isPostNotification;
        context.CurrentNotification = (RequestNotification) currentNotification;
        IHttpHandler httpHandler = (IHttpHandler) null;
        if (context.NeedToInitializeApp())
        {
          try
          {
            this.EnsureFirstRequestInit(context);
          }
          catch
          {
            if (!context.Request.IsDebuggingRequest)
              throw;
          }
          context.Response.InitResponseWriter();
          httpHandler = HttpApplicationFactory.GetApplicationInstance(context);
          if (httpHandler == null)
            throw new HttpException(System.Web.SR.GetString("Unable_create_app_object"));
          if (EtwTrace.IsTraceEnabled(5, 1))
            EtwTrace.Trace(EtwTraceType.ETW_TYPE_START_HANDLER, context.WorkerRequest, httpHandler.GetType().FullName, "Start");
          HttpApplication httpApplication = httpHandler as HttpApplication;
          if (httpApplication != null)
            httpApplication.AssignContext(context);
        }
        wr.SynchronizeVariables(context);
        if (context.ApplicationInstance != null)
        {
          if (context.ApplicationInstance.BeginProcessRequestNotification(context, this._requestNotificationCompletionCallback).CompletedSynchronously)
            status = RequestNotificationStatus.Continue;
        }
        else if (httpHandler != null)
        {
          httpHandler.ProcessRequest(context);
          status = RequestNotificationStatus.FinishRequest;
        }
        else
          status = RequestNotificationStatus.Continue;
      }
      catch (Exception ex)
      {
        status = RequestNotificationStatus.FinishRequest;
        context.Response.InitResponseWriter();
        context.AddError(ex);
      }
      if (status != RequestNotificationStatus.Pending)
        this.FinishRequestNotification(wr, context, ref status);
      return status;
    }

    private void FinishRequestNotification(IIS7WorkerRequest wr, HttpContext context, ref RequestNotificationStatus status)
    {
      HttpApplication applicationInstance = context.ApplicationInstance;
      if (context.NotificationContext.RequestCompleted)
        status = RequestNotificationStatus.FinishRequest;
      context.ReportRuntimeErrorIfExists(ref status);
      if (status == RequestNotificationStatus.FinishRequest && (context.CurrentNotification == RequestNotification.LogRequest || context.CurrentNotification == RequestNotification.EndRequest))
        status = RequestNotificationStatus.Continue;
      IntPtr requestContext = wr.RequestContext;
      bool sendHeaders = UnsafeIISMethods.MgdIsLastNotification(requestContext, status);
      try
      {
        context.Response.UpdateNativeResponse(sendHeaders);
      }
      catch (Exception ex)
      {
        wr.UnlockCachedResponseBytes();
        context.AddError(ex);
        context.ReportRuntimeErrorIfExists(ref status);
        context.Response.UpdateNativeResponse(sendHeaders);
      }
      if (sendHeaders)
        context.FinishPipelineRequest();
      if (status == RequestNotificationStatus.Pending)
        return;
      PipelineRuntime.DisposeHandler(context, requestContext, status);
    }

    private void ProcessRequestInternal(HttpWorkerRequest wr)
    {
      Interlocked.Increment(ref this._activeRequestCount);
      if (this._disposingHttpRuntime)
      {
        try
        {
          wr.SendStatus(503, "Server Too Busy");
          wr.SendKnownResponseHeader(12, "text/html; charset=utf-8");
          byte[] bytes = Encoding.ASCII.GetBytes("<html><body>Server Too Busy</body></html>");
          wr.SendResponseFromMemory(bytes, bytes.Length);
          wr.FlushResponse(true);
          wr.EndOfRequest();
        }
        finally
        {
          Interlocked.Decrement(ref this._activeRequestCount);
        }
      }
      else
      {
        HttpContext context;
        try
        {
          context = new HttpContext(wr, false);
        }
        catch
        {
          try
          {
            wr.SendStatus(400, "Bad Request");
            wr.SendKnownResponseHeader(12, "text/html; charset=utf-8");
            byte[] bytes = Encoding.ASCII.GetBytes("<html><body>Bad Request</body></html>");
            wr.SendResponseFromMemory(bytes, bytes.Length);
            wr.FlushResponse(true);
            wr.EndOfRequest();
            return;
          }
          finally
          {
            Interlocked.Decrement(ref this._activeRequestCount);
          }
        }
        wr.SetEndOfSendNotification(this._asyncEndOfSendCallback, (object) context);
        HostingEnvironment.IncrementBusyCount();
        try
        {
          try
          {
            this.EnsureFirstRequestInit(context);
          }
          catch
          {
            if (!context.Request.IsDebuggingRequest)
              throw;
          }
          context.Response.InitResponseWriter();
          IHttpHandler applicationInstance = HttpApplicationFactory.GetApplicationInstance(context);
          if (applicationInstance == null)
            throw new HttpException(System.Web.SR.GetString("Unable_create_app_object"));
          if (EtwTrace.IsTraceEnabled(5, 1))
            EtwTrace.Trace(EtwTraceType.ETW_TYPE_START_HANDLER, context.WorkerRequest, applicationInstance.GetType().FullName, "Start");
          if (applicationInstance is IHttpAsyncHandler)
          {
            IHttpAsyncHandler httpAsyncHandler = (IHttpAsyncHandler) applicationInstance;
            context.AsyncAppHandler = httpAsyncHandler;
            httpAsyncHandler.BeginProcessRequest(context, this._handlerCompletionCallback, (object) context);
          }
          else
          {
            applicationInstance.ProcessRequest(context);
            this.FinishRequest(context.WorkerRequest, context, (Exception) null);
          }
        }
        catch (Exception ex)
        {
          context.Response.InitResponseWriter();
          this.FinishRequest(wr, context, ex);
        }
      }
    }

    private void RejectRequestInternal(HttpWorkerRequest wr, bool silent)
    {
      HttpContext context = new HttpContext(wr, false);
      wr.SetEndOfSendNotification(this._asyncEndOfSendCallback, (object) context);
      Interlocked.Increment(ref this._activeRequestCount);
      HostingEnvironment.IncrementBusyCount();
      if (silent)
      {
        context.Response.InitResponseWriter();
        this.FinishRequest(wr, context, (Exception) null);
      }
      else
      {
        PerfCounters.IncrementGlobalCounter(GlobalPerfCounter.REQUESTS_REJECTED);
        PerfCounters.IncrementCounter(AppPerfCounter.APP_REQUESTS_REJECTED);
        try
        {
          throw new HttpException(503, System.Web.SR.GetString("Server_too_busy"));
        }
        catch (Exception ex)
        {
          context.Response.InitResponseWriter();
          this.FinishRequest(wr, context, ex);
        }
      }
    }

    private void FinishRequest(HttpWorkerRequest wr, HttpContext context, Exception e)
    {
      HttpResponse response = context.Response;
      if (EtwTrace.IsTraceEnabled(5, 1))
        EtwTrace.Trace(EtwTraceType.ETW_TYPE_END_HANDLER, context.WorkerRequest);
      HttpRuntime.SetExecutionTimePerformanceCounter(context);
      if (e == null)
      {
        using (new ClientImpersonationContext(context, false))
        {
          try
          {
            response.FinalFlushAtTheEndOfRequestProcessing();
          }
          catch (Exception ex)
          {
            e = ex;
          }
        }
      }
      if (e != null)
      {
        using (new DisposableHttpContextWrapper(context))
        {
          context.DisableCustomHttpEncoder = true;
          if (this._appOfflineMessage != null)
          {
            try
            {
              HttpRuntime.ReportAppOfflineErrorMessage(response, this._appOfflineMessage);
              response.FinalFlushAtTheEndOfRequestProcessing();
            }
            catch
            {
            }
          }
          else
          {
            using (new ApplicationImpersonationContext())
            {
              try
              {
                try
                {
                  response.ReportRuntimeError(e, true, false);
                }
                catch (Exception ex)
                {
                  response.ReportRuntimeError(ex, false, false);
                }
                response.FinalFlushAtTheEndOfRequestProcessing();
              }
              catch
              {
              }
            }
          }
        }
      }
      this._firstRequestCompleted = true;
      if (this._hostingInitFailed)
        HttpRuntime.ShutdownAppDomain(ApplicationShutdownReason.HostingEnvironment, "HostingInit error");
      int statusCode = response.StatusCode;
      HttpRuntime.UpdatePerfCounters(statusCode);
      context.FinishRequestForCachedPathData(statusCode);
      try
      {
        wr.EndOfRequest();
      }
      catch (Exception ex)
      {
        WebBaseEvent.RaiseRuntimeError(ex, (object) this);
      }
      HostingEnvironment.DecrementBusyCount();
      Interlocked.Decrement(ref this._activeRequestCount);
      if (this._requestQueue == null)
        return;
      this._requestQueue.ScheduleMoreWorkIfNeeded();
    }

    private bool InitiateShutdownOnce()
    {
      if (this._shutdownInProgress)
        return false;
      lock (this)
      {
        if (this._shutdownInProgress)
          return false;
        this._shutdownInProgress = true;
      }
      return true;
    }

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    private void ReleaseResourcesAndUnloadAppDomain(object state)
    {
      try
      {
        PerfCounters.IncrementGlobalCounter(GlobalPerfCounter.APPLICATION_RESTARTS);
      }
      catch
      {
      }
      try
      {
        this.Dispose();
      }
      catch
      {
      }
      Thread.Sleep(250);
      HttpRuntime.AddAppDomainTraceMessage("before Unload");
      while (true)
      {
        try
        {
          AppDomain.Unload(Thread.GetDomain());
        }
        catch (CannotUnloadAppDomainException ex)
        {
        }
        catch (Exception ex)
        {
          if (!BuildManagerHost.InClientBuildManager)
            HttpRuntime.AddAppDomainTraceMessage("Unload Exception: " + (object) ex);
          throw;
        }
      }
    }

    private static void SetExecutionTimePerformanceCounter(HttpContext context)
    {
      long num = DateTime.UtcNow.Subtract(context.WorkerRequest.GetStartTime()).Ticks / 10000L;
      if (num > (long) int.MaxValue)
        num = (long) int.MaxValue;
      PerfCounters.SetGlobalCounter(GlobalPerfCounter.REQUEST_EXECUTION_TIME, (int) num);
      PerfCounters.SetCounter(AppPerfCounter.APP_REQUEST_EXEC_TIME, (int) num);
    }

    private static void UpdatePerfCounters(int statusCode)
    {
      if (400 <= statusCode)
      {
        PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_FAILED);
        switch (statusCode)
        {
          case 401:
            PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_NOT_AUTHORIZED);
            break;
          case 404:
          case 414:
            PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_NOT_FOUND);
            break;
        }
      }
      else
        PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_SUCCEDED);
    }

    private void WaitForRequestsToFinish(int waitTimeoutMs)
    {
      DateTime dateTime = DateTime.UtcNow.AddMilliseconds((double) waitTimeoutMs);
      while (this._activeRequestCount != 0 || this._requestQueue != null && !this._requestQueue.IsEmpty)
      {
        Thread.Sleep(250);
        if (!Debugger.IsAttached && DateTime.UtcNow > dateTime)
          break;
      }
    }

    private void Dispose()
    {
      int num = 90;
      try
      {
        HttpRuntimeSection httpRuntime = RuntimeConfig.GetAppLKGConfig().HttpRuntime;
        if (httpRuntime != null)
          num = (int) httpRuntime.ShutdownTimeout.TotalSeconds;
        this.WaitForRequestsToFinish(num * 1000);
        if (this._requestQueue != null)
          this._requestQueue.Drain();
      }
      finally
      {
        this._disposingHttpRuntime = true;
      }
      this.WaitForRequestsToFinish(num * 1000 / 6);
      ISAPIWorkerRequestInProcForIIS6.WaitForPendingAsyncIo();
      if (HttpRuntime.UseIntegratedPipeline)
      {
        PipelineRuntime.WaitForRequestsToDrain();
      }
      else
      {
        while (this._activeRequestCount != 0)
          Thread.Sleep(250);
      }
      this.DisposeAppDomainShutdownTimer();
      this._timeoutManager.Stop();
      AppDomainResourcePerfCounters.Stop();
      ISAPIWorkerRequestInProcForIIS6.WaitForPendingAsyncIo();
      SqlCacheDependencyManager.Dispose(num * 1000 / 2);
      if (this._cacheInternal != null)
        this._cacheInternal.Dispose();
      HttpApplicationFactory.EndApplication();
      this._fcm.Stop();
      HealthMonitoringManager.Shutdown();
    }

    private void OnRequestNotificationCompletion(IAsyncResult ar)
    {
      try
      {
        this.OnRequestNotificationCompletionHelper(ar);
      }
      catch (Exception ex)
      {
        ApplicationManager.RecordFatalException(ex);
        throw;
      }
    }

    private void OnRequestNotificationCompletionHelper(IAsyncResult ar)
    {
      if (ar.CompletedSynchronously)
        return;
      RequestNotificationStatus status = RequestNotificationStatus.Continue;
      HttpContext context = (HttpContext) ar.AsyncState;
      IIS7WorkerRequest wr = context.WorkerRequest as IIS7WorkerRequest;
      try
      {
        int num = (int) context.ApplicationInstance.EndProcessRequestNotification(ar);
      }
      catch (Exception ex)
      {
        status = RequestNotificationStatus.FinishRequest;
        context.AddError(ex);
      }
      IntPtr requestContext = wr.RequestContext;
      this.FinishRequestNotification(wr, context, ref status);
      context.NotificationContext = (NotificationContext) null;
      Misc.ThrowIfFailedHr(UnsafeIISMethods.MgdPostCompletion(requestContext, status));
    }

    private void OnHandlerCompletion(IAsyncResult ar)
    {
      HttpContext context = (HttpContext) ar.AsyncState;
      try
      {
        context.AsyncAppHandler.EndProcessRequest(ar);
      }
      catch (Exception ex)
      {
        context.AddError(ex);
      }
      finally
      {
        context.AsyncAppHandler = (IHttpAsyncHandler) null;
      }
      this.FinishRequest(context.WorkerRequest, context, context.Error);
    }

    private void EndOfSendCallback(HttpWorkerRequest wr, object arg)
    {
      HttpContext httpContext = (HttpContext) arg;
      httpContext.Request.Dispose();
      httpContext.Response.Dispose();
    }

    private void OnCriticalDirectoryChange(object sender, FileChangeEvent e)
    {
      ApplicationShutdownReason reason = ApplicationShutdownReason.None;
      string name = new DirectoryInfo(e.FileName).Name;
      string str = FileChangesMonitor.GenerateErrorMessage(e.Action, (string) null);
      string message = str != null ? str + name : name + " dir change or directory rename";
      if (System.Web.Util.StringUtil.EqualsIgnoreCase(name, "App_Code"))
        reason = ApplicationShutdownReason.CodeDirChangeOrDirectoryRename;
      else if (System.Web.Util.StringUtil.EqualsIgnoreCase(name, "App_GlobalResources"))
        reason = ApplicationShutdownReason.ResourcesDirChangeOrDirectoryRename;
      else if (System.Web.Util.StringUtil.EqualsIgnoreCase(name, "App_Browsers"))
        reason = ApplicationShutdownReason.BrowsersDirChangeOrDirectoryRename;
      else if (System.Web.Util.StringUtil.EqualsIgnoreCase(name, "bin"))
        reason = ApplicationShutdownReason.BinDirChangeOrDirectoryRename;
      if (e.Action == FileAction.Added)
        HttpRuntime.SetUserForcedShutdown();
      HttpRuntime.ShutdownAppDomain(reason, message);
    }

    private static bool ShutdownAppDomain(string stackTrace)
    {
      if (HttpRuntime._theRuntime.LastShutdownAttemptTime == DateTime.MinValue)
      {
        if (!HttpRuntime._theRuntime._firstRequestCompleted)
        {
          if (!HttpRuntime._theRuntime._userForcedShutdown)
          {
            try
            {
              RuntimeConfig appLkgConfig = RuntimeConfig.GetAppLKGConfig();
              if (appLkgConfig != null)
              {
                HttpRuntimeSection httpRuntime = appLkgConfig.HttpRuntime;
                if (httpRuntime != null)
                {
                  int num = (int) httpRuntime.DelayNotificationTimeout.TotalSeconds;
                  if (DateTime.UtcNow < HttpRuntime._theRuntime._firstRequestStartTime.AddSeconds((double) num))
                    return false;
                }
              }
            }
            catch
            {
            }
          }
        }
      }
      try
      {
        HttpRuntime._theRuntime.RaiseShutdownWebEventOnce();
      }
      catch
      {
      }
      HttpRuntime._theRuntime.LastShutdownAttemptTime = DateTime.UtcNow;
      if (!HostingEnvironment.ShutdownInitiated)
      {
        HostingEnvironment.InitiateShutdownWithoutDemand();
        return true;
      }
      else
      {
        if (HostingEnvironment.ShutdownInProgress || !HttpRuntime._theRuntime.InitiateShutdownOnce())
          return false;
        if (string.IsNullOrEmpty(stackTrace) && !BuildManagerHost.InClientBuildManager)
        {
          new EnvironmentPermission(PermissionState.Unrestricted).Assert();
          try
          {
            HttpRuntime._theRuntime._shutDownStack = Environment.StackTrace;
          }
          finally
          {
            CodeAccessPermission.RevertAssert();
          }
        }
        else
          HttpRuntime._theRuntime._shutDownStack = stackTrace;
        HttpRuntime.OnAppDomainShutdown(new BuildManagerHostUnloadEventArgs(HttpRuntime._theRuntime._shutdownReason));
        ThreadPool.QueueUserWorkItem(HttpRuntime._theRuntime._appDomainUnloadallback);
        return true;
      }
    }

    private static void CalculateWaitTimeAndUpdatePerfCounter(HttpWorkerRequest wr)
    {
      long num = DateTime.UtcNow.Subtract(wr.GetStartTime()).Ticks / 10000L;
      if (num > (long) int.MaxValue)
        num = (long) int.MaxValue;
      PerfCounters.SetGlobalCounter(GlobalPerfCounter.REQUEST_WAIT_TIME, (int) num);
      PerfCounters.SetCounter(AppPerfCounter.APP_REQUEST_WAIT_TIME, (int) num);
    }

    private static bool HasAPTCABit(Assembly assembly)
    {
      return assembly.IsDefined(typeof (AllowPartiallyTrustedCallersAttribute), false);
    }

    private void CreateCache()
    {
      lock (this)
      {
        if (this._cacheInternal != null)
          return;
        this._cacheInternal = CacheInternal.Create();
      }
    }

    private static string GetAppDomainString(string key)
    {
      return Thread.GetDomain().GetData(key) as string;
    }

    private void SetTrustLevel(TrustSection trustSection, SecurityPolicySection securityPolicySection)
    {
      string level = trustSection.Level;
      if (trustSection.Level == "Full")
        this._trustLevel = level;
      else if (securityPolicySection == null || securityPolicySection.TrustLevels[trustSection.Level] == null)
      {
        throw new ConfigurationErrorsException(System.Web.SR.GetString("Unable_to_get_policy_file", new object[1]
        {
          (object) trustSection.Level
        }), string.Empty, 0);
      }
      else
      {
        string str = trustSection.Level == "Minimal" || trustSection.Level == "Low" || (trustSection.Level == "Medium" || trustSection.Level == "High") ? securityPolicySection.TrustLevels[trustSection.Level].LegacyPolicyFileExpanded : securityPolicySection.TrustLevels[trustSection.Level].PolicyFileExpanded;
        if (str == null || !System.Web.Util.FileUtil.FileExists(str))
        {
          throw new HttpException(System.Web.SR.GetString("Unable_to_get_policy_file", new object[1]
          {
            (object) trustSection.Level
          }));
        }
        else
        {
          bool foundGacToken = false;
          PolicyLevel policyLevel = HttpRuntime.CreatePolicyLevel(str, HttpRuntime.AppDomainAppPathInternal, HttpRuntime.CodegenDirInternal, trustSection.OriginUrl, out foundGacToken);
          if (foundGacToken)
          {
            CodeGroup rootCodeGroup = policyLevel.RootCodeGroup;
            bool flag = false;
            foreach (CodeGroup codeGroup in (IEnumerable) rootCodeGroup.Children)
            {
              if (codeGroup.MembershipCondition is GacMembershipCondition)
              {
                flag = true;
                break;
              }
            }
            if (!flag && rootCodeGroup is FirstMatchCodeGroup)
            {
              FirstMatchCodeGroup firstMatchCodeGroup = (FirstMatchCodeGroup) rootCodeGroup;
              if (firstMatchCodeGroup.MembershipCondition is AllMembershipCondition && firstMatchCodeGroup.PermissionSetName == "Nothing")
              {
                CodeGroup group1 = (CodeGroup) new UnionCodeGroup((IMembershipCondition) new GacMembershipCondition(), new PolicyStatement(new PermissionSet(PermissionState.Unrestricted)));
                CodeGroup codeGroup = (CodeGroup) new FirstMatchCodeGroup(rootCodeGroup.MembershipCondition, rootCodeGroup.PolicyStatement);
                foreach (CodeGroup group2 in (IEnumerable) rootCodeGroup.Children)
                {
                  if (group2 is UnionCodeGroup && group2.MembershipCondition is UrlMembershipCondition && (group2.PolicyStatement.PermissionSet.IsUnrestricted() && group1 != null))
                  {
                    codeGroup.AddChild(group1);
                    group1 = (CodeGroup) null;
                  }
                  codeGroup.AddChild(group2);
                }
                policyLevel.RootCodeGroup = codeGroup;
              }
            }
          }
          AppDomain.CurrentDomain.SetAppDomainPolicy(policyLevel);
          this._namedPermissionSet = policyLevel.GetNamedPermissionSet(trustSection.PermissionSetName);
          this._trustLevel = level;
          this._fcm.StartMonitoringFile(str, new FileChangeEventHandler(this.OnSecurityPolicyFileChange));
        }
      }
    }

    private static PolicyLevel CreatePolicyLevel(string configFile, string appDir, string binDir, string strOriginUrl, out bool foundGacToken)
    {
      StreamReader streamReader = new StreamReader((Stream) new FileStream(configFile, FileMode.Open, FileAccess.Read), Encoding.UTF8);
      string str1 = streamReader.ReadToEnd();
      streamReader.Close();
      appDir = System.Web.Util.FileUtil.RemoveTrailingDirectoryBackSlash(appDir);
      binDir = System.Web.Util.FileUtil.RemoveTrailingDirectoryBackSlash(binDir);
      string str2 = str1.Replace("$AppDir$", appDir).Replace("$AppDirUrl$", HttpRuntime.MakeFileUrl(appDir)).Replace("$CodeGen$", HttpRuntime.MakeFileUrl(binDir));
      if (strOriginUrl == null)
        strOriginUrl = string.Empty;
      string str3 = str2.Replace("$OriginHost$", strOriginUrl);
      if (str3.IndexOf("$Gac$", StringComparison.Ordinal) != -1)
      {
        string str4 = HttpRuntime.GetGacLocation();
        if (str4 != null)
          str4 = HttpRuntime.MakeFileUrl(str4);
        if (str4 == null)
          str4 = string.Empty;
        str3 = str3.Replace("$Gac$", str4);
        foundGacToken = true;
      }
      else
        foundGacToken = false;
      return SecurityManager.LoadPolicyLevelFromString(str3, PolicyLevelType.AppDomain);
    }

    private void SetTrustParameters(TrustSection trustSection, SecurityPolicySection securityPolicySection, PolicyLevel policyLevel)
    {
      this._trustLevel = trustSection.Level;
      if (!(this._trustLevel != "Full"))
        return;
      this._namedPermissionSet = policyLevel.GetNamedPermissionSet(trustSection.PermissionSetName);
      this._policyLevel = policyLevel;
      this._hostSecurityPolicyResolverType = trustSection.HostSecurityPolicyResolverType;
      this._fcm.StartMonitoringFile(securityPolicySection.TrustLevels[trustSection.Level].PolicyFileExpanded, new FileChangeEventHandler(this.OnSecurityPolicyFileChange));
    }

    private void OnSecurityPolicyFileChange(object sender, FileChangeEvent e)
    {
      HttpRuntime.ShutdownAppDomain(ApplicationShutdownReason.ChangeInSecurityPolicyFile, FileChangesMonitor.GenerateErrorMessage(e.Action, e.FileName) ?? "Change in code-access security policy file");
    }

    private void OnAppOfflineFileChange(object sender, FileChangeEvent e)
    {
      HttpRuntime.SetUserForcedShutdown();
      HttpRuntime.ShutdownAppDomain(ApplicationShutdownReason.ConfigurationChange, FileChangesMonitor.GenerateErrorMessage(e.Action, "App_Offline.htm") ?? "Change in App_Offline.htm");
    }

    private static string GetCurrentUserName()
    {
      try
      {
        return WindowsIdentity.GetCurrent().Name;
      }
      catch
      {
        return (string) null;
      }
    }

    private void RaiseShutdownWebEventOnce()
    {
      if (this._shutdownWebEventRaised)
        return;
      lock (this)
      {
        if (this._shutdownWebEventRaised)
          return;
        WebBaseEvent.RaiseSystemEvent((object) this, 1002, WebApplicationLifetimeEvent.DetailCodeFromShutdownReason(HttpRuntime.ShutdownReason));
        this._shutdownWebEventRaised = true;
      }
    }

    private void RelaxMapPathIfRequired()
    {
      try
      {
        RuntimeConfig appConfig = RuntimeConfig.GetAppConfig();
        if (appConfig == null || appConfig.HttpRuntime == null || !appConfig.HttpRuntime.RelaxedUrlToFileSystemMapping)
          return;
        HttpRuntime._DefaultPhysicalPathOnMapPathFailure = Path.Combine(this._appDomainAppPath, "NOT_A_VALID_FILESYSTEM_PATH");
      }
      catch
      {
      }
    }
  }
}
