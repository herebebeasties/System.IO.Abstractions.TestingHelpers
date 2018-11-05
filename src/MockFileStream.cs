﻿namespace System.IO.Abstractions.TestingHelpers
{
    [Serializable]
    public class MockFileStream : MemoryStream
    {
        private readonly IMockFileDataAccessor mockFileDataAccessor;
        private readonly string path;
        private readonly bool canWrite = true;
        private readonly FileOptions options;
        private bool disposed;

        public enum StreamType
        {
            READ,
            WRITE,
            APPEND,
            TRUNCATE
        }

        public MockFileStream(
            IMockFileDataAccessor mockFileDataAccessor,
            string path,
            StreamType streamType)
            : this(mockFileDataAccessor, path, streamType, FileOptions.None)
        {
        }

        public MockFileStream(
            IMockFileDataAccessor mockFileDataAccessor,
            string path,
            StreamType streamType,
            FileOptions options)
        {
            this.mockFileDataAccessor = mockFileDataAccessor ?? throw new ArgumentNullException(nameof(mockFileDataAccessor));
            this.path = path;
            this.options = options;

            if (mockFileDataAccessor.FileExists(path))
            {
                /* only way to make an expandable MemoryStream that starts with a particular content */
                var data = mockFileDataAccessor.GetFile(path).Contents;
                if (data != null && data.Length > 0 && streamType != StreamType.TRUNCATE)
                {
                    Write(data, 0, data.Length);
                    Seek(0, StreamType.APPEND.Equals(streamType)
                        ? SeekOrigin.End
                        : SeekOrigin.Begin);
                }
            }
            else
            {
                if (StreamType.READ.Equals(streamType))
                {
                    throw new FileNotFoundException("File not found.", path);
                }
                mockFileDataAccessor.AddFile(path, new MockFileData(new byte[] { }));
            }

            canWrite = streamType != StreamType.READ;
        }

        public override bool CanWrite => canWrite;

#if NET40
        public override void Close()
        {
            InternalFlush();
            OnClose();
        }
#else
        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            InternalFlush();
            base.Dispose(disposing);
            OnClose();
            disposed = true;
        }
#endif

        public override void Flush()
        {
            InternalFlush();
        }

        private void InternalFlush()
        {
            if (mockFileDataAccessor.FileExists(path))
            {
                var mockFileData = mockFileDataAccessor.GetFile(path);
                /* reset back to the beginning .. */
                Seek(0, SeekOrigin.Begin);
                /* .. read everything out */
                var data = new byte[Length];
                Read(data, 0, (int)Length);
                /* .. put it in the mock system */
                mockFileData.Contents = data;
            }
        }

        private void OnClose()
        {
            if (options.HasFlag(FileOptions.DeleteOnClose) && mockFileDataAccessor.FileExists(path))
            {
                mockFileDataAccessor.RemoveFile(path);
            }

#if NET40
            if (options.HasFlag(FileOptions.Encrypted) && mockFileDataAccessor.FileExists(path))
            {
                mockFileDataAccessor.FileInfo.FromFileName(path).Encrypt();
            }
#endif
        }
    }
}