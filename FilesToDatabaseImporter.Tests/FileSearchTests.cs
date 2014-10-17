using System;
using System.Collections.Generic;
using System.Linq;
using FilesToDatabaseImporter.Helpers;
using FilesToDatabaseImporter.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FilesToDatabaseImporter.Tests
{
    [TestClass]
    public class FileSearchTests
    {
        [TestMethod]
        public void FileSearch_Constructor()
        {
            var FileSearch = new FileSearch();

            Assert.IsNotNull(FileSearch);
        }        
        
        
        
        // HasExtension
        [TestMethod]
        public void FileSearch_HasExtension_Correct_ReturnsTrue()
        {
            // Arrange
            var FileSearch = new FileSearch();
            var fileName = "C:\\test.png";
            var extensions = new[] {"png"};


            // Act
            var privObject = new PrivateObject(FileSearch);

            var result = (bool)privObject.Invoke("HasExtension", new object[] { fileName, extensions });

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FileSearch_HasExtension_NotMatchingFilter_ReturnsFalse()
        {
            // Arrange
            var FileSearch = new FileSearch();
            var fileName = "C:\\test.png";
            var extensions = new[] { "jpg" };


            // Act
            var privObject = new PrivateObject(FileSearch);

            var result = (bool)privObject.Invoke("HasExtension", new object[] { fileName, extensions });

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FileSearch_HasExtension_WithoutExtension_ReturnsFalse()
        {
            // Arrange
            var FileSearch = new FileSearch();
            var fileName = "C:\\test";
            var extensions = new[] { "jpg" };


            // Act
            var privObject = new PrivateObject(FileSearch);
            var result = (bool)privObject.Invoke("HasExtension", new object[] { fileName, extensions });

            // Assert
            Assert.IsFalse(result);
        }





        
        // SetExtension
        [TestMethod]
        public void FileSearch_SetExtension()
        {
            // Arrange
            var FileSearch = new FileSearch();
            var extensions = new[] { "png", "jpg" };


            // Act
            var privObject = new PrivateObject(FileSearch);

            var result = (FileSearch)privObject.Invoke("SetExtension", new object[] { extensions });
            var privateExtensions = (string[])privObject.GetField("_extensions");

            var isFieldSet = extensions == privateExtensions;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(isFieldSet);
        }






        // SetDirectory
        [TestMethod]
        public void FileSearch_SetDirectory()
        {
            // Arrange
            var FileSearch = new FileSearch();
            var directory = "C:\test";


            // Act
            var privObject = new PrivateObject(FileSearch);

            var result = (FileSearch)privObject.Invoke("SetDirectory", new object[] { directory });
            var privateDirectory = (string)privObject.GetField("_directory");

            var isFieldSet = directory == privateDirectory;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(isFieldSet);
        }







        // Iterate
        [TestMethod]
        public void FileSearch_Iterate()
        {
            var files = new List<string>() {"test.txt"}.ToArray();

            // Arrange
            var directoryHelperMock = new Mock<IDirectoryHelper>();
            directoryHelperMock.Setup(i => i.GetCreatedDate(It.IsAny<string>())).Returns(new DateTime());
            directoryHelperMock.Setup(i => i.GetContent(It.IsAny<string>())).Returns("Content");
            directoryHelperMock.Setup(i => i.GetDirectories(It.IsAny<string>())).Returns(new List<string>());
            directoryHelperMock.Setup(i => i.GetFiles(It.IsAny<string>())).Returns(files);


            var FileSearch = new FileSearch(directoryHelperMock.Object);


            // Act
            var privObject = new PrivateObject(FileSearch);

            var result = privObject.Invoke("Iterate", new object[] { "C:\test" });

            var _files = (List<File>)privObject.GetField("_files");

            // Assert
            Assert.IsTrue(_files.Any());
        }

        [TestMethod]
        public void FileSearch_Iterate_ExtensionFilter()
        {
            var files = new List<string>() { "test.txt" }.ToArray();

            // Arrange
            var directoryHelperMock = new Mock<IDirectoryHelper>();
            directoryHelperMock.Setup(i => i.GetCreatedDate(It.IsAny<string>())).Returns(new DateTime());
            directoryHelperMock.Setup(i => i.GetContent(It.IsAny<string>())).Returns("Content");
            directoryHelperMock.Setup(i => i.GetDirectories(It.IsAny<string>())).Returns(new List<string>());
            directoryHelperMock.Setup(i => i.GetFiles(It.IsAny<string>())).Returns(files);


            var FileSearch = new FileSearch(directoryHelperMock.Object).SetExtension(new string[] { "png" });


            // Act
            var privObject = new PrivateObject(FileSearch);

            var result = privObject.Invoke("Iterate", new object[] { "C:\test" });

            var _files = (List<File>)privObject.GetField("_files");

            // Assert
            Assert.IsTrue(!_files.Any());
        }

        [TestMethod]
        public void FileSearch_Iterate_NotRecursive()
        {
            var files = new List<string>() { "test.txt" }.ToArray();

            // Arrange
            var directoryHelperMock = new Mock<IDirectoryHelper>();
            directoryHelperMock.Setup(i => i.GetCreatedDate(It.IsAny<string>())).Returns(new DateTime());
            directoryHelperMock.Setup(i => i.GetContent(It.IsAny<string>())).Returns("Content");
            directoryHelperMock.Setup(i => i.GetDirectories(It.IsAny<string>())).Verifiable();
            directoryHelperMock.Setup(i => i.GetFiles(It.IsAny<string>())).Returns(files);


            var FileSearch = new FileSearch(directoryHelperMock.Object).SetRecursive(false);


            // Act
            var privObject = new PrivateObject(FileSearch);

            var result = privObject.Invoke("Iterate", new object[] { "C:\test" });

            var _files = (List<File>)privObject.GetField("_files");

            // Assert
            Assert.IsTrue(_files.Any());
            directoryHelperMock.Verify(i => i.GetDirectories(It.IsAny<string>()), Times.Never);
        }


        // Search
        [TestMethod]
        public void FileSearch_Search()
        {
            var files = new List<string>() { "test.txt" }.ToArray();

            // Arrange
            var directoryHelperMock = new Mock<IDirectoryHelper>();
            directoryHelperMock.Setup(i => i.GetCreatedDate(It.IsAny<string>())).Returns(new DateTime());
            directoryHelperMock.Setup(i => i.GetContent(It.IsAny<string>())).Returns("Content");
            directoryHelperMock.Setup(i => i.GetDirectories(It.IsAny<string>())).Returns(new List<string>());
            directoryHelperMock.Setup(i => i.GetFiles(It.IsAny<string>())).Returns(files);


            var FileSearch = new FileSearch(directoryHelperMock.Object).SetDirectory("C:\test");


            // Act
            var result = FileSearch.Search();


            // Assert
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FileSearch_Search_NoDirectory_ThrowsException()
        {
            // Arrange

            var FileSearch = new FileSearch();


            // Act
            var result = FileSearch.Search();


            // Assert
            Assert.IsTrue(result.Any());
        }




        // SetRecursive
        [TestMethod]
        public void FileSearch_SetRecursive()
        {
            // Arrange
            var FileSearch = new FileSearch();
            var recursive = true;


            // Act
            var privObject = new PrivateObject(FileSearch);

            var result = (FileSearch)privObject.Invoke("SetRecursive", new object[] { recursive });
            var privateRecursive = (bool)privObject.GetField("_recursive");

            var isFieldSet = recursive == privateRecursive;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(isFieldSet);
        }
    }
}
