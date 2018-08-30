// ALlow Pms over the Multi, track with name, server, serial
// Mc, MultiChat, change the channel options to go to Multi

using System;
using Server;

namespace Knives.Chat3
{
    public class MultiGump : GumpPlus
    {
        public MultiGump(Mobile m)
            : base(m, 100, 100)
        {
            m.CloseGump(typeof(MultiGump));
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(283));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(177));
            AddButton(width / 2 - 80, y+3, 0x2716, "Channel Options", new GumpCallback(ChannelOptions));
            AddButton(width / 2 + 60, y+3, 0x2716, "Channel Options", new GumpCallback(ChannelOptions));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(284));
            AddButton(width/2-80, y, Data.MultiMaster ? 0x2343 : 0x2342, "Multi Master", new GumpCallback(MultiMaster));
            AddButton(width/2+60, y, Data.MultiMaster ? 0x2343 : 0x2342, "Multi Master", new GumpCallback(MultiMaster));

            if (Data.MultiMaster)
            {
                if (!MultiConnection.Connection.Connected)
                {
                    AddHtml(0, y += 40, width, "<CENTER>" + General.Local(286));
                    AddButton(width / 2 - 60, y + 4, 0x2716, "Start", new GumpCallback(Start));
                    AddButton(width / 2 + 50, y + 4, 0x2716, "Start", new GumpCallback(Start));
                }
                else
                {
                    y += 10;

                    foreach (string name in MultiConnection.Connection.Names.Values)
                    {
                        AddHtml(35, y += 20, width - 35, name);
                        AddButton(width - 40, y, Data.MultiBlocks.Contains(name) ? 0x5687 : 0x5686, "Multi Block", new GumpStateCallback(MultiBlock), name);
                    }
                }

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(Owner).DefaultBack);
                return;
            }

            AddHtml(0, y += 25, width / 2 - 10, "<DIV ALIGN=RIGHT>" + General.Local(118));
            AddTextField(width / 2 + 10, y, 100, 21, 0x480, 0xBBA, "Server", Data.MultiServer);
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            AddHtml(0, y += 20, width / 2 - 10, "<DIV ALIGN=RIGHT>" + General.Local(120));
            AddTextField(width / 2 + 10, y, 70, 21, 0x480, 0xBBA, "Port", "" + Data.MultiPort);
            AddButton(width / 2 - 5, y + 4, 0x2716, "Submit", new GumpCallback(Submit));

            int num = 139;

            if (MultiConnection.Connection.Connected)
                num = 141;
            if (MultiConnection.Connection.Connecting)
                num = 140;

            AddHtml(0, y += 40, width, "<CENTER>" + General.Local(num));
            AddButton(width / 2 - 60, y + 4, 0x2716, "Connect or Cancel or Close", new GumpCallback(ConnectCancelClose));
            AddButton(width / 2 + 50, y + 4, 0x2716, "Connect or Cancel or Close", new GumpCallback(ConnectCancelClose));

            AddBackgroundZero(0, 0, width, y+40, Data.GetData(Owner).DefaultBack);
        }

        private void ChannelOptions()
        {
            NewGump();
            new ChannelGump(Owner, Channel.GetByType(typeof(Multi)));
        }

        private void MultiBlock(object obj)
        {
            Data.MultiBlocks.Add(obj.ToString());
            MultiConnection.Connection.Block(obj.ToString());
            NewGump();
        }

        private void MultiMaster()
        {
            Data.MultiMaster = !Data.MultiMaster;
            NewGump();
        }

        private void Submit()
        {
            Data.MultiServer = GetTextField("Server");
            Data.MultiPort = GetTextFieldInt("Port");
            NewGump();
        }

        private void Start()
        {
            MultiConnection.Connection.ConnectMaster();
            NewGump();
        }

        private void ConnectCancelClose()
        {
            Data.MultiServer = GetTextField("Server");
            Data.MultiPort = GetTextFieldInt("Port");

            if (MultiConnection.Connection.Connected)
                MultiConnection.Connection.CloseSlave();
            else if (MultiConnection.Connection.Connecting)
                MultiConnection.Connection.CloseSlave();
            else if (!MultiConnection.Connection.Connected)
                MultiConnection.Connection.ConnectSlave();

            NewGump();
        }
    }
}