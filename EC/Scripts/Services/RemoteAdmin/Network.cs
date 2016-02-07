using System;
using System.Collections;
using System.Text;
using Server.Accounting;
using Server.Network;

namespace Server.RemoteAdmin
{
    public class AdminNetwork
    {
        private const string ProtocolVersion = "2";

        private static ArrayList m_Auth = new ArrayList();
        private static bool m_NewLine = true;
        private static readonly StringBuilder m_ConsoleData = new StringBuilder();

        private const string DateFormat = "MMMM dd hh:mm:ss.f tt";

        public static void Configure()
        {
            PacketHandlers.Register(0xF1, 0, false, new OnPacketReceive(OnReceive));

            #if !MONO
            Core.MultiConsoleOut.Add(new EventTextWriter(new EventTextWriter.OnConsoleChar(OnConsoleChar), new EventTextWriter.OnConsoleLine(OnConsoleLine), new EventTextWriter.OnConsoleStr(OnConsoleString)));
            #endif
            Timer.DelayCall(TimeSpan.FromMinutes(2.5), TimeSpan.FromMinutes(2.5), new TimerCallback(CleanUp));
        }

        public static void OnConsoleString(string str)
        {
            string outStr;
            if (m_NewLine)
            {
                outStr = String.Format("[{0}]: {1}", DateTime.UtcNow.ToString(DateFormat), str);
                m_NewLine = false;
            }
            else
            {
                outStr = str;
            }

            m_ConsoleData.Append(outStr);
            RoughTrimConsoleData();

            SendToAll(outStr);
        }

        public static void OnConsoleChar(char ch)
        {
            if (m_NewLine)
            {
                string outStr;
                outStr = String.Format("[{0}]: {1}", DateTime.UtcNow.ToString(DateFormat), ch);

                m_ConsoleData.Append(outStr);
                SendToAll(outStr);

                m_NewLine = false;
            }
            else
            {
                m_ConsoleData.Append(ch);
                SendToAll(ch);
            }

            RoughTrimConsoleData();
        }

        public static void OnConsoleLine(string line)
        {
            string outStr;
            if (m_NewLine)
                outStr = String.Format("[{0}]: {1}{2}", DateTime.UtcNow.ToString(DateFormat), line, Console.Out.NewLine);
            else
                outStr = String.Format("{0}{1}", line, Console.Out.NewLine);

            m_ConsoleData.Append(outStr);
            RoughTrimConsoleData();

            SendToAll(outStr);

            m_NewLine = true;
        }

        static void SendToAll(string outStr)
        {
            SendToAll(new ConsoleData(outStr));
        }

        static void SendToAll(char ch)
        {
            SendToAll(new ConsoleData(ch));
        }

        static void SendToAll(ConsoleData packet)
        {
            packet.Acquire();
            for (int i = 0; i < m_Auth.Count; i++)
                ((NetState)m_Auth[i]).Send(packet);
            packet.Release();
        }

        static void RoughTrimConsoleData()
        {
            if (m_ConsoleData.Length >= 4096)
                m_ConsoleData.Remove(0, 2048);
        }

        static void TightTrimConsoleData()
        {
            if (m_ConsoleData.Length > 1024)
                m_ConsoleData.Remove(0, m_ConsoleData.Length - 1024);
        }

        public static void OnReceive(NetState state, PacketReader pvSrc)
        {
            byte cmd = pvSrc.ReadByte();
            if (cmd == 0x02)
            {
                Authenticate(state, pvSrc);
            }
            else if (cmd == 0xFE)
            {
                state.Send(new CompactServerInfo());
                state.Dispose();
            }
            else if (cmd == 0xFF)
            {
                string statStr = String.Format(", Name={0}, Age={1}, Clients={2}, Items={3}, Chars={4}, Mem={5}K, Ver={6}", Server.Misc.ServerList.ServerName, (int)(DateTime.UtcNow - Server.Items.Clock.ServerStart).TotalHours, NetState.Instances.Count, World.Items.Count, World.Mobiles.Count, (int)(System.GC.GetTotalMemory(false) / 1024), ProtocolVersion);
                state.Send(new UOGInfo(statStr));
                state.Dispose();
            }
            else if (!IsAuth(state))
            {
                Console.WriteLine("ADMIN: Unauthorized packet from {0}, disconnecting", state);
                Disconnect(state);
            }
            else
            {
                if (!RemoteAdminHandlers.Handle(cmd, state, pvSrc))
                    Disconnect(state);
            }
        }

