using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Consumentor.ShopGun
{
    [Serializable]
    public class ActionPerformerException : Exception
    {
        public ActionPerformerException()
        { }

        public ActionPerformerException(string message)
            : base(message)
        { }

        public ActionPerformerException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public ActionPerformerException(Exception innerException, string actionName, int retryCount, int numberOfRetries, TimeSpan retryDelay)
            : base(string.Empty, innerException)
        {
            ActionName = actionName;
            RetryCount = retryCount;
            NumberOfRetries = numberOfRetries;
            RetryDelay = retryDelay;
            Timestamp = ShopGunTime.Now;
        }

        protected ActionPerformerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public int RetryCount { get; protected set; }
        public int NumberOfRetries { get; protected set; }
        public TimeSpan RetryDelay { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public string ActionName { get; protected set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "ActionPerformerException caught {0} during action {1}. Attempt {2} of {3}. Retrydelay: {4}.{5}{6}", Timestamp, ActionName, RetryCount + 1, NumberOfRetries + 1, RetryDelay, Environment.NewLine, InnerException);
        }
    }
}