#region References
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
#endregion

namespace Server.Network
{
    public class Listener : IDisposable
    {
        private static readonly byte[][] m_HttpFilters =
        {
            Encoding.UTF8.GetBytes("GET"),
            Encoding.UTF8.GetBytes("POST"),
            Encoding.UTF8.GetBytes("PUT"),
            Encoding.UTF8.GetBytes("DELETE"),
            Encoding.UTF8.GetBytes("HEAD"),
            Encoding.UTF8.GetBytes("OPTIONS"),
            Encoding.UTF8.GetBytes("PATCH"),
            Encoding.UTF8.GetBytes("CONNECT"),
            Encoding.UTF8.GetBytes("TRACE"),
        };

        private static readonly BufferPool m_ByteBufferPool = new BufferPool("Login Byte", 1024, 1);
        private static readonly BufferPool m_PeekBufferPool = new BufferPool("Login Peek", 1024, 4);
        private static readonly BufferPool m_SeedBufferPool = new BufferPool("Login Seed [0xEF]", 1024, PacketHandlers.LoginSeedLength);

        public static IPAddress Address => Config.Get("Server.Listen", IPAddress.Any);

        public static int Port => Config.Get("Server.Port", 2593);

        public static IPEndPoint[] EndPoints { get; set; } =
        {
            new IPEndPoint(Address, Port)
        };

        private long m_SliceTime, m_SliceCounter;

        private long m_Counter, m_AcceptsPerSecond;

        public long AcceptsPerSecond => m_AcceptsPerSecond;

        private Socket m_Listener;

        private readonly ConcurrentQueue<SocketState> m_Accepted = new ConcurrentQueue<SocketState>();

        private readonly AutoResetEvent m_AcceptLimiter = new AutoResetEvent(true);

        private readonly Thread m_AcceptThread;

        private SocketAsyncEventArgs m_AcceptArgs;

        public Listener(IPEndPoint ipep)
        {
            m_Listener = Bind(ipep);

            m_SliceTime = Core.TickCount;

            if (m_Listener == null)
            {
                return;
            }

            DisplayListener();

            m_AcceptThread = new Thread(AcceptLoop)
            {
                Name = $"Login Accept Processor",
                Priority = ThreadPriority.AboveNormal,
            };

            m_AcceptThread.Start();
        }

        private Socket Bind(IPEndPoint ipep)
        {
            Socket s = new Socket(ipep.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = 1000,
                ReceiveTimeout = 1000,
                ExclusiveAddressUse = false,
                UseOnlyOverlappedIO = false,
                NoDelay = true,
                LingerState =
                {
                    Enabled = true,
                    LingerTime = 0,
                },
            };

            try
            {
                s.Bind(ipep);
                s.Listen(1000);

                return s;
            }
            catch (Exception e)
            {
                if (e is SocketException se)
                {
                    if (se.ErrorCode == 10048)
                    {
                        // WSAEADDRINUSE
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine("Listener Failed: {0}:{1} (In Use)", ipep.Address, ipep.Port);
                        Utility.PopColor();
                    }
                    else if (se.ErrorCode == 10049)
                    {
                        // WSAEADDRNOTAVAIL
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine("Listener Failed: {0}:{1} (Unavailable)", ipep.Address, ipep.Port);
                        Utility.PopColor();
                    }
                    else
                    {
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine("Listener Exception:");
                        Console.WriteLine(e);
                        Utility.PopColor();
                    }
                }

                return null;
            }
        }

        private void DisplayListener()
        {
            if (!(m_Listener.LocalEndPoint is IPEndPoint ipep))
            {
                return;
            }

            if (ipep.Address.Equals(IPAddress.Any) || ipep.Address.Equals(IPAddress.IPv6Any))
            {
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface adapter in adapters)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();

                    foreach (IPAddressInformation unicast in properties.UnicastAddresses)
                    {
                        if (ipep.AddressFamily == unicast.Address.AddressFamily)
                        {
                            Utility.PushColor(ConsoleColor.Green);
                            Console.WriteLine("Listening: {0}:{1}", unicast.Address, ipep.Port);
                            Utility.PopColor();
                        }
                    }
                }
            }
            else
            {
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("Listening: {0}:{1}", ipep.Address, ipep.Port);
                Utility.PopColor();
            }

