using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Server
{
    public static class FileOperations
    {
        public const int KB = 1024;
        public const int MB = 1024 * KB;

        private const FileOptions NoBuffering = (FileOptions)0x20000000;

        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, FileShare dwShareMode, IntPtr securityAttrs, FileMode dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        private static int bufferSize = 1 * MB;
        private static int concurrency = 1;

        private static bool unbuffered = true;

        public static int BufferSize
        {
            get
            {
                return bufferSize;
            }
            set
            {
                bufferSize = value;
            }
        }

        public static int Concurrency
        {
            get
            {
                return concurrency;
            }
            set
            {
                concurrency = value;
            }
        }

        public static bool Unbuffered
        {
            get
            {
                return unbuffered;
            }
            set
            {
                unbuffered = value;
            }
        }

        public static bool AreSynchronous
        {
            get
            {
                return (concurrency < 1);
            }
        }

        public static bool AreAsynchronous
        {
            get
            {
                return (concurrency > 0);
            }
        }

        public static FileStream OpenSequentialStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            FileOptions options = FileOptions.SequentialScan;

            if (concurrency > 0)
            {
                options |= FileOptions.Asynchronous;
            }

            if (unbuffered)
            {
                options |= NoBuffering;
            }
            else
            {
                return new FileStream(path, mode, access, share, bufferSize, options);
            }

            SafeFileHandle fileHandle = CreateFile(path, (int)access, share, IntPtr.Zero, mode, (int)options, IntPtr.Zero);

            if (fileHandle.IsInvalid)
            {
                throw new IOException();
            }

            return new UnbufferedFileStream(fileHandle, access, bufferSize, (concurrency > 0));
        }

        private class UnbufferedFileStream : FileStream
        {
            private readonly SafeFileHandle fileHandle;

            public UnbufferedFileStream(SafeFileHandle fileHandle, FileAccess access, int bufferSize, bool isAsync)
                : base(fileHandle, access, bufferSize, isAsync)
            {
                this.fileHandle = fileHandle;
            }

            public override void Write(byte[] array, int offset, int count)
            {
                base.Write(array, offset, bufferSize);
            }

            public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
            {
                return base.BeginWrite(array, offset, bufferSize, userCallback, stateObject);
            }

            protected override void Dispose(bool disposing)
            {
                if (!this.fileHandle.IsClosed)
                {
                    this.fileHandle.Close();
                }

                base.Dispose(disposing);
            }
        }
    }
}