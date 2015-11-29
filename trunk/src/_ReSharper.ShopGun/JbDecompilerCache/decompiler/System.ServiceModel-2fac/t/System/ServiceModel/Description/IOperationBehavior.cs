// Type: System.ServiceModel.Description.IOperationBehavior
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.ServiceModel.dll

using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Description
{
  public interface IOperationBehavior
  {
    void Validate(OperationDescription operationDescription);

    void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation);

    void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation);

    void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters);
  }
}