            Utility.PushColor(ConsoleColor.DarkGreen);
            Console.WriteLine(@"----------------------------------------------------------------------");
            Utility.PopColor();
        }

        private SocketAsyncEventArgs Reset(ref SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();

                e.Completed += OnAccept;
            }

            e.UserToken = this;
            e.AcceptSocket = null;
            e.RemoteEndPoint = null;

            e.SocketFlags = 0;
            e.SocketError = 0;

            e.DisconnectReuseSocket = true;

            return e;
        }

        [STAThread]
        private void AcceptLoop()
        {
            while (m_Listener != null)
            {
                Accept();
            }
        }

        private void Accept()
        {
            if (!m_AcceptLimiter.WaitOne(10))
            {
                return;
            }

            if (m_Listener == null)
            {
                return;
            }

            SocketAsyncEventArgs e = Reset(ref m_AcceptArgs);

            bool iocp;

            try
            {
                iocp = m_Listener.AcceptAsync(e);
            }
            catch
            {
                iocp = false;
            }

            if (!iocp)
            {
                OnAccept(e);
            }
        }

        private void OnAccept(object sender, SocketAsyncEventArgs e)
        {
            OnAccept(e);
        }

        private void OnAccept(SocketAsyncEventArgs e)
        {
            try
            {
                Socket accepted = e.AcceptSocket;
                SocketError error = e.SocketError;

                m_AcceptArgs = Reset(ref e);

                if (accepted == null)
                {
                    return;
                }

                Interlocked.Increment(ref m_Counter);

                accepted.NoDelay = true;

                if (error != 0)
                {
                    Release(false, ref accepted);

                    return;
                }

                try
                {
                    accepted.SendTimeout = 1000;
                    accepted.ReceiveTimeout = 1000;

                    _ = ThreadPool.QueueUserWorkItem(o =>
                    {
                        Socket s = (Socket)o;

                        if (ValidateSequence(s, out byte[] seed))
                        {
                            Enqueue(s, seed);
                        }
                        else
                        {
                            Release(false, ref s);
                        }
                    }, accepted);
                }
                catch
                {
                    Release(false, ref accepted);
                }
            }
            finally
            {
                m_AcceptLimiter.Set();
            }
        }

        private bool ValidateSequence(Socket socket, out byte[] buffer)
        {
            buffer = null;

            byte[] peek;

            try
            {
                peek = m_PeekBufferPool.AcquireBuffer();
            }
            catch
            {
                return false;
            }

            try
            {
                int recv = socket.Receive(peek, peek.Length, SocketFlags.None);

                if (recv < peek.Length)
                {
                    return false;
                }

                bool allow = false;

                int index = 0;

                byte packetID = peek[index++];

                if (packetID == 0xEF)
                {
                    byte[] packet = m_SeedBufferPool.AcquireBuffer();

                    try
                    {
                        var diff = packet.Length - peek.Length;

                        recv = socket.Receive(packet, peek.Length, diff, SocketFlags.None);

                        if (recv == diff)
                        {
                            Buffer.BlockCopy(peek, 0, packet, 0, peek.Length);

                            uint seed = readU32(packet, ref index);

                            if (seed != 0)
                            {
                                uint clientMaj = readU32(packet, ref index);
                                uint clientMin = readU32(packet, ref index);
                                uint clientRev = readU32(packet, ref index);
                                uint clientPat = readU32(packet, ref index);

                                uint version = (clientMaj << 24) | (clientMin << 16) | (clientRev << 8) | clientPat;

                                if (version != 0)
                                {
                                    allow = true;

                                    buffer = packet.ToArray();
                                }
                            }
                        }
                    }
                    finally
                    {
                        m_SeedBufferPool.ReleaseBuffer(packet);
                    }
                }
                else if (!Match(peek))
                {
                    index = packetID = 0;

                    uint seed = readU32(peek, ref index);

                    if (seed != 0)
                    {
                        var one = m_ByteBufferPool.AcquireBuffer();

                        try
                        {
                            recv = socket.Receive(one, 0, one.Length, SocketFlags.None);

                            if (recv == one.Length)
                            {
                                packetID = one[0];

                                if (!MessagePump.CheckEncrypted(packetID))
                                {
                                    allow = true;

                                    buffer = new byte[peek.Length + one.Length];

                                    Buffer.BlockCopy(peek, 0, buffer, 0, peek.Length);
                                    Buffer.BlockCopy(one, 0, buffer, peek.Length, one.Length);
                                }
                            }
                        }
                        finally
                        {
                            m_ByteBufferPool.ReleaseBuffer(one);
                        }
                    }
                }

                return allow;
            }
            catch
            {
                return false;
            }
            finally
            {
                m_PeekBufferPool.ReleaseBuffer(peek);
            }

            uint readU32(byte[] b, ref int i)
            {
                return (uint)((b[i++] << 24) | (b[i++] << 16) | (b[i++] << 8) | b[i++]);
            }
        }

        private bool Match(byte[] buffer)
        {
            for (int i = 0; i < m_HttpFilters.Length; i++)
            {
                byte[] filter = m_HttpFilters[i];

                if (filter?.Length > 0)
                {
                    IEnumerable<byte> fseq = filter;
                    IEnumerable<byte> pseq = buffer;

                    if (filter.Length > buffer.Length)
                    {
                        fseq = fseq.Take(buffer.Length);
                    }
                    else if (buffer.Length > filter.Length)
                    {
                        pseq = pseq.Take(filter.Length);
                    }

                    if (Enumerable.SequenceEqual(fseq, pseq))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Enqueue(Socket socket, byte[] seed)
        {
            m_Accepted.Enqueue(new SocketState(socket, seed));

            Core.Set();
        }

        private void Release(bool graceful, ref Socket socket)
        {
            if (socket != null)
            {
                try
                {
                    if (graceful)
                    {
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException ex)
                        {
                            NetState.TraceException(ex);
                        }
                    }

                    if (socket.LingerState != null)
                    {
                        socket.LingerState.Enabled = true;
                        socket.LingerState.LingerTime = graceful ? 1 : 0;
                    }
                    else
                    {
                        socket.LingerState = new LingerOption(true, graceful ? 1 : 0);
                    }
                }
                catch
                {
                }
                finally
                {
                    socket.Close();

                    socket = null;
                }
            }
        }

        public IEnumerable<SocketState> Slice()
        {
            long now = Core.TickCount;

            if (now - m_SliceTime >= 1000)
            {
                var count = m_Counter;

                Interlocked.Exchange(ref m_SliceTime, now);
                Interlocked.Exchange(ref m_AcceptsPerSecond, count - m_SliceCounter);
                Interlocked.Exchange(ref m_SliceCounter, count);
            }

            int limit = Math.Min(m_Accepted.Count, 100);

            while (--limit >= 0 && m_Accepted.TryDequeue(out SocketState accepted))
            {
                Socket socket = accepted.Socket;

                SocketConnectEventArgs args = new SocketConnectEventArgs(socket);

                EventSink.InvokeSocketConnect(args);

                if (args.AllowConnection)
                {
                    yield return accepted;
                }
                else
                {
                    Release(true, ref socket);
                }
            }
        }

        public void Dispose()
        {
            Socket socket = Interlocked.Exchange(ref m_Listener, null);

            socket?.Close();
        }
    }

    public readonly struct SocketState
    {
        public readonly Socket Socket;
        public readonly byte[] Seed;

        public SocketState(Socket socket, byte[] seed)
        {
            Socket = socket;
            Seed = seed;
        }
    }
}
