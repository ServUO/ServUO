using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Diagnostics;
using System.IO;

namespace Server.Misc
{
    public class ClientVerification
    {
        private static readonly bool m_DetectClientRequirement = true;
        private static readonly OldClientResponse m_OldClientResponse = OldClientResponse.LenientKick;
        private static readonly TimeSpan m_AgeLeniency = TimeSpan.FromDays(10);
        private static readonly TimeSpan m_GameTimeLeniency = TimeSpan.FromHours(25);

        private static ClientVersion m_Required;
        private static ClientVersion m_RequiredEC;

        public static TimeSpan KickDelay = TimeSpan.FromSeconds(Config.Get("Client.KickDelay", 20.0));
        public static bool AllowRegular = Config.Get("Client.AllowRegular", true);
        public static bool AllowUOTD = Config.Get("Client.AllowUOTD", true);
        public static bool AllowGod = Config.Get("Client.AllowGod", true);
        public static bool AllowEC = Config.Get("Client.AllowEC", true);

        private enum OldClientResponse
        {
            Ignore,
            Warn,
            Annoy,
            LenientKick,
            Kick
        }

        public static ClientVersion Required
        {
            get
            {
                return m_Required;
            }
            set
            {
                m_Required = value;
            }
        }

        public static ClientVersion RequiredEC
        {
            get
            {
                return m_RequiredEC;
            }
            set
            {
                m_RequiredEC = value;
            }
        }

        public static void Initialize()
        {
            EventSink.ClientVersionReceived += EventSink_ClientVersionReceived;
            EventSink.ClientTypeReceived += EventSink_ClientTypeReceived;

            m_RequiredEC = new ClientVersion(67, 0, 59, 0, ClientType.SA);

            if (m_DetectClientRequirement)
            {
                string path = Core.FindDataFile("client.exe");

                if (File.Exists(path))
                {
                    FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);

                    if (info.FileMajorPart != 0 || info.FileMinorPart != 0 || info.FileBuildPart != 0 || info.FilePrivatePart != 0)
                    {
                        Required = new ClientVersion(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
                    }
                }
            }

            if (Required != null)
            {
                Utility.PushColor(ConsoleColor.White);
                Console.WriteLine("Restricting classic client version to {0}. Action to be taken: {1}", Required, m_OldClientResponse);
                Utility.PopColor();
            }

            if (RequiredEC != null)
            {
                Utility.PushColor(ConsoleColor.White);
                Console.WriteLine("Restricting enhanced client version to {0}. Action to be taken: {1}", RequiredEC, "Kick");
                Utility.PopColor();
            }
        }

