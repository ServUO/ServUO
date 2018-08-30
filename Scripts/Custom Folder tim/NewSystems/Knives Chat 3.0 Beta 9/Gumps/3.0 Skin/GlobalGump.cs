using System;
using Server;

namespace Knives.Chat3
{
    public class GlobalGump : GumpPlus
    {
        private Mobile c_Target;

        public Mobile Current { get { return (c_Target == null ? Owner : c_Target); } }

        public GlobalGump(Mobile m, Mobile targ)
            : base(m, 100, 100)
        {
            m.CloseGump(typeof(GlobalGump));

            c_Target = targ;
        }

        public GlobalGump(Mobile m)
            : this(m, null)
        {
        }

        protected override void BuildGump()
        {
            int width = 200;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(226));
            AddImage(width / 2 - 80, y + 2, 0x39);
            AddImage(width / 2 + 50, y + 2, 0x3B);

            if (c_Target != null)
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(224) + " " + c_Target.RawName);

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(43) + " | " + General.Local(10));
            AddButton(width / 2 - 70, y + 3, Data.GetData(Current).Global ? 0x939 : 0x2716, "Global", new GumpCallback(Global));
            AddButton(width / 2 + 60, y + 3, !Data.GetData(Current).Global ? 0x939 : 0x2716, "Listen", new GumpCallback(Listen));

            if (Data.GetData(Current).Global)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(44));
                AddButton(width / 2 - 70, y, Data.GetData(Current).GlobalC ? 0x2343 : 0x2342, "Chat", new GumpCallback(GlobalChat));
                AddButton(width / 2 + 50, y, Data.GetData(Current).GlobalC ? 0x2343 : 0x2342, "Chat", new GumpCallback(GlobalChat));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(45));
                AddButton(width / 2 - 70, y, Data.GetData(Current).GlobalW ? 0x2343 : 0x2342, "World", new GumpCallback(GlobalWorld));
                AddButton(width / 2 + 50, y, Data.GetData(Current).GlobalW ? 0x2343 : 0x2342, "World", new GumpCallback(GlobalWorld));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(26));
                AddButton(width / 2 - 70, y, Data.GetData(Current).GlobalM ? 0x2343 : 0x2342, "Msg", new GumpCallback(GlobalMsg));
                AddButton(width / 2 + 50, y, Data.GetData(Current).GlobalM ? 0x2343 : 0x2342, "Msg", new GumpCallback(GlobalMsg));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(192));
                AddButton(width / 2 - 70, y, Data.GetData(Current).GlobalG ? 0x2343 : 0x2342, "Guild", new GumpCallback(GlobalGuild));
                AddButton(width / 2 + 50, y, Data.GetData(Current).GlobalG ? 0x2343 : 0x2342, "Guild", new GumpCallback(GlobalGuild));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(193));
                AddButton(width / 2 - 70, y, Data.GetData(Current).GlobalF ? 0x2343 : 0x2342, "Faction", new GumpCallback(GlobalFaction));
                AddButton(width / 2 + 50, y, Data.GetData(Current).GlobalF ? 0x2343 : 0x2342, "Faction", new GumpCallback(GlobalFaction));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(159));
                AddButton(width / 2 - 70, y, Data.GetData(Current).IrcRaw ? 0x2343 : 0x2342, "Irc Raw", new GumpCallback(IrcRaw));
                AddButton(width / 2 + 50, y, Data.GetData(Current).IrcRaw ? 0x2343 : 0x2342, "Irc Raw", new GumpCallback(IrcRaw));
            }

            AddBackgroundZero(0, 0, width, y+40, Data.GetData(Current).DefaultBack);
        }

        private void Global()
        {
            Data.GetData(Current).Global = true;
            NewGump();
        }

        private void Listen()
        {
            Data.GetData(Current).Global = false;
            NewGump();
        }

        private void GlobalChat()
        {
            Data.GetData(Current).GlobalC = !Data.GetData(Current).GlobalC;
            NewGump();
        }

        private void GlobalWorld()
        {
            Data.GetData(Current).GlobalW = !Data.GetData(Current).GlobalW;
            NewGump();
        }

        private void GlobalMsg()
        {
            Data.GetData(Current).GlobalM = !Data.GetData(Current).GlobalM;
            NewGump();
        }

        private void GlobalGuild()
        {
            Data.GetData(Current).GlobalG = !Data.GetData(Current).GlobalG;
            NewGump();
        }

        private void GlobalFaction()
        {
            Data.GetData(Current).GlobalF = !Data.GetData(Current).GlobalF;
            NewGump();
        }

        private void IrcRaw()
        {
            Data.GetData(Current).IrcRaw = !Data.GetData(Current).IrcRaw;
            NewGump();
        }
    }
}