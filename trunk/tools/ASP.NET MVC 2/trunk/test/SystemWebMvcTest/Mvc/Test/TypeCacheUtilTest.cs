namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeCacheUtilTest {

        [TestMethod]
        public void GetFilteredTypesFromAssemblies_FallThrough() {
            // Arrange
            Type[] expectedTypes = new Type[] {
                typeof(TypeCacheValidFoo),
                typeof(TypeCacheValidBar)
            };

            string cacheName = "testCache";
            MockBuildManager buildManager = new MockBuildManager();
            Predicate<Type> predicate = type => type.IsDefined(typeof(TypeCacheMarkerAttribute), true);

            // Act
            List<Type> returnedTypes = TypeCacheUtil.GetFilteredTypesFromAssemblies(cacheName, predicate, buildManager);

            // Assert
            CollectionAssert.AreEquivalent(expectedTypes, returnedTypes, "Correct types were not returned.");

            MemoryStream cachedStream = buildManager.CachedFileStore[cacheName] as MemoryStream;
            Assert.IsNotNull(cachedStream, "Stream was not created.");
            Assert.AreNotEqual(0, cachedStream.ToArray().Length, "Data was not written correctly.");
        }

        [TestMethod]
        public void SaveToCache_ReadFromCache_ReturnsNullIfTypesAreInvalid() {
            //
            // SAVING
            //

            // Arrange
            Type[] expectedTypes = new Type[] {
                typeof(object),
                typeof(string)
            };

            TypeCacheSerializer serializer = new TypeCacheSerializer();
            string cacheName = "testCache";
            MockBuildManager buildManager = new MockBuildManager();

            // Act
            TypeCacheUtil.SaveTypesToCache(cacheName, expectedTypes, buildManager, serializer);

            // Assert
            MemoryStream writeStream = buildManager.CachedFileStore[cacheName] as MemoryStream;
            Assert.IsNotNull(writeStream, "Stream should've been created.");

            byte[] streamContents = writeStream.ToArray();
            Assert.AreNotEqual(0, streamContents.Length, "Data should've been written to the stream.");

            //
            // READING
            //

            // Arrange
            MemoryStream readStream = new MemoryStream(streamContents);
            buildManager.CachedFileStore[cacheName] = readStream;

            // Act
            List<Type> returnedTypes = TypeCacheUtil.ReadTypesFromCache(cacheName, _ => false /* all types are invalid */, buildManager, serializer);

            // Assert
            Assert.IsNull(returnedTypes);
        }

        [TestMethod]
        public void SaveToCache_ReadFromCache_Success() {
            //
            // SAVING
            //

            // Arrange
            Type[] expectedTypes = new Type[] {
                typeof(object),
                typeof(string)
            };

            TypeCacheSerializer serializer = new TypeCacheSerializer();
            string cacheName = "testCache";
            MockBuildManager buildManager = new MockBuildManager();

            // Act
            TypeCacheUtil.SaveTypesToCache(cacheName, expectedTypes, buildManager, serializer);

            // Assert
            MemoryStream writeStream = buildManager.CachedFileStore[cacheName] as MemoryStream;
            Assert.IsNotNull(writeStream, "Stream should've been created.");

            byte[] streamContents = writeStream.ToArray();
            Assert.AreNotEqual(0, streamContents.Length, "Data should've been written to the stream.");

            //
            // READING
            //

            // Arrange
            MemoryStream readStream = new MemoryStream(streamContents);
            buildManager.CachedFileStore[cacheName] = readStream;

            // Act
            List<Type> returnedTypes = TypeCacheUtil.ReadTypesFromCache(cacheName, _ => true /* all types are valid */, buildManager, serializer);

            // Assert
            CollectionAssert.AreEquivalent(expectedTypes, returnedTypes);
        }

    }

    public class TypeCacheMarkerAttribute : Attribute {
    }

    [TypeCacheMarker]
    public class TypeCacheValidFoo {
    }

    [TypeCacheMarker]
    public class TypeCacheValidBar {
    }

    [TypeCacheMarker]
    internal class TypeCacheInvalidInternal {
    }

    [TypeCacheMarker]
    public abstract class TypeCacheInvalidAbstract {
    }

    [TypeCacheMarker]
    public struct TypeCacheInvalidStruct {
    }
}
