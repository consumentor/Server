using System;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun.Component
{
    [Serializable]
    public class ContainerException : Exception
    {
        public ContainerException()
            :base()
        { }

        public ContainerException(string message)
            : base(message)
        { }
        
        public ContainerException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected ContainerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

    }
}