using Server.Multis;
using Server.Network;
using System.Collections.Generic;

namespace Server.Gumps
{
    public class AccessListGump : BaseShipGump
    {
        private readonly BaseGalleon m_Galleon;
        private readonly SecurityEntry m_Entry;
        private readonly List<Mobile> m_UseList;

        public AccessListGump(Mobile from, BaseGalleon galleon)
            : base(galleon)
        {
            from.CloseGump(typeof(AccessListGump));

            m_Galleon = galleon;
            m_Entry = galleon.SecurityEntry;

            if (m_Entry == null)
            {
                m_Entry = new SecurityEntry(m_Galleon);
                m_Galleon.SecurityEntry = m_Entry;
            }

            AddButton(10, 355, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 357, 100, 18, 1149777, LabelColor, false, false); // MAIN MENU

            m_UseList = new List<Mobile>(m_Entry.Manifest.Keys);

            int page = 1;
            int y = 79;

            AddPage(page);

            for (int i = 0; i < m_UseList.Count; i++)
            {
                if (page > 1)
                    AddButton(270, 390, 4014, 4016, 0, GumpButtonType.Page, page - 1);

                Mobile mob = m_UseList[i];

                if (mob == null || m_Galleon.IsOwner(mob))
                    continue;

                string name = mob.Name;
                SecurityLevel level = m_Entry.GetEffectiveLevel(mob);

                AddButton(10, y, 0xFA5, 0xFA7, i + 2, GumpButtonType.Reply, 0);
                AddLabel(45, y + 2, 0x3E7, name);
                AddHtmlLocalized(160, y + 2, 150, 18, GetLevel(level), GetHue(level), false, false);

                y += 25;

                bool pages = (i + 1) % 10 == 0;

                if (pages && m_UseList.Count - 1 != i)
                {
                    AddButton(310, 390, 4005, 4007, 0, GumpButtonType.Page, page + 1);
                    page++;
                    y = 0;

                    AddPage(page);
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
