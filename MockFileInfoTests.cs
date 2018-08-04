﻿using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace System.IO.Abstractions.TestingHelpers.Tests
{
    using XFS = MockUnixSupport;

    [TestFixture]
    public class MockFileInfoTests
    {
        [Test]
        public void MockFileInfo_Exists_ShouldReturnTrueIfFileExistsInMemoryFileSystem()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\b\c.txt"), new MockFileData("Demo text content") },
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var result = fileInfo.Exists;

            Assert.IsTrue(result);
        }

        [Test]
        public void MockFileInfo_Exists_ShouldReturnFalseIfFileDoesNotExistInMemoryFileSystem()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), new MockFileData("Demo text content") },
                { XFS.Path(@"c:\a\b\c.txt"), new MockFileData("Demo text content") },
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\foo.txt"));

            var result = fileInfo.Exists;

            Assert.IsFalse(result);
        }

        [Test]
        public void MockFileInfo_Length_ShouldReturnLengthOfFileInMemoryFileSystem()
        {
            const string fileContent = "Demo text content";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), new MockFileData(fileContent) },
                { XFS.Path(@"c:\a\b\c.txt"), new MockFileData(fileContent) },
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var result = fileInfo.Length;

            Assert.AreEqual(fileContent.Length, result);
        }

        [Test]
        public void MockFileInfo_Length_ShouldThrowFileNotFoundExceptionIfFileDoesNotExistInMemoryFileSystem()
        {
            const string fileContent = "Demo text content";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), new MockFileData(fileContent) },
                { XFS.Path(@"c:\a\b\c.txt"), new MockFileData(fileContent) },
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\foo.txt"));

            var ex = Assert.Throws<FileNotFoundException>(() => fileInfo.Length.ToString(CultureInfo.InvariantCulture));

            Assert.AreEqual(XFS.Path(@"c:\foo.txt"), ex.FileName);
        }

        [Test]
        public void MockFileInfo_CreationTimeUtc_ShouldReturnCreationTimeUtcOfFileInMemoryFileSystem()
        {
            var creationTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var result = fileInfo.CreationTimeUtc;

            Assert.AreEqual(creationTime.ToUniversalTime(), result);
        }

        [Test]
        public void MockFileInfo_CreationTimeUtc_ShouldSetCreationTimeUtcOfFileInMemoryFileSystem()
        {
            var creationTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var newUtcTime = DateTime.UtcNow;
            fileInfo.CreationTimeUtc = newUtcTime;

            Assert.AreEqual(newUtcTime, fileInfo.CreationTimeUtc);
        }


        [Test]
        public void MockFileInfo_CreationTime_ShouldReturnCreationTimeOfFileInMemoryFileSystem()
        {
            var creationTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var result = fileInfo.CreationTime;

            Assert.AreEqual(creationTime, result);
        }

        [Test]
        public void MockFileInfo_CreationTime_ShouldSetCreationTimeOfFileInMemoryFileSystem()
        {
            var creationTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { CreationTime = creationTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));
            var newTime = DateTime.Now;

            fileInfo.CreationTime = newTime;

            Assert.AreEqual(newTime, fileInfo.CreationTime);
        }

        [Test]
        public void MockFileInfo_IsReadOnly_ShouldSetReadOnlyAttributeOfFileInMemoryFileSystem()
        {
            var fileData = new MockFileData("Demo text content");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            fileInfo.IsReadOnly = true;

            Assert.AreEqual(FileAttributes.ReadOnly, fileData.Attributes & FileAttributes.ReadOnly);
        }

        [Test]
        public void MockFileInfo_IsReadOnly_ShouldSetNotReadOnlyAttributeOfFileInMemoryFileSystem()
        {
            var fileData = new MockFileData("Demo text content") {Attributes = FileAttributes.ReadOnly};
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            fileInfo.IsReadOnly = false;

            Assert.AreNotEqual(FileAttributes.ReadOnly, fileData.Attributes & FileAttributes.ReadOnly);
        }

        [Test]
        public void MockFileInfo_AppendText_ShouldAddTextToFileInMemoryFileSystem()
        {
            var fileData = new MockFileData("Demo text content");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            using (var file = fileInfo.AppendText())
                file.WriteLine("This should be at the end");

            string newcontents;
            using (var newfile = fileInfo.OpenText())
            {
                newcontents = newfile.ReadToEnd();
            }

            Assert.AreEqual($"Demo text contentThis should be at the end{Environment.NewLine}", newcontents);
        }

        [Test]
        public void MockFileInfo_OpenWrite_ShouldAddDataToFileInMemoryFileSystem()
        {
            var fileData = new MockFileData("Demo text content");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));
            var bytesToAdd = new byte[] {65, 66, 67, 68, 69};


            using (var file = fileInfo.OpenWrite())
            {
                file.Write(bytesToAdd, 0, bytesToAdd.Length);
            }

            string newcontents;
            using (var newfile = fileInfo.OpenText())
            {
                newcontents = newfile.ReadToEnd();
            }

            Assert.AreEqual("ABCDEtext content", newcontents);
        }
