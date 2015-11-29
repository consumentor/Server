namespace Microsoft.Web.Mvc.Build {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    // consumes "{areaname}.area-manifest.xml" files and performs copy of <content>
    public class CopyAreaManifests : Task {

        private IFileSystem _fileSystem;

        [Required]
        public bool CrossCopy { get; set; }

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

        [Required]
        public bool RenameViews { get; set; }

        public override bool Execute() {
            string[] files = FileSystem.GetFiles(ManifestPath, "*.area-manifest.xml", SearchOption.TopDirectoryOnly);
            AreaInfo[] areaInfos = Array.ConvertAll<string, AreaInfo>(files, DeserializeFile);

            // first, cross-copy between parent and child areas
            var infosByType = areaInfos.ToLookup(info => info.Type);
            foreach (AreaInfo parentInfo in infosByType[AreaType.Parent]) {
                foreach (AreaInfo childInfo in infosByType[AreaType.Child]) {
                    Copy(childInfo, parentInfo, CrossCopy);
                }
            }

            // next, copy content areas everywhere (if cross-copy enabled)
            var infosByIsContent = areaInfos.ToLookup(info => info.Type == AreaType.Content);
            foreach (AreaInfo contentInfo in infosByIsContent[true]) {
                foreach (AreaInfo targetInfo in infosByIsContent[false]) {
                    // don't copy content -> child if cross-copy disabled
                    if (CrossCopy || targetInfo.Type == AreaType.Parent) {
                        Copy(contentInfo, targetInfo, false /* doCrossCopy */);
                    }
                }
            }


            // if we got this far, success!
            return true;
        }

        private AreaInfo DeserializeFile(string file) {
            XmlSerializer serializer = new XmlSerializer(typeof(AreaInfo));
            using (Stream stream = FileSystem.FileOpen(file, FileMode.Open)) {
                return (AreaInfo)serializer.Deserialize(stream);
            }
        }

        private void Copy(AreaInfo originInfo, AreaInfo destinationInfo, bool doCrossCopy) {
            // need to be careful not to delete existing <content> files, but can overwrite existing non-<content> files

            { // step 1 - copy from origin to destination
                HashSet<string> existingDestinationContentFiles = new HashSet<string>(destinationInfo.ContentFiles, StringComparer.OrdinalIgnoreCase);

                var filesToCopy = from originFileName in originInfo.ContentFiles
                                  let destinationFileName = RewriteViewPath(originInfo.Name, originFileName)
                                  where !existingDestinationContentFiles.Contains(destinationFileName)
                                  select new {
                                      OriginFile = originFileName,
                                      DestinationFile = destinationFileName
                                  };

                foreach (var fileToCopy in filesToCopy) {
                    CopyFile(originInfo.Path, fileToCopy.OriginFile, destinationInfo.Path, fileToCopy.DestinationFile);
                }
            }

            if (doCrossCopy) {
                // step 2 - copy from destination to origin
                var filesToCopy = destinationInfo.ContentFiles.Except(originInfo.ContentFiles);
                foreach (string fileToCopy in filesToCopy) {
                    CopyFile(destinationInfo.Path, fileToCopy, originInfo.Path, fileToCopy);
                }
            }
        }

        private void CopyFile(string originBasePath, string originFileName, string destinationBasePath, string destinationFileName) {
            string originPath = Path.Combine(originBasePath, originFileName);
            if (!FileSystem.FileExists(originPath)) {
                // if file does not exist, assume the manifest is old and just move on
                return;
            }

            // ensure destination directory exists before copy
            string destinationPath = Path.Combine(destinationBasePath, destinationFileName);
            string destinationDir = Path.GetDirectoryName(destinationPath);
            FileSystem.CreateDirectory(destinationDir);
            FileSystem.FileCopy(originPath, destinationPath, true /* overwrite */);
        }

        private string RewriteViewPath(string areaName, string relativePath) {
            if (RenameViews && relativePath.StartsWith(@"Views\", StringComparison.OrdinalIgnoreCase)) {
                return String.Format(@"Views\Areas\{0}\{1}", areaName, relativePath.Substring(@"Views\".Length));
            }
            else {
                return relativePath;
            }
        }

    }
}
