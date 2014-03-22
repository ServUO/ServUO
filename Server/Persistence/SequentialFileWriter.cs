/***************************************************************************
*                           SequentialFileWriter.cs
*                            -------------------
*   begin                : May 1, 2002
*   copyright            : (C) The RunUO Software Team
*   email                : info@runuo.com
*
*   $Id: SequentialFileWriter.cs 4 2006-06-15 04:28:39Z mark $
*
***************************************************************************/








/***************************************************************************
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 2 of the License, or
*   (at your option) any later version.
*
***************************************************************************/
using System;
using System.IO;

namespace Server
{
    public sealed class SequentialFileWriter : Stream
    {
        private readonly SaveMetrics metrics;
        private FileStream fileStream;
        private FileQueue fileQueue;
        private AsyncCallback writeCallback;
        public SequentialFileWriter(string path, SaveMetrics metrics)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            this.metrics = metrics;

            this.fileStream = FileOperations.OpenSequentialStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            this.fileQueue = new FileQueue(
                Math.Max(1, FileOperations.Concurrency),
                FileCallback);
        }

        public override long Position
        {
            get
            {
                return this.fileQueue.Position;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
        public override long Length
        {
            get
            {
                return this.Position;
            }
        }
        public override void Write(byte[] buffer, int offset, int size)
        {
            this.fileQueue.Enqueue(buffer, offset, size);
        }

        public override void Flush()
        {
            this.fileQueue.Flush();
            this.fileStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            this.fileStream.SetLength(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.fileStream != null)
            {
                this.Flush();

                this.fileQueue.Dispose();
                this.fileQueue = null;

                this.fileStream.Close();
                this.fileStream = null;
            }

            base.Dispose(disposing);
        }

        private void FileCallback(FileQueue.Chunk chunk)
        {
            if (FileOperations.AreSynchronous)
            {
                this.fileStream.Write(chunk.Buffer, chunk.Offset, chunk.Size);

                if (this.metrics != null)
                {
                    this.metrics.OnFileWritten(chunk.Size);
                }

                chunk.Commit();
            }
            else
            {
                if (this.writeCallback == null)
                {
                    this.writeCallback = this.OnWrite;
                }

                this.fileStream.BeginWrite(chunk.Buffer, chunk.Offset, chunk.Size, this.writeCallback, chunk);
            }
        }

        private void OnWrite(IAsyncResult asyncResult)
        {
            FileQueue.Chunk chunk = asyncResult.AsyncState as FileQueue.Chunk;

            this.fileStream.EndWrite(asyncResult);

            if (this.metrics != null)
            {
                this.metrics.OnFileWritten(chunk.Size);
            }

            chunk.Commit();
        }
    }
}