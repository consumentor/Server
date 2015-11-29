using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Castle.Core.Interceptor;
using Castle.Core.Logging;

namespace Consumentor.ShopGun.Log
{
    public class LogInterceptor : IInterceptor
    {
        public ILogger Log { get; set; }

        void IInterceptor.Intercept(IInvocation invocation)
        {
            LogArguments(invocation, Log.IsDebugEnabled);
            InvokeFunction(invocation);
            LogReturnValue(invocation);
        }

        private void InvokeFunction(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                LogArguments(invocation, Log.IsErrorEnabled);
                Log.Error("\tException {0}: ", e.ToString());
                throw;
            }
        }

        private void LogReturnValue(IInvocation invocation)
        {
            if (Log.IsDebugEnabled)
            {
                if (invocation.Method.ReturnParameter.ParameterType != typeof(void))
                {
                    string value = SerializeValue(invocation.ReturnValue);
                    Log.Debug("\tReturn: ({0}){1}", invocation.Method.ReturnType, value);
                }
            }
        }

        private void LogArguments(IInvocation invocation, bool doLog)
        {
            if (doLog)
            {
                StringBuilder sb = new StringBuilder(invocation.TargetType.FullName)
                    .Append(":: ")
                    .Append(invocation.Method)
                    .Append("(");
                for (int i = 0; i < invocation.Arguments.Length; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    if (invocation.Arguments[i] == null)
                        sb.Append("<NULL>");
                    else
                    {
                        if ((invocation.Arguments[i].GetType().IsClass)
                            && (invocation.Arguments[i].GetType().GetConstructor(new Type[0]) != null))
                        {
                            string value = SerializeValue(invocation.Arguments[i]);
                            sb.Append(value);
                        }
                        else
                        {
                            sb.Append(invocation.Arguments[i]);
                        }
                    }

                }
                sb.Append(")");
                Log.Debug(sb.ToString());
            }
        }

        private string SerializeValue(object value)
        {
            if (value == null)
                return "<NULL>";

            var serializer = new DataContractSerializer(value.GetType());
            var sb = new StringBuilder();
            var stream = XmlWriter.Create(sb);
            serializer.WriteObject(stream, value);
            stream.Flush();
            return sb.ToString();
        }
    }
}