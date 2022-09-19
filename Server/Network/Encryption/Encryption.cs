#region References
using System;
using System.Net;
#endregion

namespace Server.Network.Encryption
{
	public enum EncryptionSupport
	{
		None = 0x0,

		Encrypted = 0x1,
		Decrypted = 0x2,

		Any = Encrypted | Decrypted
	}

	public enum EncryptionAction
	{
		Encrypting,
		Decrypting
	}

	public interface IClientEncryption
	{
		void ServerEncrypt(ref byte[] buffer, ref int length);
		void ClientDecrypt(ref byte[] buffer, ref int length);
	}

	public class Encryption : IPacketEncryptor
	{
		[ConfigProperty("Client.EncryptionSupport")]
		public static EncryptionSupport Support { get => Config.GetEnum("Client.EncryptionSupport", EncryptionSupport.None); set => Config.SetEnum("Client.EncryptionSupport", value); }

		[ConfigProperty("Client.EncryptionDebug")]
		public static bool Debug { get => Config.Get("Client.EncryptionDebug", false); set => Config.Set("Client.EncryptionDebug", value); }

		[CallPriority(Int32.MinValue + 1)]
		public static void Configure()
		{
			NetState.CreatedCallback += OnNetStateCreated;
		}

		private static void OnNetStateCreated(NetState state)
		{
			if (Support == EncryptionSupport.None)
			{
				return;
			}

			state.PacketEncryptor = new Encryption();

			if (Debug)
			{
				Console.WriteLine($"Client: {state}: Encryptor Instantiated");
			}
		}

		private readonly ByteQueue m_Buffer;

		private IClientEncryption m_Encryption;

		public Encryption()
		{
			m_Buffer = new ByteQueue();
		}

		public void EncryptOutgoingPacket(NetState state, ref byte[] buffer, ref int length)
		{
			if (m_Encryption != null && state.Encrypted == true)
			{
				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Encrypting Stream ({length})");
				}

				m_Encryption.ServerEncrypt(ref buffer, ref length);
			}
		}

		public void DecryptIncomingPacket(NetState state, ref byte[] buffer, ref int length)
		{
			if (m_Encryption != null && m_Buffer.Length <= 0 && state.Encrypted == true)
			{
				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Decrypting Stream ({length})");
				}

				m_Encryption.ClientDecrypt(ref buffer, ref length);

				return;
			}

			if (state.Encrypted == false)
			{
				if (m_Buffer.Length > 0)
				{
					m_Buffer.Enqueue(buffer, 0, length);

					length = m_Buffer.Dequeue(buffer, 0, m_Buffer.Length);
				}

				return;
			}

			var port = ((IPEndPoint)state.Socket.LocalEndPoint).Port;

			if (!Array.Exists(Listener.EndPoints, o => o.Port == port))
			{
				state.Encrypted = false;

				m_Encryption = new NoEncryption();

				return;
			}

			m_Buffer.Enqueue(buffer, 0, length);

			length = 0;

			if (m_Buffer.Length <= 0)
			{
				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Buffering...");
				}

