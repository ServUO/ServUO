#region Header
// **********
// ServUO - NativeReader.cs
// **********
#endregion

#region References
using System;
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
			{
				m_NativeReader = new NativeReaderUnix();
			}
			else
			{
				m_NativeReader = new NativeReaderWin32();
			}
		}

		public static unsafe void Read(IntPtr ptr, void* buffer, int length)
		{
			m_NativeReader.Read(ptr, buffer, length);
		}
	}

	public interface INativeReader
	{
		unsafe void Read(IntPtr ptr, void* buffer, int length);
	}

	public sealed class NativeReaderWin32 : INativeReader
	{
		internal class UnsafeNativeMethods
		{
			/*[DllImport("kernel32")]
			internal unsafe static extern int _lread(IntPtr hFile, void* lpBuffer, int wBytes);*/

			[DllImport("kernel32")]
			internal static extern unsafe bool ReadFile(
				IntPtr hFile,
				void* lpBuffer,
				uint nNumberOfBytesToRead,
				ref uint lpNumberOfBytesRead,
				NativeOverlapped* lpOverlapped);
		}

		public unsafe void Read(IntPtr ptr, void* buffer, int length)
		{
			//UnsafeNativeMethods._lread( ptr, buffer, length );
			uint lpNumberOfBytesRead = 0;
			UnsafeNativeMethods.ReadFile(ptr, buffer, (uint)length, ref lpNumberOfBytesRead, null);
		}
	}

	public sealed class NativeReaderUnix : INativeReader
	{
		internal class UnsafeNativeMethods
		{
			[DllImport("libc")]
			internal static extern unsafe int read(IntPtr ptr, void* buffer, int length);
		}

		public unsafe void Read(IntPtr ptr, void* buffer, int length)
		{
			UnsafeNativeMethods.read(ptr, buffer, length);
		}
	}
}