#region Header
// **********
// ServUO - NetState.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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

	public delegate void NetStateCreatedCallback(NetState ns);

	public class NetState : IComparable<NetState>
	{
		private Socket m_Socket;
		private readonly IPAddress m_Address;
		private ByteQueue m_Buffer;
		private byte[] m_RecvBuffer;
		private readonly SendQueue m_SendQueue;
		private bool m_Running;

#if NewAsyncSockets
		private SocketAsyncEventArgs m_ReceiveEventArgs, m_SendEventArgs;
#else
		private AsyncCallback m_OnReceive, m_OnSend;
#endif

		private readonly MessagePump m_MessagePump;
		private ServerInfo[] m_ServerInfo;
		private IAccount m_Account;
		private Mobile m_Mobile;
		private CityInfo[] m_CityInfo;
		private List<Gump> m_Gumps;
		private List<HuePicker> m_HuePickers;
		private List<IMenu> m_Menus;
		private readonly List<SecureTrade> m_Trades;
		private bool m_CompressionEnabled;
		private readonly string m_ToString;
		private ClientVersion m_Version;
		private bool m_BlockAllPackets;

		private readonly DateTime m_ConnectedOn;

		public DateTime ConnectedOn { get { return m_ConnectedOn; } }

		public TimeSpan ConnectedFor { get { return (DateTime.UtcNow - m_ConnectedOn); } }

		internal int m_Seed;
		internal int m_AuthID;

		public IPAddress Address { get { return m_Address; } }

		private ClientFlags m_Flags;

		private static bool m_Paused;

		[Flags]
		private enum AsyncState
		{
			Pending = 0x01,
			Paused = 0x02
		}

		private AsyncState m_AsyncState;
		private readonly object m_AsyncLock = new object();

		private IPacketEncoder m_Encoder;

		public IPacketEncoder PacketEncoder { get { return m_Encoder; } set { m_Encoder = value; } }

		private static NetStateCreatedCallback m_CreatedCallback;

		public static NetStateCreatedCallback CreatedCallback { get { return m_CreatedCallback; } set { m_CreatedCallback = value; } }

		public bool SentFirstPacket { get; set; }

		public bool BlockAllPackets { get { return m_BlockAllPackets; } set { m_BlockAllPackets = value; } }

		public ClientFlags Flags { get { return m_Flags; } set { m_Flags = value; } }

		public ClientVersion Version
		{
			get { return m_Version; }
			set
			{
				m_Version = value;

				if (value >= m_Version704565)
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

		private ProtocolChanges _ProtocolChanges;

		private enum ProtocolChanges
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
			Version704565 = Version70331 | NewSecureTrading
		}

		public bool NewSpellbook { get { return ((_ProtocolChanges & ProtocolChanges.NewSpellbook) != 0); } }
		public bool DamagePacket { get { return ((_ProtocolChanges & ProtocolChanges.DamagePacket) != 0); } }
		public bool Unpack { get { return ((_ProtocolChanges & ProtocolChanges.Unpack) != 0); } }
		public bool BuffIcon { get { return ((_ProtocolChanges & ProtocolChanges.BuffIcon) != 0); } }
		public bool NewHaven { get { return ((_ProtocolChanges & ProtocolChanges.NewHaven) != 0); } }
		public bool ContainerGridLines { get { return ((_ProtocolChanges & ProtocolChanges.ContainerGridLines) != 0); } }
		public bool ExtendedSupportedFeatures { get { return ((_ProtocolChanges & ProtocolChanges.ExtendedSupportedFeatures) != 0); } }
		public bool StygianAbyss { get { return ((_ProtocolChanges & ProtocolChanges.StygianAbyss) != 0); } }
		public bool HighSeas { get { return ((_ProtocolChanges & ProtocolChanges.HighSeas) != 0); } }
		public bool NewCharacterList { get { return ((_ProtocolChanges & ProtocolChanges.NewCharacterList) != 0); } }
		public bool NewCharacterCreation { get { return ((_ProtocolChanges & ProtocolChanges.NewCharacterCreation) != 0); } }
		public bool ExtendedStatus { get { return ((_ProtocolChanges & ProtocolChanges.ExtendedStatus) != 0); } }
		public bool NewMobileIncoming { get { return ((_ProtocolChanges & ProtocolChanges.NewMobileIncoming) != 0); } }
		public bool NewSecureTrading { get { return ((_ProtocolChanges & ProtocolChanges.NewSecureTrading) != 0); } }

		public bool IsUOTDClient { get { return ((m_Flags & ClientFlags.UOTD) != 0 || (m_Version != null && m_Version.Type == ClientType.UOTD)); } }

		public bool IsSAClient { get { return (m_Version != null && m_Version.Type == ClientType.SA); } }

		public List<SecureTrade> Trades { get { return m_Trades; } }

		public void ValidateAllTrades()
		{
			for (int i = m_Trades.Count - 1; i >= 0; --i)
			{
				if (i >= m_Trades.Count)
				{
					continue;
				}

				SecureTrade trade = m_Trades[i];

				if (trade.From.Mobile.Deleted || trade.To.Mobile.Deleted || !trade.From.Mobile.Alive || !trade.To.Mobile.Alive ||
					!trade.From.Mobile.InRange(trade.To.Mobile, 2) || trade.From.Mobile.Map != trade.To.Mobile.Map)
				{
					trade.Cancel();
				}
			}
		}

		public void CancelAllTrades()
		{
			for (int i = m_Trades.Count - 1; i >= 0; --i)
			{
				if (i < m_Trades.Count)
				{
					m_Trades[i].Cancel();
				}
			}
		}

		public void RemoveTrade(SecureTrade trade)
		{
			m_Trades.Remove(trade);
		}

		public SecureTrade FindTrade(Mobile m)
		{
			for (int i = 0; i < m_Trades.Count; ++i)
			{
				SecureTrade trade = m_Trades[i];

				if (trade.From.Mobile == m || trade.To.Mobile == m)
				{
					return trade;
				}
			}

			return null;
		}

		public SecureTradeContainer FindTradeContainer(Mobile m)
		{
			for (int i = 0; i < m_Trades.Count; ++i)
			{
				SecureTrade trade = m_Trades[i];

				SecureTradeInfo from = trade.From;
				SecureTradeInfo to = trade.To;

				if (from.Mobile == m_Mobile && to.Mobile == m)
				{
					return from.Container;
				}
				else if (from.Mobile == m && to.Mobile == m_Mobile)
				{
					return to.Container;
				}
			}

			return null;
		}

		public SecureTradeContainer AddTrade(NetState state)
		{
			SecureTrade newTrade = new SecureTrade(m_Mobile, state.m_Mobile);

			m_Trades.Add(newTrade);
			state.m_Trades.Add(newTrade);

			return newTrade.From.Container;
		}

		public bool CompressionEnabled { get { return m_CompressionEnabled; } set { m_CompressionEnabled = value; } }

		public int Sequence { get; set; }

		public List<Gump> Gumps { get { return m_Gumps; } }

		public List<HuePicker> HuePickers { get { return m_HuePickers; } }

		public List<IMenu> Menus { get { return m_Menus; } }

		private static int m_GumpCap = 512, m_HuePickerCap = 512, m_MenuCap = 512;

		public static int GumpCap { get { return m_GumpCap; } set { m_GumpCap = value; } }

		public static int HuePickerCap { get { return m_HuePickerCap; } set { m_HuePickerCap = value; } }

		public static int MenuCap { get { return m_MenuCap; } set { m_MenuCap = value; } }

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
			if (m_Menus == null)
			{
				m_Menus = new List<IMenu>();
			}

			if (m_Menus.Count < m_MenuCap)
			{
				m_Menus.Add(menu);
			}
			else
			{
				Utility.PushColor(ConsoleColor.DarkRed);
				WriteConsole("Exceeded menu cap, disconnecting...");
				Utility.PopColor();
				Dispose();
			}
		}

		public void RemoveMenu(IMenu menu)
		{
			if (m_Menus != null)
			{
				m_Menus.Remove(menu);
			}
		}

		public void RemoveMenu(int index)
		{
			if (m_Menus != null)
			{
				m_Menus.RemoveAt(index);
			}
		}

		public void ClearMenus()
		{
			if (m_Menus != null)
			{
				m_Menus.Clear();
			}
		}

		public void AddHuePicker(HuePicker huePicker)
		{
			if (m_HuePickers == null)
			{
				m_HuePickers = new List<HuePicker>();
			}

			if (m_HuePickers.Count < m_HuePickerCap)
			{
				m_HuePickers.Add(huePicker);
			}
			else
			{
				Utility.PushColor(ConsoleColor.DarkRed);
				WriteConsole("Exceeded hue picker cap, disconnecting...");
				Utility.PopColor();
				Dispose();
			}
		}

		public void RemoveHuePicker(HuePicker huePicker)
		{
			if (m_HuePickers != null)
			{
				m_HuePickers.Remove(huePicker);
			}
		}

		public void RemoveHuePicker(int index)
		{
			if (m_HuePickers != null)
			{
				m_HuePickers.RemoveAt(index);
			}
		}

		public void ClearHuePickers()
		{
			if (m_HuePickers != null)
			{
				m_HuePickers.Clear();
			}
		}

		public void AddGump(Gump gump)
		{
			if (m_Gumps == null)
			{
				m_Gumps = new List<Gump>();
			}

			if (m_Gumps.Count < m_GumpCap)
			{
				m_Gumps.Add(gump);
			}
			else
			{
				Utility.PushColor(ConsoleColor.DarkRed);
				WriteConsole("Exceeded gump cap, disconnecting...");
				Utility.PopColor();
				Dispose();
			}
		}

		public void RemoveGump(Gump gump)
		{
			if (m_Gumps != null)
			{
				m_Gumps.Remove(gump);
			}
		}

		public void RemoveGump(int index)
		{
			if (m_Gumps != null)
			{
				m_Gumps.RemoveAt(index);
			}
		}

		public void ClearGumps()
		{
			if (m_Gumps != null)
			{
				m_Gumps.Clear();
			}
		}

		public void LaunchBrowser(string url)
		{
			Send(new MessageLocalized(Serial.MinusOne, -1, MessageType.Label, 0x35, 3, 501231, "", ""));
			Send(new LaunchBrowser(url));
		}

		public CityInfo[] CityInfo { get { return m_CityInfo; } set { m_CityInfo = value; } }

		public Mobile Mobile { get { return m_Mobile; } set { m_Mobile = value; } }

		public ServerInfo[] ServerInfo { get { return m_ServerInfo; } set { m_ServerInfo = value; } }

		public IAccount Account { get { return m_Account; } set { m_Account = value; } }

		public override string ToString()
		{
			return m_ToString;
		}

		private static readonly List<NetState> m_Instances = new List<NetState>();

		public static List<NetState> Instances { get { return m_Instances; } }

		private static readonly BufferPool m_ReceiveBufferPool = new BufferPool("Receive", 2048, 2048);

		public NetState(Socket socket, MessagePump messagePump)
		{
			m_Socket = socket;
			m_Buffer = new ByteQueue();
			Seeded = false;
			m_Running = false;
			m_RecvBuffer = m_ReceiveBufferPool.AcquireBuffer();
			m_MessagePump = messagePump;
			m_Gumps = new List<Gump>();
			m_HuePickers = new List<HuePicker>();
			m_Menus = new List<IMenu>();
			m_Trades = new List<SecureTrade>();

			m_SendQueue = new SendQueue();

			m_NextCheckActivity = Core.TickCount + 30000;

			m_Instances.Add(this);

			try
			{
				m_Address = Utility.Intern(((IPEndPoint)m_Socket.RemoteEndPoint).Address);
				m_ToString = m_Address.ToString();
			}
			catch (Exception ex)
			{
				TraceException(ex);
				m_Address = IPAddress.None;
				m_ToString = "(error)";
			}

			m_ConnectedOn = DateTime.UtcNow;

			if (m_CreatedCallback != null)
			{
				m_CreatedCallback(this);
			}
		}

		private bool _sending;
		private readonly object _sendL = new object();

		public virtual void Send(Packet p)
		{
			if (m_Socket == null || m_BlockAllPackets)
			{
				p.OnSend();
				return;
			}

			int length;
			var buffer = p.Compile(m_CompressionEnabled, out length);

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

				if (m_Encoder != null)
				{
					m_Encoder.EncodeOutgoingPacket(this, ref buffer, ref length);
				}

				try
				{
					SendQueue.Gram gram;

					lock (_sendL)
					{
						lock (m_SendQueue)
							gram = m_SendQueue.Enqueue(buffer, length);

						if (gram != null && !_sending)
						{
							_sending = true;
#if NewAsyncSockets
						m_SendEventArgs.SetBuffer( gram.Buffer, 0, gram.Length );
						Send_Start();
#else
							try
							{
								m_Socket.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, m_OnSend, m_Socket);
							}
							catch (Exception ex)
							{
								TraceException(ex);
								Dispose(false);
							}
#endif
						}
					}
				}
				catch (CapacityExceededException)
				{
					Utility.PushColor(ConsoleColor.DarkRed);
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
				Utility.PushColor(ConsoleColor.DarkRed);
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

#if NewAsyncSockets
		public void Start() {
			m_ReceiveEventArgs = new SocketAsyncEventArgs();
			m_ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>( Receive_Completion );
			m_ReceiveEventArgs.SetBuffer( m_RecvBuffer, 0, m_RecvBuffer.Length );

			m_SendEventArgs = new SocketAsyncEventArgs();
			m_SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>( Send_Completion );

			m_Running = true;

			if ( m_Socket == null || m_Paused ) {
				return;
			}

			Receive_Start();
		}

		private void Receive_Start()
		{
			try {
				bool result = false;

				do {
					lock ( m_AsyncLock ) {
						if ( ( m_AsyncState & ( AsyncState.Pending | AsyncState.Paused ) ) == 0 ) {
							m_AsyncState |= AsyncState.Pending;
							result = !m_Socket.ReceiveAsync( m_ReceiveEventArgs );

							if ( result )
								Receive_Process( m_ReceiveEventArgs );
						}
					}
				} while ( result );
			} catch ( Exception ex ) {
				TraceException( ex );
				Dispose( false );
			}
		}

		private void Receive_Completion( object sender, SocketAsyncEventArgs e )
		{
			Receive_Process( e );

			if ( !m_Disposing )
				Receive_Start();
		}

		private void Receive_Process( SocketAsyncEventArgs e )
		{
			int byteCount = e.BytesTransferred;

			if ( e.SocketError != SocketError.Success || byteCount <= 0 ) {
				Dispose( false );
				return;
			} else if ( m_Disposing ) {
				return;
			}

			m_NextCheckActivity = Core.TickCount + 90000;

			byte[] buffer = m_RecvBuffer;

			if ( m_Encoder != null )
				m_Encoder.DecodeIncomingPacket( this, ref buffer, ref byteCount );

			lock ( m_Buffer )
				m_Buffer.Enqueue( buffer, 0, byteCount );

			m_MessagePump.OnReceive( this );

			lock ( m_AsyncLock ) {
				m_AsyncState &= ~AsyncState.Pending;
			}
		}

		private void Send_Start()
		{
			try {
				bool result = false;

				do {
					result = !m_Socket.SendAsync( m_SendEventArgs );

					if ( result )
						Send_Process( m_SendEventArgs );
				} while ( result ); 
			} catch ( Exception ex ) {
				TraceException( ex );
				Dispose( false );
			}
		}

		private void Send_Completion( object sender, SocketAsyncEventArgs e )
		{
			Send_Process( e );

			if ( m_Disposing )
				return;

			if ( m_CoalesceSleep >= 0 ) {
				Thread.Sleep( m_CoalesceSleep );
			}

			SendQueue.Gram gram;

			lock ( m_SendQueue ) {
				gram = m_SendQueue.Dequeue();

				if (gram == null && m_SendQueue.IsFlushReady)
					gram = m_SendQueue.CheckFlushReady();
			}

			if ( gram != null ) {
				m_SendEventArgs.SetBuffer( gram.Buffer, 0, gram.Length );
				Send_Start();
			} else {
				lock (_sendL)
					_sending = false;
			}
		}

		private void Send_Process( SocketAsyncEventArgs e )
		{
			int bytes = e.BytesTransferred;

			if ( e.SocketError != SocketError.Success || bytes <= 0 ) {
				Dispose( false );
				return;
			}

			m_NextCheckActivity = Core.TickCount + 90000;
		}

		public static void Pause() {
			m_Paused = true;

			for ( int i = 0; i < m_Instances.Count; ++i ) {
				NetState ns = m_Instances[i];

				lock ( ns.m_AsyncLock ) {
					ns.m_AsyncState |= AsyncState.Paused;
				}
			}
		}

		public static void Resume() {
			m_Paused = false;

			for ( int i = 0; i < m_Instances.Count; ++i ) {
				NetState ns = m_Instances[i];

				if ( ns.m_Socket == null ) {
					continue;
				}

				lock ( ns.m_AsyncLock ) {
					ns.m_AsyncState &= ~AsyncState.Paused;

					if ( ( ns.m_AsyncState & AsyncState.Pending ) == 0 )
						ns.Receive_Start();
				}
			}
		}

		public bool Flush() {
			if ( m_Socket == null )
					return false;

			lock (_sendL) {
				if (_sending)
				return false;

			SendQueue.Gram gram;

			lock ( m_SendQueue ) {
					if (!m_SendQueue.IsFlushReady)
						return false;

				gram = m_SendQueue.CheckFlushReady();
			}

			if ( gram != null ) {
					_sending = true;
				m_SendEventArgs.SetBuffer( gram.Buffer, 0, gram.Length );
				Send_Start();
			}
			}

			return false;
		}

        #else

		public void Start()
		{
			m_OnReceive = OnReceive;
			m_OnSend = OnSend;

			m_Running = true;

			if (m_Socket == null || m_Paused)
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

			m_Socket.BeginReceive(m_RecvBuffer, 0, m_RecvBuffer.Length, SocketFlags.None, m_OnReceive, m_Socket);
		}

		private void OnReceive(IAsyncResult asyncResult)
		{
			Socket s = (Socket)asyncResult.AsyncState;

			try
			{
				int byteCount = s.EndReceive(asyncResult);

				if (byteCount > 0)
				{
					m_NextCheckActivity = Core.TickCount + 90000;

					var buffer = m_RecvBuffer;

					if (m_Encoder != null)
					{
						m_Encoder.DecodeIncomingPacket(this, ref buffer, ref byteCount);
					}

					lock (m_Buffer)
						m_Buffer.Enqueue(buffer, 0, byteCount);

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
					lock (_sendL)
						_sending = false;
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

			for (int i = 0; i < m_Instances.Count; ++i)
			{
				NetState ns = m_Instances[i];

				lock (ns.m_AsyncLock)
				{
					ns.m_AsyncState |= AsyncState.Paused;
				}
			}
		}

		public static void Resume()
		{
			m_Paused = false;

			for (int i = 0; i < m_Instances.Count; ++i)
			{
				NetState ns = m_Instances[i];

				if (ns.m_Socket == null)
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
			if (m_Socket == null)
			{
				return false;
			}

			lock (_sendL)
			{
				if (_sending)
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
						_sending = true;
						m_Socket.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, m_OnSend, m_Socket);
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
#endif

		public PacketHandler GetHandler(int packetID)
		{
			if (ContainerGridLines)
			{
				return PacketHandlers.Get6017Handler(packetID);
			}
			else
			{
				return PacketHandlers.GetHandler(packetID);
			}
		}

		public static void FlushAll()
		{
			if (m_Instances.Count > 1024)
			{
				Parallel.ForEach(m_Instances, ns => ns.Flush());
			}
			else
			{
				for (int i = 0; i < m_Instances.Count; ++i)
				{
					m_Instances[i].Flush();
				}
			}
		}

		private static int m_CoalesceSleep = -1;

		public static int CoalesceSleep { get { return m_CoalesceSleep; } set { m_CoalesceSleep = value; } }

		private long m_NextCheckActivity;

		public void CheckAlive(long curTicks)
		{
			if (m_Socket == null)
			{
				return;
			}

			if (m_NextCheckActivity - curTicks >= 0)
			{
				return;
			}

			Utility.PushColor(ConsoleColor.DarkRed);
			Console.WriteLine("Client: {0}: Disconnecting due to inactivity...", this);
			Utility.PopColor();

			Dispose();
			return;
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

		public void Dispose()
		{
			Dispose(true);
		}

		public virtual void Dispose(bool flush)
		{
			if (m_Socket == null || m_Disposing)
			{
				return;
			}

			m_Disposing = true;

			if (flush)
			{
				flush = Flush();
			}

			try
			{
				m_Socket.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException ex)
			{
				TraceException(ex);
			}

			try
			{
				m_Socket.Close();
			}
			catch (SocketException ex)
			{
				TraceException(ex);
			}

			if (m_RecvBuffer != null)
			{
				lock (m_ReceiveBufferPool)
					m_ReceiveBufferPool.ReleaseBuffer(m_RecvBuffer);
			}

			m_Socket = null;

			m_Buffer = null;
			m_RecvBuffer = null;

#if NewAsyncSockets
			m_ReceiveEventArgs = null;
			m_SendEventArgs = null;
            #else
			m_OnReceive = null;
			m_OnSend = null;
#endif

			m_Running = false;

			lock (m_Disposed)
				m_Disposed.Enqueue(this);

			lock (m_SendQueue)
				if ( /*!flush &&*/ !m_SendQueue.IsEmpty)
				{
					m_SendQueue.Clear();
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

				if (m_Instances.Count >= 1024)
				{
					Parallel.ForEach(m_Instances, ns => ns.CheckAlive(curTicks));
				}
				else
				{
					for (int i = 0; i < m_Instances.Count; ++i)
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
				int breakout = 0;

				while (breakout < 200 && m_Disposed.Count > 0)
				{
					++breakout;
					NetState ns = m_Disposed.Dequeue();

					Mobile m = ns.m_Mobile;
					IAccount a = ns.m_Account;

					if (m != null)
					{
						m.NetState = null;
						ns.m_Mobile = null;
					}

					ns.m_Gumps.Clear();
					ns.m_Menus.Clear();
					ns.m_HuePickers.Clear();
					ns.m_Account = null;
					ns.m_ServerInfo = null;
					ns.m_CityInfo = null;

					m_Instances.Remove(ns);

					Utility.PushColor(ConsoleColor.DarkRed);

					if (a != null)
					{
						ns.WriteConsole("Disconnected. [{0} Online] [{1}]", m_Instances.Count, a);
					}
					else
					{
						ns.WriteConsole("Disconnected. [{0} Online]", m_Instances.Count);
					}

					Utility.PopColor();
				}
			}
		}

		public bool Running { get { return m_Running; } }

		public bool Seeded { get; set; }

		public Socket Socket { get { return m_Socket; } }

		public ByteQueue Buffer { get { return m_Buffer; } }

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

		public Expansion Expansion { get { return (Expansion)ExpansionInfo.ID; } }

		public bool SupportsExpansion(ExpansionInfo info, bool checkCoreExpansion)
		{
			if (info == null || (checkCoreExpansion && (int)Core.Expansion < info.ID))
			{
				return false;
			}

			if (info.RequiredClient != null)
			{
				return (Version >= info.RequiredClient);
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

			return m_ToString.CompareTo(other.m_ToString);
		}
	}
}