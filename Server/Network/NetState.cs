#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Server.Accounting;
using Server.Diagnostics;
using Server.Gumps;
using Server.HuePickers;
using Server.Items;
using Server.Menus;
#endregion

namespace Server.Network
{
	public interface IPacketEncoder
	{
		void EncodeOutgoingPacket(NetState to, ref byte[] buffer, ref int length);
		void DecodeIncomingPacket(NetState from, ref byte[] buffer, ref int length);
	}

	public interface IPacketEncryptor
	{
		void EncryptOutgoingPacket(NetState to, ref byte[] buffer, ref int length);
		void DecryptIncomingPacket(NetState from, ref byte[] buffer, ref int length);
	}

	public delegate void NetStateCreatedCallback(NetState ns);

	[PropertyObject]
	public class NetState : IComparable<NetState>
	{
		public static bool BufferStaticPackets = false;

		public static event NetStateCreatedCallback CreatedCallback;

		private byte[] m_RecvBuffer;

		private readonly SendQueue m_SendQueue;

		private AsyncCallback m_OnReceive, m_OnSend;

		private readonly MessagePump m_MessagePump;

		private readonly string m_ToString;

		private ClientVersion m_Version;

		[CommandProperty(AccessLevel.Administrator, true)]
		public DateTime ConnectedOn { get; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public TimeSpan ConnectedFor => DateTime.UtcNow - ConnectedOn;

		[CommandProperty(AccessLevel.Administrator, true)]
		public uint AuthID { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public uint Seed { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public IPAddress Address { get; }

		private static bool m_Paused;

		public static bool Paused => m_Paused;

		[Flags]
		private enum AsyncState
		{
			Pending = 0x01,
			Paused = 0x02
		}

		private AsyncState m_AsyncState;

		private readonly object m_AsyncLock = new object();

		[CommandProperty(AccessLevel.Administrator, true)]
		public IPacketEncoder PacketEncoder { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public IPacketEncryptor PacketEncryptor { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool SentFirstPacket { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool BlockAllPackets { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool? Encrypted { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public ClientFlags Flags { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public ClientVersion Version
		{
			get => m_Version;
			set
			{
				m_Version = value;

				if (value >= m_Version70610)
				{
					_ProtocolChanges = ProtocolChanges.Version70610;
				}
				else if (value >= m_Version706047)
				{
					_ProtocolChanges = ProtocolChanges.Version706047;
				}
				else if (value >= m_Version70595)
				{
					_ProtocolChanges = ProtocolChanges.Version70595;
				}
				else if (value >= m_Version70560)
				{
					_ProtocolChanges = ProtocolChanges.Version70560;
				}
				else if (value >= m_Version70500)
				{
					_ProtocolChanges = ProtocolChanges.Version70500;
				}
				else if (value >= m_Version704565)
				{
					_ProtocolChanges = ProtocolChanges.Version704565;
				}
				else if (value >= m_Version70331)
				{
					_ProtocolChanges = ProtocolChanges.Version70331;
				}
				else if (value >= m_Version70300)
				{
					_ProtocolChanges = ProtocolChanges.Version70300;
				}
				else if (value >= m_Version70160)
				{
					_ProtocolChanges = ProtocolChanges.Version70160;
				}
				else if (value >= m_Version70130)
				{
					_ProtocolChanges = ProtocolChanges.Version70130;
				}
				else if (value >= m_Version7090)
				{
					_ProtocolChanges = ProtocolChanges.Version7090;
				}
				else if (value >= m_Version7000)
				{
					_ProtocolChanges = ProtocolChanges.Version7000;
				}
				else if (value >= m_Version60142)
				{
					_ProtocolChanges = ProtocolChanges.Version60142;
				}
				else if (value >= m_Version6017)
				{
					_ProtocolChanges = ProtocolChanges.Version6017;
				}
				else if (value >= m_Version6000)
				{
					_ProtocolChanges = ProtocolChanges.Version6000;
				}
				else if (value >= m_Version502b)
				{
					_ProtocolChanges = ProtocolChanges.Version502b;
				}
				else if (value >= m_Version500a)
				{
					_ProtocolChanges = ProtocolChanges.Version500a;
				}
				else if (value >= m_Version407a)
				{
					_ProtocolChanges = ProtocolChanges.Version407a;
				}
				else if (value >= m_Version400a)
				{
					_ProtocolChanges = ProtocolChanges.Version400a;
				}
			}
		}

		private static readonly ClientVersion m_Version400a = new ClientVersion("4.0.0a");
		private static readonly ClientVersion m_Version407a = new ClientVersion("4.0.7a");
		private static readonly ClientVersion m_Version500a = new ClientVersion("5.0.0a");
		private static readonly ClientVersion m_Version502b = new ClientVersion("5.0.2b");
		private static readonly ClientVersion m_Version6000 = new ClientVersion("6.0.0.0");
		private static readonly ClientVersion m_Version6017 = new ClientVersion("6.0.1.7");
		private static readonly ClientVersion m_Version60142 = new ClientVersion("6.0.14.2");
		private static readonly ClientVersion m_Version7000 = new ClientVersion("7.0.0.0");
		private static readonly ClientVersion m_Version7090 = new ClientVersion("7.0.9.0");
		private static readonly ClientVersion m_Version70130 = new ClientVersion("7.0.13.0");
		private static readonly ClientVersion m_Version70160 = new ClientVersion("7.0.16.0");
		private static readonly ClientVersion m_Version70300 = new ClientVersion("7.0.30.0");
		private static readonly ClientVersion m_Version70331 = new ClientVersion("7.0.33.1");
		private static readonly ClientVersion m_Version704565 = new ClientVersion("7.0.45.65");
		private static readonly ClientVersion m_Version70500 = new ClientVersion("7.0.50.0");
		private static readonly ClientVersion m_Version70560 = new ClientVersion("7.0.56.0");
		private static readonly ClientVersion m_Version70595 = new ClientVersion("7.0.59.5");
		private static readonly ClientVersion m_Version706047 = new ClientVersion("7.0.60.47");
		private static readonly ClientVersion m_Version70610 = new ClientVersion("7.0.61.0");

		private ProtocolChanges _ProtocolChanges;

		private enum ProtocolChanges : ulong
		{
			NewSpellbook = 0x00000001,
			DamagePacket = 0x00000002,
			Unpack = 0x00000004,
			BuffIcon = 0x00000008,
			NewHaven = 0x00000010,
			ContainerGridLines = 0x00000020,
			ExtendedSupportedFeatures = 0x00000040,
			StygianAbyss = 0x00000080,
			HighSeas = 0x00000100,
			NewCharacterList = 0x00000200,
			NewCharacterCreation = 0x00000400,
			ExtendedStatus = 0x00000800,
			NewMobileIncoming = 0x00001000,
			NewSecureTrading = 0x00002000,
			UltimaStore = 0x00004000,
			WeddingKit = 0x00008000,
			WarpGates = 0x00010000,
			DragonMount = 0x00020000,
			EndlessJourney = 0x00040000,

			Version400a = NewSpellbook,
			Version407a = Version400a | DamagePacket,
			Version500a = Version407a | Unpack,
			Version502b = Version500a | BuffIcon,
			Version6000 = Version502b | NewHaven,
			Version6017 = Version6000 | ContainerGridLines,
			Version60142 = Version6017 | ExtendedSupportedFeatures,
			Version7000 = Version60142 | StygianAbyss,
			Version7090 = Version7000 | HighSeas,
			Version70130 = Version7090 | NewCharacterList,
			Version70160 = Version70130 | NewCharacterCreation,
			Version70300 = Version70160 | ExtendedStatus,
			Version70331 = Version70300 | NewMobileIncoming,
			Version704565 = Version70331 | NewSecureTrading,
			Version70500 = Version704565 | UltimaStore,
			Version70560 = Version70500 | WeddingKit,
			Version70595 = Version70560 | WarpGates,
			Version706047 = Version70595 | DragonMount,
			Version70610 = Version706047 | EndlessJourney,
		}

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool NewSpellbook => _ProtocolChanges.HasFlag(ProtocolChanges.NewSpellbook);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool DamagePacket => _ProtocolChanges.HasFlag(ProtocolChanges.DamagePacket);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool Unpack => _ProtocolChanges.HasFlag(ProtocolChanges.Unpack);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool BuffIcon => _ProtocolChanges.HasFlag(ProtocolChanges.BuffIcon);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool NewHaven => _ProtocolChanges.HasFlag(ProtocolChanges.NewHaven);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool ContainerGridLines => _ProtocolChanges.HasFlag(ProtocolChanges.ContainerGridLines);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool ExtendedSupportedFeatures => _ProtocolChanges.HasFlag(ProtocolChanges.ExtendedSupportedFeatures);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool StygianAbyss => _ProtocolChanges.HasFlag(ProtocolChanges.StygianAbyss);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool HighSeas => _ProtocolChanges.HasFlag(ProtocolChanges.HighSeas);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool NewCharacterList => _ProtocolChanges.HasFlag(ProtocolChanges.NewCharacterList);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool NewCharacterCreation => _ProtocolChanges.HasFlag(ProtocolChanges.NewCharacterCreation);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool ExtendedStatus => _ProtocolChanges.HasFlag(ProtocolChanges.ExtendedStatus);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool NewMobileIncoming => _ProtocolChanges.HasFlag(ProtocolChanges.NewMobileIncoming);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool NewSecureTrading => _ProtocolChanges.HasFlag(ProtocolChanges.NewSecureTrading);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool UltimaStore => _ProtocolChanges.HasFlag(ProtocolChanges.UltimaStore);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool WeddingKit => _ProtocolChanges.HasFlag(ProtocolChanges.WeddingKit);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool WarpGates => _ProtocolChanges.HasFlag(ProtocolChanges.WarpGates);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool DragonMount => _ProtocolChanges.HasFlag(ProtocolChanges.DragonMount);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool EndlessJourney => _ProtocolChanges.HasFlag(ProtocolChanges.EndlessJourney);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool IsUOTDClient => Flags.HasFlag(ClientFlags.UOTD) || (m_Version != null && m_Version.Type == ClientType.UOTD);

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool IsSAClient => m_Version != null && m_Version.Type >= ClientType.SA;

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool IsEnhancedClient => IsUOTDClient || (m_Version != null && m_Version.Major >= 67);

		public List<SecureTrade> Trades { get; }

		public void ValidateAllTrades()
		{
			if (Trades == null)
			{
				return;
			}

			for (var i = Trades.Count - 1; i >= 0; --i)
			{
				if (i >= Trades.Count)
				{
					continue;
				}

				var trade = Trades[i];

				if (trade.From.Mobile.Deleted || trade.To.Mobile.Deleted
				 || !trade.From.Mobile.Alive || !trade.To.Mobile.Alive
				 || !trade.From.Mobile.InRange(trade.To.Mobile, 2)
				 || trade.From.Mobile.Map != trade.To.Mobile.Map)
				{
					trade.Cancel();
				}
			}
		}

		public void CancelAllTrades()
		{
			if (Trades == null)
			{
				return;
			}

			for (var i = Trades.Count - 1; i >= 0; --i)
			{
				if (i < Trades.Count)
				{
					Trades[i].Cancel();
				}
			}
		}

		public void RemoveTrade(SecureTrade trade)
		{
			if (Trades != null)
			{
				Trades.Remove(trade);
			}
		}

		public SecureTrade FindTrade(Mobile m)
		{
			if (Trades == null)
			{
				return null;
			}

			foreach (var trade in Trades)
			{
				if (trade.From.Mobile == m || trade.To.Mobile == m)
				{
					return trade;
				}
			}

			return null;
		}

		public SecureTradeContainer FindTradeContainer(Mobile m)
		{
			if (Trades == null)
			{
				return null;
			}

			foreach (var trade in Trades)
			{
				var from = trade.From;
				var to = trade.To;

				if (from.Mobile == Mobile && to.Mobile == m)
				{
					return from.Container;
				}

				if (from.Mobile == m && to.Mobile == Mobile)
				{
					return to.Container;
				}
			}

			return null;
		}

		public SecureTradeContainer AddTrade(NetState state)
		{
			if (Trades == null || state.Trades == null)
			{
				return null;
			}

			var newTrade = new SecureTrade(Mobile, state.Mobile);

			Trades.Add(newTrade);
			state.Trades.Add(newTrade);

			return newTrade.From.Container;
		}

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool CompressionEnabled { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public int Sequence { get; set; }

		public List<Gump> Gumps { get; private set; }
		public List<HuePicker> HuePickers { get; private set; }
		public List<IMenu> Menus { get; private set; }

		public static int GumpCap { get; set; } = 512;
		public static int HuePickerCap { get; set; } = 512;
		public static int MenuCap { get; set; } = 512;

		[CommandProperty(AccessLevel.Administrator, true)]
		public int UpdateRange { get; set; } = Core.GlobalUpdateRange;

		public void WriteConsole(string text)
		{
			Console.WriteLine("Client: {0}: {1}", this, text);
		}

		public void WriteConsole(string format, params object[] args)
		{
			WriteConsole(String.Format(format, args));
		}

		public void AddMenu(IMenu menu)
		{
			if (Menus == null)
			{
				Menus = new List<IMenu>();
			}

			if (Menus.Count < MenuCap)
			{
				Menus.Add(menu);
			}
			else
			{
				Utility.PushColor(ConsoleColor.Red);
				WriteConsole("Exceeded menu cap, disconnecting...");
				Utility.PopColor();

				Dispose();
			}
		}

		public void RemoveMenu(IMenu menu)
		{
			if (Menus != null)
			{
				Menus.Remove(menu);
			}
		}

		public void RemoveMenu(int index)
		{
			if (Menus != null)
			{
				Menus.RemoveAt(index);
			}
		}

		public void ClearMenus()
		{
			if (Menus != null)
			{
				Menus.Clear();
			}
		}

		public void AddHuePicker(HuePicker huePicker)
		{
			if (HuePickers == null)
			{
				HuePickers = new List<HuePicker>();
			}

			if (HuePickers.Count < HuePickerCap)
			{
				HuePickers.Add(huePicker);
			}
			else
			{
				Utility.PushColor(ConsoleColor.Red);
				WriteConsole("Exceeded hue picker cap, disconnecting...");
				Utility.PopColor();

				Dispose();
			}
		}

		public void RemoveHuePicker(HuePicker huePicker)
		{
			if (HuePickers != null)
			{
				HuePickers.Remove(huePicker);
			}
		}

		public void RemoveHuePicker(int index)
		{
			if (HuePickers != null)
			{
				HuePickers.RemoveAt(index);
			}
		}

		public void ClearHuePickers()
		{
			if (HuePickers != null)
			{
				HuePickers.Clear();
			}
		}

		public void AddGump(Gump gump)
		{
			if (Gumps == null)
			{
				Gumps = new List<Gump>();
			}

			if (Gumps.Count < GumpCap)
			{
				Gumps.Add(gump);
			}
			else
			{
				Utility.PushColor(ConsoleColor.Red);
				WriteConsole("Exceeded gump cap, disconnecting...");
				Utility.PopColor();

				Dispose();
			}
		}

		public void RemoveGump(Gump gump)
		{
			if (Gumps != null)
			{
				Gumps.Remove(gump);
			}
		}

		public void RemoveGump(int index)
		{
			if (Gumps != null)
			{
				Gumps.RemoveAt(index);
			}
		}

		public void ClearGumps()
		{
			if (Gumps != null)
			{
				Gumps.Clear();
			}
		}

		public void LaunchBrowser(string url)
		{
			Send(new MessageLocalized(Serial.MinusOne, -1, MessageType.Label, 0x35, 3, 501231, "", ""));
			Send(new LaunchBrowser(url));
		}

		[CommandProperty(AccessLevel.Administrator, true)]
		public CityInfo[] CityInfo { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public Mobile Mobile { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public ServerInfo[] ServerInfo { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public IAccount Account { get; set; }

		public override string ToString()
		{
			return m_ToString;
		}

		public static List<NetState> Instances { get; } = new List<NetState>();

		public const int SendBufferCapacity = 1024, SendBufferSize = 8092;
		public const int ReceiveBufferCapacity = 1024, ReceiveBufferSize = 2048;

		public static BufferPool SendBuffers { get; } = new BufferPool("Send", SendBufferCapacity, SendBufferSize);
		public static BufferPool ReceiveBuffers { get; } = new BufferPool("Receive", ReceiveBufferCapacity, ReceiveBufferSize);

		public NetState(Socket socket, MessagePump messagePump)
		{
			Socket = socket;

			for (var i = 0; i < Buffers.Length; i++)
			{
				Buffers[i] = new ByteQueue();
			}

			m_RecvBuffer = ReceiveBuffers.AcquireBuffer();

			m_MessagePump = messagePump;

			Gumps = new List<Gump>();
			HuePickers = new List<HuePicker>();
			Menus = new List<IMenu>();
			Trades = new List<SecureTrade>();

			m_SendQueue = new SendQueue();

			m_NextCheckActivity = Core.TickCount + 30000;

			Instances.Add(this);

			try
			{
				Address = Utility.Intern(((IPEndPoint)Socket.RemoteEndPoint).Address);

				m_ToString = Address.ToString();
			}
			catch (Exception ex)
			{
				TraceException(ex);

				Address = IPAddress.None;

				m_ToString = "(error)";
			}

			ConnectedOn = DateTime.UtcNow;

			UpdateRange = Core.GlobalUpdateRange;

			if (CreatedCallback != null)
			{
				CreatedCallback(this);
			}
		}

		public SpeedControl SpeedControl { get; private set; }

		private bool _Sending;

		private readonly object _SendLock = new object();

		public virtual void Send(Packet p)
		{
			if (p == null)
			{
				return;
			}

			if (Socket == null || BlockAllPackets)
			{
				p.OnSend();
				return;
			}
			/*
			if (Core.Debug && IPAddress.IsLoopback(Address))
			{
				using (var s = File.AppendText(Path.Combine("Logs", $"{m_ToString.Replace('.', '-')}.log")))
				{
					var pb = p.Stream?.ToArray();

					s.WriteLine();

					if (pb != null)
					{
						using (var ms = new MemoryStream(pb))
						{
							s.WriteLine($"[CV {m_Version}] {p.GetType()} 0x{p.PacketID:X2} ({ms.Length:N0} bytes)");

							Utility.FormatBuffer(s, ms, pb.Length);
						}
					}
					else
					{
						s.WriteLine($"[CV {m_Version}] {p.GetType()} 0x{p.PacketID:X2} (Compiled)");
					}

					s.Flush();
				}
			}
			*/
			var buffer = p.Compile(CompressionEnabled, out var length);

			if (buffer != null)
			{
				if (buffer.Length <= 0 || length <= 0)
				{
					p.OnSend();
					return;
				}

				PacketSendProfile prof = null;

				if (Core.Profiling)
				{
					prof = PacketSendProfile.Acquire(p.GetType());
				}

				if (prof != null)
				{
					prof.Start();
				}

				var buffered = false;

				if (PacketEncoder != null || PacketEncryptor != null)
				{
					var packetBuffer = buffer;
					var packetLength = length;

					if (BufferStaticPackets && p.State.HasFlag(PacketState.Acquired))
					{
						if (packetLength <= SendBufferSize)
						{
							packetBuffer = SendBuffers.AcquireBuffer();
						}
						else
						{
							packetBuffer = new byte[packetLength];
						}

						System.Buffer.BlockCopy(buffer, 0, packetBuffer, 0, packetLength);
					}

					if (PacketEncoder != null)
					{
						PacketEncoder.EncodeOutgoingPacket(this, ref packetBuffer, ref packetLength);
					}

					if (PacketEncryptor != null)
					{
						PacketEncryptor.EncryptOutgoingPacket(this, ref packetBuffer, ref packetLength);
					}

					buffered = buffer != packetBuffer && packetBuffer.Length == SendBufferSize;

					buffer = packetBuffer;
					length = packetLength;
				}

				try
				{
					SendQueue.Gram gram;

					lock (_SendLock)
					{
						lock (m_SendQueue)
						{
							gram = m_SendQueue.Enqueue(buffer, length);
						}

						if (buffered && SendBuffers.Count < SendBufferCapacity)
						{
							SendBuffers.ReleaseBuffer(ref buffer);
						}

						if (gram != null && !_Sending)
						{
							_Sending = true;

							try
							{
								Socket.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, m_OnSend, Socket);
							}
							catch (Exception ex)
							{
								TraceException(ex);
								Dispose(false);
							}
						}
					}
				}
				catch (CapacityExceededException)
				{
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine("Client: {0}: Too much data pending, disconnecting...", this);
					Utility.PopColor();

					Dispose(false);
				}

				p.OnSend();

				if (prof != null)
				{
					prof.Finish(length);
				}

				if (p is SpeedControl)
				{
					if (p == SpeedControl.Disable)
					{
						SpeedControl = null;
					}
					else
					{
						SpeedControl = (SpeedControl)p;
					}
				}
			}
			else
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Client: {0}: null buffer send, disconnecting...", this);
				Utility.PopColor();

				using (var op = new StreamWriter("null_send.log", true))
				{
					op.WriteLine("{0} Client: {1}: null buffer send, disconnecting...", DateTime.UtcNow, this);
					op.WriteLine(new StackTrace());
				}

				Dispose();
			}
		}

		public void Start()
		{
			m_OnReceive = OnReceive;
			m_OnSend = OnSend;

			Running = true;

			if (Socket == null || m_Paused)
			{
				return;
			}

			try
			{
				lock (m_AsyncLock)
				{
					if ((m_AsyncState & (AsyncState.Pending | AsyncState.Paused)) == 0)
					{
						InternalBeginReceive();
					}
				}
			}
			catch (Exception ex)
			{
				TraceException(ex);
				Dispose(false);
			}
		}

		private void InternalBeginReceive()
		{
			m_AsyncState |= AsyncState.Pending;

			Socket.BeginReceive(m_RecvBuffer, 0, m_RecvBuffer.Length, SocketFlags.None, m_OnReceive, Socket);
		}

		private void OnReceive(IAsyncResult asyncResult)
		{
			try
			{
				var s = (Socket)asyncResult.AsyncState;

				var byteCount = s.EndReceive(asyncResult);

				if (byteCount > 0)
				{
					m_NextCheckActivity = Core.TickCount + 90000;

					byte[] buffer;

					lock (m_AsyncLock)
					{
						buffer = m_RecvBuffer;
					}

					if (PacketEncryptor != null)
					{
						PacketEncryptor.DecryptIncomingPacket(this, ref buffer, ref byteCount);
					}

					if (PacketEncoder != null)
					{
						PacketEncoder.DecodeIncomingPacket(this, ref buffer, ref byteCount);
					}

					lock (Buffer)
					{
						Buffer.Enqueue(buffer, 0, byteCount);
					}

					m_MessagePump.OnReceive(this);

					lock (m_AsyncLock)
					{
						m_AsyncState &= ~AsyncState.Pending;

						if ((m_AsyncState & AsyncState.Paused) == 0)
						{
							try
							{
								InternalBeginReceive();
							}
							catch (Exception ex)
							{
								TraceException(ex);
								Dispose(false);
							}
						}
					}
				}
				else
				{
					Dispose(false);
				}
			}
			catch
			{
				Dispose(false);
			}
		}

		private void OnSend(IAsyncResult asyncResult)
		{
			try
			{
				var s = (Socket)asyncResult.AsyncState;

				var bytes = s.EndSend(asyncResult);

				if (bytes <= 0)
				{
					Dispose(false);
					return;
				}

				m_NextCheckActivity = Core.TickCount + 90000;

				if (CoalesceSleep >= 0)
				{
					Thread.Sleep(CoalesceSleep);
				}

				SendQueue.Gram gram;

				lock (m_SendQueue)
				{
					gram = m_SendQueue.Dequeue();

					if (gram == null && m_SendQueue.IsFlushReady)
					{
						gram = m_SendQueue.CheckFlushReady();
					}
				}

				if (gram != null)
				{
					try
					{
						s.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, m_OnSend, s);
					}
					catch (Exception ex)
					{
						TraceException(ex);
						Dispose(false);
					}
				}
				else
				{
					lock (_SendLock)
					{
						_Sending = false;
					}
				}
			}
			catch
			{
				Dispose(false);
			}
		}

		public static void Pause()
		{
			if (m_Paused)
			{
				return;
			}

			m_Paused = true;

			foreach (var ns in Instances)
			{
				lock (ns.m_AsyncLock)
				{
					ns.m_AsyncState |= AsyncState.Paused;
				}
			}
		}

		public static void Resume()
		{
			if (!m_Paused)
			{
				return;
			}

			m_Paused = false;

			World.ProcessSafetyQueues();

			foreach (var ns in Instances)
			{
				if (ns.Socket == null)
				{
					continue;
				}

				lock (ns.m_AsyncLock)
				{
					ns.m_AsyncState &= ~AsyncState.Paused;

					try
					{
						if ((ns.m_AsyncState & AsyncState.Pending) == 0)
						{
							ns.InternalBeginReceive();
						}
					}
					catch (Exception ex)
					{
						TraceException(ex);

						ns.Dispose(false);
					}
				}
			}
		}

		public bool Flush()
		{
			if (Socket == null)
			{
				return false;
			}

			lock (_SendLock)
			{
				if (_Sending)
				{
					return false;
				}

				SendQueue.Gram gram;

				lock (m_SendQueue)
				{
					if (!m_SendQueue.IsFlushReady)
					{
						return false;
					}

					gram = m_SendQueue.CheckFlushReady();
				}

				if (gram != null)
				{
					try
					{
						_Sending = true;

						Socket.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, m_OnSend, Socket);

						return true;
					}
					catch (Exception ex)
					{
						_Sending = false;

						TraceException(ex);

						Dispose(false);
					}
				}
			}

			return false;
		}

		public PacketHandler GetHandler(int packetID)
		{
			return PacketHandlers.GetHandler(packetID);
		}

		public static void FlushAll()
		{
			var index = Instances.Count;

			while (--index >= 0)
			{
				if (index < Instances.Count)
					Instances[index]?.Flush();
			}
		}

		public static int CoalesceSleep { get; set; } = -1;

		private long m_NextCheckActivity;

		public void CheckAlive(long curTicks)
		{
			if (Socket == null)
			{
				return;
			}

			if (m_NextCheckActivity - curTicks >= 0)
			{
				return;
			}

			Utility.PushColor(ConsoleColor.Red);
			Console.WriteLine("Client: {0}: Disconnecting due to inactivity...", this);
			Utility.PopColor();

			Dispose();
		}

		public static void TraceException(Exception ex)
		{
			if (!Core.Debug)
			{
				return;
			}

			try
			{
				using (var op = new StreamWriter("network-errors.log", true))
				{
					op.WriteLine("# {0}", DateTime.UtcNow);

					op.WriteLine(ex);

					op.WriteLine();
					op.WriteLine();
				}
			}
			catch
			{ }

			try
			{
				Console.WriteLine(ex);
			}
			catch
			{ }
		}

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool IsDisposing { get; private set; }

		public void Dispose()
		{
			Dispose(true);
		}

		public virtual void Dispose(bool flush)
		{
			if (Socket == null || IsDisposing)
			{
				return;
			}

			IsDisposing = true;

			if (flush)
			{
				Flush();
			}

			try
			{
				Socket.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException ex)
			{
				TraceException(ex);
			}

			try
			{
				Socket.Close();
			}
			catch (SocketException ex)
			{
				TraceException(ex);
			}

			ReceiveBuffers.ReleaseBuffer(ref m_RecvBuffer);

			Socket = null;

			PacketEncoder = null;
			PacketEncryptor = null;

			for (var i = 0; i < Buffers.Length; i++)
			{
				Buffers[i] = null;
			}

			m_RecvBuffer = null;

			m_OnReceive = null;
			m_OnSend = null;

			Running = false;

			lock (m_Disposed)
			{
				m_Disposed.Enqueue(this);
			}

			lock (m_SendQueue)
			{
				if (!m_SendQueue.IsEmpty)
				{
					m_SendQueue.Clear();
				}
			}
		}

		public static void Initialize()
		{
			Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.5), CheckAllAlive);
		}

		public static void CheckAllAlive()
		{
			try
			{
				var curTicks = Core.TickCount;

				var i = Instances.Count;

				while (--i >= 0)
				{
					if (i < Instances.Count)
					{
						Instances[i]?.CheckAlive(curTicks);
					}
				}
			}
			catch (Exception ex)
			{
				TraceException(ex);
			}
		}

		private static readonly Queue<NetState> m_Disposed = new Queue<NetState>();

		public static void ProcessDisposedQueue()
		{
			lock (m_Disposed)
			{
				var breakout = 100;

				while (--breakout >= 0 && m_Disposed.Count > 0)
				{
					var ns = m_Disposed.Dequeue();

					var m = ns.Mobile;
					var a = ns.Account;

					if (m != null)
					{
						m.CloseAllGumps();

						m.NetState = null;
						ns.Mobile = null;
					}

					ns.Gumps.Clear();
					ns.Menus.Clear();
					ns.HuePickers.Clear();

					ns.Account = null;
					ns.ServerInfo = null;
					ns.CityInfo = null;

					Instances.Remove(ns);

					Utility.PushColor(ConsoleColor.Red);

					if (a != null)
					{
						ns.WriteConsole("Disconnected. [{0} Online] [{1}]", Instances.Count, a);
					}
					else if (MessagePump.Display(ns))
					{
						ns.WriteConsole("Disconnected. [{0} Online]", Instances.Count);
					}

					Utility.PopColor();
				}
			}
		}

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool Running { get; private set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool Seeded { get; set; }

		public Socket Socket { get; private set; }

		public ByteQueue[] Buffers { get; } = new ByteQueue[2];

		public ByteQueue Buffer => Buffers[0];
		public ByteQueue BufferSlice => Buffers[1];

		public ExpansionInfo ExpansionInfo
		{
			get
			{
				for (var i = ExpansionInfo.Table.Length - 1; i >= 0; i--)
				{
					var info = ExpansionInfo.Table[i];

					if ((info.RequiredClient != ClientVersion.Zero && Version >= info.RequiredClient) || (Flags & info.ClientFlags) != 0)
					{
						return info;
					}
				}

				return ExpansionInfo.GetInfo(Expansion.None);
			}
		}

		[CommandProperty(AccessLevel.Administrator, true)]
		public Expansion Expansion => (Expansion)ExpansionInfo.ID;

		public bool SupportsExpansion(ExpansionInfo info, bool checkCoreExpansion)
		{
			if (info == null || (checkCoreExpansion && (int)Core.Expansion < info.ID))
			{
				return false;
			}

			if (info.RequiredClient != ClientVersion.Zero)
			{
				return IsEnhancedClient || Version >= info.RequiredClient;
			}

			return (Flags & info.ClientFlags) != 0;
		}

		public bool SupportsExpansion(Expansion ex, bool checkCoreExpansion)
		{
			return SupportsExpansion(ExpansionInfo.GetInfo(ex), checkCoreExpansion);
		}

		public bool SupportsExpansion(Expansion ex)
		{
			return SupportsExpansion(ex, true);
		}

		public bool SupportsExpansion(ExpansionInfo info)
		{
			return SupportsExpansion(info, true);
		}

		public int CompareTo(NetState other)
		{
			if (other == null)
			{
				return 1;
			}

			return Insensitive.Compare(m_ToString, other.m_ToString);
		}

		#region Packet Throttling

		private readonly long[] _Throttles = new long[Byte.MaxValue];

		public void SetPacketTime(byte packetID)
		{
			_Throttles[packetID] = Core.TickCount;
		}

		public long GetPacketTime(byte packetID)
		{
			return _Throttles[packetID];
		}

		public bool IsThrottled(byte packetID, int delayMS)
		{
			return _Throttles[packetID] + delayMS > Core.TickCount;
		}

		#endregion
	}
}
