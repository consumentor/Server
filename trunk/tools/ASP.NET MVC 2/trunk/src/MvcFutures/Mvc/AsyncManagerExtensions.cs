﻿namespace Microsoft.Web.Mvc {
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc.Async;

    public static class AsyncManagerExtensions {

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "An unhandled exception here will bring down the worker process.")]
        public static void RegisterTask(this AsyncManager asyncManager, Func<AsyncCallback, IAsyncResult> beginDelegate, Action<IAsyncResult> endDelegate) {
            if (asyncManager == null) {
                throw new ArgumentNullException("asyncManager");
            }
            if (beginDelegate == null) {
                throw new ArgumentNullException("beginDelegate");
            }
            if (endDelegate == null) {
                throw new ArgumentNullException("endDelegate");
            }

            // need to wait to execute the callback until after BeginXxx() has completed
            object delegateExecutingLockObj = new object();

            AsyncCallback callback = ar => {
                lock (delegateExecutingLockObj) { }
                if (!ar.CompletedSynchronously) {
                    try {
                        asyncManager.Sync(() => endDelegate(ar)); // called on different thread, so have to take application lock
                    }
                    catch {
                        // Need to swallow exceptions, as otherwise unhandled exceptions on a ThreadPool thread
                        // can bring down the entire worker process.
                    }
                    finally {
                        asyncManager.OutstandingOperations.Decrement();
                    }
                }
            };

            IAsyncResult asyncResult;
            asyncManager.OutstandingOperations.Increment();
            try {
                lock (delegateExecutingLockObj) {
                    asyncResult = beginDelegate(callback);
                }
            }
            catch {
                asyncManager.OutstandingOperations.Decrement();
                throw;
            }

            if (asyncResult.CompletedSynchronously) {
                try {
                    endDelegate(asyncResult); // call on same thread
                }
                finally {
                    asyncManager.OutstandingOperations.Decrement();
                }
            }
        }

    }
}