				return;
			}

			var packetID = m_Buffer.GetPacketID();

			// UOG Status Packet
			if (packetID == 0xF1)
			{
				if (m_Buffer.Length < 3 || m_Buffer.Length < m_Buffer.GetPacketLength())
				{
					return;
				}

				state.Encrypted = false;

				m_Encryption = new NoEncryption();

				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Not Encrypted (0x{packetID:X2})");
				}

				return;
			}

			if (!state.Seeded && !state.SentFirstPacket)
			{
				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Seeding ({m_Buffer.Length})");
				}

				if (packetID == 0xEF)
				{
					if (m_Buffer.Length < 21)
					{
						return;
					}

					var packet = new byte[21];

					m_Buffer.Dequeue(packet, 0, packet.Length);

					state.Seeded = true;

					var handler = PacketHandlers.GetHandler(0xEF);

					if (handler != null)
					{
						handler.OnReceive(state, new PacketReader(packet, packet.Length, true));
					}
					else
					{
						var i = 0;

						state.Seed = (uint)((packet[++i] << 24) | (packet[++i] << 16) | (packet[++i] << 8) | packet[++i]);

						var maj = (packet[++i] << 24) | (packet[++i] << 16) | (packet[++i] << 8) | packet[++i];
						var min = (packet[++i] << 24) | (packet[++i] << 16) | (packet[++i] << 8) | packet[++i];
						var rev = (packet[++i] << 24) | (packet[++i] << 16) | (packet[++i] << 8) | packet[++i];
						var pat = (packet[++i] << 24) | (packet[++i] << 16) | (packet[++i] << 8) | packet[++i];

						state.Version = new ClientVersion(maj, min, rev, pat);
					}

					if (Debug)
					{
						Console.WriteLine($"Client: {state}: Version {state.Version}");
					}
				}
				else if (m_Buffer.Length >= 4)
				{
					var peek = new byte[4];

					m_Buffer.Dequeue(peek, 0, peek.Length);

					var i = -1;

					state.Seeded = true;
					state.Seed = (uint)((peek[++i] << 24) | (peek[++i] << 16) | (peek[++i] << 8) | peek[++i]);
				}

				if (!state.Seeded)
				{
					return;
				}

				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Seeded ({state.Seed})");
				}
			}

			if (m_Buffer.Length <= 0)
			{
				return;
			}

			if (m_Encryption != null)
			{
				length = m_Buffer.Dequeue(buffer, 0, m_Buffer.Length);

				m_Encryption.ClientDecrypt(ref buffer, ref length);

				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Decrypted Packet (0x{buffer[0]:X2}, {length})");
				}
			}
			else
			{
				packetID = m_Buffer.GetPacketID();

				var packetLength = m_Buffer.Length;

				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Packet (0x{packetID:X2}, {packetLength})");
				}

				switch (packetLength)
				{
					case 62:
					{
						length = m_Buffer.Dequeue(buffer, 0, packetLength);

						if (buffer[0] == 0x80 || (buffer[30] == 0 && buffer[60] == 0))
						{
							state.Encrypted = false;

							m_Encryption = new NoEncryption();

							if (Debug)
							{
								Console.WriteLine($"Client: {state}: Not Encrypted (0x{buffer[0]:X2})");
							}

							if (!Support.HasFlag(EncryptionSupport.Decrypted))
							{
								Console.WriteLine($"Client: {state}: Not Encrypted (Disallowed)");

								PacketHandlers.AccountLogin_ReplyRej(state, ALRReason.BadComm);
								return;
							}
						}
						else
						{
							var enc = new LoginEncryption();

							if (!enc.Init(state.Version, state.Seed, buffer, 0, length))
							{
								Console.WriteLine($"Client: {state}: Unknown (0x{state.Seed:X8})");

								state.Encrypted = false;

								m_Encryption = new NoEncryption();

								if (!Support.HasFlag(EncryptionSupport.Decrypted))
								{
									Console.WriteLine($"Client: {state}: Not Encrypted (Disallowed)");

									PacketHandlers.AccountLogin_ReplyRej(state, ALRReason.BadComm);
									return;
								}
							}
							else
							{
								Console.WriteLine($"Client: {state}: Encrypted ({enc.Keys})");

								state.Encrypted = true;

								m_Encryption = enc;

								if (Debug)
								{
									Console.WriteLine($"Client: {state}: Using {m_Encryption.GetType().Name}");
								}

								m_Encryption.ClientDecrypt(ref buffer, ref length);

								if (Debug)
								{
									Console.WriteLine($"Client: {state}: Decrypted (0x{buffer[0]:X2}, {length})");
								}

								if (!Support.HasFlag(EncryptionSupport.Encrypted))
								{
									Console.WriteLine($"Client: {state}: Encrypted (Disallowed)");

									PacketHandlers.AccountLogin_ReplyRej(state, ALRReason.BadComm);
									return;
								}
							}
						}
					}
					break;
					case 65:
					{
						length = m_Buffer.Dequeue(buffer, 0, packetLength);

						var auth = (uint)((buffer[1] << 24) | (buffer[2] << 16) | (buffer[3] << 8) | buffer[4]);

						if (buffer[0] == 0x91 || auth == state.Seed)
						{
							state.Encrypted = false;

							m_Encryption = new NoEncryption();

							if (Debug)
							{
								Console.WriteLine($"Client: {state}: Not Encrypted (0x{buffer[0]:X2}, {length})");
							}

							if (!Support.HasFlag(EncryptionSupport.Decrypted))
							{
								Console.WriteLine($"Client: {state}: Not Encrypted (Disallowed)");

								PacketHandlers.AccountLogin_ReplyRej(state, ALRReason.BadComm);
								return;
							}
						}
						else
						{
							state.Encrypted = true;

							Console.WriteLine($"Client: {state}: Encrypted (0x{state.Seed:X8})");

							m_Encryption = new GameEncryption(state.Seed);

							if (Debug)
							{
								Console.WriteLine($"Client: {state}: Using {m_Encryption.GetType().Name}");
							}

							m_Encryption.ClientDecrypt(ref buffer, ref length);

							if (Debug)
							{
								Console.WriteLine($"Client: {state}: Decrypted (0x{buffer[0]:X2}, {length})");
							}

							if (!Support.HasFlag(EncryptionSupport.Encrypted))
							{
								Console.WriteLine($"Client: {state}: Encrypted (Disallowed)");

								PacketHandlers.AccountLogin_ReplyRej(state, ALRReason.BadComm);
								return;
							}
						}
					}
					break;

					default:
					{
						if (Debug)
						{
							Console.WriteLine($"Client: {state}: Waiting (0x{buffer[0]:X2}, {packetLength})");
						}
					}
					break;
				}
			}

			if (m_Buffer.Length > 0)
			{
				if (Debug)
				{
					Console.WriteLine($"Client: {state}: Buffered ({m_Buffer.Length})");
				}

				if (state.Encrypted == false)
				{
					length = m_Buffer.Dequeue(buffer, 0, m_Buffer.Length);
				}
			}
		}
	}
}
