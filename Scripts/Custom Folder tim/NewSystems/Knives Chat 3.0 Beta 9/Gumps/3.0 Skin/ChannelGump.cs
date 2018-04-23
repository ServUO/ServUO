using System;
using Server;

namespace Knives.Chat3
{
    public class ChannelGump : GumpPlus
    {
        private Channel c_Channel;
        protected Channel Channel { get { return c_Channel; } set { c_Channel = value; } }

        public ChannelGump(Mobile m)
            : this(m, null)
        {
        }

        public ChannelGump(Mobile m, Channel c)
            : base(m, 100, 100)
        {
            m.CloseGump(typeof(ChannelGump));

            c_Channel = c;
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(177));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddImage(width/2-73, (y += 25) - 1, 0x9C5);
            AddHtml(width/2-50, y, 100, "<CENTER>" + (c_Channel == null ? "" : c_Channel.Name));
            AddButton(width / 2 - 65, y + 3, 0x2716, "Channel Select", new GumpCallback(ChannelSelect));
            AddButton(width / 2 + 52, y + 3, 0x2716, "Channel Select", new GumpCallback(ChannelSelect));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(178));
            AddButton(width/2-60, y+3, 0x2716, "New Channel", new GumpCallback(NewChannel));
            AddButton(width/2+50, y+3, 0x2716, "New Channel", new GumpCallback(NewChannel));

