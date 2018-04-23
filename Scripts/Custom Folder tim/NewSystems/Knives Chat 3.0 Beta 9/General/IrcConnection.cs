using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections;
using Server;
using Server.Guilds;
using Server.Network;

namespace Knives.Chat3
{
	public enum IrcColor
	{
		White = 0,
		Black = 1,
		Blue = 2,
		Green = 3,
		LightRed = 4,
		Brown = 5,
		Purple = 6,
		Orange = 7,
		Yellow = 8,
		LightGreen = 9,
		Cyan = 10,
		LightCyan = 11,
		LightBlue = 12,
		Pink = 13,
		Grey = 14,
		LightGrey = 15,
	};

	public class IrcConnection
	{
		private static IrcConnection s_Connection = new IrcConnection();

		public static IrcConnection Connection{ get{ return s_Connection; } }

		private TcpClient c_Tcp;
		private Thread c_Thread;
		private StreamReader c_Reader;
		private StreamWriter c_Writer;
		private bool c_Connecting, c_Connected;
		private int c_Attempts;
		private DateTime c_LastPong;
        private DateTime c_NextStatus = DateTime.Now;
        private Server.Timer c_ConnectTimer;

		public bool Connecting{ get{ return c_Connecting; } }
		public bool Connected{ get{ return c_Connected; } }
		public bool HasMoreAttempts{ get{ return c_Attempts <= Data.IrcMaxAttempts; } }

        public string Status
        {
            get
            {
                return "Server: RunUO 2.0   Creatures: " + World.Mobiles.Count + "   Items: " + World.Items.Count + "   Guilds: " + BaseGuild.List.Count + "   Players: " + NetState.Instances.Count;
            }
        }

		public IrcConnection()
		{
		}

		public void Connect( Mobile m )
		{
			Data data = Data.GetData( m );

			if ( c_Connecting )
			{
				m.SendMessage( data.SystemC, General.Local(102) );
				return;
			}

			if ( c_Connected )
			{
				m.SendMessage( data.SystemC, General.Local(103) );
				return;
			}

			Connect();
		}

		public void Connect()
		{
			new Thread( new ThreadStart( ConnectTcp ) ).Start();
		}

		public void CancelConnect()
		{
			c_Attempts = Data.IrcMaxAttempts;
		}

		private void Reconnect()
		{
			c_Attempts++;

			if ( !HasMoreAttempts )
			{
				c_Attempts = 1;
				BroadcastSystem( General.Local(104) );
				return;
			}

			BroadcastSystem( General.Local(105) + c_Attempts );

			Connect();
		}

		private void ConnectTcp()
		{
			try{ c_Tcp = new TcpClient( Data.IrcServer, Data.IrcPort ); }
			catch
			{
				BroadcastSystem( General.Local(106) );

				Reconnect();

				return;
			}

			ConnectStream();
		}

