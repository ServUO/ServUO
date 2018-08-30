using System;
using System.Collections.Generic;
using System.IO;
using Server.Network;
using System.Drawing;
using Server.Gumps;
using Server.Items;
using Server.Engines.Craft;

namespace Server.Customs.MessageLog
{
    class MessageLog
    {
        private const string m_File = @"Saves\ServerMessages.bin";
        private static List<LogObject> logs;

        public static void Configure()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler(EventSink_WorldLoad);
            EventSink.WorldSave += new WorldSaveEventHandler(EventSink_WorldSave);
            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        static void EventSink_Login(LoginEventArgs e)
        {
            List<LogObject> toremove = new List<LogObject>();
            foreach (LogObject l in logs)
            {
                if (l.mob == e.Mobile)
                {
                    if (l.mob.Deleted) break;
                    SendGump(l.mob, l.message);
                    toremove.Add(l);
                    break;
                }

                if (l.mob.Deleted) toremove.Add(l);
            }

            foreach (LogObject l in toremove)
                logs.Remove(l);
        }

        static void EventSink_WorldSave(WorldSaveEventArgs e)
        {
            List<LogObject> toremove = new List<LogObject>();
            foreach (LogObject l in logs)
                if (l.mob.Deleted) toremove.Add(l);

            foreach (LogObject l in toremove)
                logs.Remove(l);


            BinaryFileWriter bn = new BinaryFileWriter(m_File, true);
            
            bn.Write((int)logs.Count);
            for (int i = 0; i < logs.Count; i++)
            {
                bn.Write(logs[i].mob);
                bn.Write(logs[i].message);
            }
            bn.Close();

        }

        static void EventSink_WorldLoad()
        {
            logs = new List<LogObject>();
            if (!File.Exists(m_File)) return;
            if (new FileInfo(m_File).Length <= 3) return;
            BinaryFileReader br = new BinaryFileReader(new BinaryReader(new FileStream(m_File, FileMode.OpenOrCreate, FileAccess.Read)));
            int read = br.ReadInt();
            for (int i = 0; i < read; i++)
            {
                logs.Add(new LogObject(br.ReadMobile(), br.ReadString()));
            }
        }

        public static void Log(Mobile mob, string message)
        {
            foreach (NetState ns in NetState.Instances)
            {
                if (ns.Mobile == mob)
                {
                    SendGump(mob, message);
                    return;
                }
            }
            logs.Add(new LogObject(mob, message));
        }

        public static void SendGump(Mobile m, string message)
        {
            Gump g = new Gump(10, 10);
            int xo = (640 - 340) / 2, yo = (480 - 180) / 2;
            g.Closable = false;

            g.AddPage(0);
            g.AddPage(1);
            g.AddBackground(0, 0, 92, 75, 0xA3C);
            g.AddImageTiled(5, 7, 82, 61, 0xA40);
            g.AddAlphaRegion(5, 7, 82, 61);
            g.AddImageTiled(9, 11, 21, 53, 0xBBC);
            g.AddButton(10, 12, 0x7D2, 0x7D2, 2, GumpButtonType.Page, 2);
            g.AddHtmlLocalized(34, 28, 65, 24, 3001002, 0xFFFFFF, false, false); // Message
            g.AddPage(2);
            g.AddBackground(xo + 0, yo + 0, 340, 180, 5054);
            g.AddImageTiled(xo + 10, yo + 10, 340 - 20, 20, 2624);
            g.AddAlphaRegion(xo + 10, yo + 10, 340 - 20, 20);
            g.AddHtmlLocalized(xo + 10, yo + 10, 340 - 20, 20, 1074862, 0xffffff, false, false);
            g.AddImageTiled(xo + 10, yo + 40, 340 - 20, 180 - 80, 2624);
            g.AddAlphaRegion(xo + 10, yo + 40, 340 - 20, 180 - 80);
            g.AddHtml(xo + 10, yo + 40, 340 - 20, 180 - 80, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", 0xffffff, message), false, true);
            g.AddImageTiled(xo + 10, yo + 180 - 30, 340 - 20, 20, 2624);
            g.AddAlphaRegion(xo + 10, yo + 180 - 30, 340 - 20, 20);
            g.AddButton(xo + 10, yo + 180 - 30, 4005, 4007, 1, GumpButtonType.Reply, 0);
            g.AddHtmlLocalized(xo + 40, yo + 180 - 30, 120, 20, 1011036, 32767, false, false); // OKAY
            m.SendGump(g);
        }

        private class LogObject
        {
            public string message;
            public Mobile mob;

            public LogObject(Mobile m, string s)
            {
                message = s;
                mob = m;
            }
        }
    }
}
