#region References
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
#endregion

namespace Server
{
	public static class NativeReader
	{
		private static readonly INativeReader m_NativeReader;

		static NativeReader()
		{
			if (Core.Unix)
				m_NativeReader = new NativeReaderUnix();
			else
				m_NativeReader = new NativeReaderWin32();
		}

		public static unsafe int Read(FileStream source, void* buffer, int length)
		{
			return m_NativeReader.Read(source, buffer, length);
		}

		public static unsafe int Read(FileStream source, void* buffer, int bufferIndex, int length)
		{
			return m_NativeReader.Read(source, buffer, bufferIndex, length);
		}

		public static unsafe int Read(FileStream source, int sourceIndex, void* buffer, int length)
		{
			return m_NativeReader.Read(source, sourceIndex, buffer, length);
		}

		public static unsafe int Read(FileStream source, int sourceIndex, void* buffer, int bufferIndex, int length)
		{
			return m_NativeReader.Read(source, sourceIndex, buffer, bufferIndex, length);
		}
	}

	public interface INativeReader
	{
		unsafe int Read(FileStream source, void* buffer, int length);
		unsafe int Read(FileStream source, void* buffer, int bufferIndex, int length);
		unsafe int Read(FileStream source, int sourceIndex, void* buffer, int length);
		unsafe int Read(FileStream source, int sourceIndex, void* buffer, int bufferIndex, int length);
	}

	public sealed class NativeReaderWin32 : INativeReader
	{
		internal class UnsafeNativeMethods
		{
			[DllImport("kernel32")]
			static internal unsafe extern bool ReadFile(IntPtr hFile, void* lpBuffer, uint nNumberOfBytesToRead, ref uint lpNumberOfBytesRead, NativeOverlapped* lpOverlapped);
		}

		public unsafe int Read(FileStream source, void* buffer, int length)
		{
			return InternalRead(source, buffer, 0, length);
		}

		public unsafe int Read(FileStream source, void* buffer, int bufferIndex, int length)
		{
			return InternalRead(source, buffer, bufferIndex, length);
		}

		public unsafe int Read(FileStream source, int sourceIndex, void* buffer, int length)
		{
			if (source.Seek(sourceIndex, SeekOrigin.Begin) == sourceIndex)
				return InternalRead(source, buffer, 0, length);

			return -1;
		}

		public unsafe int Read(FileStream source, int sourceIndex, void* buffer, int bufferIndex, int length)
		{
			if (source.Seek(sourceIndex, SeekOrigin.Begin) == sourceIndex)
				return InternalRead(source, buffer, bufferIndex, length);

			return -1;
		}

		internal unsafe int InternalRead(FileStream source, void* buffer, int bufferIndex, int length)
		{
			var byteCount = 0U;

			if (!UnsafeNativeMethods.ReadFile(source.SafeFileHandle.DangerousGetHandle(), (byte*)buffer + bufferIndex, (uint)length, ref byteCount, null))
				return -1;

			if (byteCount > 0)
				source.Seek(byteCount, SeekOrigin.Current);

			return (int)byteCount;
		}
	}

	public sealed class NativeReaderUnix : INativeReader
	{
		internal class UnsafeNativeMethods
		{
			[DllImport("libc")]
			static internal unsafe extern int read(IntPtr ptr, void* buffer, int length);
		}

		public unsafe int Read(FileStream source, void* buffer, int length)
		{
			return InternalRead(source, buffer, 0, length);
		}

		public unsafe int Read(FileStream source, void* buffer, int bufferIndex, int length)
		{
			return InternalRead(source, buffer, bufferIndex, length);
		}

		public unsafe int Read(FileStream source, int sourceIndex, void* buffer, int length)
		{
			if (source.Seek(sourceIndex, SeekOrigin.Begin) == sourceIndex)
				return InternalRead(source, buffer, 0, length);

			return -1;
		}

		public unsafe int Read(FileStream source, int sourceIndex, void* buffer, int bufferIndex, int length)
		{
			if (source.Seek(sourceIndex, SeekOrigin.Begin) == sourceIndex)
				return InternalRead(source, buffer, bufferIndex, length);

			return -1;
		}

#pragma warning disable CS0618
		internal unsafe int InternalRead(FileStream source, void* buffer, int bufferIndex, int length)
		{
			var byteCount = UnsafeNativeMethods.read(source.Handle, (byte*)buffer + bufferIndex, length);

			if (byteCount > 0)
				source.Seek(byteCount, SeekOrigin.Current);

			return byteCount;
		}
#pragma warning restore CS0618
	}
}
