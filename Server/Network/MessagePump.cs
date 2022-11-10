#region References
using System;
using System.Collections.Concurrent;
using System.Threading;

using Server.Diagnostics;
#endregion

namespace Server.Network
{
	public class MessagePump
	{
		private ConcurrentQueue<NetState> m_Queue;
		private ConcurrentQueue<NetState> m_WorkingQueue;

		private readonly ConcurrentQueue<NetState> m_Throttled;

		public Listener[] Listeners { get; set; }

		public MessagePump()
		{
			var ipep = Listener.EndPoints;

			Listeners = new Listener[ipep.Length];

			var success = false;

			do
			{
				for (var i = 0; i < ipep.Length; i++)
				{
					Listeners[i] = new Listener(ipep[i]);

					success = true;
				}

				if (!success)
				{
					Utility.WriteLine(ConsoleColor.Yellow, "Retrying...");

					Thread.Sleep(10000);
				}
			}
			while (!success);

			m_Queue = new ConcurrentQueue<NetState>();
			m_WorkingQueue = new ConcurrentQueue<NetState>();
			m_Throttled = new ConcurrentQueue<NetState>();
		}

		public void AddListener(Listener l)
		{
			var old = Listeners;

			Listeners = new Listener[old.Length + 1];

			for (var i = 0; i < old.Length; ++i)
			{
				Listeners[i] = old[i];
			}

			Listeners[old.Length] = l;
		}

		private void CheckListener()
		{
			foreach (var l in Listeners)
			{
				var accepted = l.Slice();

				foreach (var s in accepted)
				{
					var ns = new NetState(s, this);

					ns.Start();

					if (ns.Running && Display(ns))
					{
						Utility.WriteLine(ConsoleColor.Green, $"Client: {ns}: Connected. [{NetState.Instances.Count:N0} Online]");
					}
				}
			}
		}

		public static bool Display(NetState ns)
		{
			if (ns == null)
			{
				return false;
			}

			var state = ns.ToString();

			foreach (var str in _NoDisplay)
			{
				if (str == state)
				{
					return false;
				}
			}

			return true;
		}

		private static readonly string[] _NoDisplay =
		{
			"192.99.10.155",
			"192.99.69.21",
		};

		public void OnReceive(NetState ns)
		{
			m_Queue.Enqueue(ns);

			Core.Set();
		}

		public void Slice()
		{
			CheckListener();

			m_Queue = Interlocked.Exchange(ref m_WorkingQueue, m_Queue);

			NetState ns;

			while (m_WorkingQueue.TryDequeue(out ns))
			{
				if (ns.Running)
				{
					HandleReceive(ns);
				}
			}

			while (m_Throttled.TryDequeue(out ns))
			{
				m_Queue.Enqueue(ns);
			}
		}

		private const int BufferSize = 4096;
		private readonly BufferPool m_Buffers = new BufferPool("Processor", 4, BufferSize);

		public static bool HandleSeed(NetState ns, ByteQueue buffer)
		{
			if (buffer.GetPacketID() == 0xEF)
			{
				// new packet in client	6.0.5.0	replaces the traditional seed method with a	seed packet
				// 0xEF	= 239 =	multicast IP, so this should never appear in a normal seed.	 So	this is	backwards compatible with older	clients.
				ns.Seeded = true;
				return true;
			}

			if (buffer.Length >= 4)
			{
				var m_Peek = new byte[4];

				_ = buffer.Dequeue(m_Peek, 0, 4);

				var seed = (uint)((m_Peek[0] << 24) | (m_Peek[1] << 16) | (m_Peek[2] << 8) | m_Peek[3]);

				if (seed == 0)
				{
					Utility.WriteLine(ConsoleColor.Red, $"Login: {ns}: Invalid Client");

					ns.Dispose();

					return false;
				}

				ns.Seed = seed;
				ns.Seeded = true;

				return true;
			}

			return false;
		}

		public static bool CheckEncrypted(NetState ns, int packetID)
		{
			if (ns.SentFirstPacket)
			{
				return false;
			}

			switch (packetID)
			{
				case 0xF0:
				case 0xF1:
				case 0xCF:
				case 0x80:
				case 0x91:
				case 0xA4:
				case 0xEF:
				case 0xE4:
				case 0xFF:
				{
					return false;
				}
			}

			Utility.WriteLine(ConsoleColor.Red, $"Client: {ns}: Encrypted Client Unsupported");

			ns.Dispose();

			return true;
		}

