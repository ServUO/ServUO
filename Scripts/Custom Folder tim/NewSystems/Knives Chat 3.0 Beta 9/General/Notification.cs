using System;
using System.IO;
using System.Collections;
using Server;
using Server.Network;

namespace Knives.Chat3
{
    public enum MacroPenalty { None, Kick }

    public class Notification
    {
        private string c_Text, c_Name;
        private bool c_Gump, c_AntiMacro;
        private TimeSpan c_Recur = TimeSpan.Zero;
        private Timer c_Timer;

        public string Text { get { return c_Text; } set { c_Text = value; } }
        public string Name { get { return c_Name; } set { c_Name = value; } }
        public bool Gump { get { return c_Gump; } set { c_Gump = value; } }
        public bool AntiMacro { get { return c_AntiMacro; } set { c_AntiMacro = value; } }
        public TimeSpan Recur { get { return c_Recur; } set { c_Recur = value; StartNotify(); } }

        public Notification()
        {
            c_Name = "New";

            Data.Notifications.Add(this);
        }

        private void StartNotify()
        {
            if (c_Timer != null)
                c_Timer.Stop();

            if (c_Recur != TimeSpan.Zero && Data.Notifications.Contains(this))
                c_Timer = Timer.DelayCall(c_Recur+TimeSpan.FromSeconds(Utility.Random(20)), new TimerCallback(Notify));
        }

        private void Notify()
        {
            StartNotify();

            if (c_Gump)
            {
                foreach (NetState ns in NetState.Instances)
                    if (ns.Mobile != null)
                        new NotAlertGump(ns.Mobile, this);
            }
            else
            {
                foreach (NetState ns in NetState.Instances)
                    if (ns.Mobile != null)
                        ns.Mobile.SendMessage(Data.GetData(ns.Mobile).SystemC, c_Text);
            }
        }

        public void Save(GenericWriter writer)
        {
            writer.Write(0); // Version

            writer.Write(c_Text);
            writer.Write(c_Name);
            writer.Write(c_Gump);
            writer.Write(c_AntiMacro);
            writer.Write(c_Recur);
        }

        public void Load(GenericReader reader)
        {
            int version = reader.ReadInt();

            c_Text = reader.ReadString();
            c_Name = reader.ReadString();
            c_Gump = reader.ReadBool();
            c_AntiMacro = reader.ReadBool();
            c_Recur = reader.ReadTimeSpan();

            StartNotify();
        }

        private class NotAlertGump : GumpPlus
        {
            private Notification c_Not;
            private Timer c_Timer;

            public NotAlertGump(Mobile m, Notification not)
                : base(m, 300, 50)
            {
                c_Not = not;

                if(c_Not.AntiMacro)
                    c_Timer = Timer.DelayCall(TimeSpan.FromSeconds(Data.AntiMacroDelay), new TimerCallback(AntiMacro));
            }

            protected override void BuildGump()
            {
                AddImage(0, 0, 0x15D5, Data.GetData(Owner).SystemC);
                AddButton(13, 13, 0xFC4, "Open", new GumpCallback(Open));
            }

            private void AntiMacro()
            {
                switch (Data.MacroPenalty)
                {
                    case MacroPenalty.None:
                        Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(276));
                        break;
                    case MacroPenalty.Kick:
                        Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(279));
                        Owner.NetState.Dispose();
                        break;
                }

                foreach (Data data in Data.Datas.Values)
                    if (data.Mobile.AccessLevel >= AccessLevel.GameMaster)
                        data.AddMessage(new Message(data.Mobile, General.Local(277), Owner.RawName + General.Local(278), MsgType.System));
            }

            private void Open()
            {
                new NotMessageGump(Owner, c_Not);
                if (c_Timer != null)
                    c_Timer.Stop();
            }
        }

        private class NotMessageGump : GumpPlus
        {
            private Notification c_Not;

            public NotMessageGump(Mobile m, Notification not)
                : base(m, 100, 100)
            {
                c_Not = not;
            }

            protected override void BuildGump()
            {
                int width = 200;
                int y = 10;

                AddHtml(50, y, 100, 45, "<CENTER>" + General.Local(271), false, false);
                AddImage(width / 2 - 80, y + 7, 0x39);
                AddImage(width / 2 + 50, y + 7, 0x3B);

                AddHtml(20, y += 45, width - 40, 80, HTML.Black + c_Not.Text, true, true);

                AddBackgroundZero(0, 0, width, y += 100, Data.GetData(Owner).DefaultBack);
            }
        }
    }
}