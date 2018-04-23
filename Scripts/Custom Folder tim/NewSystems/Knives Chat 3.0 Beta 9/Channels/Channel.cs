// Chat karma titles, with settable points required for the title
// Custom chat titles by staff
// Change filter display to number based and staff can input number to remove them
// A suite of text tools for private messages, including the text color, html formatting
// Current addition to status selection on ListGump
// Chat emotes
// QUick chatting (Players can assign text to morph into other text)
// Find a way to get Alliance history working
// Fun filter word exchanges (*bleep*)
// Filter exceptions (Jew -> Jewelery)
// Chatting duels, ratings
// Let players chose how to display their chat (like: {Chat} or (Chat) or /Chat\ etc)
// Method to send Web Pms, Irc Pms
// Link chat to the EA chat interface
// Debug var that displays various timer information
// Conversations
// Skin 1.0
// Set up the script regions
// Kick filter penalty
// Expand History gump to include links to other channels
// Status and other similar commands for getting RunUO status from IRC

using System;
using System.Collections;
using System.IO;
using Server;

namespace Knives.Chat3
{
    public enum ChatStyle { Global, Regional }

    public class Channel
    {
        #region Statics

        private static ArrayList s_Channels = new ArrayList();
        
        public static ArrayList Channels { get{ return s_Channels; } }
        
        public static void Register(Channel c)
        {
            foreach (string str in c.Commands)
                RUOVersion.AddCommand(str, AccessLevel.Player, new ChatCommandHandler(ChannelCommand));
        }

        public static Channel GetByName(string str)
        {
            foreach (Channel c in s_Channels)
                if (c.Name == str)
                    return c;

            return null;
        }

        public static Channel GetByType(Type type)
        {
            foreach (Channel c in s_Channels)
                if (c.GetType() == type)
                    return c;

            return null;
        }

        public static ArrayList GetHistoryFrom(Mobile m, string name)
        {
            Channel c = GetByName(name);
            if (c == null)
                return null;

            return c.GetHistory(m);
        }

        private static void ChannelCommand(CommandInfo e)
        {
            foreach (Channel c in s_Channels)
                foreach (string str in c.Commands)
                    if (str == e.Command)
                    {
                        c.OnChat(e.Mobile, e.ArgString);
                        return;
                    }
        }

        public static void AddCommand(string str)
        {
            RUOVersion.AddCommand(str, AccessLevel.Player, new ChatCommandHandler(ChannelCommand));
        }

        public static void RemoveCommand(string str)
        {
            RUOVersion.RemoveCommand(str);
        }

