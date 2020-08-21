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

		[CommandProperty(AccessLevel.Administrator, true)]
		public DateTime ConnectedOn { get; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public TimeSpan ConnectedFor => (DateTime.UtcNow - ConnectedOn);

		[CommandProperty(AccessLevel.Administrator, true)]
		public uint AuthID { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public uint Seed { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public IPAddress Address { get; }

		private static bool m_Paused;

		[Flags]
		private enum AsyncState
		{
			Pending = 0x01,
			Paused = 0x02
		}

		private AsyncState m_AsyncState;
		private readonly object m_AsyncLock = new object();

		public IPacketEncoder PacketEncoder { get; set; }
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
		public ClientVersion Version { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool IsUOTDClient => ((Flags & ClientFlags.UOTD) != 0 || (Version != null && Version.Type == ClientType.UOTD));

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool IsEnhancedClient => IsUOTDClient || (Version != null && Version.Major >= 67);

		public List<SecureTrade> Trades { get; }

		public void ValidateAllTrades()
		{
			if (Trades == null)
			{
				return;
			}

			for (int i = Trades.Count - 1; i >= 0; --i)
			{
				if (i >= Trades.Count)
				{
					continue;
				}

				SecureTrade trade = Trades[i];

				if (trade.From.Mobile.Deleted || trade.To.Mobile.Deleted || !trade.From.Mobile.Alive || !trade.To.Mobile.Alive ||
					!trade.From.Mobile.InRange(trade.To.Mobile, 2) || trade.From.Mobile.Map != trade.To.Mobile.Map)
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

			for (int i = Trades.Count - 1; i >= 0; --i)
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

			foreach (SecureTrade trade in Trades)
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

			foreach (SecureTrade trade in Trades)
			{
				SecureTradeInfo from = trade.From;
				SecureTradeInfo to = trade.To;

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

			SecureTrade newTrade = new SecureTrade(Mobile, state.Mobile);

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

		private static int m_GumpCap = 512, m_HuePickerCap = 512, m_MenuCap = 512;

		public static int GumpCap { get => m_GumpCap; set => m_GumpCap = value; }
		public static int HuePickerCap { get => m_HuePickerCap; set => m_HuePickerCap = value; }
		public static int MenuCap { get => m_MenuCap; set => m_MenuCap = value; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public int UpdateRange { get; set; }

		public void WriteConsole(string text)
		{
			Console.WriteLine("Client: {0}: {1}", this, text);
		}

		public void WriteConsole(string format, params object[] args)
		{
			WriteConsole(string.Format(format, args));
		}

		public void AddMenu(IMenu menu)
		{
			if (Menus == null)
			{
				Menus = new List<IMenu>();
			}

			if (Menus.Count < m_MenuCap)
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

			if (HuePickers.Count < m_HuePickerCap)
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

			if (Gumps.Count < m_GumpCap)
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

		public CityInfo[] CityInfo { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public Mobile Mobile { get; set; }

		public ServerInfo[] ServerInfo { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public IAccount Account { get; set; }

		public override string ToString()
		{
			return m_ToString;
		}

		private static readonly List<NetState> m_Instances = new List<NetState>();

		public static List<NetState> Instances => m_Instances;

		public const int SendBufferCapacity = 1024, SendBufferSize = 8092;
		public const int ReceiveBufferCapacity = 1024, ReceiveBufferSize = 2048;

		private static readonly BufferPool m_SendBufferPool = new BufferPool("Send", SendBufferCapacity, SendBufferSize);
		private static readonly BufferPool m_ReceiveBufferPool = new BufferPool("Receive", ReceiveBufferCapacity, ReceiveBufferSize);

		public static BufferPool SendBuffers => m_SendBufferPool;
		public static BufferPool ReceiveBuffers => m_ReceiveBufferPool;

		public NetState(Socket socket, MessagePump messagePump)
		{
			Socket = socket;
			Buffer = new ByteQueue();

			m_RecvBuffer = m_ReceiveBufferPool.AcquireBuffer();
			m_MessagePump = messagePump;

			Gumps = new List<Gump>();
			HuePickers = new List<HuePicker>();
			Menus = new List<IMenu>();
			Trades = new List<SecureTrade>();

			m_SendQueue = new SendQueue();

			m_NextCheckActivity = Core.TickCount + 30000;

			m_Instances.Add(this);

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

		private bool _Sending;

		private readonly object _SendLock = new object();

		public virtual void Send(Packet p)
		{
			if (Socket == null || BlockAllPackets)
			{
				p.OnSend();
				return;
			}


			byte[] buffer = p.Compile(CompressionEnabled, out int length);

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

				bool buffered = false;

				if (PacketEncoder != null || PacketEncryptor != null)
				{
					byte[] packetBuffer = buffer;
					int packetLength = length;

					if (BufferStaticPackets && p.State.HasFlag(PacketState.Acquired))
					{
						if (packetLength <= SendBufferSize)
						{
							packetBuffer = m_SendBufferPool.AcquireBuffer();
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

						if (buffered && m_SendBufferPool.Count < SendBufferCapacity)
						{
							m_SendBufferPool.ReleaseBuffer(buffer);
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
			}
			else
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Client: {0}: null buffer send, disconnecting...", this);
				Utility.PopColor();

				using (StreamWriter op = new StreamWriter("null_send.log", true))
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
				Socket s = (Socket)asyncResult.AsyncState;

				int byteCount = s.EndReceive(asyncResult);

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
						Buffer.Enqueue(buffer, 0, byteCount);

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
			Socket s = (Socket)asyncResult.AsyncState;

			try
			{
				int bytes = s.EndSend(asyncResult);

				if (bytes <= 0)
				{
					Dispose(false);
					return;
				}

				m_NextCheckActivity = Core.TickCount + 90000;

				if (m_CoalesceSleep >= 0)
				{
					Thread.Sleep(m_CoalesceSleep);
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
			catch (Exception)
			{
				Dispose(false);
			}
		}

		public static void Pause()
		{
			m_Paused = true;

			foreach (NetState ns in m_Instances)
			{
				lock (ns.m_AsyncLock)
				{
					ns.m_AsyncState |= AsyncState.Paused;
				}
			}
		}

		public static void Resume()
		{
			m_Paused = false;

			foreach (NetState ns in m_Instances)
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
			int index = m_Instances.Count;

			while (--index >= 0)
			{
				if (index < m_Instances.Count)
					m_Instances[index]?.Flush();
			}
		}

		private static int m_CoalesceSleep = -1;

		public static int CoalesceSleep { get => m_CoalesceSleep; set => m_CoalesceSleep = value; }

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
				using (StreamWriter op = new StreamWriter("network-errors.log", true))
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

		private bool m_Disposing;

		[CommandProperty(AccessLevel.Administrator, true)]
		public bool IsDisposing => m_Disposing;

		public void Dispose()
		{
			Dispose(true);
		}

		public virtual void Dispose(bool flush)
		{
			if (Socket == null || m_Disposing)
			{
				return;
			}

			m_Disposing = true;

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

			if (m_RecvBuffer != null)
			{
				lock (m_ReceiveBufferPool)
				{
					m_ReceiveBufferPool.ReleaseBuffer(m_RecvBuffer);
				}
			}

			Socket = null;

			PacketEncoder = null;
			PacketEncryptor = null;

			Buffer = null;
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
				long curTicks = Core.TickCount;

				int i = m_Instances.Count;

				while (--i >= 0)
				{
					if (m_Instances[i] != null)
					{
						m_Instances[i].CheckAlive(curTicks);
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
				int breakout = 200;

				while (--breakout >= 0 && m_Disposed.Count > 0)
				{
					NetState ns = m_Disposed.Dequeue();

					Mobile m = ns.Mobile;
					IAccount a = ns.Account;

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

					m_Instances.Remove(ns);

					Utility.PushColor(ConsoleColor.Red);

					if (a != null)
					{
						ns.WriteConsole("Disconnected. [{0} Online] [{1}]", m_Instances.Count, a);
					}
					else if (MessagePump.Display(ns))
					{
						ns.WriteConsole("Disconnected. [{0} Online]", m_Instances.Count);
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

		public ByteQueue Buffer { get; private set; }

		public ExpansionInfo ExpansionInfo
		{
			get
			{
				for (int i = ExpansionInfo.Table.Length - 1; i >= 0; i--)
				{
					ExpansionInfo info = ExpansionInfo.Table[i];

					if ((info.RequiredClient != null && Version >= info.RequiredClient) || ((Flags & info.ClientFlags) != 0))
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

			if (info.RequiredClient != null)
			{
				return (IsEnhancedClient || Version >= info.RequiredClient);
			}

			return ((Flags & info.ClientFlags) != 0);
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

			return string.Compare(m_ToString, other.m_ToString, StringComparison.Ordinal);
		}
	}
}