		public void HandleReceive(NetState ns)
		{
			var queue = ns.Buffer;
			var slice = ns.BufferSlice;

			if (queue == null || slice == null)
			{
				return;
			}

			var buffer = slice.Length > 0 ? slice : queue;

			if (buffer.Length <= 0)
			{
				return;
			}

			lock (queue)
			{
				if (!ns.Seeded && !HandleSeed(ns, buffer))
				{
					return;
				}

				var length = buffer.Length;

				while (length > 0 && ns.Running)
				{
					var packetID = buffer.GetPacketID();

					if (CheckEncrypted(ns, packetID))
					{
						return;
					}

					var handler = ns.GetHandler(packetID);

					if (handler == null)
					{
#if DEBUG
						var data = new byte[length];
						length = buffer.Dequeue(data, 0, length);
						new PacketReader(data, length, false).Trace(ns);
#else
						length = buffer.Dequeue(null, 0, length);
#endif
						return;
					}

					var remainLength = 0;
					var packetLength = handler.Length;

					if (packetLength <= 0)
					{
						remainLength = 3;

						if (length >= remainLength)
						{
							packetLength = buffer.GetPacketLength();

							if (packetLength < remainLength)
							{
								ns.Dispose();
								return;
							}

							remainLength = packetLength - length;
						}
					}

					if (remainLength > 0)
					{
						if (buffer == slice)
						{
							_ = queue.CopyTo(slice, remainLength);
						}

						return;
					}

					if (handler.Ingame)
					{
						if (ns.Mobile == null)
						{
							Utility.WriteLine(ConsoleColor.Red, $"Client: {ns}: Packet (0x{packetID:X2}) Requires State Mobile");

							ns.Dispose();

							return;
						}

						if (ns.Mobile.Deleted)
						{
							Utility.WriteLine(ConsoleColor.Red, $"Client: {ns}: Packet (0x{packetID:X2}) Ivalid State Mobile");

							ns.Dispose();

							return;
						}
					}

					var throttler = handler.ThrottleCallback;

					if (throttler != null)
					{
						if (!throttler(packetID, ns, out var drop))
						{
							if (!drop)
							{
								m_Throttled.Enqueue(ns);
							}
							else
							{
								_ = buffer.Dequeue(null, 0, packetLength);
							}

							return;
						}
					}

					PacketReceiveProfile prof = null;

					if (Core.Profiling)
					{
						prof = PacketReceiveProfile.Acquire(packetID);
					}

					if (prof != null)
					{
						prof.Start();
					}

					byte[] packetBuffer;

					if (packetLength < BufferSize)
					{
						packetBuffer = m_Buffers.AcquireBuffer();
					}
					else
					{
						packetBuffer = new byte[packetLength];
					}

					packetLength = buffer.Dequeue(packetBuffer, 0, packetLength);

					if (packetBuffer != null && packetBuffer.Length > 0 && packetLength > 0)
					{
						var reader = false;
						var handle = false;

						try
						{
							var r = new PacketReader(packetBuffer, packetLength, handler.Length != 0);

							reader = true;

							handler.OnReceive(ns, r);

							handle = true;

							if (r.Chop > 0)
							{
								if (buffer != slice)
								{
									slice.Enqueue(packetBuffer, r.Index, r.Chop);
								}
								else
								{
									Utility.WriteLine(ConsoleColor.Red, $"Client: {ns}: Packet (0x{packetID:X2}) sliced more than once");

									ns.Dispose();

									return;
								}
							}
							else
							{
								ns.SetPacketTime(packetID);
							}
						}
						catch (Exception ex)
						{
							ExceptionLogging.LogException(ex);

							if (!reader)
							{
								Utility.WriteLine(ConsoleColor.Red, $"Client: {ns}: Packet (0x{packetID:X2}) reader fatal exception");
							}
							else if (!handle)
							{
								Utility.WriteLine(ConsoleColor.Red, $"Client: {ns}: Packet (0x{packetID:X2}) handler fatal exception");
							}

							ns.Dispose();

							return;
						}
						finally
						{
							if (BufferSize >= packetLength)
							{
								m_Buffers.ReleaseBuffer(ref packetBuffer);
							}
						}
					}

					if (prof != null)
					{
						prof.Finish(packetLength);
					}

					length = buffer.Length;
				}
			}
		}
	}
}