        private static void EventSink_ClientVersionReceived(ClientVersionReceivedArgs e)
        {
            string kickMessage = null;
            NetState state = e.State;
            ClientVersion version = e.Version;

            if (state.Mobile != null && state.Mobile.IsStaff())
                return;

            ClientVersion required = Required;

            if (required != null && version < required && (m_OldClientResponse == OldClientResponse.Kick || (m_OldClientResponse == OldClientResponse.LenientKick && (DateTime.UtcNow - state.Mobile.CreationTime) > m_AgeLeniency && state.Mobile is PlayerMobile && ((PlayerMobile)state.Mobile).GameTime > m_GameTimeLeniency)))
            {
                kickMessage = string.Format("This server requires your client version be at least {0}.", required);
            }
            else if (!AllowGod || !AllowRegular || !AllowUOTD)
            {
                if (!AllowGod && version.Type == ClientType.God)
                    kickMessage = "This server does not allow god clients to connect.";
                else if (!AllowRegular && version.Type == ClientType.Regular)
                    kickMessage = "This server does not allow regular clients to connect.";
                else if (!AllowUOTD && state.IsUOTDClient)
                    kickMessage = "This server does not allow UO:TD clients to connect.";

                if (!AllowGod && !AllowRegular && !AllowUOTD)
                {
                    kickMessage = "This server does not allow any clients to connect.";
                }
                else if (AllowGod && !AllowRegular && !AllowUOTD && version.Type != ClientType.God)
                {
                    kickMessage = "This server requires you to use the god client.";
                }
                else if (kickMessage != null)
                {
                    if (AllowRegular && AllowUOTD)
                        kickMessage += " You can use regular or UO:TD clients.";
                    else if (AllowRegular)
                        kickMessage += " You can use regular clients.";
                    else if (AllowUOTD)
                        kickMessage += " You can use UO:TD clients.";
                }
            }

            if (kickMessage != null)
            {
                state.Mobile.SendMessage(0x22, kickMessage);
                state.Mobile.SendMessage(0x22, "You will be disconnected in {0} seconds.", KickDelay.TotalSeconds);

                Timer.DelayCall(KickDelay, delegate
                {
                    if (state.Socket != null)
                    {
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine("Client: {0}: Disconnecting, bad version", state);
                        Utility.PopColor();
                        state.Dispose();
                    }
                });
            }
            else if (Required != null && version < Required)
            {
                switch (m_OldClientResponse)
                {
                    case OldClientResponse.Warn:
                        {
                            state.Mobile.SendMessage(0x22, "Your client is out of date. Please update your client.", Required);
                            state.Mobile.SendMessage(0x22, "This server recommends that your client version be at least {0}.", Required);
                            break;
                        }
                    case OldClientResponse.LenientKick:
                    case OldClientResponse.Annoy:
                        {
                            SendAnnoyGump(state.Mobile);
                            break;
                        }
                }
            }
        }

        private static void EventSink_ClientTypeReceived(ClientTypeReceivedArgs e)
        {
            NetState state = e.State;
            ClientVersion version = state.Version;

            if (state.IsEnhancedClient)
            {
                if (!AllowEC)
                {
                    Utility.PushColor(ConsoleColor.DarkRed);
                    Console.WriteLine("Client: {0}: Disconnecting, Enhanced Client", state);
                    Utility.PopColor();

                    state.Dispose();
                }
                else
                {
                    ClientVersion required = RequiredEC;

                    if (required != null && version < required)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(5), () =>
                            {
                                if (state.Mobile != null && !state.Mobile.IsStaff())
                                {
                                    state.Mobile.SendMessage("This server requires your enhanced client version be at least {0}. You will be disconnected in 5 seconds.", required);

                                    Timer.DelayCall(TimeSpan.FromSeconds(5), () =>
                                        {
                                            Utility.PushColor(ConsoleColor.DarkRed);
                                            Console.WriteLine("Client: {0}: Disconnecting, bad enhanced client version.", state);
                                            Utility.PopColor();

                                            state.Dispose();
                                        });
                                }
                            });
                    }
                }

                return;
            }
        }

        private static void SendAnnoyGump(Mobile m)
        {
            if (m.NetState != null && m.NetState.Version < Required)
            {
                Gump g = new WarningGump(1060637, 30720, string.Format("Your client is out of date. Please update your client.<br>This server recommends that your client version be at least {0}.<br> <br>You are currently using version {1}.<br> <br>To patch, run UOPatch.exe inside your Ultima Online folder.", Required, m.NetState.Version), 0xFFC000, 480, 360,
                    delegate (Mobile mob, bool selection, object o)
                    {
                        m.SendMessage("You will be reminded of this again.");

                        if (m_OldClientResponse == OldClientResponse.LenientKick)
                            m.SendMessage("Old clients will be kicked after {0} days of character age and {1} hours of play time", m_AgeLeniency, m_GameTimeLeniency);

                        Timer.DelayCall(TimeSpan.FromMinutes(Utility.Random(5, 15)), delegate { SendAnnoyGump(m); });
                    }, null, false)
                {
                    Dragable = false,
                    Closable = false,
                    Resizable = false
                };

                m.SendGump(g);
            }
        }
    }
}
