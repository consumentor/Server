using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Castle.Core.Logging;

namespace Consumentor.ShopGun
{
    public class ActionPerformer : IActionPerformer
    {
        public ILogger Log { get; set; }

        private readonly IList<ActionPerformerException> _taskPerformerExceptionList = new List<ActionPerformerException>();
        private readonly object _lockExceptionList = new object();

        private int _numberOfCurrentlyPerformingTasks;

        public event EventHandler<ActionCompletionEventArgs> InvokeActionCompleted;
        public event EventHandler<ActionStartEventArgs> InvokeActionStarted;

        TimeSpan IActionPerformer.RetryDelay { get; set; }
        int IActionPerformer.NumberOfRetries { get; set; }

        int IActionPerformer.NumberOfCurrentlyPerformingActions
        {
            get { return _numberOfCurrentlyPerformingTasks; }
        }

        bool IActionPerformer.IsCurrentlyPerformingAction
        {
            get { return _numberOfCurrentlyPerformingTasks > 0; }
        }

        IList<ActionPerformerException> IActionPerformer.Exceptions
        {
            get { return _taskPerformerExceptionList; }
        }

        bool IActionPerformer.HasCaughtExceptions
        {
            get
            {
                return ((IActionPerformer)this).Exceptions.Count > 0;
            }
        }

        void IActionPerformer.InvokeAction(INamedAction namedAction)
        {
            bool success = false;
            try
            {
                Interlocked.Increment(ref _numberOfCurrentlyPerformingTasks);
                ActionStarted();

                int retryCount = 0;
                while ((retryCount <= ((IActionPerformer)this).NumberOfRetries) && (success == false))
                {
                    try
                    {
                        PerformAction(namedAction, retryCount);
                        success = true;
                    }
                    catch (ActionPerformerException ex)
                    {
                        //swallow
                        Log.Debug("ActionPerformer caught an exception when invoking the action {0}. Attempt {1} of {2}. Retrydelay: {3}. Exception: {4}", namedAction.ActionName, retryCount + 1, ((IActionPerformer)this).NumberOfRetries + 1, ((IActionPerformer)this).RetryDelay.ToString(), ex.InnerException.Message);
                        //Wait if there are retries left
                        if (retryCount < ((IActionPerformer)this).NumberOfRetries)
                            Thread.Sleep(((IActionPerformer)this).RetryDelay);
                    }
                    finally
                    {
                        retryCount++;
                    }
                }
            }
            finally
            {
                Interlocked.Decrement(ref _numberOfCurrentlyPerformingTasks);
                ActionCompleted(success, namedAction);
            }
        }

        void IActionPerformer.InvokeActionAsync(INamedAction namedAction)
        {
            Action a = () => ((IActionPerformer)this).InvokeAction(namedAction);
            a.BeginInvoke(null, null);
        }

        private void PerformAction(INamedAction namedAction, int retryCount)
        {
            try
            {
                Log.Debug("Invoking action.");
                namedAction.Action.Invoke();
            }
            catch (Exception ex)
            {
                ActionPerformerException exception = new ActionPerformerException(ex, namedAction.ActionName, retryCount, ((IActionPerformer)this).NumberOfRetries, ((IActionPerformer)this).RetryDelay);
                lock (_lockExceptionList)
                    ((IActionPerformer)this).Exceptions.Add(exception);
                throw exception;
            }
        }

        private void ActionCompleted(bool success, INamedAction namedAction)
        {
            if (success)
                Log.Debug("Action {0} completed successfully after {1} tries.", namedAction.ActionName, ((IActionPerformer)this).NumberOfRetries + 1);
            else
                Log.Debug("Action {0} failed after {1} tries.", namedAction.ActionName, ((IActionPerformer)this).NumberOfRetries + 1);
            if (InvokeActionCompleted != null)
                InvokeActionCompleted.Invoke(this, new ActionCompletionEventArgs(success));
        }

        private void ActionStarted()
        {
            if (InvokeActionStarted != null)
                InvokeActionStarted.Invoke(this, new ActionStartEventArgs());
        }

        void IActionPerformer.LogAndClearCaughtExceptions()
        {
            if(((IActionPerformer)this).HasCaughtExceptions)
            {
                StringBuilder logBuilder = new StringBuilder();
                logBuilder.Append(string.Format(CultureInfo.CurrentCulture, "Actionperformer has caught {0} exceptions:{1}", _taskPerformerExceptionList.Count, Environment.NewLine));
                ActionPerformerException previousExeption = null;
                lock (_lockExceptionList)
                {
                    int exceptionCount = 0;
                    foreach (ActionPerformerException ex in _taskPerformerExceptionList)
                    {
                        string message;
                        if (ExceptionsAreSame(previousExeption, ex))
                            message = "Same as previous exception...";
                        else
                            message = ex.ToString();
                        logBuilder.Append(string.Format(CultureInfo.CurrentCulture, "{0})\t{1}{2}", ++exceptionCount, message, Environment.NewLine));
                        previousExeption = ex;
                    }
                    _taskPerformerExceptionList.Clear();
                }
                Log.Error(logBuilder.ToString());
            }
        }

        private static bool ExceptionsAreSame(ActionPerformerException previousExeption, ActionPerformerException newException)
        {
            if (previousExeption == null || newException == null)
                return false;
            bool equalActionName = previousExeption.ActionName.Equals(newException.ActionName, StringComparison.OrdinalIgnoreCase);
            bool equalInnerException = previousExeption.InnerException.Message.Equals(newException.InnerException.Message, StringComparison.OrdinalIgnoreCase);
            return equalActionName && equalInnerException;
        }
    }
}