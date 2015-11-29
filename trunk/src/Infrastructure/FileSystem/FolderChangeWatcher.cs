using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Consumentor.ShopGun.FileSystem
{
    public class FolderChangeWatcher : IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private readonly FileSystemEventHandler _actionOnFolderCreated;

        /// <summary>
        /// Watches a folder for folder creation
        /// </summary>
        /// <param name="path">Which folder or fileshare to watch</param>
        /// <param name="actionOnFolderCreated">Action to perform when a change is detected. The action is invoked asynchronosly</param>
        public FolderChangeWatcher(string path, FileSystemEventHandler actionOnFolderCreated)
        {
            if (actionOnFolderCreated == null)
                throw new ArgumentNullException("actionOnFolderCreated");

            _actionOnFolderCreated = actionOnFolderCreated;
            _watcher = new FileSystemWatcher(path);
            _watcher.Created += OnFolderCreated;

            _watcher.InternalBufferSize = 16384; //16KB buffer instead of 4KB (default).
            _watcher.IncludeSubdirectories = false;
            _watcher.EnableRaisingEvents = true;
        }

        private const long TicksDiff = 100000; //10 000 000 == 1s
        private readonly Dictionary<string, long> _files = new Dictionary<string, long>();

        private void OnFolderCreated(object sender, FileSystemEventArgs e)
        {
            // Events may be fired multiple times for same file, this will filter out some of them
            if (FlagSet(e.ChangeType, WatcherChangeTypes.Created) && IsSameEvent(e) == false)
            {
                Action<FileSystemEventArgs> action = InvokeAction;
                action.BeginInvoke(e, InvokeActionCallback, e);
            }
        }

        private void InvokeActionCallback(IAsyncResult ar)
        {
            var handle = ar.AsyncWaitHandle;
            if (handle != null)
            {
                handle.WaitOne();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031", Justification = "")]
        private void InvokeAction(FileSystemEventArgs eventArgs)
        {
            try
            {
                _actionOnFolderCreated.Invoke(this, eventArgs);
            }
            catch (Exception) 
            {
                // Suppress Exception
            }
        }

        public bool IsSameEvent(FileSystemEventArgs args)
        {
            long currentEventTick = ShopGunTime.Now.Ticks;
            long lastEventTick;

            if (_files.TryGetValue(args.Name, out lastEventTick))
            {
                if (currentEventTick > lastEventTick + TicksDiff)
                {
                    _files[args.Name] = currentEventTick;
                    return false;
                }
                return true;
            }
            else
            {
                _files.Add(args.Name, ShopGunTime.Now.Ticks);
                return false;
            }
        }

        private bool FlagSet(WatcherChangeTypes changeType, WatcherChangeTypes flagSet)
        {
            return (changeType & flagSet) == flagSet;
        }

        #region Implementation of IDisposable

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
                    if (_watcher != null)
                    {
                        _watcher.Dispose();
                    }
                }
                _disposed = true;
            }
        }

        #endregion
    }
}