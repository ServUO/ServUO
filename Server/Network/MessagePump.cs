#region References
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

using Server.Diagnostics;
#endregion

namespace Server.Network
{
	public static class MessagePump
	{
		private static readonly ConcurrentQueue<NetState> m_Queue = new ConcurrentQueue<NetState>();

        private static readonly ConcurrentQueue<NetState> m_Throttled = new ConcurrentQueue<NetState>();

        public static Listener[] Listeners { get; private set; }

        public static long AcceptsPerSecond => Listeners.Sum(l => l.AcceptsPerSecond);

        public static void Start()
		{
			var ipep = Listener.EndPoints;

			Listeners = new Listener[ipep.Length];

			bool success = false;

			do
			{
				for (int i = 0; i < ipep.Length; i++)
				{
					Listeners[i] = new Listener(ipep[i]);

					success = true;
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
		}

        public static void AddListener(Listener l)
		{
			var old = Listeners;

			Listeners = new Listener[old.Length + 1];

			for (int i = 0; i < old.Length; ++i)
			{
				Listeners[i] = old[i];
			}

			Listeners[old.Length] = l;
		}

        private static void CheckListener()
		{
			foreach (Listener l in Listeners)
			{
				foreach (SocketState state in l.Slice())
				{
					NetState ns = new NetState(state);

					ns.Start();

                    if (ns.Running && Display(ns))
					{
						Utility.PushColor(ConsoleColor.Green);
						Console.WriteLine("Client: {0}: Connected. [{1} Online]", ns, NetState.Instances.Count);
						Utility.PopColor();
					}
				}
			}
		}

        public static bool Display(NetState ns)
        {
            if (ns == null)
                return false;

            string state = ns.ToString();

            foreach (var str in _NoDisplay)
            {
                if (str == state)
                    return false;
            }

            return true;
        }

        private static string[] _NoDisplay =
        {
            "54.39.99.106",
        };

        public static void OnReceive(NetState ns)
		{
            m_Queue.Enqueue(ns);

			Core.Set();
		}

        public static void Slice()
        {
            CheckListener();

            var limit = m_Queue.Count;

            while (--limit >= 0 && m_Queue.TryDequeue(out NetState ns))
            {
                if (ns.Running)
                {
                    HandleReceive(ns);
                }
            }

            while (m_Throttled.TryDequeue(out NetState ns))
            {
            	if (ns.Running)
                {
	                m_Queue.Enqueue(ns);
	            }
            }
        }

		private const int BufferSize = 4096;
        private static readonly BufferPool m_Buffers = new BufferPool("Processor", 4, BufferSize);

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

				buffer.Dequeue(m_Peek, 0, 4);

				uint seed = (uint)((m_Peek[0] << 24) | (m_Peek[1] << 16) | (m_Peek[2] << 8) | m_Peek[3]);

				if (seed == 0)
				{
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine("Login: {0}: Invalid Client", ns);
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

        public static bool CheckEncrypted(NetState ns, int packetID)
        {
            if (ns.SentFirstPacket || !CheckEncrypted(packetID))
            {
                return false;
            }

            Utility.PushColor(ConsoleColor.Red);
            Console.WriteLine("Client: {0}: Encrypted Client Unsupported", ns);
            Utility.PopColor();

            ns.Dispose();

            return true;
        }

        public static bool CheckEncrypted(int packetID)
        {
            return packetID != 0xF0
                && packetID != 0xF1
                && packetID != 0xCF
                && packetID != 0x80
                && packetID != 0x91
                && packetID != 0xA4
                && packetID != 0xEF
                && packetID != 0xE4
                && packetID != 0xFF;
		}

        public static void HandleReceive(NetState ns)
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
#if DEBUG
                        var data = new byte[length];
						length = buffer.Dequeue(data, 0, length);
						new PacketReader(data, length, false).Trace(ns);
#else
                        buffer.Dequeue(null, 0, length);
#endif
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
							Utility.PushColor(ConsoleColor.Red);
							Console.WriteLine("Client: {0}: Packet (0x{1:X2}) Requires State Mobile", ns, packetID);
							Utility.PopColor();

							ns.Dispose();
							return;
						}

						if (ns.Mobile.Deleted)
						{
							Utility.PushColor(ConsoleColor.Red);
							Console.WriteLine("Client: {0}: Packet (0x{1:X2}) Ivalid State Mobile", ns, packetID);
							Utility.PopColor();

							ns.Dispose();
							return;
						}
					}

					ThrottlePacketCallback throttler = handler.ThrottleCallback;

					if (throttler != null)
					{
						bool drop;

						if (!throttler((byte)packetID, ns, out drop))
						{
							if (!drop)
							{
								m_Throttled.Enqueue(ns);
							}
                            else
                            {
                                buffer.Dequeue(null, 0, packetLength);
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

                        ns.SetPacketTime((byte)packetID);

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
