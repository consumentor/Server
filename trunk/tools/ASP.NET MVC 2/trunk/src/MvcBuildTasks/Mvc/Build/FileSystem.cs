namespace Microsoft.Web.Mvc.Build {
    using System;
    using System.IO;

    internal sealed class FileSystem : IFileSystem {

        public void CreateDirectory(string path) {
            Directory.CreateDirectory(path);
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption) {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }

        public void FileCopy(string sourceFileName, string destFileName, bool overwrite) {
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        public bool FileExists(string path) {
            return File.Exists(path);
        }

        public Stream FileOpen(string path, FileMode mode) {
            return File.Open(path, mode);
        }

    }
}
