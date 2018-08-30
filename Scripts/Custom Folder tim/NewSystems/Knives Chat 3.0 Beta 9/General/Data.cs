using System;
using System.IO;
using System.Collections;
using Server;
using Server.Accounting;

namespace Knives.Chat3
{
    public enum OnlineStatus { Online, Away, Busy, Hidden }
    public enum Skin { Three, Two, One }
    
    public class Data
    {
        #region Statics

        public static void Save()
        {
            try { SaveGlobalOptions(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(175), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { SavePlayerOptions(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(228), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { SaveFriends(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(230), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { SaveIgnores(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(232), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { SaveGlobalListens(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(234), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { SaveMsgs(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(236), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void Load()
        {
            try { LoadGlobalOptions(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(174), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { LoadPlayerOptions(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(227), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { LoadFriends(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(229), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { LoadIgnores(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(231), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { LoadGlobalListens(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(233), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
            try { LoadMsgs(); }
            catch (Exception e)
            {
                Errors.Report(General.Local(235), e);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void SaveGlobalOptions()
        {
            CleanUpData();

            if (!Directory.Exists(General.SavePath))
                Directory.CreateDirectory(General.SavePath);

            GenericWriter writer = new BinaryFileWriter(Path.Combine(General.SavePath, "GlobalOptions.bin"), true);

            writer.Write(2); // version

            writer.Write(s_MultiPort);
            writer.Write(s_MultiServer);

            writer.Write(s_Notifications.Count);
            foreach (Notification not in s_Notifications)
                not.Save(writer);

            writer.Write(s_Filters.Count);
            foreach (string str in s_Filters)
                writer.Write(str);

            writer.Write((int)s_FilterPenalty);
            writer.Write((int)s_MacroPenalty);
            writer.Write(s_MaxMsgs);
            writer.Write(s_ChatSpam);
            writer.Write(s_MsgSpam);
            writer.Write(s_RequestSpam);
            writer.Write(s_FilterBanLength);
            writer.Write(s_FilterWarnings);
            writer.Write(s_AntiMacroDelay);
            writer.Write(s_IrcPort);
            writer.Write(s_IrcMaxAttempts);
            writer.Write(s_IrcEnabled);
            writer.Write(s_IrcAutoConnect);
            writer.Write(s_IrcAutoReconnect);
            writer.Write(s_FilterSpeech);
            writer.Write(s_FilterMsg);
            writer.Write(s_Debug);
            writer.Write(s_LogChat);
            writer.Write(s_LogPms);
            writer.Write((int)s_IrcStaffColor);
            writer.Write(s_IrcServer);
            writer.Write(s_IrcRoom);
            writer.Write(s_IrcNick);
            writer.Write(s_TotalChats+1);

            writer.Close();
        }

        public static void SavePlayerOptions()
        {
            if (!Directory.Exists(General.SavePath))
                Directory.CreateDirectory(General.SavePath);

            GenericWriter writer = new BinaryFileWriter(Path.Combine(General.SavePath, "PlayerOptions.bin"), true);

            writer.Write(0); // version

            writer.Write(s_Datas.Count);
            foreach (Data data in s_Datas.Values)
            {
                writer.Write(data.Mobile);
                data.SaveOptions(writer);
            }

            writer.Close();
        }

        public static void SaveFriends()
        {
            if (!Directory.Exists(General.SavePath))
                Directory.CreateDirectory(General.SavePath);

            GenericWriter writer = new BinaryFileWriter(Path.Combine(General.SavePath, "Friends.bin"), true);

            writer.Write(0); // version

            writer.Write(s_Datas.Count);
            foreach (Data data in s_Datas.Values)
            {
                writer.Write(data.Mobile);
                data.SaveFriends(writer);
            }

            writer.Close();
        }

        public static void SaveIgnores()
        {
            if (!Directory.Exists(General.SavePath))
                Directory.CreateDirectory(General.SavePath);

            GenericWriter writer = new BinaryFileWriter(Path.Combine(General.SavePath, "Ignores.bin"), true);

            writer.Write(0); // version

            writer.Write(s_Datas.Count);
            foreach (Data data in s_Datas.Values)
            {
                writer.Write(data.Mobile);
                data.SaveIgnores(writer);
            }

            writer.Close();
        }

        public static void SaveGlobalListens()
        {
            if (!Directory.Exists(General.SavePath))
                Directory.CreateDirectory(General.SavePath);

            GenericWriter writer = new BinaryFileWriter(Path.Combine(General.SavePath, "GlobalListens.bin"), true);

            writer.Write(0); // version

            writer.Write(s_Datas.Count);
            foreach (Data data in s_Datas.Values)
            {
                writer.Write(data.Mobile);
                data.SaveGlobalListens(writer);
            }

            writer.Close();
        }

        public static void SaveMsgs()
        {
            if (!Directory.Exists(General.SavePath))
                Directory.CreateDirectory(General.SavePath);

            GenericWriter writer = new BinaryFileWriter(Path.Combine(General.SavePath, "Pms.bin"), true);

            writer.Write(0); // version

            writer.Write(s_Datas.Count);
            foreach (Data data in s_Datas.Values)
            {
                writer.Write(data.Mobile);
                data.SaveMsgs(writer);
            }

            writer.Close();
        }

        public static void LoadGlobalOptions()
        {
            if (!File.Exists(Path.Combine(General.SavePath, "GlobalOptions.bin")))
                return;

            using (FileStream bin = new FileStream(Path.Combine(General.SavePath, "GlobalOptions.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                GenericReader reader = new BinaryFileReader(new BinaryReader(bin));

                int version = reader.ReadInt();

                if (version >= 2) s_MultiPort = reader.ReadInt();
                if (version >= 2) s_MultiServer = reader.ReadString();

                int count = 0;
                if (version >= 1)
                {
                    count = reader.ReadInt();
                    Notification not = null;
                    for (int i = 0; i < count; ++i)
                    {
                        not = new Notification();
                        not.Load(reader);
                    }
                }

                count = reader.ReadInt();
                string txt = "";
                for (int i = 0; i < count; ++i)
                {
                    txt = reader.ReadString();
                    if(!s_Filters.Contains(txt))
                        s_Filters.Add(txt);
                }

                s_FilterPenalty = (FilterPenalty)reader.ReadInt();
                if(version >= 1) s_MacroPenalty = (MacroPenalty)reader.ReadInt();
                s_MaxMsgs = reader.ReadInt();
                s_ChatSpam = reader.ReadInt();
                s_MsgSpam = reader.ReadInt();
                s_RequestSpam = reader.ReadInt();
                s_FilterBanLength = reader.ReadInt();
                s_FilterWarnings = reader.ReadInt();
                if (version >= 1) s_AntiMacroDelay = reader.ReadInt();
                s_IrcPort = reader.ReadInt();
                s_IrcMaxAttempts = reader.ReadInt();
                s_IrcEnabled = reader.ReadBool();
                s_IrcAutoConnect = reader.ReadBool();
                s_IrcAutoReconnect = reader.ReadBool();
                s_FilterSpeech = reader.ReadBool();
                s_FilterMsg = reader.ReadBool();
                s_Debug = reader.ReadBool();
                s_LogChat = reader.ReadBool();
                s_LogPms = reader.ReadBool();
                s_IrcStaffColor = (IrcColor)reader.ReadInt();
                s_IrcServer = reader.ReadString();
                s_IrcRoom = reader.ReadString();
                s_IrcNick = reader.ReadString();
                s_TotalChats = reader.ReadULong() - 1;
            }
        }

        public static void LoadPlayerOptions()
        {
            if (!File.Exists(Path.Combine(General.SavePath, "PlayerOptions.bin")))
                return;

            using (FileStream bin = new FileStream(Path.Combine(General.SavePath, "PlayerOptions.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                GenericReader reader = new BinaryFileReader(new BinaryReader(bin));

                int version = reader.ReadInt();

                Mobile m = null;
                int count = reader.ReadInt();
                for (int i = 0; i < count; ++i)
                {
                    m = reader.ReadMobile();
                    if (m != null)
                        GetData(m).LoadOptions(reader);
                    else
                        (new Data()).LoadOptions(reader);
                }
            }
        }

        public static void LoadFriends()
        {
            if (!File.Exists(Path.Combine(General.SavePath, "Friends.bin")))
                return;

            using (FileStream bin = new FileStream(Path.Combine(General.SavePath, "Friends.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                GenericReader reader = new BinaryFileReader(new BinaryReader(bin));

                int version = reader.ReadInt();

                Mobile m = null;
                int count = reader.ReadInt();
                for (int i = 0; i < count; ++i)
                {
                    m = reader.ReadMobile();
                    if (m != null)
                        GetData(m).LoadFriends(reader);
                    else
                        (new Data()).LoadFriends(reader);
                }
            }
        }

        public static void LoadIgnores()
        {
            if (!File.Exists(Path.Combine(General.SavePath, "Ignores.bin")))
                return;

            using (FileStream bin = new FileStream(Path.Combine(General.SavePath, "Ignores.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                GenericReader reader = new BinaryFileReader(new BinaryReader(bin));

                int version = reader.ReadInt();

                Mobile m = null;
                int count = reader.ReadInt();
                for (int i = 0; i < count; ++i)
                {
                    m = reader.ReadMobile();
                    if (m != null)
                        GetData(m).LoadIgnores(reader);
                    else
                        (new Data()).LoadIgnores(reader);
                }
            }
        }

        public static void LoadGlobalListens()
        {
            if (!File.Exists(Path.Combine(General.SavePath, "GlobalListens.bin")))
                return;

            using (FileStream bin = new FileStream(Path.Combine(General.SavePath, "GlobalListens.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                GenericReader reader = new BinaryFileReader(new BinaryReader(bin));

                int version = reader.ReadInt();

                Mobile m = null;
                int count = reader.ReadInt();
                for (int i = 0; i < count; ++i)
                {
                    m = reader.ReadMobile();
                    if (m != null)
                        GetData(m).LoadGlobalListens(reader);
                    else
                        (new Data()).LoadGlobalListens(reader);
                }
            }
        }

        public static void LoadMsgs()
        {
            if (!File.Exists(Path.Combine(General.SavePath, "Pms.bin")))
                return;

            using (FileStream bin = new FileStream(Path.Combine(General.SavePath, "Pms.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                GenericReader reader = new BinaryFileReader(new BinaryReader(bin));

                int version = reader.ReadInt();

                Mobile m = null;
                int count = reader.ReadInt();
                for (int i = 0; i < count; ++i)
                {
                    m = reader.ReadMobile();
                    if (m != null)
                        GetData(m).LoadMsgs(reader);
                    else
                        (new Data()).LoadMsgs(reader);
                }
            }
        }

        private static void CleanUpData()
        {
            Data data = null;
            foreach (Mobile m in new ArrayList(s_Datas.Keys))
            {
                data = (Data)s_Datas[m];

                if (m.Deleted || data.Mobile == null || data.Mobile.Deleted)
                    s_Datas.Remove(data.Mobile);
                else if (data.Mobile.Player && data.Mobile.Account != null && ((Account)data.Mobile.Account).LastLogin < DateTime.Now - TimeSpan.FromDays(30))
                    s_Datas.Remove(data.Mobile);
            }
        }

        #endregion

        #region Static Definitions

        private static Hashtable s_Datas = new Hashtable();
        private static ArrayList s_Notifications = new ArrayList();
        private static ArrayList s_MultiBlocks = new ArrayList();
        private static ArrayList s_Filters = new ArrayList();
        private static ArrayList s_IrcList = new ArrayList();
        private static FilterPenalty s_FilterPenalty;
        private static MacroPenalty s_MacroPenalty;
        private static int s_MaxMsgs = 50;
        private static int s_ChatSpam = 2;
        private static int s_MsgSpam = 5;
        private static int s_RequestSpam = 24;
        private static int s_FilterBanLength = 5;
        private static int s_FilterWarnings = 3;
        private static int s_AntiMacroDelay = 60;
        private static int s_IrcPort = 6667;
        private static int s_IrcMaxAttempts = 3;
        private static int s_MultiPort = 8112;
        private static ulong s_TotalChats;
        private static bool s_IrcEnabled = false;
        private static bool s_IrcAutoConnect = false;
        private static bool s_IrcAutoReconnect = false;
        private static bool s_MultiMaster = false;
        private static bool s_FilterSpeech = false;
        private static bool s_FilterMsg = false;
        private static bool s_Debug = false;
        private static bool s_LogChat = false;
        private static bool s_LogPms = false;
        private static IrcColor s_IrcStaffColor = IrcColor.Black;
        private static string s_IrcServer = "";
        private static string s_IrcRoom = "";
        private static string s_IrcNick = Server.Misc.ServerList.ServerName;
        private static string s_MultiServer = "127.0.0.1";

        public static Hashtable Datas { get { return s_Datas; } }
        public static ArrayList Notifications { get { return s_Notifications; } }
        public static ArrayList MultiBlocks { get { return s_MultiBlocks; } }
        public static ArrayList Filters { get { return s_Filters; } }
        public static ArrayList IrcList { get { return s_IrcList; } }
        public static FilterPenalty FilterPenalty { get { return s_FilterPenalty; } set { s_FilterPenalty = value; } }
        public static MacroPenalty MacroPenalty { get { return s_MacroPenalty; } set { s_MacroPenalty = value; } }
        public static int MaxMsgs { get { return s_MaxMsgs; } set { s_MaxMsgs = value; } }
        public static int ChatSpam { get { return s_ChatSpam; } set { s_ChatSpam = value; } }
        public static int MsgSpam { get { return s_MsgSpam; } set { s_MsgSpam = value; } }
        public static int RequestSpam { get { return s_RequestSpam; } set { s_RequestSpam = value; } }
        public static int FilterBanLength { get { return s_FilterBanLength; } set { s_FilterBanLength = value; } }
        public static int FilterWarnings { get { return s_FilterWarnings; } set { s_FilterWarnings = value; } }
        public static int AntiMacroDelay { get { return s_AntiMacroDelay; } set { s_AntiMacroDelay = value; } }
        public static int IrcPort { get { return s_IrcPort; } set { s_IrcPort = value; } }
        public static int IrcMaxAttempts { get { return s_IrcMaxAttempts; } set { s_IrcMaxAttempts = value; } }
        public static int MultiPort { get { return s_MultiPort; } set { s_MultiPort = value; } }
        public static ulong TotalChats { get { return s_TotalChats; } set { s_TotalChats = value; } }
        public static bool IrcAutoConnect { get { return s_IrcAutoConnect; } set { s_IrcAutoConnect = value; } }
        public static bool IrcAutoReconnect { get { return s_IrcAutoReconnect; } set { s_IrcAutoReconnect = value; } }
        public static bool FilterSpeech { get { return s_FilterSpeech; } set { s_FilterSpeech = value; } }
        public static bool FilterMsg { get { return s_FilterMsg; } set { s_FilterMsg = value; } }
        public static bool Debug { get { return s_Debug; } set { s_Debug = value; } }
        public static bool LogChat { get { return s_LogChat; } set { s_LogChat = value; } }
        public static bool LogPms { get { return s_LogPms; } set { s_LogPms = value; } }
        public static string IrcServer { get { return s_IrcServer; } set { s_IrcServer = value; } }
        public static string IrcNick { get { return s_IrcNick; } set { s_IrcNick = value; } }
        public static string MultiServer { get { return s_MultiServer; } set { s_MultiServer = value; } }

        public static bool IrcEnabled
        {
            get { return s_IrcEnabled; }
            set
            {
                s_IrcEnabled = value;
                if (!value)
                {
                    IrcConnection.Connection.CancelConnect();
                    IrcConnection.Connection.Disconnect(false);
                }
            }
        }

        public static bool MultiMaster
        {
            get { return s_MultiMaster; }
            set
            {
                s_MultiMaster = value;
                if (!value)
                {
                    MultiConnection.Connection.CloseMaster();
                    MultiConnection.Connection.CloseSlave();
                }
            }
        }

        public static IrcColor IrcStaffColor
        {
            get { return s_IrcStaffColor; }
            set
            {
                if ((int)value > 15)
                    value = (IrcColor)0;

                if ((int)value < 0)
                    value = (IrcColor)15;

                s_IrcStaffColor = value;
            }
        }

        public static Data GetData(Mobile m)
        {
            if (s_Datas[m] == null)
                return new Data(m);

            return (Data)s_Datas[m];
        }

        public static string IrcRoom
        {
            get { return s_IrcRoom; }
            set
            {
                s_IrcRoom = value;

                if (s_IrcRoom.IndexOf("#") != 0)
                    s_IrcRoom = "#" + s_IrcRoom;
            }
        }

        #endregion

        #region Class Definitions

        private Mobile c_Mobile;
        private Channel c_CurrentChannel;
        private OnlineStatus c_Status;
        private Skin c_MenuSkin;
        private object c_Recording;
        private ArrayList c_Friends, c_Ignores, c_Messages, c_GIgnores, c_GListens, c_IrcIgnores;
        private Hashtable c_Sounds;
        private int c_GlobalMC, c_GlobalCC, c_GlobalGC, c_GlobalFC, c_GlobalWC, c_SystemC, c_MultiC, c_MsgC, c_PerPage, c_DefaultSound, c_StaffC, c_Avatar, c_Karma, c_Warnings;
        private bool c_GlobalAccess, c_Global, c_GlobalM, c_GlobalC, c_GlobalG, c_GlobalF, c_GlobalW, c_Banned, c_FriendsOnly, c_MsgSound, c_ByRequest, c_FriendAlert, c_SevenDays, c_WhenFull, c_ReadReceipt, c_IrcRaw, c_QuickBar, c_ExtraPm;
        private string c_AwayMsg, c_Signature;
        private DateTime c_BannedUntil, c_LastKarma;

        public Mobile Mobile { get { return c_Mobile; } }
        public Channel CurrentChannel { get { return c_CurrentChannel; } set { c_CurrentChannel = value; } }
        public OnlineStatus Status { get { return c_Status; } set { c_Status = value; } }
        public Skin MenuSkin { get { return c_MenuSkin; } set { c_MenuSkin = value; } }
        public object Recording{ get{ return c_Recording; } set{ c_Recording = value; } }
        public ArrayList Friends { get { return c_Friends; } }
        public ArrayList Ignores { get { return c_Ignores; } }
        public ArrayList Messages { get { return c_Messages; } }
        public ArrayList GIgnores { get { return c_GIgnores; } }
        public ArrayList GListens { get { return c_GListens; } }
        public ArrayList IrcIgnores { get { return c_IrcIgnores; } }
        public bool Global { get { return c_Global; } set { c_Global = value; } }
        public bool GlobalM { get { return c_GlobalM && c_Global; } set { c_GlobalM = value; } }
        public bool GlobalC { get { return c_GlobalC && c_Global; } set { c_GlobalC = value; } }
        public bool GlobalG { get { return c_GlobalG && c_Global; } set { c_GlobalG = value; } }
        public bool GlobalF { get { return c_GlobalF && c_Global; } set { c_GlobalF = value; } }
        public bool GlobalW { get { return c_GlobalW && c_Global; } set { c_GlobalW = value; } }
        public bool FriendsOnly { get { return c_FriendsOnly; } set { c_FriendsOnly = value; } }
        public bool MsgSound { get { return c_MsgSound; } set { c_MsgSound = value; } }
        public bool ByRequest { get { return c_ByRequest; } set { c_ByRequest = value; } }
        public bool FriendAlert { get { return c_FriendAlert; } set { c_FriendAlert = value; } }
        public bool SevenDays { get { return c_SevenDays; } set { c_SevenDays = value; } }
        public bool WhenFull { get { return c_WhenFull; } set { c_WhenFull = value; } }
        public bool ReadReceipt { get { return c_ReadReceipt; } set { c_ReadReceipt = value; } }
        public bool IrcRaw { get { return c_IrcRaw; } set { c_IrcRaw = value; } }
        public bool QuickBar { get { return c_QuickBar; } set { c_QuickBar = value; } }
        public bool ExtraPm { get { return c_ExtraPm; } set { c_ExtraPm = value; } }
        public int GlobalMC { get { return c_GlobalMC; } set { c_GlobalMC = value; } }
        public int GlobalCC { get { return c_GlobalCC; } set { c_GlobalCC = value; } }
        public int GlobalGC { get { return c_GlobalGC; } set { c_GlobalGC = value; } }
        public int GlobalFC { get { return c_GlobalFC; } set { c_GlobalFC = value; } }
        public int GlobalWC { get { return c_GlobalWC; } set { c_GlobalWC = value; } }
        public int SystemC { get { return c_SystemC; } set { c_SystemC = value; } }
        public int MultiC { get { return c_MultiC; } set { c_MultiC = value; } }
        public int MsgC { get { return c_MsgC; } set { c_MsgC = value; } }
        public int StaffC { get { return c_StaffC; } set { c_StaffC = value; } }
        public int Avatar { get { return c_Avatar; } set { c_Avatar = value; } }
        public int Warnings { get { return c_Warnings; } set { c_Warnings = value; } }
        public string AwayMsg { get { return c_AwayMsg; } set { c_AwayMsg = value; } }
        public string Signature { get { return c_Signature; } set { c_Signature = value; c_Mobile.SendMessage(c_SystemC, General.Local(246));} }

        public int Karma
        {
            get { return c_Karma; }
            set
            {
                if (c_LastKarma + TimeSpan.FromHours(24) > DateTime.Now)
                    return;

                c_Karma = value;
                c_LastKarma = DateTime.Now;
            }
        }

        public int PerPage
        {
            get { return c_PerPage; }
            set
            {
                c_PerPage = value;

                if (c_PerPage < 5)
                    c_PerPage = 5;
                if (c_PerPage > 15)
                    c_PerPage = 15;
            }
        }

        public int DefaultSound
        {
            get { return c_DefaultSound; }
            set
            {
                foreach (Mobile m in c_Sounds.Keys)
                    if ((int)c_Sounds[m] == c_DefaultSound)
                        c_Sounds[m] = value;

                c_DefaultSound = value;

                if (c_DefaultSound < 0)
                    c_DefaultSound = 0;
            }
        }

        public bool GlobalAccess
        {
            get { return c_GlobalAccess || c_Mobile.AccessLevel >= AccessLevel.Administrator; }
            set
            {
                c_GlobalAccess = value;

                if (value)
                    c_Mobile.SendMessage(c_SystemC, General.Local(92));
                else
                    c_Mobile.SendMessage(c_SystemC, General.Local(93));
            }
        }

        public bool Banned
        {
            get{ return c_Banned; }
            set
            {
                c_Banned = value;

                if (value)
                    c_Mobile.SendMessage(c_SystemC, General.Local(90));
                else
                    c_Mobile.SendMessage(c_SystemC, General.Local(91));
            }
        }

        public int DefaultBack
        {
            get
            {
                switch (c_MenuSkin)
                {
                    case Skin.Three:
                        return 0x1400;
                    case Skin.Two:
                        return 0x13BE;
                    default:
                        return 0xE10;
                }
            }
        }

        #endregion

        #region Constructors

        public Data(Mobile m)
        {
            c_Mobile = m;

            c_Friends = new ArrayList();
            c_Ignores = new ArrayList();
            c_Messages = new ArrayList();
            c_GIgnores = new ArrayList();
            c_GListens = new ArrayList();
            c_IrcIgnores = new ArrayList();
            c_Sounds = new Hashtable();
            c_PerPage = 10;
            c_SystemC = 0x161;
            c_GlobalMC = 0x26;
            c_GlobalCC = 0x47E;
            c_GlobalGC = 0x44;
            c_GlobalFC = 0x17;
            c_GlobalWC = 0x3;
            c_StaffC = 0x3B4;
            c_MsgC = 0x480;
            c_AwayMsg = "";
            c_Signature = "";
            c_BannedUntil = DateTime.Now;

            if (m.AccessLevel >= AccessLevel.Administrator)
                c_GlobalAccess = true;

            s_Datas[m] = this;

            foreach (Channel c in Channel.Channels)
                if (c.NewChars)
                    c.Join(m);
        }

        public Data()
        {
            c_Friends = new ArrayList();
            c_Ignores = new ArrayList();
            c_Messages = new ArrayList();
            c_GIgnores = new ArrayList();
            c_GListens = new ArrayList();
            c_IrcIgnores = new ArrayList();
            c_Sounds = new Hashtable();
            c_PerPage = 10;
            c_SystemC = 0x161;
            c_GlobalMC = 0x26;
            c_GlobalCC = 0x47E;
            c_GlobalGC = 0x44;
            c_GlobalFC = 0x17;
            c_GlobalWC = 0x3;
            c_StaffC = 0x3B4;
            c_MsgC = 0x480;
            c_AwayMsg = "";
            c_Signature = "";
            c_BannedUntil = DateTime.Now;
        }

        #endregion

        #region Methods

        public bool NewMsg()
        {
            foreach (Message msg in c_Messages)
                if (!msg.Read)
                    return true;

            return false;
        }

        public bool NewMsgFrom(Mobile m)
        {
            foreach (Message msg in c_Messages)
                if (!msg.Read && msg.From == m)
                    return true;

            return false;
        }

        public Message GetNewMsgFrom(Mobile m)
        {
            foreach (Message msg in c_Messages)
                if (!msg.Read && msg.From == m)
                    return msg;

            return null;
        }

        public void CheckMsg()
        {
            foreach( Message msg in c_Messages )
                if (!msg.Read)
                {
                    new MessageGump(c_Mobile, msg);
                    return;
                }
        }

        public Message GetMsg()
        {
            if (c_Messages.Count == 0)
                return null;

            return (Message)c_Messages[c_Messages.Count - 1];
        }

        public void CheckMsgFrom(Mobile m)
        {
            foreach(Message msg in c_Messages)
                if (!msg.Read && msg.From == m)
                {
                    new MessageGump(c_Mobile, msg);
                    return;
                }
        }

        public int GetSound(Mobile m)
        {
            if (c_Sounds[m] == null)
                c_Sounds[m] = c_DefaultSound;

            return (int)c_Sounds[m];
        }

        public void SetSound(Mobile m, int num)
        {
            if (num < 0)
                num = 0;

            c_Sounds[m] = num;
        }

        public void AddFriend(Mobile m)
        {
            if (c_Friends.Contains(m) || m == c_Mobile)
                return;

            c_Friends.Add(m);
            c_Mobile.SendMessage(c_SystemC, m.Name + " " + General.Local(73));
        }

        public void RemoveFriend(Mobile m)
        {
            if (!c_Friends.Contains(m))
                return;

            c_Friends.Remove(m);
            c_Mobile.SendMessage(c_SystemC, m.Name + " " + General.Local(72));
        }

        public void AddIgnore(Mobile m)
        {
            if (c_Mobile == m)
                return;

            if (c_Ignores.Contains(m) || m == c_Mobile)
                return;

            c_Ignores.Add(m);
            c_Mobile.SendMessage(c_SystemC, General.Local(68) + " " + m.Name);
        }

        public void RemoveIgnore(Mobile m)
        {
            if (!c_Ignores.Contains(m))
                return;

            c_Ignores.Remove(m);
            c_Mobile.SendMessage(c_SystemC, General.Local(74) + " " + m.Name);
        }

        public void AddGIgnore(Mobile m)
        {
            if (c_GIgnores.Contains(m))
                return;

            c_GIgnores.Add(m);
            c_Mobile.SendMessage(c_SystemC, General.Local(80) + " " + m.Name);
        }

        public void RemoveGIgnore(Mobile m)
        {
            if (!c_GIgnores.Contains(m))
                return;

            c_GIgnores.Remove(m);
            c_Mobile.SendMessage(c_SystemC, General.Local(79) + " " + m.Name);
        }

        public void AddGListen(Mobile m)
        {
            if (c_GListens.Contains(m))
                return;

            c_GListens.Add(m);
            c_Mobile.SendMessage(c_SystemC, General.Local(82) + " " + m.Name);
        }

        public void RemoveGListen(Mobile m)
        {
            if (!c_GListens.Contains(m))
                return;

            c_GListens.Remove(m);
            c_Mobile.SendMessage(c_SystemC, General.Local(81) + " " + m.Name);
        }

        public void AddIrcIgnore(string str)
        {
            if (c_IrcIgnores.Contains(str))
                return;

            c_IrcIgnores.Add(str);
            c_Mobile.SendMessage(c_SystemC, General.Local(68) + " " + str);
        }

        public void RemoveIrcIgnore(string str)
        {
            if (!c_IrcIgnores.Contains(str))
                return;

            c_IrcIgnores.Remove(str);
            c_Mobile.SendMessage(c_SystemC, General.Local(74) + " " + str);
        }

        public void AddMessage(Message msg)
        {
            c_Messages.Add(msg);

            if(c_MsgSound)
                c_Mobile.SendSound(GetSound(msg.From));

            if (c_WhenFull && c_Messages.Count > s_MaxMsgs)
                c_Messages.RemoveAt(0);
        }

        public void DeleteMessage(Message msg)
        {
            c_Messages.Remove(msg);

            c_Mobile.SendMessage(c_SystemC, General.Local(69));
        }

        public void Ban(TimeSpan ts)
        {
            c_BannedUntil = DateTime.Now + ts;
            c_Banned = true;
            Mobile.SendMessage(c_SystemC, General.Local(90));

            Timer.DelayCall(ts, new TimerCallback(RemoveBan));
        }

        public void RemoveBan()
        {
            c_BannedUntil = DateTime.Now;
            c_Banned = false;
            if(Mobile != null)
                Mobile.SendMessage(c_SystemC, General.Local(91));
        }

        public void AvatarUp()
        {
            if (c_Avatar == 0)
            {
                c_Avatar = (int)Chat3.Avatar.AvaKeys[0];
                return;
            }

            ArrayList list = new ArrayList(Chat3.Avatar.AvaKeys);

            for (int i = 0; i < list.Count; ++i)
                if (c_Avatar == (int)list[i])
                {
                    if (i == list.Count - 1)
                    {
                        c_Avatar = (int)list[0];
                        return;
                    }

                    c_Avatar = (int)list[i + 1];
                    return;
                }
        }

        public void AvatarDown()
        {
            if (c_Avatar == 0)
            {
                c_Avatar = (int)Chat3.Avatar.AvaKeys[0];
                return;
            }

            ArrayList list = new ArrayList(Chat3.Avatar.AvaKeys);

            for (int i = 0; i < list.Count; ++i)
                if (c_Avatar == (int)list[i])
                {
                    if (i == 0)
                    {
                        c_Avatar = (int)list[list.Count - 1];
                        return;
                    }

                    c_Avatar = (int)list[i - 1];
                    return;
                }
        }

        public void SaveOptions(GenericWriter writer)
        {
            writer.Write(3); // Version

            writer.Write(c_MultiC);

            writer.Write(c_Karma);

            writer.Write(c_ReadReceipt);
            writer.Write(c_QuickBar);
            writer.Write(c_ExtraPm);
            writer.Write((int)c_Status);

            foreach (Mobile m in new ArrayList(c_Sounds.Keys))
                if (m.Deleted)
                    c_Sounds.Remove(m);

            writer.Write(c_Sounds.Count);
            foreach (Mobile m in c_Sounds.Keys)
            {
                writer.Write(m);
                writer.Write((int)c_Sounds[m]);
            }

            writer.Write(c_GlobalMC);
            writer.Write(c_GlobalCC);
            writer.Write(c_GlobalGC);
            writer.Write(c_GlobalFC);
            writer.Write(c_GlobalWC);
            writer.Write(c_SystemC);
            writer.Write(c_MsgC);
            writer.Write(c_PerPage);
            writer.Write(c_DefaultSound);
            writer.Write(c_StaffC);
            writer.Write(c_Avatar);
            writer.Write((int)c_MenuSkin);
            writer.Write(c_GlobalAccess);
            writer.Write(c_Global);
            writer.Write(c_GlobalM);
            writer.Write(c_GlobalC);
            writer.Write(c_GlobalG);
            writer.Write(c_GlobalF);
            writer.Write(c_GlobalW);
            writer.Write(c_Banned);
            writer.Write(c_FriendsOnly);
            writer.Write(c_MsgSound);
            writer.Write(c_ByRequest);
            writer.Write(c_FriendAlert);
            writer.Write(c_SevenDays);
            writer.Write(c_WhenFull);
            writer.Write(c_IrcRaw);
            writer.Write(c_AwayMsg);
            writer.Write(c_Signature);
            writer.Write(c_BannedUntil);
        }

        public void SaveFriends(GenericWriter writer)
        {
            writer.Write(1); // Version

            writer.WriteMobileList(c_Friends, true);
        }

        public void SaveIgnores(GenericWriter writer)
        {
            writer.Write(1); // Version

            writer.WriteMobileList(c_Ignores, true);
        }

        public void SaveMsgs(GenericWriter writer)
        {
            writer.Write(1); // Version

            foreach (Message msg in new ArrayList(c_Messages))
                if (msg.From.Deleted )
                    c_Messages.Remove(msg);

            writer.Write(c_Messages.Count);
            foreach (Message msg in c_Messages)
                msg.Save(writer);
        }

        public void SaveGlobalListens(GenericWriter writer)
        {
            writer.Write(1); // Version

            writer.WriteMobileList(c_GIgnores, true);
            writer.WriteMobileList(c_GListens, true);
        }

        public void LoadOptions(GenericReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 3)
                c_MultiC = reader.ReadInt();
            if (version >= 2)
                c_Karma = reader.ReadInt();

            c_ReadReceipt = reader.ReadBool();
            c_QuickBar = reader.ReadBool();
            c_ExtraPm = reader.ReadBool();
            c_Status = (OnlineStatus)reader.ReadInt();

            Mobile m;
            int count = reader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                m = reader.ReadMobile();
                if (m != null)
                    c_Sounds[m] = reader.ReadInt();
                else
                    reader.ReadInt();
            }

            c_GlobalMC = reader.ReadInt();
            c_GlobalCC = reader.ReadInt();
            c_GlobalGC = reader.ReadInt();
            c_GlobalFC = reader.ReadInt();
            c_GlobalWC = reader.ReadInt();
            c_SystemC = reader.ReadInt();
            c_MsgC = reader.ReadInt();
            c_PerPage = reader.ReadInt();
            c_DefaultSound = reader.ReadInt();
            c_StaffC = reader.ReadInt();
            c_Avatar = reader.ReadInt();
            c_MenuSkin = (Skin)reader.ReadInt();
            c_GlobalAccess = reader.ReadBool();
            c_Global = reader.ReadBool();
            c_GlobalM = reader.ReadBool();
            c_GlobalC = reader.ReadBool();
            c_GlobalG = reader.ReadBool();
            c_GlobalF = reader.ReadBool();
            c_GlobalW = reader.ReadBool();
            c_Banned = reader.ReadBool();
            c_FriendsOnly = reader.ReadBool();
            c_MsgSound = reader.ReadBool();
            c_ByRequest = reader.ReadBool();
            c_FriendAlert = reader.ReadBool();
            c_SevenDays = reader.ReadBool();
            c_WhenFull = reader.ReadBool();
            c_IrcRaw = reader.ReadBool();
            c_AwayMsg = reader.ReadString();
            c_Signature = reader.ReadString();
            c_BannedUntil = reader.ReadDateTime();

            if (c_BannedUntil > DateTime.Now)
                Ban(c_BannedUntil - DateTime.Now);
            else
                RemoveBan();
        }

        public void LoadFriends(GenericReader reader)
        {
            int version = reader.ReadInt();

            c_Friends = reader.ReadMobileList();
        }

        public void LoadIgnores(GenericReader reader)
        {
            int version = reader.ReadInt();

            c_Ignores = reader.ReadMobileList();
        }

        public void LoadMsgs(GenericReader reader)
        {
            int version = reader.ReadInt();

            Message msg;
            int count = reader.ReadInt();
            for (int i = 0; i < count; ++i)
            {
                msg = new Message();
                msg.Load(reader);

                if (msg.From != null )
                    c_Messages.Add(msg);
            }
        }

        public void LoadGlobalListens(GenericReader reader)
        {
            int version = reader.ReadInt();

            c_GIgnores = reader.ReadMobileList();
            c_GListens = reader.ReadMobileList();
        }

        #endregion

    }
}