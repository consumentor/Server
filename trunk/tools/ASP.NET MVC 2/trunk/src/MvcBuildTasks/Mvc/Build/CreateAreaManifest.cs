namespace Microsoft.Web.Mvc.Build {
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    // creates a manifest file "{areaname}.area-manifest.xml" that the cross-copy task can consume
    public class CreateAreaManifest : Task {

        private IFileSystem _fileSystem;

        [Required]
        public string AreaName { get; set; }

        [Required]
        public string AreaPath { get; set; }

        [Required]
        public string AreaType { get; set; }

        [Required]
        public ITaskItem[] ContentFiles { get; set; }

        internal IFileSystem FileSystem {
            get {
                if (_fileSystem == null) {
                    _fileSystem = new FileSystem();
                }
                return _fileSystem;
            }
            set {
                _fileSystem = value;
            }
        }

        [Required]
        public string ManifestPath { get; set; }

        public override bool Execute() {
            string outputFileName = String.Format("{0}.area-manifest.xml", AreaName);
            string outputFullPath = Path.Combine(ManifestPath, outputFileName);

            AreaInfo info = new AreaInfo() {
                Name = AreaName,
                Path = AreaPath,
                Type = (AreaType)Enum.Parse(typeof(AreaType), AreaType),
                ContentFiles = ContentFiles.Select(o => o.ItemSpec).ToArray()
            };

            // ensure output directory exists before file creation
            string outputDir = Path.GetDirectoryName(outputFullPath);
            FileSystem.CreateDirectory(outputDir);

            XmlSerializer serializer = new XmlSerializer(typeof(AreaInfo));
            using (Stream stream = FileSystem.FileOpen(outputFullPath, FileMode.Create)) {
                serializer.Serialize(stream, info);
            }

            // if we got this far, success!
            return true;
        }

    }
}