        public static void Save()
        {
            try
            {
                if (!Directory.Exists(General.SavePath))
                    Directory.CreateDirectory(General.SavePath);

                GenericWriter writer = new BinaryFileWriter(Path.Combine(General.SavePath, "Channels.bin"), true);

                writer.Write(0); // version

                writer.Write(s_Channels.Count);
                foreach (Channel c in s_Channels)
                {
                    writer.Write(c.GetType().ToString());
                    c.Save(writer);
                }

                writer.Close();
            }
            catch (Exception e)
            {
                Errors.Report(General.Local(187));
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(Path.Combine(General.SavePath, "Channels.bin")))
                {
                    PredefinedChannels();
                    return;
                }

                using (FileStream bin = new FileStream(Path.Combine(General.SavePath, "Channels.bin"), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    GenericReader reader = new BinaryFileReader(new BinaryReader(bin));

                    int version = reader.ReadInt();

                    int count = reader.ReadInt();
                    Channel c;
                    for (int i = 0; i < count; ++i)
                    {
                        c = Activator.CreateInstance(ScriptCompiler.FindTypeByFullName(reader.ReadString())) as Channel;
                        if (c == null)
                        {
                            c = new Channel();
                            c.Load(reader);
                            s_Channels.Remove(c);
                        }
                        else
                        {
                            c.Load(reader);
                        }
                    }
                }

                PredefinedChannels();
            }
            catch(Exception e)
            {
                Errors.Report(General.Local(186));
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void PredefinedChannels()
        {
            if (!Exists(typeof(Alliance)))
                new Alliance();
            if (!Exists(typeof(Faction)))
                new Faction();
            if (!Exists(typeof(Guild)))
                new Guild();
            if (!Exists(typeof(Irc)))
                new Irc();
            if (!Exists(typeof(Staff)))
                new Staff();
            if (!Exists(typeof(Public)))
                new Public();
            if (!Exists(typeof(Multi)))
                new Multi();
        }

        public static bool Exists(Type type)
        {
            foreach (Channel c in s_Channels)
                if (c.GetType().ToString() == type.ToString())
                    return true;

            return false;
        }

        #endregion

        #region Class Definitions

        private string c_Name;
        private ArrayList c_Mobiles = new ArrayList();
        private Hashtable c_Colors = new Hashtable();
        private ArrayList c_Commands = new ArrayList();
        private ArrayList c_History = new ArrayList();
        private int c_DefaultC;
        private ChatStyle c_Style;
        private bool c_ToIrc, c_NewChars, c_Filter, c_Delay, c_ShowStaff, c_Enabled;

        public string Name { get { return c_Name; } set { c_Name = value; } }
        public Hashtable Colors { get { return c_Colors; } }
        public ArrayList Commands { get { return c_Commands; } }
        public ArrayList History { get { return c_History; } }
        public int DefaultC { get { return c_DefaultC; } set { c_DefaultC = value; } }
        public ChatStyle Style { get { return c_Style; } set { c_Style = value; } }
        public bool ToIrc { get { return c_ToIrc; } set { c_ToIrc = value; } }
        public bool NewChars { get { return c_NewChars; } set { c_NewChars = value; } }
        public bool Filter { get { return c_Filter; } set { c_Filter = value; } }
        public bool Delay { get { return c_Delay; } set { c_Delay = value; } }
        public bool ShowStaff { get { return c_ShowStaff; } set { c_ShowStaff = value; } }
        public bool Enabled { get { return c_Enabled; } set { c_Enabled = value; } }

        #endregion

        #region Constructors

        public Channel( string name )
        {
            c_Name = name;

            c_DefaultC = 0x47E;
            c_Filter = true;
            c_Delay = true;
            c_Enabled = true;

            s_Channels.Add(this);
        }

        public Channel()
        {
            // For Loading

            s_Channels.Add(this);
        }

        #endregion

        #region Methods

        public virtual bool IsIn(Mobile m)
        {
            return c_Mobiles.Contains(m);
        }

        public void Join(Mobile m)
        {
            if (!c_Mobiles.Contains(m))
                c_Mobiles.Add(m);
        }

        public void Leave(Mobile m)
        {
            c_Mobiles.Remove(m);
        }

        public virtual ArrayList GetHistory(Mobile m)
        {
            return c_History;
        }

        public virtual void AddHistory(Mobile m, string msg)
        {
            c_History.Add(new ChatHistory(m, msg));
        }

        public virtual void UpdateHistory(Mobile m)
        {
            if (c_History.Count > 50)
                c_History.RemoveAt(0);
        }

        public virtual int ColorFor(Mobile m)
        {
            if (c_Colors[m] == null)
                c_Colors[m] = c_DefaultC;

            return (int)c_Colors[m];
        }

        public virtual string NameFor(Mobile m)
        {
            if (c_Style == ChatStyle.Regional && m.Region != null && m.Region.Name != "")
                return c_Name + " (" + m.Region.Name + ")";

            return c_Name;
        }

        public virtual bool CanChat(Mobile m, bool say)
        {
            if (!Enabled)
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(213));
                return false;
            }

            if (m.Squelched)
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(260));
                return false;
            }

