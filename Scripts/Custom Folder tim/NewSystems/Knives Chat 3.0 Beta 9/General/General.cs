// Reload help contents

using System;
using System.Collections;
using System.IO;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Knives.Chat3
{
    public class General
    {
        private static string s_Version = "3.0 Beta 9";
        private static string s_SavePath = "Saves/ChatBeta8";
        private static DateTime s_ReleaseDate = new DateTime(2007, 3, 1);

        public static string Version { get { return s_Version; } }
        public static string SavePath { get { return s_SavePath; } }
        public static DateTime ReleaseDate { get { return s_ReleaseDate; } }

        private static ArrayList s_Locals = new ArrayList();

        private static Hashtable s_Help = new Hashtable();
        public static Hashtable Help { get { return s_Help; } }

        public static void Configure()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler(OnLoad);
            EventSink.WorldSave += new WorldSaveEventHandler(OnSave);
        }

        public static void Initialize()
        {
            EventSink.Speech += new SpeechEventHandler(OnSpeech);
            EventSink.Login += new LoginEventHandler(OnLogin);
            EventSink.CharacterCreated += new CharacterCreatedEventHandler(OnCreate);
        }

        private static void OnLoad()
        {
            LoadLocalFile();
            LoadHelpFile();
            LoadFilterFile();
            Data.Load();
            Channel.Load();
            GumpInfo.Load();

            if (Data.IrcAutoConnect)
                IrcConnection.Connection.Connect();
        }

        private static void OnSave(WorldSaveEventArgs args)
        {
            DateTime time = DateTime.Now;
            if (Data.Debug)
            {
                Console.WriteLine("");
                Console.WriteLine(General.Local(241));
            }

            Data.Save();
            Channel.Save();
            GumpInfo.Save();

            foreach (Data data in Data.Datas.Values)
                if (data.SevenDays)
                    foreach (Message msg in new ArrayList(data.Messages))
                        if (msg.Received < DateTime.Now - TimeSpan.FromDays(7))
                            data.Messages.Remove(msg);

            if (Data.Debug)
            {
                TimeSpan elapsed = DateTime.Now - time;
                Console.WriteLine(General.Local(240) + " {0}", (elapsed.Minutes != 0 ? elapsed.Minutes + " minutes" : "") + (elapsed.Seconds != 0 ? elapsed.Seconds + " seconds" : "") + elapsed.Milliseconds + " milliseconds");
            }
        }

        private static void OnSpeech(SpeechEventArgs args)
        {
            if (Data.GetData(args.Mobile).Recording is SendMessageGump)
            {
                if (!args.Mobile.HasGump(typeof(SendMessageGump)))
                {
                    Data.GetData(args.Mobile).Recording = null;
                    return;
                }

                args.Mobile.CloseGump(typeof(SendMessageGump));
                ((SendMessageGump)Data.GetData(args.Mobile).Recording).AddText(" " + args.Speech);
                args.Handled = true;
                args.Speech = "";
                return;
            }

            if (Data.FilterSpeech)
                args.Speech = Filter.FilterText(args.Mobile, args.Speech, false);

            foreach (Data data in Data.Datas.Values)
                if (data.Mobile.AccessLevel >= args.Mobile.AccessLevel && ((data.GlobalW && !data.GIgnores.Contains(args.Mobile)) || data.GListens.Contains(args.Mobile)) && !data.Mobile.InRange(args.Mobile.Location, 10))
                    data.Mobile.SendMessage(data.GlobalWC, String.Format("(Global) <World> {0}: {1}", args.Mobile.RawName, args.Speech));
        }

        private static void OnLogin(LoginEventArgs args)
        {
            if (Data.GetData(args.Mobile).NewMsg())
                PmNotify(args.Mobile);

            if(!Data.GetData(args.Mobile).WhenFull)
                args.Mobile.SendMessage(Data.GetData(args.Mobile).SystemC, General.Local(258), Data.GetData(args.Mobile).Messages.Count, Data.MaxMsgs);

            foreach (Data data in Data.Datas.Values)
            {
                if (data.Friends.Contains(args.Mobile) && data.FriendAlert)
                    data.Mobile.SendMessage(data.SystemC, args.Mobile.RawName + " " + Local(173));
            }
        }

        private static void OnCreate(CharacterCreatedEventArgs args)
        {
            if (args.Mobile == null)
                return;

            Data data = Data.GetData(args.Mobile);

            foreach (Channel c in Channel.Channels)
                if (c.NewChars)
                    c.Join(args.Mobile);
        }

        public static void LoadLocalFile()
        {
            s_Locals.Clear();

            if (File.Exists("Data/ChatLocal.txt"))
            {
                using (FileStream bin = new FileStream("Data/ChatLocal.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(bin);
                    string text = "";

                    while (true)
                    {
                        try
                        {
                            text = reader.ReadLine();

                            if (text.IndexOf("//") == 0 || text.Trim().Length == 0)
                                continue;

                            if (text == "EndOfFile")
                                break;

                            s_Locals.Add(text);
                        }
                        catch { break; }
                    }

                    reader.Close();
                }
            }

            if (s_Locals.Count == 0)
                s_Locals = DefaultLocal.Load();
        }

        public static void LoadHelpFile()
        {
            s_Help.Clear();

            if (File.Exists("Data/HelpContents.txt"))
            {
                using (FileStream bin = new FileStream("Data/HelpContents.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(bin);
                    string name = "";
                    string text = "";

                    while (true)
                    {
                        try
                        {
                            name = reader.ReadLine();

                            if (name.IndexOf("//") == 0 || name.Trim().Length == 0)
                                continue;

                            if (name == "EndOfFile")
                                break;

                            text = reader.ReadLine();

                            if (text.IndexOf("//") == 0 || text.Trim().Length == 0)
                                continue;

                            if (text == "EndOfFile")
                                break;

                            s_Help[name] = text;
                        }
                        catch { break; }
                    }

                    reader.Close();
                }
            }
        }

        public static string Local(int num)
        {
            if (num < 0 || num >= s_Locals.Count)
                return "Local Error";

            return s_Locals[num].ToString();
        }

        public static string GetHelp(string str)
        {
            if (s_Help[str] == null)
                return "Help Error";

            return s_Help[str].ToString();
        }

        public static void LoadFilterFile()
        {
            if (File.Exists("Data/ChatFilters.txt"))
            {
                using (FileStream bin = new FileStream("Data/ChatFilters.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(bin);
                    string text = "";

                    while (true)
                    {
                        try
                        {
                            text = reader.ReadLine();

                            if (text.IndexOf("//") == 0 || text.Trim().Length == 0)
                                continue;

                            if (text == "EndOfFile")
                                break;

                            if (!Data.Filters.Contains(text.ToLower()))
                                Data.Filters.Add(text.ToLower());
                        }
                        catch { break; }
                    }

                    reader.Close();
                }
            }
        }

        public static void LoadBacksFile()
        {
            if (File.Exists("Data/ChatBacks.txt"))
            {
                using (FileStream bin = new FileStream("Data/ChatBacks.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(bin);
                    string text = "";

                    while (true)
                    {
                        try
                        {
                            text = reader.ReadLine();

                            if (text.IndexOf("//") == 0 || text.Trim().Length == 0)
                                continue;

                            if (text == "EndOfFile")
                                break;

                            if(!GumpInfo.Backgrounds.Contains(Utility.ToInt32(text)))
                                GumpInfo.Backgrounds.Add(Utility.ToInt32(text));
                        }
                        catch { break; }
                    }

                    reader.Close();
                }
            }
        }

        public static void LoadColorsFile()
        {
            if (File.Exists("Data/ChatColors.txt"))
            {
                using (FileStream bin = new FileStream("Data/ChatColors.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(bin);
                    string text = "";

                    while (true)
                    {
                        try
                        {
                            text = reader.ReadLine();

                            if (text.IndexOf("//") == 0 || text.Trim().Length == 0)
                                continue;

                            if (text == "EndOfFile")
                                break;

                            if (!GumpInfo.TextColors.Contains(text))
                                GumpInfo.TextColors.Add(text);
                        }
                        catch { break; }
                    }

                    reader.Close();
                }
            }
        }

        public static void LoadAvatarFile()
        {
            if (File.Exists("Data/ChatAvatars.txt"))
            {
                using (FileStream bin = new FileStream("Data/ChatAvatars.txt", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StreamReader reader = new StreamReader(bin);
                    string id = "";
                    string xoff = "";
                    string yoff = "";

                    while (true)
                    {
                        try
                        {
                            id = reader.ReadLine();

                            if (id.IndexOf("//") == 0 || id.Trim().Length == 0)
                                continue;

                            if (id == "EndOfFile")
                                break;

                            xoff = reader.ReadLine();
                            yoff = reader.ReadLine();

                            if (!Avatar.Avatars.Contains(Utility.ToInt32(id)))
                                new Avatar(Utility.ToInt32(id), Utility.ToInt32(xoff), Utility.ToInt32(yoff));
                        }
                        catch { break; }
                    }

                    reader.Close();
                }
            }
        }

        public static void List(Mobile m, int page)
        {
            switch (Data.GetData(m).MenuSkin)
            {
                case Skin.Three:
                    new ListGump(m, page);
                    break;
                case Skin.Two:
                    new ListGump20(m, page);
                    break;
                default:
                    new ListGump10(m, page);
                    break;
            }
        }

        public static void List(Mobile m, Mobile targ)
        {
            switch (Data.GetData(m).MenuSkin)
            {
                case Skin.Three:
                    new ListGump(m, targ);
                    break;
                case Skin.Two:
                    new ListGump20(m, targ);
                    break;
                default:
                    break;
            }
        }

        public static void PmNotify(Mobile m)
        {
            switch (Data.GetData(m).MenuSkin)
            {
                case Skin.Three:
                    new PmNotifyGump(m);
                    break;
                case Skin.Two:
                    new PmNotifyGump20(m);
                    break;
                default:
                    new PmNotifyGump10(m);
                    break;
            }
        }

        public static bool IsInFaction(Mobile m)
        {
            if (m == null || !m.Player)
                return false;

            return (((PlayerMobile)m).FactionPlayerState != null && ((PlayerMobile)m).FactionPlayerState.Faction != null);
        }

        public static string FactionName(Mobile m)
        {
            if (!IsInFaction(m))
                return "";

            return ((PlayerMobile)m).FactionPlayerState.Faction.Definition.PropName;
        }

        public static string FactionTitle(Mobile m)
        {
            if (!IsInFaction(m))
                return "";

            return ((PlayerMobile)m).FactionPlayerState.Rank.Title;
        }
    }
}