#if NET40
        [Test]
        public void MockFileInfo_Encrypt_ShouldSetEncryptedAttributeOfFileInMemoryFileSystem()
        {
            var fileData = new MockFileData("Demo text content");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            fileInfo.Encrypt();

            Assert.AreEqual(FileAttributes.Encrypted, fileData.Attributes & FileAttributes.Encrypted);
        }

        [Test]
        public void MockFileInfo_Decrypt_ShouldUnsetEncryptedAttributeOfFileInMemoryFileSystem()
        {
            var fileData = new MockFileData("Demo text content");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));
            fileInfo.Encrypt();

            fileInfo.Decrypt();

            Assert.AreNotEqual(FileAttributes.Encrypted, fileData.Attributes & FileAttributes.Encrypted);
        }
#endif

        [Test]
        public void MockFileInfo_LastAccessTimeUtc_ShouldReturnLastAccessTimeUtcOfFileInMemoryFileSystem()
        {
            var lastAccessTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { LastAccessTime = lastAccessTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var result = fileInfo.LastAccessTimeUtc;

            Assert.AreEqual(lastAccessTime.ToUniversalTime(), result);
        }

        [Test]
        public void MockFileInfo_LastAccessTimeUtc_ShouldSetCreationTimeUtcOfFileInMemoryFileSystem()
        {
            var lastAccessTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { LastAccessTime = lastAccessTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var newUtcTime = DateTime.UtcNow;
            fileInfo.LastAccessTimeUtc = newUtcTime;

            Assert.AreEqual(newUtcTime, fileInfo.LastAccessTimeUtc);
        }

        [Test]
        public void MockFileInfo_LastWriteTimeUtc_ShouldReturnLastWriteTimeUtcOfFileInMemoryFileSystem()
        {
            var lastWriteTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { LastWriteTime = lastWriteTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var result = fileInfo.LastWriteTimeUtc;

            Assert.AreEqual(lastWriteTime.ToUniversalTime(), result);
        }

        [Test]
        public void MockFileInfo_LastWriteTimeUtc_ShouldSetLastWriteTimeUtcOfFileInMemoryFileSystem()
        {
            var lastWriteTime = DateTime.Now.AddHours(-4);
            var fileData = new MockFileData("Demo text content") { LastWriteTime = lastWriteTime };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\a.txt"), fileData }
            });
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var newUtcTime = DateTime.UtcNow;
            fileInfo.LastWriteTime = newUtcTime;

            Assert.AreEqual(newUtcTime, fileInfo.LastWriteTime);
        }

        [Test]
        public void MockFileInfo_GetExtension_ShouldReturnExtension()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a.txt"));

            var result = fileInfo.Extension;

            Assert.AreEqual(".txt", result);
        }

        [Test]
        public void MockFileInfo_GetExtensionWithoutExtension_ShouldReturnEmptyString()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
            var fileInfo = new MockFileInfo(fileSystem, XFS.Path(@"c:\a"));

            var result = fileInfo.Extension;

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void MockFileInfo_GetDirectoryName_ShouldReturnCompleteDirectoryPath()
        {
            var fileInfo = new MockFileInfo(new MockFileSystem(), XFS.Path(@"c:\temp\level1\level2\file.txt"));

            var result = fileInfo.DirectoryName;

            Assert.AreEqual(XFS.Path(@"c:\temp\level1\level2"), result);
        }

        [Test]
        public void MockFileInfo_GetDirectory_ShouldReturnDirectoryInfoWithCorrectPath()
        {
            var fileInfo = new MockFileInfo(new MockFileSystem(), XFS.Path(@"c:\temp\level1\level2\file.txt"));

            var result = fileInfo.Directory;

            Assert.AreEqual(XFS.Path(@"c:\temp\level1\level2"), result.FullName);
        }

        [Test]
        public void MockFileInfo_OpenRead_ShouldReturnByteContentOfFile()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\temp\file.txt"), new MockFileData(new byte[] { 1, 2 }));
            var fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));

            byte[] result = new byte[2];
            using (var stream = fileInfo.OpenRead())
            {
                stream.Read(result, 0, 2);
            }

            Assert.AreEqual(new byte[] { 1, 2 }, result);
        }

        [Test]
        public void MockFileInfo_OpenText_ShouldReturnStringContentOfFile()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\temp\file.txt"), new MockFileData(@"line 1\r\nline 2"));
            var fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));

            string result;
            using (var streamReader = fileInfo.OpenText())
            {
                result = streamReader.ReadToEnd();
            }

            Assert.AreEqual(@"line 1\r\nline 2", result);
        }

        [Test]
        public void MockFileInfo_MoveTo_ShouldUpdateFileInfoDirectoryAndFullName()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\temp\file.txt"), new MockFileData(@"line 1\r\nline 2"));
            var fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));
            string destinationFolder = XFS.Path(@"c:\temp2");
            string destination = XFS.Path(destinationFolder + @"\file.txt");
            fileSystem.AddDirectory(destination);

            fileInfo.MoveTo(destination);

            Assert.AreEqual(fileInfo.DirectoryName, destinationFolder);
            Assert.AreEqual(fileInfo.FullName, destination);
        }

        [Test]
        public void MockFileInfo_MoveTo_SameSourceAndTargetIsANoOp()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(XFS.Path(@"c:\temp\file.txt"), new MockFileData(@"line 1\r\nline 2"));
            var fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));
            string destination = XFS.Path(XFS.Path(@"c:\temp\file.txt"));

            fileInfo.MoveTo(destination);

            Assert.AreEqual(fileInfo.FullName, destination);
            Assert.True(fileInfo.Exists);
        }

        [Test]
        public void MockFileInfo_MoveTo_SameSourceAndTargetThrowsExceptionIfSourceDoesntExist()
        {
            var fileSystem = new MockFileSystem();
            var fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));
            string destination = XFS.Path(XFS.Path(@"c:\temp\file.txt"));

            TestDelegate action = () => fileInfo.MoveTo(destination);

            Assert.Throws<FileNotFoundException>(action);
        }

        [Test]
        public void MockFileInfo_MoveTo_ThrowsExceptionIfSourceDoesntExist()
        {
            var fileSystem = new MockFileSystem();
            var fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));
            string destination = XFS.Path(XFS.Path(@"c:\temp\file2.txt"));

            TestDelegate action = () => fileInfo.MoveTo(destination);

            Assert.Throws<FileNotFoundException>(action);
        }

        [Test]
        public void MockFileInfo_CopyTo_ThrowsExceptionIfSourceDoesntExist()
        {
            var fileSystem = new MockFileSystem();
            var fileInfo = fileSystem.FileInfo.FromFileName(XFS.Path(@"c:\temp\file.txt"));
            string destination = XFS.Path(XFS.Path(@"c:\temp\file2.txt"));

            TestDelegate action = () => fileInfo.CopyTo(destination);

            Assert.Throws<FileNotFoundException>(action);
        }

