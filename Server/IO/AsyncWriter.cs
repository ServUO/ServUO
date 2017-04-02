using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using CustomsFramework;
using Server.Guilds;

namespace Server
{
	public sealed class AsyncWriter : GenericWriter
	{
		public static int ThreadCount { get; private set; }

		private readonly int BufferSize;

		private long m_LastPos, m_CurPos;
		private bool m_Closed;
		private readonly bool PrefixStrings;

		private MemoryStream m_Mem;
		private BinaryWriter m_Bin;
		private readonly FileStream m_File;

		private readonly Queue m_WriteQueue;
		private Thread m_WorkerThread;

		public AsyncWriter(string filename, bool prefix)
			: this(filename, 1048576, prefix) //1 mb buffer
		{ }

		public AsyncWriter(string filename, int buffSize, bool prefix)
		{
			PrefixStrings = prefix;
			m_Closed = false;
			m_WriteQueue = Queue.Synchronized(new Queue());
			BufferSize = buffSize;

			m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
			m_Mem = new MemoryStream(BufferSize + 1024);
			m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
		}

		private void Enqueue(MemoryStream mem)
		{
			m_WriteQueue.Enqueue(mem);

			if (m_WorkerThread == null || !m_WorkerThread.IsAlive)
			{
				m_WorkerThread = new Thread(new WorkerThread(this).Worker);
				m_WorkerThread.Priority = ThreadPriority.BelowNormal;
				m_WorkerThread.Start();
			}
		}

		private class WorkerThread
		{
			private readonly AsyncWriter m_Owner;

			public WorkerThread(AsyncWriter owner)
			{
				m_Owner = owner;
			}

			public void Worker()
			{
				ThreadCount++;
				while (m_Owner.m_WriteQueue.Count > 0)
				{
					MemoryStream mem = (MemoryStream)m_Owner.m_WriteQueue.Dequeue();

					if (mem != null && mem.Length > 0)
					{
						mem.WriteTo(m_Owner.m_File);
					}
				}

				if (m_Owner.m_Closed)
				{
					m_Owner.m_File.Close();
				}

				ThreadCount--;

				if (ThreadCount <= 0)
				{
					World.NotifyDiskWriteComplete();
				}
			}
		}

		private void OnWrite()
		{
			long curlen = m_Mem.Length;
			m_CurPos += curlen - m_LastPos;
			m_LastPos = curlen;
			if (curlen >= BufferSize)
			{
				Enqueue(m_Mem);
				m_Mem = new MemoryStream(BufferSize + 1024);
				m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
				m_LastPos = 0;
			}
		}

		public MemoryStream MemStream
		{
			get { return m_Mem; }
			set
			{
				if (m_Mem.Length > 0)
				{
					Enqueue(m_Mem);
				}

				m_Mem = value;
				m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
				m_LastPos = 0;
				m_CurPos = m_Mem.Length;
				m_Mem.Seek(0, SeekOrigin.End);
			}
		}

		public override void Close()
		{
			Enqueue(m_Mem);
			m_Closed = true;
		}

		public override long Position { get { return m_CurPos; } }

		public override void Write(IPAddress value)
		{
			m_Bin.Write(Utility.GetLongAddressValue(value));
			OnWrite();
		}

		public override void Write(string value)
		{
			if (PrefixStrings)
			{
				if (value == null)
				{
					m_Bin.Write((byte)0);
				}
				else
				{
					m_Bin.Write((byte)1);
					m_Bin.Write(value);
				}
			}
			else
			{
				m_Bin.Write(value);
			}
			OnWrite();
		}

		public override void Write(DateTime value)
		{
			m_Bin.Write(value.Ticks);
			OnWrite();
		}

		public override void Write(DateTimeOffset value)
		{
			m_Bin.Write(value.Ticks);
			m_Bin.Write(value.Offset.Ticks);
			OnWrite();
		}

		public override void Write(TimeSpan value)
		{
			m_Bin.Write(value.Ticks);
			OnWrite();
		}

		public override void Write(decimal value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(long value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(ulong value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void WriteEncodedInt(int value)
		{
			uint v = (uint)value;

			while (v >= 0x80)
			{
				m_Bin.Write((byte)(v | 0x80));
				v >>= 7;
			}

			m_Bin.Write((byte)v);
			OnWrite();
		}

		public override void Write(int value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(uint value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(short value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(ushort value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(double value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(float value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(char value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(byte value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(sbyte value)
		{
			m_Bin.Write(value);
			OnWrite();
		}

		public override void Write(bool value)
		{
			m_Bin.Write(value);
			OnWrite();
		}
	}
}