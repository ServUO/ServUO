using System;
using System.Text;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.ACC.YS
{
    public class YGSettingsGump : Gump
    {
        public YardShovel m_Shovel;
        public YGSettingsGump(YardShovel shovel, Mobile from)
            : base(0, 0)
        {
            m_Shovel = shovel;
            string xstart = m_Shovel.XStart.ToString();
            string ystart = m_Shovel.YStart.ToString();
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            AddBackground(0, 0, 200, 200, 9200);
            AddAlphaRegion(10, 9, 180, 180);
            AddLabel(71, 16, 0, @"Settings");
            AddBackground(46, 51, 108, 28, 0x2486);
            AddBackground(46, 91, 108, 28, 0x2486);
            AddLabel(15, 55, 0, "X - ");
            AddTextEntry(50, 55, 100, 20, 0, (int)Buttons.XCoordTextBox, xstart.ToString());
            AddLabel(15, 95, 0, "Y - ");
            AddTextEntry(50, 95, 100, 20, 0, (int)Buttons.YCoordTextBox, ystart.ToString());
            AddButton(68, 145, 238, 239, (int)Buttons.OK, GumpButtonType.Reply, 0);

        }

        public enum Buttons
        {
            Exit,
            XCoordTextBox,
            YCoordTextBox,
            OK,
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            switch (info.ButtonID)
            {
                case (int)Buttons.OK:
                    {
                        TextRelay xrelay = info.GetTextEntry((int)Buttons.XCoordTextBox);
                        TextRelay yrelay = info.GetTextEntry((int)Buttons.YCoordTextBox);
                        string xtext = (xrelay == null ? null : xrelay.Text.Trim());
                        string ytext = (yrelay == null ? null : yrelay.Text.Trim());
                        if (xtext == null || xtext.Length == 0 || ytext == null || ytext.Length == 0)
                        {
                            from.SendMessage("You must enter an integer value in each box. (0 , 400, 245 )");
                        }
                        else
                        {
                            int x = m_Shovel.XStart;
                            int y = m_Shovel.YStart;
                            try
                            {
                                x = Int32.Parse(xtext);
                                y = Int32.Parse(ytext);
                                m_Shovel.XStart = x;
                                m_Shovel.YStart = y;
                            }
                            catch
                            {
                                from.SendMessage("You must enter an integer value in each box. (0 , 400, 245 )");
                            }
                        }

                        from.SendGump(new YardGump(from, m_Shovel, m_Shovel.Category, m_Shovel.Page, 0, 0));
                        break;
                    }
            }
        }
    }
}