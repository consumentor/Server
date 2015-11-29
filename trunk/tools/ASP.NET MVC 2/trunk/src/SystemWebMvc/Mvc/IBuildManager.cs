namespace System.Web.Mvc {
    using System.Collections;
    using System.IO;

    // REVIEW: Should we make this public?
    internal interface IBuildManager {
        object CreateInstanceFromVirtualPath(string virtualPath, Type requiredBaseType);
        ICollection GetReferencedAssemblies();

        // ASP.NET 4 methods
        Stream ReadCachedFile(string fileName);
        Stream CreateCachedFile(string fileName);
    }
}