#if NET40
        [Test]
        public void MockFileInfo_Replace_ShouldReplaceFileContents()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var path1 = XFS.Path(@"c:\temp\file1.txt");
            var path2 = XFS.Path(@"c:\temp\file2.txt");
            fileSystem.AddFile(path1, new MockFileData("1"));
            fileSystem.AddFile(path2, new MockFileData("2"));
            var fileInfo1 = fileSystem.FileInfo.FromFileName(path1);
            var fileInfo2 = fileSystem.FileInfo.FromFileName(path2);

            // Act
            fileInfo1.Replace(path2, null);

            Assert.AreEqual("1", fileInfo2.OpenText().ReadToEnd());
        }

        [Test]
        public void MockFileInfo_Replace_ShouldCreateBackup()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var path1 = XFS.Path(@"c:\temp\file1.txt");
            var path2 = XFS.Path(@"c:\temp\file2.txt");
            var path3 = XFS.Path(@"c:\temp\file3.txt");
            fileSystem.AddFile(path1, new MockFileData("1"));
            fileSystem.AddFile(path2, new MockFileData("2"));
            var fileInfo1 = fileSystem.FileInfo.FromFileName(path1);
            var fileInfo3 = fileSystem.FileInfo.FromFileName(path3);

            // Act
            fileInfo1.Replace(path2, path3);

            Assert.AreEqual("2", fileInfo3.OpenText().ReadToEnd());
        }

        [Test]
        public void MockFileInfo_Replace_ShouldThrowIfDirectoryOfBackupPathDoesNotExist()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var path1 = XFS.Path(@"c:\temp\file1.txt");
            var path2 = XFS.Path(@"c:\temp\file2.txt");
            var path3 = XFS.Path(@"c:\temp\subdirectory\file3.txt");
            fileSystem.AddFile(path1, new MockFileData("1"));
            fileSystem.AddFile(path2, new MockFileData("2"));
            var fileInfo1 = fileSystem.FileInfo.FromFileName(path1);

            // Act
            Assert.Throws<DirectoryNotFoundException>(() => fileInfo1.Replace(path2, path3));
        }

        [Test]
        public void MockFileInfo_Replace_ShouldReturnDestinationFileInfo()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var path1 = XFS.Path(@"c:\temp\file1.txt");
            var path2 = XFS.Path(@"c:\temp\file2.txt");
            fileSystem.AddFile(path1, new MockFileData("1"));
            fileSystem.AddFile(path2, new MockFileData("2"));
            var fileInfo1 = fileSystem.FileInfo.FromFileName(path1);
            var fileInfo2 = fileSystem.FileInfo.FromFileName(path2);

            // Act
            var result = fileInfo1.Replace(path2, null);

            Assert.AreEqual(fileInfo2.FullName, result.FullName);
        }

        [Test]
        public void MockFileInfo_Replace_ShouldThrowIfSourceFileDoesNotExist()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var path1 = XFS.Path(@"c:\temp\file1.txt");
            var path2 = XFS.Path(@"c:\temp\file2.txt");
            fileSystem.AddFile(path2, new MockFileData("1"));
            var fileInfo = fileSystem.FileInfo.FromFileName(path1);

            Assert.Throws<FileNotFoundException>(() => fileInfo.Replace(path2, null));
        }

        [Test]
        public void MockFileInfo_Replace_ShouldThrowIfDestinationFileDoesNotExist()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var path1 = XFS.Path(@"c:\temp\file1.txt");
            var path2 = XFS.Path(@"c:\temp\file2.txt");
            fileSystem.AddFile(path1, new MockFileData("1"));
            var fileInfo = fileSystem.FileInfo.FromFileName(path1);

            Assert.Throws<FileNotFoundException>(() => fileInfo.Replace(path2, null));
        }
#endif
    }
}
