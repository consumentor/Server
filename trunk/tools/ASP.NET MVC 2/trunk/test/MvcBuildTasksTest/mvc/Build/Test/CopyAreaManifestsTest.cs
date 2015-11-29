namespace Microsoft.Web.Mvc.Build.Test {
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Language.Flow;

    [TestClass]
    public class CopyAreaManifestTests {

        private const string _childManifest = @"<?xml version=""1.0""?>
<AreaInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ContentFiles>
    <string>Content\FromChild.stuff</string>
    <string>Content\NonExistent.stuff</string>
    <string>Views\Billing\FromChild.aspx</string>
    <string>Views\Shared\FileCollision.Master</string>
  </ContentFiles>
  <Name>TheChild</Name>
  <Path>X:\path_to_child\</Path>
  <Type>Child</Type>
</AreaInfo>";

        private const string _parentManifest = @"<?xml version=""1.0""?>
<AreaInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ContentFiles>
    <string>Content\FromParent.stuff</string>
    <string>Views\Home\Index.aspx</string>
    <string>Views\Shared\FileCollision.Master</string>
  </ContentFiles>
  <Name>TheParent</Name>
  <Path>X:\path_to_parent\</Path>
  <Type>Parent</Type>
</AreaInfo>";

        private const string _contentManifest = @"<?xml version=""1.0""?>
<AreaInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ContentFiles>
    <string>Content\FromContent.stuff</string>
    <string>Scripts\SomeScript.js</string>
  </ContentFiles>
  <Name>TheContent</Name>
  <Path>X:\path_to_content\</Path>
  <Type>Content</Type>
</AreaInfo>";

        private const string _manifestPath = @"X:\path_to_manifests\";

        [TestMethod]
        public void Execute() {
            // Arrange
            Mock<IFileSystem> mockFs = CreateMockFileSystem();

            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Content");
            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Scripts");
            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Views\Billing");

            ExpectFileCopy(mockFs, @"X:\path_to_child\Content\FromChild.stuff", @"X:\path_to_parent\Content\FromChild.stuff");
            ExpectFileCopy(mockFs, @"X:\path_to_child\Views\Billing\FromChild.aspx", @"X:\path_to_parent\Views\Billing\FromChild.aspx");
            ExpectFileCopy(mockFs, @"X:\path_to_content\Content\FromContent.stuff", @"X:\path_to_parent\Content\FromContent.stuff");
            ExpectFileCopy(mockFs, @"X:\path_to_content\Scripts\SomeScript.js", @"X:\path_to_parent\Scripts\SomeScript.js");

            CopyAreaManifests task = new CopyAreaManifests() {
                ManifestPath = _manifestPath,
                FileSystem = mockFs.Object
            };

            // Act
            bool retVal = task.Execute();

            // Assert
            Assert.IsTrue(retVal);
            mockFs.Verify();
        }

        [TestMethod]
        public void ExecuteWithCrossCopy() {
            // Arrange
            Mock<IFileSystem> mockFs = CreateMockFileSystem();

            // copy step
            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Content");
            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Scripts");
            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Views\Billing");

            ExpectFileCopy(mockFs, @"X:\path_to_child\Content\FromChild.stuff", @"X:\path_to_parent\Content\FromChild.stuff");
            ExpectFileCopy(mockFs, @"X:\path_to_child\Views\Billing\FromChild.aspx", @"X:\path_to_parent\Views\Billing\FromChild.aspx");
            ExpectFileCopy(mockFs, @"X:\path_to_content\Content\FromContent.stuff", @"X:\path_to_parent\Content\FromContent.stuff");
            ExpectFileCopy(mockFs, @"X:\path_to_content\Scripts\SomeScript.js", @"X:\path_to_parent\Scripts\SomeScript.js");

            // cross-copy step
            ExpectCreateDirectory(mockFs, @"X:\path_to_child\Content");
            ExpectCreateDirectory(mockFs, @"X:\path_to_child\Scripts");
            ExpectCreateDirectory(mockFs, @"X:\path_to_child\Views\Home");

            ExpectFileCopy(mockFs, @"X:\path_to_parent\Content\FromParent.stuff", @"X:\path_to_child\Content\FromParent.stuff");
            ExpectFileCopy(mockFs, @"X:\path_to_parent\Views\Home\Index.aspx", @"X:\path_to_child\Views\Home\Index.aspx");
            ExpectFileCopy(mockFs, @"X:\path_to_content\Content\FromContent.stuff", @"X:\path_to_child\Content\FromContent.stuff");
            ExpectFileCopy(mockFs, @"X:\path_to_content\Scripts\SomeScript.js", @"X:\path_to_child\Scripts\SomeScript.js");

            CopyAreaManifests task = new CopyAreaManifests() {
                ManifestPath = _manifestPath,
                CrossCopy = true,
                FileSystem = mockFs.Object
            };

            // Act
            bool retVal = task.Execute();

            // Assert
            Assert.IsTrue(retVal);
            mockFs.Verify();
        }

        [TestMethod]
        public void ExecuteWithViewRewriting() {
            // Arrange
            Mock<IFileSystem> mockFs = CreateMockFileSystem();

            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Content");
            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Scripts");
            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Views\Areas\TheChild\Billing");
            ExpectCreateDirectory(mockFs, @"X:\path_to_parent\Views\Areas\TheChild\Shared");

            ExpectFileCopy(mockFs, @"X:\path_to_child\Content\FromChild.stuff", @"X:\path_to_parent\Content\FromChild.stuff");
            ExpectFileCopy(mockFs, @"X:\path_to_child\Views\Billing\FromChild.aspx", @"X:\path_to_parent\Views\Areas\TheChild\Billing\FromChild.aspx");
            ExpectFileCopy(mockFs, @"X:\path_to_child\Views\Shared\FileCollision.Master", @"X:\path_to_parent\Views\Areas\TheChild\Shared\FileCollision.Master");
            ExpectFileCopy(mockFs, @"X:\path_to_content\Content\FromContent.stuff", @"X:\path_to_parent\Content\FromContent.stuff");
            ExpectFileCopy(mockFs, @"X:\path_to_content\Scripts\SomeScript.js", @"X:\path_to_parent\Scripts\SomeScript.js");

            CopyAreaManifests task = new CopyAreaManifests() {
                ManifestPath = _manifestPath,
                RenameViews = true,
                FileSystem = mockFs.Object
            };

            // Act
            bool retVal = task.Execute();

            // Assert
            Assert.IsTrue(retVal);
            mockFs.Verify();
        }

        private static MemoryStream CreateMemoryStream(string contents) {
            byte[] contentsRawBytes = Encoding.UTF8.GetBytes(contents);
            return new MemoryStream(contentsRawBytes);
        }

        private static Mock<IFileSystem> CreateMockFileSystem() {
            Mock<IFileSystem> mockFs = new Mock<IFileSystem>(MockBehavior.Strict);

            ExpectFileList(mockFs, "child", "parent", "content");
            ExpectReadFile(mockFs, "child", _childManifest);
            ExpectReadFile(mockFs, "parent", _parentManifest);
            ExpectReadFile(mockFs, "content", _contentManifest);

            ExpectFileExists(mockFs, @"X:\path_to_child\Content\FromChild.stuff", true);
            ExpectFileExists(mockFs, @"X:\path_to_child\Content\NonExistent.stuff", false);
            ExpectFileExists(mockFs, @"X:\path_to_child\Views\Billing\FromChild.aspx", true);
            ExpectFileExists(mockFs, @"X:\path_to_child\Views\Shared\FileCollision.Master", true);
            ExpectFileExists(mockFs, @"X:\path_to_parent\Content\FromParent.stuff", true);
            ExpectFileExists(mockFs, @"X:\path_to_parent\Views\Home\Index.aspx", true);
            ExpectFileExists(mockFs, @"X:\path_to_parent\Views\Shared\FileCollision.Master", true);
            ExpectFileExists(mockFs, @"X:\path_to_content\Content\FromContent.stuff", true);
            ExpectFileExists(mockFs, @"X:\path_to_content\Scripts\SomeScript.js", true);

            return mockFs;
        }

        private static void ExpectCreateDirectory(Mock<IFileSystem> mockFs, string path) {
            mockFs.Expect(o => o.CreateDirectory(path)).Verifiable();
        }

        private static void ExpectFileCopy(Mock<IFileSystem> mockFs, string source, string destination) {
            mockFs.Expect(o => o.FileCopy(source, destination, true)).Verifiable();
        }

        private static IReturnsResult ExpectFileList(Mock<IFileSystem> mockFs, params string[] files) {
            return mockFs.Expect(o => o.GetFiles(_manifestPath, "*.area-manifest.xml", SearchOption.TopDirectoryOnly)).Returns(files);
        }

        private static IReturnsResult ExpectFileExists(Mock<IFileSystem> mockFs, string fileName, bool exists) {
            return mockFs.Expect(o => o.FileExists(fileName)).Returns(exists);
        }

        private static IReturnsResult ExpectReadFile(Mock<IFileSystem> mockFs, string fileName, string fileContents) {
            return mockFs.Expect(o => o.FileOpen(fileName, FileMode.Open)).Returns(CreateMemoryStream(fileContents));
        }

    }
}
