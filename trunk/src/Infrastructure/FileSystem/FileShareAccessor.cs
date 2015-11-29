using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Consumentor.ShopGun.Log;

namespace Consumentor.ShopGun.FileSystem
{
    public class FileShareAccessor : LogBase, IFileShareAccessor
    {
        private string _domain;
        private string _fileShare;
        private bool _isDisposed;
        private string _password;
        private string _user;

        void IFileShareAccessor.EnsureAccess(string fileShare, string domain, string userName, string password)
        {
            bool connected = ((IFileShareAccessor)this).Connect(fileShare, domain, userName, password);
            if (connected == false)
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, "EnsureAccess failed. {0}", this));
            Log.Info("Access successfully ensured. {0}", this);
        }

        string IFileShareAccessor.Password
        {
            get { return _password; }
        }

        string IFileShareAccessor.User
        {
            get { return _user; }
        }

        string IFileShareAccessor.Domain
        {
            get { return _domain; }
        }

        string IFileShareAccessor.FileShare
        {
            get { return _fileShare; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        bool IFileShareAccessor.IsAccessible
        {
            get { return Directory.Exists(_fileShare); }
        }

        bool IFileShareAccessor.Connect(string fileShare, string domain, string userName, string password)
        {
            SetShareInfo(fileShare, domain, userName, password);

            if (((IFileShareAccessor)this).IsAccessible)
                return true;

            string userArgument = string.Empty;
            if (string.IsNullOrEmpty(_user) == false)
            {
                userArgument += "/user:";
                if (string.IsNullOrEmpty(_domain) == false)
                    userArgument += _domain + @"\";
                userArgument += _user;
                userArgument += " " + _password;
            }
            string arguments = string.Format(CultureInfo.CurrentCulture, @"use {0} {1}", _fileShare, userArgument);
            string output = ExecuteNet(arguments);

            if (((IFileShareAccessor)this).IsAccessible)
                Log.Info("Succesfully connected to fileshare. {0}. Output: {1}.", this, output);
            else
            {
                Log.Error("Failed to connect to fileshare. {0}. Output: {1}.", this, output);
                ((IFileShareAccessor)this).Disconnect();
            }
            return ((IFileShareAccessor)this).IsAccessible;
        }

        bool IFileShareAccessor.Connect(string fileShare, string userName, string password)
        {
            return ((IFileShareAccessor)this).Connect(fileShare, null, userName, password);
        }

        bool IFileShareAccessor.Connect(string fileShare)
        {
            return ((IFileShareAccessor)this).Connect(fileShare, null, null, null);
        }

        void IFileShareAccessor.Disconnect()
        {
            string arguments = string.Format(CultureInfo.CurrentCulture, @" use {0} /d", _fileShare);
            string output = ExecuteNet(arguments);

            if (((IFileShareAccessor)this).IsAccessible == false)
                Log.Info("Succesfully disconnected from fileshare. {0}. Output: {1}.", this, output);
            else
                Log.Error("Failed to disconnect from fileshare. {0}. Output: {1}.", this, output);
        }

        private void SetShareInfo(string fileShare, string domain, string userName, string password)
        {
            if (FileshareHasChanged(fileShare))
                ((IFileShareAccessor)this).Disconnect();

            if (string.IsNullOrEmpty(fileShare))
                throw new ArgumentException("A path must always be provided", "fileShare");
            if (string.IsNullOrEmpty(userName) == false && string.IsNullOrEmpty(password))
                throw new ArgumentException("When a username is specified, a password must also always be provided", "password");

            _fileShare = fileShare;
            _domain = domain;
            _user = userName;
            _password = password;
        }

        private bool FileshareHasChanged(string fileShare)
        {
            return string.IsNullOrEmpty(_fileShare) == false && _fileShare.Equals(fileShare, StringComparison.OrdinalIgnoreCase) == false;
        }

        private static string ExecuteNet(string arguments)
        {
            using (Process net = new Process())
            {
                net.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                net.StartInfo.CreateNoWindow = true;
                net.StartInfo.RedirectStandardOutput = true;
                net.StartInfo.UseShellExecute = false;
                net.StartInfo.FileName = "net.exe";
                net.StartInfo.Arguments = arguments;
                net.Start();
                net.WaitForExit();
                return net.StandardOutput.ReadToEnd();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed == false)
            {
                if (disposing)
                    ((IFileShareAccessor)this).Disconnect();
                _isDisposed = true;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, @"Fileshare: {0}. Domain: {1}. Username: {2}", _fileShare ?? string.Empty, _domain ?? string.Empty, _user ?? string.Empty);
        }
    }
}