            if (Data.GetData(m).Banned)
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(33));
                return false;
            }

            if (c_Style == ChatStyle.Regional && (m.Region == null || m.Region.Name == ""))
            {
                if (say) m.SendMessage(Data.GetData(m).SystemC, General.Local(35));
                return false;
            }

            return true;
        }

        private void OnChat(object o)
        {
            if (!(o is object[]))
                return;

            object[] obj = (object[])o;

            if (obj.Length != 2 || !(obj[0] is Mobile) || !(obj[1] is string))
                return;

            OnChat((Mobile)obj[0], obj[1].ToString(), false);
        }

        public virtual void OnChat(Mobile m, string msg)
        {
            OnChat(m, msg, true);
        }

        public virtual void OnChat(Mobile m, string msg, bool spam)
        {
            if (msg == null || msg == "")
            {
                if (!CanChat(m, false))
                {
                    General.List(m, 0);
                    return;
                }

                if (c_Mobiles.Contains(m))
                    Data.GetData(m).CurrentChannel = this;

                General.List(m, 1);
                return;
            }

            if (!CanChat(m, true))
                return;

            if(c_Filter)
                msg = Chat3.Filter.FilterText(m, msg);

            if (!CanChat(m, false))
                return;

            if (!c_Mobiles.Contains(m))
            {
                m.SendMessage(Data.GetData(m).SystemC, General.Local(34));
                return;
            }

            if (c_Delay && !TrackSpam.LogSpam(m, "Chat", TimeSpan.FromSeconds(Data.ChatSpam)))
            {
                if (spam) m.SendMessage(Data.GetData(m).SystemC, General.Local(97));
                Timer.DelayCall(TimeSpan.FromSeconds(4), new TimerStateCallback(OnChat), new object[] { m, msg });
                return;
            }

            AddHistory(m, msg);
            UpdateHistory(m);
            Events.InvokeChat(new ChatEventArgs(m, this, msg));

            if (Data.LogChat)
                Logging.LogChat(String.Format(DateTime.Now + " <{0}{1}> {2}: {3}", c_Name, (c_Style == ChatStyle.Regional && m.Region != null ? "-" + m.Region.Name : ""), m.RawName, msg));

            Data.TotalChats++;
            Data.GetData(m).Karma++;

            Broadcast(m, msg);

            if (c_ToIrc && IrcConnection.Connection.Connected)
                IrcConnection.Connection.SendUserMessage(m, "(" + c_Name + ") " + msg);
        }

        protected virtual void Broadcast(Mobile m, string msg)
        {
            foreach (Data data in Data.Datas.Values)
            {
                if (c_Mobiles.Contains(data.Mobile) && !data.Ignores.Contains(m))
                {
                    if (c_Style == ChatStyle.Regional && data.Mobile.Region != m.Region)
                        continue;

                    data.Mobile.SendMessage(m.AccessLevel == AccessLevel.Player ? ColorFor(data.Mobile) : Data.GetData(m).StaffC, String.Format("<{0}{1}> {2}: {3}", NameFor(m), (c_Style == ChatStyle.Regional && m.Region != null ? "-" + m.Region.Name : ""), m.RawName, msg));
                }
                else if (data.Mobile.AccessLevel >= m.AccessLevel && ((data.GlobalC && !data.GIgnores.Contains(m)) || data.GListens.Contains(m)))
                    data.Mobile.SendMessage(data.GlobalCC, String.Format("(Global) <{0}{1}> {2}: {3}", c_Name, (c_Style == ChatStyle.Regional && m.Region != null ? "-" + m.Region.Name : ""), m.RawName, msg ));
            }
        }

        public void BroadcastSystem(string msg)
        {
            foreach (Mobile m in c_Mobiles)
                if (!Data.GetData(m).Banned)
                    m.SendMessage(Data.GetData(m).SystemC, msg);
        }

        public virtual ArrayList BuildList(Mobile m)
        {
            ArrayList list = new ArrayList();

            foreach (Mobile tolist in new ArrayList(c_Mobiles))
            {
                if (tolist.NetState == null)
                    continue;

                if (m.AccessLevel < tolist.AccessLevel && !c_ShowStaff)
                    continue;

                if (Data.GetData(tolist).Status == OnlineStatus.Hidden && tolist.AccessLevel >= m.AccessLevel)
                    continue;

                if (c_Style == ChatStyle.Regional && tolist.Region != m.Region)
                    continue;

                list.Add(tolist);
            }

            return list;
        }

        protected void Save(GenericWriter writer)
        {
            writer.Write(1); // Version

            writer.WriteMobileList(c_Mobiles, true);
            writer.Write(c_Filter);
            writer.Write(c_Delay);
            writer.Write(c_Name);
            writer.Write((int)c_Style);
            writer.Write(c_ToIrc);
            writer.Write(c_NewChars);
            writer.Write(c_ShowStaff);
            writer.Write(c_Enabled);

            writer.Write(c_Colors.Count);
            foreach(Mobile m in c_Colors.Keys)
            {
                writer.Write(m);
                writer.Write((int)c_Colors[m]);
            }

            writer.Write(c_Commands.Count);
            foreach (string str in c_Commands)
                writer.Write(str);
        }

        protected void Load(GenericReader reader)
        {
            int version = reader.ReadInt();

            c_Mobiles = reader.ReadMobileList();
            c_Filter = reader.ReadBool();
            c_Delay = reader.ReadBool();
            c_Name = reader.ReadString();
            c_Style = (ChatStyle)reader.ReadInt();
            c_ToIrc = reader.ReadBool();
            c_NewChars = reader.ReadBool();
            c_ShowStaff = reader.ReadBool();
            c_Enabled = reader.ReadBool();

            int count = reader.ReadInt();
            Mobile m;
            for (int i = 0; i < count; ++i)
            {
                m = reader.ReadMobile();
                if (m != null)
                    c_Colors[m] = reader.ReadInt();
                else
                    reader.ReadInt();
            }

            c_Commands.Clear();
            count = reader.ReadInt();
            for (int i = 0; i < count; ++i)
                c_Commands.Add(reader.ReadString());

            foreach (string str in c_Commands)
                AddCommand(str);

            ArrayList list = new ArrayList();
            foreach(Mobile mob in c_Mobiles)
                if (!list.Contains(mob))
                    list.Add(mob);

            c_Mobiles = new ArrayList(list);
        }

        #endregion

        #region Internal Classes

        public class ChatHistory
        {
            private Mobile c_Mobile;
            private string c_Txt;

            public Mobile Mobile { get { return c_Mobile; } }
            public string Txt { get { return c_Txt; } }

            public ChatHistory(Mobile m, string txt)
            {
                c_Mobile = m;
                c_Txt = txt;
            }
        }

        #endregion
    }
}