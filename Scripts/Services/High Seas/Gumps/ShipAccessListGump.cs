using System;
using Server;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using Server.Network;

namespace Server.Gumps
{
    public class AccessListGump : BaseShipGump
    {
        private BaseGalleon m_Galleon;
        private SecurityEntry m_Entry;
        private List<Mobile> m_UseList;

        public AccessListGump(Mobile from, BaseGalleon galleon) : base(50, 50, galleon)
        {
            from.CloseGump(typeof(AccessListGump));

            m_Galleon = galleon;
            m_Entry = galleon.SecurityEntry;

            if (m_Entry == null)
            {
                m_Entry = new SecurityEntry(m_Galleon);
                m_Galleon.SecurityEntry = m_Entry;
            }

            AddButton(10, 370, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 370, 100, 20, 1149777, LabelColor, false, false);  //MAIN MENU

            List<Mobile> list = new List<Mobile>(m_Entry.Manifest.Keys);
            m_UseList = list;

            int pages = (int)Math.Ceiling(list.Count / 10.0);
            int index = 0;

            for (int i = 1; i <= pages; i++)
            {
                int y = 85;
                AddPage(i);

                for (int c = 0; c < 10; c++)
                {
                    if (index >= list.Count)
                        break;

                    Mobile mob = list[index];

                    if (mob == null)
                        continue;

                    string name = mob.Name;
                    SecurityLevel level = m_Entry.GetEffectiveLevel(mob);

                    AddButton(10, y, 4005, 4007, index + 2, GumpButtonType.Reply, 0);
                    AddHtml(45, y, 130, 20, Color(name, "DarkCyan"), false, false);
                    AddHtmlLocalized(180, y, 100, 20, GetLevel(level), GetHue(level), false, false);

                    index++;
                    y += 25;
                }

                if (i < pages)
                {
                    AddButton(310, 390, 4005, 4007, 0, GumpButtonType.Page, i + 1);
                }

                if (i > 1)
                {
                    AddButton(270, 390, 4014, 4016, 0, GumpButtonType.Page, i - 1);
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 1)
                from.SendGump(new ShipSecurityGump(from, m_Galleon));
            else if (info.ButtonID > 1)
            {
                int index = info.ButtonID - 2;

                if (index < 0 || index >= m_UseList.Count)
                    return;

                Mobile mob = m_UseList[index];

                from.SendGump(new GrantAccessGump(mob, m_Galleon));
            }
        }
    }
}