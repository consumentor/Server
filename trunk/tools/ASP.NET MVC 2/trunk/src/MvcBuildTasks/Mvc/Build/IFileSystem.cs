namespace Microsoft.Web.Mvc.Build {
    using System;
    using System.IO;

    public interface IFileSystem {

        void CreateDirectory(string path);
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
        void FileCopy(string sourceFileName, string destFileName, bool overwrite);
        bool FileExists(string path);
        Stream FileOpen(string path, FileMode mode);

    }
}
