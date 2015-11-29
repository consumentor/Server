using System;

namespace Consumentor.ShopGun.FileSystem
{
    public interface IFileShareAccessor : IDisposable
    {
        string Password { get; }
        string User { get; }
        string Domain { get; }
        string FileShare { get; }
        bool IsAccessible { get; }
        bool Connect(string fileShare, string domain, string userName, string password);
        bool Connect(string fileShare, string userName, string password);
        bool Connect(string fileShare);
        void Disconnect();
        void EnsureAccess(string fileShare, string domain, string userName, string password);
    }
}