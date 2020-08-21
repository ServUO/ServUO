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

			fileStream = FileOperations.OpenSequentialStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

			fileQueue = new FileQueue(
				Math.Max(1, FileOperations.Concurrency),
				FileCallback);
		}

		public override long Position
		{
			get => fileQueue.Position;
			set => throw new InvalidOperationException();
		}
		public override bool CanRead => false;
		public override bool CanSeek => false;
		public override bool CanWrite => true;
		public override long Length => Position;
		public override void Write(byte[] buffer, int offset, int size)
		{
			fileQueue.Enqueue(buffer, offset, size);
		}

		public override void Flush()
		{
			fileQueue.Flush();
			fileStream.Flush();
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
			fileStream.SetLength(value);
		}

		protected override void Dispose(bool disposing)
		{
			if (fileStream != null)
			{
				Flush();

				fileQueue.Dispose();
				fileQueue = null;

				fileStream.Close();
				fileStream = null;
			}

			base.Dispose(disposing);
		}

		private void FileCallback(FileQueue.Chunk chunk)
		{
			if (FileOperations.AreSynchronous)
			{
				fileStream.Write(chunk.Buffer, chunk.Offset, chunk.Size);

				if (metrics != null)
				{
					metrics.OnFileWritten(chunk.Size);
				}

				chunk.Commit();
			}
			else
			{
				if (writeCallback == null)
				{
					writeCallback = OnWrite;
				}

				fileStream.BeginWrite(chunk.Buffer, chunk.Offset, chunk.Size, writeCallback, chunk);
			}
		}

		private void OnWrite(IAsyncResult asyncResult)
		{
			FileQueue.Chunk chunk = asyncResult.AsyncState as FileQueue.Chunk;

			fileStream.EndWrite(asyncResult);

			if (metrics != null)
			{
				metrics.OnFileWritten(chunk.Size);
			}

			chunk.Commit();
		}
	}
}
