namespace Microsoft.Web.Mvc.Build.Test {
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Build.Framework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class CreateAreaManifestTest {

        [TestMethod]
        public void ExecuteCreatesManifestFile() {
            // Arrange
            string[] contentFiles = new string[] {
                @"Content\Billing.css",
                @"Content\CreditCard.jpg",
                @"Views\Billing\Payment.aspx",
                @"Global.asax",
                @"Web.config"
            };

            MemoryStream ms = new MemoryStream();
            Mock<IFileSystem> mockFS = new Mock<IFileSystem>();
            mockFS.Expect(o => o.CreateDirectory(@"X:\path_to_manifest")).Verifiable();
            mockFS.Expect(o => o.FileOpen(@"X:\path_to_manifest\TheAreaName.area-manifest.xml", FileMode.Create)).Returns(ms);

            CreateAreaManifest task = new CreateAreaManifest() {
                AreaName = "TheAreaName",
                AreaPath = @"X:\path_to_area",
                AreaType = "Child",
                ManifestPath = @"X:\path_to_manifest",
                ContentFiles = Array.ConvertAll<string, ITaskItem>(contentFiles, StringToTaskItem),
                FileSystem = mockFS.Object
            };

            // Act
            bool retVal = task.Execute();

            // Assert
            Assert.IsTrue(retVal);
            mockFS.Verify();

            byte[] rawStreamBytes = ms.ToArray();
            string contents = Encoding.UTF8.GetString(rawStreamBytes);
            Assert.AreEqual(@"<?xml version=""1.0""?>
<AreaInfo xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <ContentFiles>
    <string>Content\Billing.css</string>
    <string>Content\CreditCard.jpg</string>
    <string>Views\Billing\Payment.aspx</string>
    <string>Global.asax</string>
    <string>Web.config</string>
  </ContentFiles>
  <Name>TheAreaName</Name>
  <Path>X:\path_to_area</Path>
  <Type>Child</Type>
</AreaInfo>", contents);
        }

        private static ITaskItem StringToTaskItem(string input) {
            Mock<ITaskItem> mockTaskItem = new Mock<ITaskItem>();
            mockTaskItem.Expect(o => o.ItemSpec).Returns(input);
            return mockTaskItem.Object;
        }

    }
}