            if (c_Channel == null)
            {
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(179));
                AddHtml(0, y += 25, width, 61, "<CENTER>" + General.Local(219), false, false);
                AddBackgroundZero(0, 0, width, y + 100, Data.GetData(Owner).DefaultBack);
                return;
            }

            if (c_Channel.GetType() == typeof(Channel))
            {
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(211));
                AddButton(width / 2 - 65, y + 1, 0x5686, 0x5687, "Delete Channel", new GumpCallback(DeleteChannel));
                AddButton(width / 2 + 50, y + 1, 0x5686, 0x5687, "Delete Channel", new GumpCallback(DeleteChannel));
            }

            AddHtml(0, y += 25, width/2-10, "<DIV ALIGN=RIGHT>" + General.Local(180));
            AddTextField(width / 2 + 10, y, 70, 21, 0x480, 0xBBA, "Channel Name", "" + c_Channel.Name);
            AddButton(width/2-5, y + 3, 0x2716, "Channel Name", new GumpCallback(ChannelName));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(212));
            AddButton(width / 2 - 100, y, c_Channel.Enabled ? 0x2343 : 0x2342, "Enable", new GumpCallback(Enable));
            AddButton(width / 2 + 80, y, c_Channel.Enabled ? 0x2343 : 0x2342, "Enable", new GumpCallback(Enable));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(182) + " | " + General.Local(183));
            AddButton(width/2-100, y + 3, c_Channel.Style == ChatStyle.Global ? 0x939 : 0x2716, "Global", new GumpCallback(Global));
            AddButton(width/2+90, y + 3, c_Channel.Style == ChatStyle.Regional ? 0x939 : 0x2716, "Regional", new GumpCallback(Regional));

            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(223));
            AddButton(width / 2 - 100, y, c_Channel.ShowStaff ? 0x2343 : 0x2342, "Show Staff", new GumpCallback(ShowStaff));
            AddButton(width / 2 + 80, y, c_Channel.ShowStaff ? 0x2343 : 0x2342, "Show Staff", new GumpCallback(ShowStaff));

            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(184));
            AddButton(width / 2 - 100, y, c_Channel.ToIrc ? 0x2343 : 0x2342, "Send to IRC", new GumpCallback(SendToIrc));
            AddButton(width / 2 + 80, y, c_Channel.ToIrc ? 0x2343 : 0x2342, "Send to IRC", new GumpCallback(SendToIrc));

            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(188));
            AddButton(width / 2 - 100, y, c_Channel.NewChars ? 0x2343 : 0x2342, "Auto join new characters", new GumpCallback(AutoNewChars));
            AddButton(width / 2 + 80, y, c_Channel.NewChars ? 0x2343 : 0x2342, "Auto join new characters", new GumpCallback(AutoNewChars));

            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(208));
            AddButton(width / 2 - 100, y, c_Channel.Filter ? 0x2343 : 0x2342, "Apply Filter", new GumpCallback(ApplyFilter));
            AddButton(width / 2 + 80, y, c_Channel.Filter ? 0x2343 : 0x2342, "Apply Filter", new GumpCallback(ApplyFilter));

            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(220));
            AddButton(width/2-100, y, c_Channel.Delay ? 0x2343 : 0x2342, "Apply Delay", new GumpCallback(ApplyDelay));
            AddButton(width/2+80, y, c_Channel.Delay ? 0x2343 : 0x2342, "Apply Delay", new GumpCallback(ApplyDelay));

            AddHtml(0, y += 25, width/2-10, "<DIV ALIGN=RIGHT>" + General.Local(185));
            AddTextField(width/2+10, y, 70, 21, 0x480, 0xBBA, "Add/Remove", "");
            AddButton(width/2-5, y + 4, 0x2716, "Add/Remove Command", new GumpCallback(AddCommand));

            string txt = General.Local(42) + ": ";

            foreach (string str in c_Channel.Commands)
                txt += str + " ";

            AddHtml(20, y += 25, width-40, 60, txt, false, false);

            AddBackgroundZero(0, 0, width, y+100, Data.GetData(Owner).DefaultBack);
        }

        private void ChannelSelect()
        {
            new ChannelSelectGump(this);
        }

        private void NewChannel()
        {
            c_Channel = new Channel("New");

            NewGump();
        }

        private void DeleteChannel()
        {
            if (c_Channel == null)
                return;

            foreach (string str in c_Channel.Commands)
                Channel.RemoveCommand(str);
            Channel.Channels.Remove(c_Channel);

            c_Channel = null;

            NewGump();
        }

        private void AutoNewChars()
        {
            if (c_Channel == null)
                return;

            c_Channel.NewChars = !c_Channel.NewChars;

            if (c_Channel.NewChars)
                foreach (Data data in Data.Datas.Values)
                    c_Channel.Join(data.Mobile);

            NewGump();
        }

        private void Enable()
        {
            if (c_Channel != null)
                c_Channel.Enabled = !c_Channel.Enabled;

            NewGump();
        }

        private void ShowStaff()
        {
            if (c_Channel != null)
                c_Channel.ShowStaff = !c_Channel.ShowStaff;

            NewGump();
        }

        private void ApplyFilter()
        {
            if (c_Channel != null)
                c_Channel.Filter = !c_Channel.Filter;

            NewGump();
        }

        private void ApplyDelay()
        {
            if (c_Channel != null)
                c_Channel.Delay = !c_Channel.Delay;

            NewGump();
        }

        private void ChannelName()
        {
            if(c_Channel != null)
                c_Channel.Name = GetTextField("Channel Name");

            NewGump();
        }

        private void AddCommand()
        {
            if (GetTextField("Add/Remove") == "")
            {
                NewGump();
                return;
            }

            if (c_Channel.Commands.Contains(GetTextField("Add/Remove").ToLower()))
            {
                c_Channel.Commands.Remove(GetTextField("Add/Remove").ToLower());
                Channel.RemoveCommand(GetTextField("Add/Remove").ToLower());
            }
            else
            {
                c_Channel.Commands.Add(GetTextField("Add/Remove").ToLower());
                Channel.AddCommand(GetTextField("Add/Remove").ToLower());
            }

            NewGump();
        }

        private void Global()
        {
            if(c_Channel != null)
                c_Channel.Style = ChatStyle.Global;

            NewGump();
        }

        private void Regional()
        {
            if(c_Channel != null)
                c_Channel.Style = ChatStyle.Regional;

            NewGump();
        }

        private void SendToIrc()
        {
            if(c_Channel != null)
                c_Channel.ToIrc = !c_Channel.ToIrc;

            NewGump();
        }


        private class ChannelSelectGump : GumpPlus
        {
            private ChannelGump c_Gump;

            public ChannelSelectGump(ChannelGump g)
                : base(g.Owner, 100, 100)
            {
                c_Gump = g;
            }

            protected override void BuildGump()
            {
                int width = 200;
                int y = 10;

                AddHtml(0, y, width, "<CENTER>" + General.Local(38));
                AddImage(width / 2 - 70, y + 2, 0x39);
                AddImage(width / 2 + 40, y + 2, 0x3B);

                foreach (Channel c in Channel.Channels)
                {
                    AddHtml(0, y += 20, width, "<CENTER>" + c.NameFor(Owner));
                    AddButton(width / 2 - 60, y + 3, 0x2716, "Select Channel", new GumpStateCallback(SelectChannel), c);
                    AddButton(width / 2 + 50, y + 3, 0x2716, "Select Channel", new GumpStateCallback(SelectChannel), c);
                }

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(Owner).DefaultBack);
            }

            private void SelectChannel(object o)
            {
                Channel c = o as Channel;

                if (c == null)
                    return;

                c_Gump.Channel = c;

                c_Gump.NewGump();
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }
    }
}