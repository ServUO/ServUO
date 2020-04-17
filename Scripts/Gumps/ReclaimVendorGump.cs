using Server.Multis;
using Server.Network;
using System.Collections;

namespace Server.Gumps
{
    public class ReclaimVendorGump : Gump
    {
        private readonly BaseHouse m_House;
        private readonly ArrayList m_Vendors;
        public ReclaimVendorGump(BaseHouse house)
            : base(50, 50)
        {
            m_House = house;
            m_Vendors = new ArrayList(house.InternalizedVendors);

            AddBackground(0, 0, 170, 50 + m_Vendors.Count * 20, 0x13BE);

            AddImageTiled(10, 10, 150, 20, 0xA40);
            AddHtmlLocalized(10, 10, 150, 20, 1061827, 0x7FFF, false, false); // <CENTER>Reclaim Vendor</CENTER>

            AddImageTiled(10, 40, 150, m_Vendors.Count * 20, 0xA40);

            for (int i = 0; i < m_Vendors.Count; i++)
            {
                Mobile m = (Mobile)m_Vendors[i];

                int y = 40 + i * 20;

                AddButton(10, y, 0xFA5, 0xFA7, i + 1, GumpButtonType.Reply, 0);
                AddLabel(45, y, 0x481, m.Name);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 0 || !m_House.IsActive || !m_House.IsInside(from) || !m_House.IsOwner(from) || !from.CheckAlive())
                return;

            int index = info.ButtonID - 1;

            if (index < 0 || index >= m_Vendors.Count)
                return;

            Mobile mob = (Mobile)m_Vendors[index];

            if (!m_House.InternalizedVendors.Contains(mob))
                return;

            if (mob.Deleted)
            {
                m_House.InternalizedVendors.Remove(mob);
            }
            else
            {
                bool vendor, contract;
                BaseHouse.IsThereVendor(from.Location, from.Map, out vendor, out contract);

                if (vendor)
                {
                    from.SendLocalizedMessage(1062677); // You cannot place a vendor or barkeep at this location.
                }
                else if (contract)
                {
                    from.SendLocalizedMessage(1062678); // You cannot place a vendor or barkeep on top of a rental contract!
                }
                else
                {
                    m_House.InternalizedVendors.Remove(mob);
                    mob.MoveToWorld(from.Location, from.Map);
                }
            }
        }
    }
}