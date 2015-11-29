using Castle.Core.Logging;

namespace Consumentor.ShopGun.Log
{
    public interface ILogWrapper
    {
        bool IsDebugEnabled { get; }
        void Info(string message);
        void Debug(string message);
        void Warn(string message);
        void Info(string format, params object[] args);
        void Debug(string format, params object[] args);
        void Warn(string format, params object[] args);
    }

    public class NullLogWrapperLogger : ILogWrapper
    {
        public static ILogWrapper Instance { get { return new NullLogWrapperLogger(); } }

        bool ILogWrapper.IsDebugEnabled
        {
            get { return false; }
        }

        void ILogWrapper.Info(string message)
        {
        }

        void ILogWrapper.Info(string format, params object[] args)
        {
        }

        void ILogWrapper.Debug(string message)
        {
        }

        void ILogWrapper.Debug(string format, params object[] args)
        {
        }

        void ILogWrapper.Warn(string message)
        {
        }

        void ILogWrapper.Warn(string format, params object[] args)
        {
        }
    }

    public class CastleLoggerLogWrapper : ILogWrapper
    {
        private readonly ILogger _log;

        public CastleLoggerLogWrapper(ILogger log)
        {
            _log = log;
        }

        bool ILogWrapper.IsDebugEnabled { get { return _log.IsDebugEnabled; } }

        void ILogWrapper.Info(string message)
        {
            _log.Info(message);
        }

        void ILogWrapper.Info(string format, params object[] args)
        {
            _log.Info(format, args);
        }

        void ILogWrapper.Debug(string message)
        {
            _log.Debug(message);
        }

        void ILogWrapper.Debug(string format, params object[] args)
        {
            _log.Debug(format, args);
        }

        void ILogWrapper.Warn(string message)
        {
            _log.Warn(message);
        }

        void ILogWrapper.Warn(string format, params object[] args)
        {
            _log.Warn(format, args);
        }
    }
}
