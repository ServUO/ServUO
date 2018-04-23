// Get name lists for online players not hidden

using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server;

namespace Knives.Chat3
{
    public class MultiConnection
    {
        private static MultiConnection s_Connection = new MultiConnection();
        public static MultiConnection Connection { get { return s_Connection; } }

        private Socket c_Master, c_Slave;
        private ArrayList c_Clients = new ArrayList();
        private Hashtable c_Names = new Hashtable();
        private bool c_Server;

        private bool c_Connecting, c_Connected;
        private Server.Timer c_ConnectTimer;

        public Hashtable Names { get { return c_Names; } }
        public bool Connecting { get { return c_Connecting; } }
        public bool Connected { get { return c_Connected; } }

        public MultiConnection()
        {
        }

        public void Block(string str)
        {
            foreach (Socket s in c_Clients)
                if (c_Names[s].ToString() == str)
                {
                    c_Names.Remove(s);
                    c_Clients.Remove(s);
                    s.Close();
                }
        }

        public void ConnectMaster()
        {
            if (c_Connected && c_Server)
                return;

            try
            {
                c_Server = true;
                //Console.WriteLine("CM Close Slave");
                CloseSlave();

                c_Master = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                c_Master.Bind(new IPEndPoint(IPAddress.Any, Data.MultiPort));
                c_Master.Listen(4);
                c_Master.BeginAccept(new AsyncCallback(OnClientConnect), null);
                //Console.WriteLine("Started");

                c_Connecting = false;
                c_Connected = true;
            }
            catch (Exception e)
            {
                c_Server = false;
                //Console.WriteLine(e.Message);
                //Console.WriteLine(e.StackTrace);
            }
        }

        public void CloseMaster()
        {
            if (c_Master != null)
                c_Master.Close();

            foreach (Socket sok in c_Clients)
                sok.Close();

            c_Clients.Clear();
            c_Names.Clear();

            c_Connecting = false;
            c_Connected = false;
        }

        public void ConnectSlave()
        {
            if (c_Connected && !c_Server)
                return;

            CloseMaster();
            new Thread(new ThreadStart(ConnectToMaster)).Start();
        }

        private void ConnectToMaster()
        {
            try
            {
                c_Slave = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                c_Slave.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(Data.MultiServer), Data.MultiPort));
            }
            catch(Exception e)
            {
                BroadcastSystem(General.Local(288));
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }

            BeginReceive();
        }

        private void BeginReceive()
        {
            try
            {
                BroadcastSystem(General.Local(289));

                byte[] msg = System.Text.Encoding.ASCII.GetBytes("    " + Server.Misc.ServerList.ServerName);

                c_Slave.Send(msg, 0, msg.Length, SocketFlags.None);

                c_Connected = true;
                c_Connecting = false;

                WaitForData(c_Slave);

                foreach (Data data in Data.Datas.Values)
                    if (data.Mobile.HasGump(typeof(MultiGump)))
                        GumpPlus.RefreshGump(data.Mobile, typeof(MultiGump));
            }
            catch (Exception e)
            {
                Errors.Report("Error opening stream for slave.");
                //Console.WriteLine(e.Message);
                //Console.WriteLine(e.StackTrace);
            }
        }

        private void HandleInput(string str)
        {
            Broadcast(str);
            //Console.WriteLine(str);
        }

        public void CloseSlave()
        {
            try
            {
                if (c_Connected)
                    BroadcastSystem(General.Local(290));

                c_Connected = false;
                c_Connecting = false;

                if (c_Slave != null)
                    c_Slave.Close();

                foreach (Data data in Data.Datas.Values)
                    if (data.Mobile.HasGump(typeof(MultiGump)))
                        GumpPlus.RefreshGump(data.Mobile, typeof(MultiGump));
            }
            catch (Exception e)
            {
                Errors.Report("Error disconnecting slave.");
                //Console.WriteLine(e.Message);
                //Console.WriteLine(e.Source);
                //Console.WriteLine(e.StackTrace);
            }
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                //Console.WriteLine("Connection Made");
                Socket sok = c_Master.EndAccept(asyn);
                c_Clients.Add(sok);
                //sok.Close();
                WaitForData(sok);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Connection Died");
                //Console.WriteLine(e.Message);
                //Console.WriteLine(e.StackTrace);
            }
        }

        public void WaitForData(Socket sok)
        {
            try
            {
                MultiPacket pak = new MultiPacket();
                pak.Socket = sok;
                //Console.WriteLine("Waiting for input.");
                sok.BeginReceive(pak.Buffer, 0, pak.Buffer.Length, SocketFlags.None, new AsyncCallback(OnDataReceived), pak);
            }
            catch (Exception e)
            {
                if (c_Server)
                {
                    if(c_Names[sok] != null)
                        BroadcastSystem(c_Names[sok].ToString() + General.Local(291));
                    c_Clients.Remove(sok);
                    c_Names.Remove(sok);
                }
                else
                {
                    BroadcastSystem(General.Local(290));
                    CloseSlave();
                }
            }
        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                MultiPacket pak = (MultiPacket)asyn.AsyncState;

                byte[] buffer = new byte[1024];
                int count = pak.Socket.Receive(buffer);
                char[] chars = new char[count];
                System.Text.Encoding.ASCII.GetDecoder().GetChars(buffer, 0, count, chars, 0);
                string input = new System.String(chars).Trim();

                //Console.WriteLine(pak.Socket + " " + input);

                if (c_Server)
                {
                    if (input.ToLower().IndexOf("<") != 0)
                    {
                        c_Names[pak.Socket] = input;
                        BroadcastSystem(input + General.Local(292));
                    }
                    else
                    {
                        Broadcast(input);
                        SendMaster(input);
                    }
                }
                else
                {
                    HandleInput(input);
                }

                WaitForData(pak.Socket);
            }
            catch (Exception e)
            {
                if (c_Server)
                    CloseMaster();
                else
                    CloseSlave();

                //Console.WriteLine(e.Message);
                //Console.WriteLine(e.StackTrace);
            }
        }

        public void Broadcast(string str)
        {
            if (Channel.GetByType(typeof(Multi)) == null)
                return;

            ((Multi)Channel.GetByType(typeof(Multi))).Broadcast(str);
        }

        public void SendMaster(string str)
        {
            if (!c_Server || c_Master == null)
                return;
                
            try
            {
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("   " + str);

                foreach (Socket sok in c_Clients)
                    sok.Send(msg, 0, msg.Length, SocketFlags.None);
            }
            catch
            {
                Errors.Report("Error in sending to clients.");
            }
        }

        private void BroadcastSystem(string msg)
        {
            if (Channel.GetByType(typeof(Multi)) == null)
                return;

            Channel.GetByType(typeof(Multi)).BroadcastSystem(msg);
        }

        public void SendMessage(Mobile m, string str)
        {
            //Console.WriteLine("Sending: {0} {1}", c_Server, str);

            if(c_Server)
            {
                SendMaster("<" + Server.Misc.ServerList.ServerName + "> " + m.RawName + ": " + str);
                Broadcast("<" + Server.Misc.ServerList.ServerName + "> " + m.RawName + ": " + str);
            }
            else
            {
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("    <" + Server.Misc.ServerList.ServerName + "> " + m.RawName + ": " + str);

                //Console.WriteLine("Sending to Master: <" + Server.Misc.ServerList.ServerName + "> " + m.RawName + ": " + str);
                c_Slave.Send(msg, 0, msg.Length, SocketFlags.None);
            }
        }


        public class MultiPacket
        {
            public Socket Socket;
            public byte[] Buffer = new byte[1];
        }
    }
}