using System;
using System.Collections.Generic;

namespace Consumentor.ShopGun
{
    public interface IActionPerformer
    {
        TimeSpan RetryDelay { get; set; }
        int NumberOfRetries { get; set; }
        int NumberOfCurrentlyPerformingActions { get; }
        bool IsCurrentlyPerformingAction { get; }
        IList<ActionPerformerException> Exceptions { get; }
        bool HasCaughtExceptions { get; }

        /// <summary>
        /// This will invoke your action. 
        /// InvokeActionAsync IS the preffered way to use this interface to not block any server threads
        /// </summary>
        /// <param name="namedAction"></param>
        void InvokeAction(INamedAction namedAction);
        //Async event: http://msdn.microsoft.com/en-us/library/e7a34yad.aspx
        void InvokeActionAsync(INamedAction namedAction);
        event EventHandler<ActionCompletionEventArgs> InvokeActionCompleted;
        event EventHandler<ActionStartEventArgs> InvokeActionStarted;
        void LogAndClearCaughtExceptions();
    }
}