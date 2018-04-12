#region Header
// **********
// ServUO - MessagePump.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Threading;

using Server.Diagnostics;
#endregion

namespace Server.Network
{
	public class MessagePump
	{
		private Listener[] m_Listeners;
		private Queue<NetState> m_Queue;
		private Queue<NetState> m_WorkingQueue;
		private readonly Queue<NetState> m_Throttled;

		public MessagePump()
		{
			var ipep = Listener.EndPoints;

			m_Listeners = new Listener[ipep.Length];

			bool success = false;

			do
			{
				for (int i = 0; i < ipep.Length; i++)
				{
					Listener l = new Listener(ipep[i]);

					if (!success)
					{
						success = true;
					}

					m_Listeners[i] = l;
				}

				if (!success)
				{
					Utility.PushColor(ConsoleColor.Yellow);
					Console.WriteLine("Retrying...");
					Utility.PopColor();

					Thread.Sleep(10000);
				}
			}
			while (!success);

			m_Queue = new Queue<NetState>();
			m_WorkingQueue = new Queue<NetState>();
			m_Throttled = new Queue<NetState>();
		}

		public Listener[] Listeners { get { return m_Listeners; } set { m_Listeners = value; } }

		public void AddListener(Listener l)
		{
			var old = m_Listeners;

			m_Listeners = new Listener[old.Length + 1];

			for (int i = 0; i < old.Length; ++i)
			{
				m_Listeners[i] = old[i];
			}

			m_Listeners[old.Length] = l;
		}

		private void CheckListener()
		{
			for (int j = 0; j < m_Listeners.Length; ++j)
			{
				var accepted = m_Listeners[j].Slice();

				for (int i = 0; i < accepted.Length; ++i)
				{
					NetState ns = new NetState(accepted[i], this);
					ns.Start();

					if (ns.Running)
					{
						Utility.PushColor(ConsoleColor.Green);
						Console.WriteLine("Client: {0}: Connected. [{1} Online]", ns, NetState.Instances.Count);
						Utility.PopColor();
					}
				}
			}
		}

		public void OnReceive(NetState ns)
		{
			lock (this)
				m_Queue.Enqueue(ns);

			Core.Set();
		}

		public void Slice()
		{
			CheckListener();

			lock (this)
			{
				var temp = m_WorkingQueue;
				m_WorkingQueue = m_Queue;
				m_Queue = temp;
			}

			while (m_WorkingQueue.Count > 0)
			{
				NetState ns = m_WorkingQueue.Dequeue();

				if (ns.Running)
				{
					HandleReceive(ns);
				}
			}

			lock (this)
			{
				while (m_Throttled.Count > 0)
				{
					m_Queue.Enqueue(m_Throttled.Dequeue());
				}
			}
		}

		private const int BufferSize = 4096;
		private readonly BufferPool m_Buffers = new BufferPool("Processor", 4, BufferSize);

		private static bool HandleSeed(NetState ns, ByteQueue buffer)
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

				buffer.Dequeue(m_Peek, 0, 4);

				uint seed = (uint)((m_Peek[0] << 24) | (m_Peek[1] << 16) | (m_Peek[2] << 8) | m_Peek[3]);

				if (seed == 0)
				{
					Utility.PushColor(ConsoleColor.Green);
					Console.WriteLine("Login: {0}: Invalid client detected, disconnecting", ns);
					Utility.PopColor();

					ns.Dispose();

					return false;
				}

				ns.Seed = seed;
				ns.Seeded = true;

				return true;
			}

			return false;
		}

		private static bool CheckEncrypted(NetState ns, int packetID)
		{
			if (!ns.SentFirstPacket && packetID != 0xF0 && packetID != 0xF1 && packetID != 0xCF && packetID != 0x80 &&
				packetID != 0x91 && packetID != 0xA4 && packetID != 0xEF && packetID != 0xE4 && packetID != 0xFF)
			{
				Console.WriteLine("Client: {0}: Encrypted client detected, disconnecting", ns);
				
				ns.Dispose();

				return true;
			}

			return false;
		}

		public void HandleReceive(NetState ns)
		{
			ByteQueue buffer = ns.Buffer;

			if (buffer == null || buffer.Length <= 0)
			{
				return;
			}

			lock (buffer)
			{
				if (!ns.Seeded && !HandleSeed(ns, buffer))
				{
					return;
				}

				int length = buffer.Length;

				while (length > 0 && ns.Running)
				{
					int packetID = buffer.GetPacketID();

					if (CheckEncrypted(ns, packetID))
					{
						return;
					}

					PacketHandler handler = ns.GetHandler(packetID);

					if (handler == null)
					{
						var data = new byte[length];
						length = buffer.Dequeue(data, 0, length);
						new PacketReader(data, length, false).Trace(ns);
						return;
					}

					int packetLength = handler.Length;

					if (packetLength <= 0)
					{
						if (length >= 3)
						{
							packetLength = buffer.GetPacketLength();

							if (packetLength < 3)
							{
								ns.Dispose();
								return;
							}
						}
						else
						{
							return;
						}
					}

					if (length < packetLength)
					{
						return;
					}

					if (handler.Ingame)
					{
						if (ns.Mobile == null)
						{
							Utility.PushColor(ConsoleColor.DarkRed);
							Console.WriteLine("Client: {0}: Sent ingame packet (0x{1:X2}) before having been attached to a mobile", ns, packetID);
							Utility.PopColor();

							ns.Dispose();
							return;
						}

						if (ns.Mobile.Deleted)
						{
							ns.Dispose();
							break;
						}
					}

					ThrottlePacketCallback throttler = handler.ThrottleCallback;

					if (throttler != null && !throttler(ns))
					{
						m_Throttled.Enqueue(ns);
						return;
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

					if (BufferSize >= packetLength)
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
						PacketReader r = new PacketReader(packetBuffer, packetLength, handler.Length != 0);

						handler.OnReceive(ns, r);

						if (BufferSize >= packetLength)
						{
							m_Buffers.ReleaseBuffer(packetBuffer);
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