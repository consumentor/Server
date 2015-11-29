namespace System.Web.Mvc.Test {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    // Custom mock IBuildManager since the mock framework doesn't support mocking internal types
    public class MockBuildManager : IBuildManager {
        private Assembly[] _referencedAssemblies;

        private string _expectedVirtualPath;
        private Type _expectedBaseType;
        private object _instanceResult;
        private Exception _createInstanceFromVirtualPathException;

        public readonly Dictionary<string, Stream> CachedFileStore = new Dictionary<string, Stream>(StringComparer.OrdinalIgnoreCase);

        public MockBuildManager()
            : this(new Assembly[] { typeof(MockBuildManager).Assembly }) {
        }

        public MockBuildManager(Assembly[] referencedAssemblies) {
            _referencedAssemblies = referencedAssemblies;
        }

        public MockBuildManager(Exception createInstanceFromVirtualPathException) {
            _createInstanceFromVirtualPathException = createInstanceFromVirtualPathException;
        }

        public MockBuildManager(string expectedVirtualPath, Type expectedBaseType, object instanceResult) {
            _expectedVirtualPath = expectedVirtualPath;
            _expectedBaseType = expectedBaseType;
            _instanceResult = instanceResult;
        }

        #region IBuildManager Members
        object IBuildManager.CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType) {
            if (_createInstanceFromVirtualPathException != null) {
                throw _createInstanceFromVirtualPathException;
            }

            if ((_expectedVirtualPath == virtualPath) && (_expectedBaseType == requiredBaseType)) {
                return _instanceResult;
            }
            throw new InvalidOperationException("Unexpected call to IBuildManager.CreateInstanceFromVirtualPath()");
        }

        ICollection IBuildManager.GetReferencedAssemblies() {
            return _referencedAssemblies;
        }

        Stream IBuildManager.ReadCachedFile(string fileName) {
            Stream stream;
            CachedFileStore.TryGetValue(fileName, out stream);
            return stream;
        }

        Stream IBuildManager.CreateCachedFile(string fileName) {
            MemoryStream stream = new MemoryStream();
            CachedFileStore[fileName] = stream;
            return stream;
        }
        #endregion
    }
}
