using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Timers;
using Castle.Core.Logging;
using Consumentor.ShopGun.Component;
using Consumentor.ShopGun.Configuration;
using Timer=System.Timers.Timer;

namespace Consumentor.ShopGun
{
    public class ActionScheduler<TActionGenerator, TSchedulerConfiguration> : ActionScheduler
        where TActionGenerator : class, INamedAction
        where TSchedulerConfiguration : ISchedulerConfiguration
    {
        public ActionScheduler(IActionPerformer actionPerformer, TActionGenerator actionGenerator, TSchedulerConfiguration settings, IContainer container)
            : base(actionPerformer, actionGenerator, settings, container)
        {
        }
    }

    public class ActionScheduler : IScheduler, IDisposable
    {
        private IActionPerformer ActionPerformer { get; set; }
        private Timer _timer;
        private readonly INamedAction _namedAction;
        private DateTime _start;
        private readonly IContainer _container;
        //private readonly string _name = "Anonymous";

        public ILogger Log { get; set; }

        public ActionScheduler(IActionPerformer actionPerformer, INamedAction namedAction, ISchedulerConfiguration settings, IContainer container)
        {
            SetActionSchedulerSettings(actionPerformer, settings);
            _namedAction = namedAction;
            if (_namedAction.HasActionName == false)
                _namedAction.ActionName = _namedAction.GetType().Name;
            _container = container;
        }

        private void SetActionSchedulerSettings(IActionPerformer actionPerformer, ISchedulerConfiguration settings)
        {
            ActionPerformer = actionPerformer;

            ActionPerformer.NumberOfRetries = settings.NumberOfRetries;
            ActionPerformer.RetryDelay = settings.RetryInterval;
            ActionPerformer.InvokeActionStarted += OnInvokeActionStarted;
            ActionPerformer.InvokeActionCompleted += OnInvokeActionCompleted;

            _timer = new Timer { Enabled = false, Interval = settings.Interval.TotalMilliseconds };
            _timer.Elapsed += OnTimerElapsed;
        }

        private void OnInvokeActionStarted(object sender, ActionStartEventArgs e)
        {
            _start = e.DateTime;
            Log.Debug("{0} Start: {1}, NumberOfRetries: {2}, RetryDelay: {3} seconds", _namedAction.ActionName, e.DateTime, ActionPerformer.NumberOfRetries, ActionPerformer.RetryDelay.TotalSeconds);
        }

        private void OnInvokeActionCompleted(object sender, ActionCompletionEventArgs e)
        {
            Log.Debug("{0} End: {1}, Duration: {2}, Succeeded: {3}", _namedAction.ActionName, e.DateTime, new TimeSpan(e.DateTime.Ticks - _start.Ticks).TotalSeconds, e.Success);
            ActionPerformer.LogAndClearCaughtExceptions();
        }

        void IScheduler.StartScheduler()
        {
            _timer.Start();
        }

        void IScheduler.StopScheduler()
        {
            _timer.Stop();
        }

        void IScheduler.OnScheduledEvent()
        {
            ActionPerformer.InvokeAction(_namedAction);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            SetCurrentCultureOnThreadFromThreadPool();

            _timer.Stop();
            ((IScheduler)this).OnScheduledEvent();

            if (_disposed == false)
                _timer.Start();
        }

        private void SetCurrentCultureOnThreadFromThreadPool()
        {
            var cultureConfiguration = _container.Resolve<IServiceCultureConfiguration>();
            Thread.CurrentThread.CurrentCulture = cultureConfiguration.CultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureConfiguration.UICulture;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Dispose();
                    }
                    var disposable = _namedAction as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }

                }
                _disposed = true;
            }
        }
    }
}
