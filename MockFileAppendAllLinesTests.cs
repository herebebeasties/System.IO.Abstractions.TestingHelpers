using System.Collections.Generic;
using NUnit.Framework;
using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace System.IO.Abstractions.TestingHelpers.Tests
{
    public class MockFileAppendAllLinesTests
    {
        [Test]
        public void MockFile_AppendAllLines_ShouldPersistNewLinesToExistingFile()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { path, new MockFileData("Demo text content") }
            });

            var file = new MockFile(fileSystem);

            // Act
            file.AppendAllLines(path, new[] { "line 1", "line 2", "line 3" });

            // Assert
            Assert.AreEqual(
                "Demo text contentline 1" + Environment.NewLine + "line 2" + Environment.NewLine + "line 3" + Environment.NewLine,
                file.ReadAllText(path));
        }

        [Test]
        public void MockFile_AppendAllLines_ShouldPersistNewLinesToNewFile()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\demo.txt");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { XFS.Path(@"c:\something\"), new MockDirectoryData() }
            });
            var file = new MockFile(fileSystem);

            // Act
            file.AppendAllLines(path, new[] { "line 1", "line 2", "line 3" });

            // Assert
            Assert.AreEqual(
                "line 1" + Environment.NewLine + "line 2" + Environment.NewLine + "line 3" + Environment.NewLine,
                file.ReadAllText(path));
        }

        [Test]
        public void MockFile_AppendAllLines_ShouldThrowArgumentExceptionIfPathIsZeroLength()
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.AppendAllLines(string.Empty, new[] { "does not matter" });

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [TestCase(" ")]
        [TestCase("   ")]
        public void MockFile_AppendAllLines_ShouldThrowArgumentExceptionIfPathContainsOnlyWhitespaces(string path)
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.AppendAllLines(path, new[] { "does not matter" });

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [TestCase("\"")]
        [TestCase("<")]
        [TestCase(">")]
        [TestCase("|")]
        [WindowsOnly(WindowsSpecifics.StrictPathRules)]
        public void MockFile_AppendAllLines_ShouldThrowArgumentExceptionIfPathContainsInvalidChar(string path)
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.AppendAllLines(path, new[] { "does not matter" });

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Test]
        public void MockFile_AppendAllLines_ShouldThrowArgumentNullExceptionIfContentIsNull()
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.AppendAllLines("foo", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.That(exception.ParamName, Is.EqualTo("contents"));
        }

        [Test]
        public void MockFile_AppendAllLines_ShouldThrowArgumentNullExceptionIfEncodingIsNull()
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.AppendAllLines("foo.txt", new [] { "bar" }, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.That(exception.ParamName, Is.EqualTo("encoding"));
        }
    }
}
