namespace System.Web.Mvc.Test {
    using System.Web.Hosting;
    using System.Web.Mvc;
    using System.Web.TestUtil;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class WebFormViewEngineTest {

        [TestMethod]
        public void BuildManagerProperty() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            MockBuildManager buildManagerMock = new MockBuildManager(null, null, null);

            // Act
            engine.BuildManager = buildManagerMock;

            // Assert
            Assert.AreEqual(engine.BuildManager, buildManagerMock);
        }

        [TestMethod]
        public void CreatePartialViewCreatesWebFormView() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Act
            WebFormView result = (WebFormView)engine.CreatePartialView("partial path");

            // Assert
            Assert.AreEqual("partial path", result.ViewPath);
            Assert.AreEqual(String.Empty, result.MasterPath);
        }

        [TestMethod]
        public void CreateViewCreatesWebFormView() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Act
            WebFormView result = (WebFormView)engine.CreateView("view path", "master path");

            // Assert
            Assert.AreEqual("view path", result.ViewPath);
            Assert.AreEqual("master path", result.MasterPath);
        }

        [TestMethod]
        public void FileExistsReturnsTrueForExistingPath() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            object instanceResult = new object();
            MockBuildManager buildManagerMock = new MockBuildManager("some path", typeof(object), instanceResult);
            engine.BuildManager = buildManagerMock;

            // Act
            bool foundResult = engine.PublicFileExists(null, "some path");

            // Assert
            Assert.IsTrue(foundResult);
        }

        [TestMethod]
        public void FileExistsReturnsFalseWhenBuildManagerReturnsNull() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            MockBuildManager buildManagerMock = new MockBuildManager("some path", typeof(object), null);
            engine.BuildManager = buildManagerMock;

            // Act
            bool notFoundResult = engine.PublicFileExists(null, "some path");

            // Assert
            Assert.IsFalse(notFoundResult);
        }

        [TestMethod]
        public void FileExistsReturnsFalseWhenBuildManagerThrows404AndViewFileDoesNotExist() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            object instanceResult = new object();
            MockBuildManager buildManagerMock = new MockBuildManager(new HttpException(404, "HTTP message Not Found"));
            engine.BuildManager = buildManagerMock;
            engine.PublicVirtualPathProvider = new Mock<VirtualPathProvider>().Object;

            // Act
            bool notFoundResult = engine.PublicFileExists(null, "some other path");

            // Assert
            Assert.IsFalse(notFoundResult);
        }

        [TestMethod]
        public void FileExistsThrowsWhenBuildManagerThrows404ButViewFileExists() {
            // Arrange
            Mock<VirtualPathProvider> mockVpp = new Mock<VirtualPathProvider>();
            mockVpp.Expect(o => o.FileExists("some path")).Returns(true);

            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            MockBuildManager buildManagerMock = new MockBuildManager(new HttpException(404, "HTTP message Not Found"));
            engine.BuildManager = buildManagerMock;
            engine.PublicVirtualPathProvider = mockVpp.Object;

            // Act & assert
            ExceptionHelper.ExpectHttpException(
                delegate {
                    engine.PublicFileExists(null, "some path");
                }, "HTTP message Not Found", 404);
        }

        [TestMethod]
        public void FileExistsThrowsWhenBuildManagerThrowsHttpParseException() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            MockBuildManager buildManagerMock = new MockBuildManager(new HttpParseException());
            engine.BuildManager = buildManagerMock;

            // Act & assert
            ExceptionHelper.ExpectException<HttpParseException>(
                delegate {
                    engine.PublicFileExists(null, "some path");
                });
        }

        [TestMethod]
        public void FileExistsThrowsWhenBuildManagerThrowsNon404() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            object instanceResult = new object();
            MockBuildManager buildManagerMock = new MockBuildManager(new HttpException(123, "HTTP random message"));
            engine.BuildManager = buildManagerMock;

            // Act & Assert
            ExceptionHelper.ExpectHttpException(
                () => engine.PublicFileExists(null, "some other path"),
                "HTTP random message",
                123);
        }

        [TestMethod]
        public void FileExistsThrowsWhenBuildManagerThrowsNonHttpException() {
            // Arrange
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();
            MockBuildManager buildManagerMock = new MockBuildManager(new InvalidOperationException("Some exception message."));
            engine.BuildManager = buildManagerMock;

            // Act & Assert
            ExceptionHelper.ExpectInvalidOperationException(
                delegate {
                    engine.PublicFileExists(null, "some path");
                }, "Some exception message.");
        }

        [TestMethod]
        public void MasterLocationFormatsProperty() {
            // Arrange
            string[] expected = new string[] {
                "~/Views/{1}/{0}.master",
                "~/Views/Shared/{0}.master"
            };

            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            CollectionAssert.AreEqual(expected, engine.MasterLocationFormats);
        }

        [TestMethod]
        public void AreaMasterLocationFormatsProperty() {
            // Arrange
            string[] expected = new string[] {
                "~/Areas/{2}/Views/{1}/{0}.master",
                "~/Areas/{2}/Views/Shared/{0}.master",
            };

            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            CollectionAssert.AreEqual(expected, engine.AreaMasterLocationFormats);
        }

        [TestMethod]
        public void PartialViewLocationFormatsProperty() {
            // Arrange
            string[] expected = new string[] {
                "~/Views/{1}/{0}.aspx",
                "~/Views/{1}/{0}.ascx",
                "~/Views/Shared/{0}.aspx",
                "~/Views/Shared/{0}.ascx"
            };

            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            CollectionAssert.AreEqual(expected, engine.PartialViewLocationFormats);
        }

        [TestMethod]
        public void AreaPartialViewLocationFormatsProperty() {
            // Arrange
            string[] expected = new string[] {
                "~/Areas/{2}/Views/{1}/{0}.aspx",
                "~/Areas/{2}/Views/{1}/{0}.ascx",
                "~/Areas/{2}/Views/Shared/{0}.aspx",
                "~/Areas/{2}/Views/Shared/{0}.ascx",
            };

            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            CollectionAssert.AreEqual(expected, engine.AreaPartialViewLocationFormats);
        }

        [TestMethod]
        public void ViewLocationFormatsProperty() {
            // Arrange
            string[] expected = new string[] {
                "~/Views/{1}/{0}.aspx",
                "~/Views/{1}/{0}.ascx",
                "~/Views/Shared/{0}.aspx",
                "~/Views/Shared/{0}.ascx"
            };

            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            CollectionAssert.AreEqual(expected, engine.ViewLocationFormats);
        }

        [TestMethod]
        public void AreaViewLocationFormatsProperty() {
            // Arrange
            string[] expected = new string[] {
                "~/Areas/{2}/Views/{1}/{0}.aspx",
                "~/Areas/{2}/Views/{1}/{0}.ascx",
                "~/Areas/{2}/Views/Shared/{0}.aspx",
                "~/Areas/{2}/Views/Shared/{0}.ascx",
            };

            // Act
            TestableWebFormViewEngine engine = new TestableWebFormViewEngine();

            // Assert
            CollectionAssert.AreEqual(expected, engine.AreaViewLocationFormats);
        }

        internal sealed class TestableWebFormViewEngine : WebFormViewEngine {

            public VirtualPathProvider PublicVirtualPathProvider {
                get {
                    return VirtualPathProvider;
                }
                set {
                    VirtualPathProvider = value;
                }
            }

            public IView CreatePartialView(string partialPath) {
                return base.CreatePartialView(null, partialPath);
            }

            public IView CreateView(string viewPath, string masterPath) {
                return base.CreateView(null, viewPath, masterPath);
            }

            public bool PublicFileExists(ControllerContext controllerContext, string virtualPath) {
                return base.FileExists(controllerContext, virtualPath);
            }
        }

    }
}