		private void ConnectStream()
		{
            try
            {
                c_Connecting = true;
                c_ConnectTimer = Server.Timer.DelayCall(TimeSpan.FromSeconds(30), new Server.TimerCallback(TimerFail));
                c_LastPong = DateTime.Now;

                c_Reader = new StreamReader(c_Tcp.GetStream(), System.Text.Encoding.Default);
                c_Writer = new StreamWriter(c_Tcp.GetStream(), System.Text.Encoding.Default);

                BroadcastSystem(General.Local(107));

                SendMessage(String.Format("USER {0} 1 * :Hello!", Data.IrcNick));
                SendMessage(String.Format("NICK {0}", Data.IrcNick));

                c_Thread = new Thread(new ThreadStart(ReadStream));
                c_Thread.Start();

                Server.Timer.DelayCall(TimeSpan.FromSeconds(15.0), new Server.TimerCallback(Names));

                foreach (Data data in Data.Datas.Values)
                    if (data.Mobile.HasGump(typeof(IrcGump)))
                        GumpPlus.RefreshGump(data.Mobile, typeof(IrcGump));

            }
            catch(Exception e)
            { 
                Errors.Report(General.Local(266), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }

        private void TimerFail()
        {
            if (!c_Connecting || c_Connected)
                return;

            c_Connecting = false;
            BroadcastSystem("IRC connection attempt timed out.");

            try
            {
                if (c_Thread != null)
                    c_Thread.Abort();
            }
            catch
            {
            }

            c_Thread = null;

            foreach (Data data in Data.Datas.Values)
                if (data.Mobile.HasGump(typeof(IrcGump)))
                    GumpPlus.RefreshGump(data.Mobile, typeof(IrcGump));
        }

        private void Names()
        {
            if (c_Connected)
                SendMessage("NAMES " + Data.IrcRoom);
            else
                return;

            Server.Timer.DelayCall( TimeSpan.FromSeconds( 15.0 ), new Server.TimerCallback( Names ) );
        }

		private void PingPong(string str)
		{
            if (!c_Connecting && !c_Connected)
                return;

            if (str.IndexOf("PING") != -1)
                str = str.Replace("PING", "PONG");
            else
                str = str.Replace("PONG", "PING");
            
			SendMessage( str );
		}

		public void SendMessage( string msg )
		{
			try{

			BroadcastRaw( msg );

			c_Writer.WriteLine( msg );
			c_Writer.Flush();

			}catch{ Disconnect(); }
		}

		public void SendUserMessage( Mobile m, string msg )
		{
			if ( !Connected )
				return;

            msg = OutParse(m, m.RawName + ": " + msg);
			s_Connection.SendMessage( String.Format( "PRIVMSG {0} : {1}", Data.IrcRoom, msg  ));

            if (msg.ToLower().IndexOf("!status") != -1 && c_NextStatus < DateTime.Now)
            {
                c_NextStatus = DateTime.Now + TimeSpan.FromSeconds(15);
                s_Connection.SendMessage(String.Format("PRIVMSG {0} : {1}", Data.IrcRoom, Status));
                BroadcastSystem(Status);
            }

            BroadcastRaw(String.Format("PRIVMSG {0} : {1}", Data.IrcRoom, msg));
		}

		private string OutParse( Mobile m, string str )
		{
			if ( m.AccessLevel != AccessLevel.Player )
				return str = '\x0003' + ((int)Data.IrcStaffColor).ToString() + str;

			return str;
		}

		private void BroadcastSystem( string msg )
		{
            if (Channel.GetByType(typeof(Irc)) == null)
                return;

            Channel.GetByType(typeof(Irc)).BroadcastSystem(msg);
		}

		private void Broadcast( string name, string msg )
		{
            if (Channel.GetByType(typeof(Irc)) == null)
                return;

            ((Irc)Channel.GetByType(typeof(Irc))).Broadcast(name, msg);
		}

		private void Broadcast( Mobile m, string msg )
		{
		}

		private void BroadcastRaw( string msg )
		{
			foreach( Data data in Data.Datas.Values )
				if ( data.IrcRaw )
					data.Mobile.SendMessage( data.SystemC, "RAW: " + msg );
		}

		private void ReadStream()
		{
            try
            {

                string input = "";

                while (c_Thread.IsAlive)
                {
                    input = c_Reader.ReadLine();

                    if (input == null)
                        break;

                    HandleInput(input);
                }

                if (c_Connected)
                    Disconnect();

            }
            catch { if (c_Connected) Disconnect(); }
        }

        private void HandleInput(string str)
        {
            try
            {
                if (str == null)
                    return;

                BroadcastRaw(str);

                if (str.IndexOf("PONG") != -1 || str.IndexOf("PING") != -1)
                {
                    PingPong(str);
                    return;
                }

                if (str.IndexOf("353") != -1)
                {
                    BroadcastRaw(General.Local(109));

                    int index = str.ToLower().IndexOf(Data.IrcRoom.ToLower()) + Data.IrcRoom.Length + 2;

                    if (index == 1)
                        return;

                    string strList = str.Substring(index, str.Length - index);

                    string[] strs = strList.Trim().Split(' ');

                    Data.IrcList.Clear();
                    Data.IrcList.AddRange(strs);
                    Data.IrcList.Remove(Data.IrcNick);
                }

                if (str.IndexOf("001") != -1 && c_Connecting)
                {
                    c_Connected = true;
                    c_Connecting = false;

                    if (c_ConnectTimer != null)
                        c_ConnectTimer.Stop();

                    BroadcastSystem(General.Local(108));
                    c_Attempts = 1;

                    SendMessage(String.Format("JOIN {0}", Data.IrcRoom));

                    foreach (Data data in Data.Datas.Values)
                        if (data.Mobile.HasGump(typeof(IrcGump)))
                            GumpPlus.RefreshGump(data.Mobile, typeof(IrcGump));
                }

                if (str.Length > 300)
                    return;

                if (str.IndexOf("PRIVMSG") != -1)
                {
                    string parOne = str.Substring(1, str.IndexOf("!") - 1);

                    string parThree = str.Substring(str.IndexOf("!") + 1, str.Length - str.IndexOf("!") - (str.Length - str.IndexOf("PRIVMSG")) - 1);

                    int index = 0;

                    index = str.ToLower().IndexOf(Data.IrcRoom.ToLower()) + Data.IrcRoom.Length + 2;

                    if (index == 1)
                        return;

                    string parTwo = str.Substring(index, str.Length - index);

                    if (parTwo.IndexOf("ACTION") != -1)
                    {
                        index = parTwo.IndexOf("ACTION") + 7;
                        parTwo = parTwo.Substring(index, parTwo.Length - index);
                        str = String.Format("<{0}> {1} {2}", Data.IrcRoom, parOne, parTwo);
                    }
                    else
                        str = String.Format("<{0}> {1}: {2}", Data.IrcRoom, parOne, parTwo);

                    Broadcast(parOne, str);

                    if (str.ToLower().IndexOf("!status") != -1 && c_NextStatus < DateTime.Now)
                    {
                        c_NextStatus = DateTime.Now + TimeSpan.FromSeconds(15);
                        s_Connection.SendMessage(String.Format("PRIVMSG {0} : {1}", Data.IrcRoom, Status));
                        BroadcastSystem(Status);
                    }
                }
            }
            catch (Exception e)
            { 
                Errors.Report(General.Local(267), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Message);
            }
        }

		public void Disconnect()
		{
			Disconnect( true );
		}

        public void Disconnect(bool reconn)
        {
            try
            {
                if (c_Connected)
                    BroadcastSystem(General.Local(110));

                c_Connected = false;
                c_Connecting = false;

                if (c_Thread != null)
                {
                    try { c_Thread.Abort(); }
                    catch { }
                    c_Thread = null;
                }
                if (c_Reader != null)
                    c_Reader.Close();
                if (c_Writer != null)
                    c_Writer.Close();
                if (c_Tcp != null)
                    c_Tcp.Close();

                if (reconn)
                    Reconnect();

                foreach (Data data in Data.Datas.Values)
                    if (data.Mobile.HasGump(typeof(IrcGump)))
                        GumpPlus.RefreshGump(data.Mobile, typeof(IrcGump));
            }
            catch (Exception e)
            {
                Errors.Report(General.Local(268), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}