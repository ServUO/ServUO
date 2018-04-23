using System;
using Server;

namespace Knives.Chat3
{
    public class IrcGump : GumpPlus
    {
        public IrcGump(Mobile m)
            : base(m, 100, 100)
        {
            m.CloseGump(typeof(IrcGump));
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(100));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(177));
            AddButton(width / 2 - 80, y, 0x2716, "Channel Options", new GumpCallback(ChannelOptions));
            AddButton(width / 2 + 60, y, 0x2716, "Channel Options", new GumpCallback(ChannelOptions));

            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(98));
            AddButton(width/2-80, y+3, Data.IrcEnabled ? 0x2343 : 0x2342, "IRC Enabled", new GumpCallback(IrcEnabled));
            AddButton(width/2+60, y+3, Data.IrcEnabled ? 0x2343 : 0x2342, "IRC Enabled", new GumpCallback(IrcEnabled));

            if (!Data.IrcEnabled)
            {
                AddBackgroundZero(0, 0, width, y + 100, Data.GetData(Owner).DefaultBack);
                return;
            }

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(115));
            AddButton(width/2-80, y, Data.IrcAutoConnect ? 0x2343 : 0x2342, "IRC Auto Connect", new GumpCallback(IrcAutoConnect));
            AddButton(width/2+60, y, Data.IrcAutoConnect ? 0x2343 : 0x2342, "IRC Auto Connect", new GumpCallback(IrcAutoConnect));

            AddHtml(0, y+=20, width, "<CENTER>" + General.Local(116));
            AddButton(width / 2 - 80, y, Data.IrcAutoReconnect ? 0x2343 : 0x2342, "IRC Auto Reconnect", new GumpCallback(IrcAutoReconnect));
            AddButton(width / 2 + 60, y, Data.IrcAutoReconnect ? 0x2343 : 0x2342, "IRC Auto Reconnect", new GumpCallback(IrcAutoReconnect));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(138) + ": " + General.Local(121 + (int)Data.IrcStaffColor));
            AddButton(width / 2 - 80, y + 4, 0x2716, "Irc Staff Color", new GumpCallback(IrcStaffColor));
            AddButton(width / 2 + 70, y + 4, 0x2716, "Irc Staff Color", new GumpCallback(IrcStaffColor));

            AddHtml(0, y += 25, width / 2 - 10, "<DIV ALIGN=RIGHT>" + General.Local(117));
            AddTextField(width / 2 + 10, y, 100, 21, 0x480, 0xBBA, "Nick", Data.IrcNick);
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            AddHtml(0, y += 20, width / 2 - 10, "<DIV ALIGN=RIGHT>" + General.Local(118));
            AddTextField(width / 2 + 10, y, 100, 21, 0x480, 0xBBA, "Server", Data.IrcServer);
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            AddHtml(0, y += 20, width / 2 - 10, "<DIV ALIGN=RIGHT>" + General.Local(119));
            AddTextField(width / 2 + 10, y, 100, 21, 0x480, 0xBBA, "Room", Data.IrcRoom);
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            AddHtml(0, y += 20, width / 2 - 10, "<DIV ALIGN=RIGHT>" + General.Local(120));
            AddTextField(width / 2 + 10, y, 70, 21, 0x480, 0xBBA, "Port", "" + Data.IrcPort);
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            int num = 139;

            if (IrcConnection.Connection.Connected)
                num = 141;
            if (IrcConnection.Connection.Connecting)
                num = 140;

            AddHtml(0, y += 40, width, "<CENTER>" + General.Local(num));
            AddButton(width / 2 - 60, y + 4, 0x2716, "Connect or Cancel or Close", new GumpCallback(ConnectCancelClose));
            AddButton(width / 2 + 50, y + 4, 0x2716, "Connect or Cancel or Close", new GumpCallback(ConnectCancelClose));

            AddBackgroundZero(0, 0, width, y+40, Data.GetData(Owner).DefaultBack);
        }

        private void ChannelOptions()
        {
            NewGump();
            new ChannelGump(Owner, Channel.GetByType(typeof(Irc)));
        }

        private void IrcEnabled()
        {
            Data.IrcEnabled = !Data.IrcEnabled;
            NewGump();
        }

        private void IrcAutoConnect()
        {
            Data.IrcAutoConnect = !Data.IrcAutoConnect;
            NewGump();
        }

        private void IrcAutoReconnect()
        {
            Data.IrcAutoReconnect = !Data.IrcAutoReconnect;
            NewGump();
        }

        private void Submit()
        {
            Data.IrcNick = GetTextField("Nick");
            Data.IrcServer = GetTextField("Server");
            Data.IrcRoom = GetTextField("Room");
            Data.IrcPort = GetTextFieldInt("Port");

            NewGump();
        }

        private void IrcStaffColor()
        {
            new IrcStaffColorGump(Owner, this);
        }

        private void ConnectCancelClose()
        {
            Data.IrcNick = GetTextField("Nick");
            Data.IrcServer = GetTextField("Server");
            Data.IrcRoom = GetTextField("Room");
            Data.IrcPort = GetTextFieldInt("Port");

            if (IrcConnection.Connection.Connected)
                IrcConnection.Connection.Disconnect(false);
            else if (IrcConnection.Connection.Connecting)
                IrcConnection.Connection.CancelConnect();
            else if (!IrcConnection.Connection.Connected)
                IrcConnection.Connection.Connect(Owner);

            NewGump();
        }


        private class IrcStaffColorGump : GumpPlus
        {
            private GumpPlus c_Gump;

            public IrcStaffColorGump(Mobile m, GumpPlus g)
                : base(m, 100, 100)
            {
                c_Gump = g;
            }

            protected override void BuildGump()
            {
                int width = 200;
                int y = 10;

                AddHtml(0, y, width, "<CENTER>" + General.Local(137));
                AddImage(width / 2 - 70, y + 2, 0x39);
                AddImage(width / 2 + 40, y + 2, 0x3B);

                y += 5;

                for (int i = 0; i < 16; ++i)
                {
                    AddHtml(0, y += 20, width, "<CENTER>" + General.Local(121 + i));
                    AddButton(width / 2 - 60, y + 3, 0x2716, "Select", new GumpStateCallback(Select), i);
                    AddButton(width / 2 + 50, y + 3, 0x2716, "Select", new GumpStateCallback(Select), i);
                }

                AddBackgroundZero(0, 0, 150, y + 40, Data.GetData(Owner).DefaultBack);
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }

            private void Select(object o)
            {
                if (!(o is int))
                    return;

                Data.IrcStaffColor = (IrcColor)(int)o;
                c_Gump.NewGump();
            }
        }
    }
}