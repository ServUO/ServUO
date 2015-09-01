#region References
using System;
using System.IO;
#endregion

namespace Ultima
{
	public abstract unsafe class ProcessStream : Stream
	{
		private const int ProcessAllAccess = 0x1F0FFF;

		protected bool m_Open;
		protected ClientProcessHandle m_Process;

		protected int m_Position;

		public abstract ClientProcessHandle ProcessID { get; }

		public virtual bool BeginAccess()
		{
			if (m_Open)
			{
				return false;
			}

			m_Process = NativeMethods.OpenProcess(ProcessAllAccess, 0, ProcessID);
			m_Open = true;

			return true;
		}

		public virtual void EndAccess()
		{
			if (!m_Open)
			{
				return;
			}

			m_Process.Close();
			m_Open = false;
		}

		public override void Flush()
		{ }

		public override int Read(byte[] buffer, int offset, int count)
		{
			bool end = !BeginAccess();

			int res = 0;

			fixed (byte* p = buffer)
			{
				NativeMethods.ReadProcessMemory(m_Process, m_Position, p + offset, count, ref res);
			}

			m_Position += count;

			if (end)
			{
				EndAccess();
			}

			return res;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			bool end = !BeginAccess();

			fixed (byte* p = buffer)
			{
				NativeMethods.WriteProcessMemory(m_Process, m_Position, p + offset, count, 0);
			}

			m_Position += count;

			if (end)
			{
				EndAccess();
			}
		}

		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return true; } }
		public override bool CanSeek { get { return true; } }

		public override long Length { get { throw new NotSupportedException(); } }
		public override long Position { get { return m_Position; } set { m_Position = (int)value; } }

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					m_Position = (int)offset;
					break;
				case SeekOrigin.Current:
					m_Position += (int)offset;
					break;
				case SeekOrigin.End:
					throw new NotSupportedException();
			}

			return m_Position;
		}
	}
}