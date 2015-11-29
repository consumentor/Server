// Type: System.ServiceModel.ServiceBehaviorAttribute
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.ServiceModel.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Transactions;

namespace System.ServiceModel
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ServiceBehaviorAttribute : Attribute, IServiceBehavior
  {
    internal static IsolationLevel DefaultIsolationLevel = IsolationLevel.Unspecified;
    private bool releaseServiceInstanceOnTransactionComplete = true;
    private bool validateMustUnderstand = true;
    private int maxItemsInObjectGraph = 65536;
    private IsolationLevel transactionIsolationLevel = ServiceBehaviorAttribute.DefaultIsolationLevel;
    private bool automaticSessionShutdown = true;
    private TimeSpan transactionTimeout = TimeSpan.Zero;
    private bool useSynchronizationContext = true;
    private ConcurrencyMode concurrencyMode;
    private string configurationName;
    private bool includeExceptionDetailInFaults;
    private InstanceContextMode instanceMode;
    private bool releaseServiceInstanceOnTransactionCompleteSet;
    private bool transactionAutoCompleteOnSessionClose;
    private bool transactionAutoCompleteOnSessionCloseSet;
    private object wellKnownSingleton;
    private object hiddenSingleton;
    private bool ignoreExtensionDataObject;
    private bool isolationLevelSet;
    private IInstanceProvider instanceProvider;
    private string transactionTimeoutString;
    private bool transactionTimeoutSet;
    private string serviceName;
    private string serviceNamespace;
    private AddressFilterMode addressFilterMode;

    [DefaultValue(null)]
    public string Name
    {
      get
      {
        return this.serviceName;
      }
      set
      {
        this.serviceName = value;
      }
    }

    [DefaultValue(null)]
    public string Namespace
    {
      get
      {
        return this.serviceNamespace;
      }
      set
      {
        this.serviceNamespace = value;
      }
    }

    internal IInstanceProvider InstanceProvider
    {
      set
      {
        this.instanceProvider = value;
      }
    }

    [DefaultValue(AddressFilterMode.Exact)]
    public AddressFilterMode AddressFilterMode
    {
      get
      {
        return this.addressFilterMode;
      }
      set
      {
        if (!AddressFilterModeHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.addressFilterMode = value;
      }
    }

    [DefaultValue(true)]
    public bool AutomaticSessionShutdown
    {
      get
      {
        return this.automaticSessionShutdown;
      }
      set
      {
        this.automaticSessionShutdown = value;
      }
    }

    [DefaultValue(null)]
    public string ConfigurationName
    {
      get
      {
        return this.configurationName;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        if (value == string.Empty)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", System.ServiceModel.SR.GetString("SFxConfigurationNameCannotBeEmpty")));
        this.configurationName = value;
      }
    }

    public IsolationLevel TransactionIsolationLevel
    {
      get
      {
        return this.transactionIsolationLevel;
      }
      set
      {
        switch (value)
        {
          case IsolationLevel.Serializable:
          case IsolationLevel.RepeatableRead:
          case IsolationLevel.ReadCommitted:
          case IsolationLevel.ReadUncommitted:
          case IsolationLevel.Snapshot:
          case IsolationLevel.Chaos:
          case IsolationLevel.Unspecified:
            this.transactionIsolationLevel = value;
            this.isolationLevelSet = true;
            break;
          default:
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        }
      }
    }

    internal bool IsolationLevelSet
    {
      get
      {
        return this.isolationLevelSet;
      }
    }

    [DefaultValue(false)]
    public bool IncludeExceptionDetailInFaults
    {
      get
      {
        return this.includeExceptionDetailInFaults;
      }
      set
      {
        this.includeExceptionDetailInFaults = value;
      }
    }

    [DefaultValue(ConcurrencyMode.Single)]
    public ConcurrencyMode ConcurrencyMode
    {
      get
      {
        return this.concurrencyMode;
      }
      set
      {
        if (!ConcurrencyModeHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.concurrencyMode = value;
      }
    }

    [DefaultValue(InstanceContextMode.PerSession)]
    public InstanceContextMode InstanceContextMode
    {
      get
      {
        return this.instanceMode;
      }
      set
      {
        if (!InstanceContextModeHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.instanceMode = value;
      }
    }

    public bool ReleaseServiceInstanceOnTransactionComplete
    {
      get
      {
        return this.releaseServiceInstanceOnTransactionComplete;
      }
      set
      {
        this.releaseServiceInstanceOnTransactionComplete = value;
        this.releaseServiceInstanceOnTransactionCompleteSet = true;
      }
    }

    internal bool ReleaseServiceInstanceOnTransactionCompleteSet
    {
      get
      {
        return this.releaseServiceInstanceOnTransactionCompleteSet;
      }
    }

    public bool TransactionAutoCompleteOnSessionClose
    {
      get
      {
        return this.transactionAutoCompleteOnSessionClose;
      }
      set
      {
        this.transactionAutoCompleteOnSessionClose = value;
        this.transactionAutoCompleteOnSessionCloseSet = true;
      }
    }

    internal bool TransactionAutoCompleteOnSessionCloseSet
    {
      get
      {
        return this.transactionAutoCompleteOnSessionCloseSet;
      }
    }

    public string TransactionTimeout
    {
      get
      {
        return this.transactionTimeoutString;
      }
      set
      {
        if (value == null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentNullException("value"));
        try
        {
          TimeSpan timeSpan = TimeSpan.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture);
          if (timeSpan < TimeSpan.Zero)
          {
            string @string = System.ServiceModel.SR.GetString("SFxTimeoutOutOfRange0");
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, @string));
          }
          else
          {
            this.transactionTimeout = timeSpan;
            this.transactionTimeoutString = value;
            this.transactionTimeoutSet = true;
          }
        }
        catch (FormatException ex)
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentException(System.ServiceModel.SR.GetString("SFxTimeoutInvalidStringFormat"), "value", (Exception) ex));
        }
        catch (OverflowException ex)
        {
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        }
      }
    }

    internal TimeSpan TransactionTimeoutTimespan
    {
      get
      {
        return this.transactionTimeout;
      }
    }

    internal bool TransactionTimeoutSet
    {
      get
      {
        return this.transactionTimeoutSet;
      }
    }

    [DefaultValue(true)]
    public bool ValidateMustUnderstand
    {
      get
      {
        return this.validateMustUnderstand;
      }
      set
      {
        this.validateMustUnderstand = value;
      }
    }

    [DefaultValue(false)]
    public bool IgnoreExtensionDataObject
    {
      get
      {
        return this.ignoreExtensionDataObject;
      }
      set
      {
        this.ignoreExtensionDataObject = value;
      }
    }

    [DefaultValue(65536)]
    public int MaxItemsInObjectGraph
    {
      get
      {
        return this.maxItemsInObjectGraph;
      }
      set
      {
        this.maxItemsInObjectGraph = value;
      }
    }

    [DefaultValue(true)]
    public bool UseSynchronizationContext
    {
      get
      {
        return this.useSynchronizationContext;
      }
      set
      {
        this.useSynchronizationContext = value;
      }
    }

    static ServiceBehaviorAttribute()
    {
    }

    public bool ShouldSerializeTransactionIsolationLevel()
    {
      return this.IsolationLevelSet;
    }

    public bool ShouldSerializeConfigurationName()
    {
      return this.configurationName != null;
    }

    public bool ShouldSerializeReleaseServiceInstanceOnTransactionComplete()
    {
      return this.ReleaseServiceInstanceOnTransactionCompleteSet;
    }

    public bool ShouldSerializeTransactionAutoCompleteOnSessionClose()
    {
      return this.TransactionAutoCompleteOnSessionCloseSet;
    }

    public bool ShouldSerializeTransactionTimeout()
    {
      return this.TransactionTimeoutSet;
    }

    public object GetWellKnownSingleton()
    {
      return this.wellKnownSingleton;
    }

    public void SetWellKnownSingleton(object value)
    {
      if (value == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
      this.wellKnownSingleton = value;
    }

    internal object GetHiddenSingleton()
    {
      return this.hiddenSingleton;
    }

    internal void SetHiddenSingleton(object value)
    {
      if (value == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
      this.hiddenSingleton = value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void SetIsolationLevel(ChannelDispatcher channelDispatcher)
    {
      if (channelDispatcher == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelDispatcher");
      channelDispatcher.TransactionIsolationLevel = this.transactionIsolationLevel;
    }

    void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
    {
    }

    void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
    {
    }

    void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
    {
      for (int index = 0; index < serviceHostBase.ChannelDispatchers.Count; ++index)
      {
        ChannelDispatcher channelDispatcher = serviceHostBase.ChannelDispatchers[index] as ChannelDispatcher;
        if (channelDispatcher != null)
        {
          channelDispatcher.IncludeExceptionDetailInFaults = this.includeExceptionDetailInFaults;
          if (channelDispatcher.HasApplicationEndpoints())
          {
            channelDispatcher.TransactionTimeout = this.transactionTimeout;
            if (this.isolationLevelSet)
              this.SetIsolationLevel(channelDispatcher);
            foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
            {
              if (!endpointDispatcher.IsSystemEndpoint)
              {
                DispatchRuntime dispatchRuntime = endpointDispatcher.DispatchRuntime;
                dispatchRuntime.ConcurrencyMode = this.concurrencyMode;
                dispatchRuntime.ValidateMustUnderstand = this.validateMustUnderstand;
                dispatchRuntime.AutomaticInputSessionShutdown = this.automaticSessionShutdown;
                dispatchRuntime.TransactionAutoCompleteOnSessionClose = this.transactionAutoCompleteOnSessionClose;
                dispatchRuntime.ReleaseServiceInstanceOnTransactionComplete = this.releaseServiceInstanceOnTransactionComplete;
                if (!this.useSynchronizationContext)
                  dispatchRuntime.SynchronizationContext = (SynchronizationContext) null;
                if (!endpointDispatcher.AddressFilterSetExplicit)
                {
                  EndpointAddress originalAddress = endpointDispatcher.OriginalAddress;
                  if (originalAddress == (EndpointAddress) null || this.AddressFilterMode == AddressFilterMode.Any)
                    endpointDispatcher.AddressFilter = (MessageFilter) new MatchAllMessageFilter();
                  else if (this.AddressFilterMode == AddressFilterMode.Prefix)
                    endpointDispatcher.AddressFilter = (MessageFilter) new PrefixEndpointAddressMessageFilter(originalAddress);
                  else if (this.AddressFilterMode == AddressFilterMode.Exact)
                    endpointDispatcher.AddressFilter = (MessageFilter) new EndpointAddressMessageFilter(originalAddress);
                }
              }
            }
          }
        }
      }
      DataContractSerializerServiceBehavior.ApplySerializationSettings(description, this.ignoreExtensionDataObject, this.maxItemsInObjectGraph);
      this.ApplyInstancing(description, serviceHostBase);
    }

    private void ApplyInstancing(ServiceDescription description, ServiceHostBase serviceHostBase)
    {
      System.Type serviceType = description.ServiceType;
      InstanceContext instanceContext = (InstanceContext) null;
      for (int index = 0; index < serviceHostBase.ChannelDispatchers.Count; ++index)
      {
        ChannelDispatcher channelDispatcher = serviceHostBase.ChannelDispatchers[index] as ChannelDispatcher;
        if (channelDispatcher != null)
        {
          foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
          {
            if (!endpointDispatcher.IsSystemEndpoint)
            {
              DispatchRuntime dispatchRuntime = endpointDispatcher.DispatchRuntime;
              if (dispatchRuntime.InstanceProvider == null)
              {
                if (this.instanceProvider == null)
                {
                  if (serviceType == (System.Type) null && this.wellKnownSingleton == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(System.ServiceModel.SR.GetString("InstanceSettingsMustHaveTypeOrWellKnownObject0")));
                  if (this.instanceMode != InstanceContextMode.Single && this.wellKnownSingleton != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new InvalidOperationException(System.ServiceModel.SR.GetString("SFxWellKnownNonSingleton0")));
                }
                else
                  dispatchRuntime.InstanceProvider = this.instanceProvider;
              }
              dispatchRuntime.Type = serviceType;
              dispatchRuntime.InstanceContextProvider = InstanceContextProviderBase.GetProviderForMode(this.instanceMode, dispatchRuntime);
              if (this.instanceMode == InstanceContextMode.Single && dispatchRuntime.SingletonInstanceContext == null)
              {
                if (instanceContext == null)
                {
                  instanceContext = this.wellKnownSingleton == null ? (this.hiddenSingleton == null ? new InstanceContext(serviceHostBase, false) : new InstanceContext(serviceHostBase, this.hiddenSingleton, false, false)) : new InstanceContext(serviceHostBase, this.wellKnownSingleton, true, false);
                  instanceContext.AutoClose = false;
                }
                dispatchRuntime.SingletonInstanceContext = instanceContext;
              }
            }
          }
        }
      }
    }
  }
}