        private static void DelayedDisconnect(NetState state)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(15.0), new TimerStateCallback(Disconnect), state);
        }
		
        private static void Disconnect(object state)
        {
            m_Auth.Remove(state);
            ((NetState)state).Dispose();
        }
		
        public static void Authenticate(NetState state, PacketReader pvSrc)
        {
            string user = pvSrc.ReadString(30);
            string pw = pvSrc.ReadString(30);

            Account a = Accounts.GetAccount(user) as Account;
            if (a == null)
            {
                state.Send(new Login(LoginResponse.NoUser));
                Console.WriteLine("ADMIN: Invalid username '{0}' from {1}", user, state);
                DelayedDisconnect(state);
            }
            else if (!a.HasAccess(state))
            {
                state.Send(new Login(LoginResponse.BadIP));
                Console.WriteLine("ADMIN: Access to '{0}' from {1} denied.", user, state);
                DelayedDisconnect(state);
            }
            else if (!a.CheckPassword(pw))
            {
                state.Send(new Login(LoginResponse.BadPass));
                Console.WriteLine("ADMIN: Invalid password for user '{0}' from {1}", user, state);
                DelayedDisconnect(state);
            }
            else if (a.AccessLevel < AccessLevel.Administrator || a.Banned)
            {
                Console.WriteLine("ADMIN: Account '{0}' does not have admin access. Connection Denied.", user);
                state.Send(new Login(LoginResponse.NoAccess)); 
                DelayedDisconnect(state);
            }
            else
            {
                Console.WriteLine("ADMIN: Access granted to '{0}' from {1}", user, state);
                state.Account = a;
                a.LogAccess(state);
                a.LastLogin = DateTime.UtcNow;

                state.Send(new Login(LoginResponse.OK));
                TightTrimConsoleData();
                state.Send(Compress(new ConsoleData(m_ConsoleData.ToString())));
                m_Auth.Add(state);
            }
        }

        public static bool IsAuth(NetState state)
        {
            return m_Auth.Contains(state);
        }

        private static void CleanUp()
        { //remove dead instances from m_Auth
            ArrayList list = new ArrayList();
            for (int i = 0; i < m_Auth.Count; i++)
            {
                NetState ns = (NetState)m_Auth[i];
                if (ns.Running)
                    list.Add(ns);
            }

            m_Auth = list;
        }

        public static Packet Compress(Packet p)
        {
            int length;
            byte[] source = p.Compile(false, out length);

            if (length > 100 && length < 60000)
            {
                byte[] dest = new byte[(int)(length * 1.001) + 10];
                int destSize = dest.Length;

                ZLibError error = Compression.Pack(dest, ref destSize, source, length, ZLibQuality.Default);

                if (error != ZLibError.Okay)
                {
                    Console.WriteLine("WARNING: Unable to compress admin packet, zlib error: {0}", error);
                    return p;
                }
                else
                {
                    return new AdminCompressedPacket(dest, destSize, length);
                }
            }
            else
            {
                return p;
            }
        }
    }
	
    public class EventTextWriter : System.IO.TextWriter
    {
        public delegate void OnConsoleChar(char ch);
        public delegate void OnConsoleLine(string line);
        public delegate void OnConsoleStr(string str);

        private readonly OnConsoleChar m_OnChar;
        private readonly OnConsoleLine m_OnLine;
        private readonly OnConsoleStr m_OnStr;

        public EventTextWriter(OnConsoleChar onChar, OnConsoleLine onLine, OnConsoleStr onStr)
        {
            this.m_OnChar = onChar;
            this.m_OnLine = onLine;
            this.m_OnStr = onStr;
        }

        public override void Write(char ch)
        {
            if (this.m_OnChar != null)
                this.m_OnChar(ch);
        }

        public override void Write(string str)
        {
            if (this.m_OnStr != null)
                this.m_OnStr(str);
        }

        public override void WriteLine(string line)
        {
            if (this.m_OnLine != null)
                this.m_OnLine(line);
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.ASCII;
            }
        }
    }
}