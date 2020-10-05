using Server.Gumps;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public class JewelryBoxGump : Gump
    {
        private readonly Mobile m_From;
        private readonly JewelryBox m_Box;
        private readonly List<Item> m_List;

        private readonly int m_Page;

        private const int LabelColor = 0x7FFF;

        public bool CheckFilter(Item item)
        {
            JewelryBoxFilter f = m_Box.Filter;

            if (f.IsDefault)
                return true;

            if (f.Ring && item is BaseRing)
            {
                return true;
            }
            else if (f.Bracelet && item is BaseBracelet)
            {
                return true;
            }
            else if (f.Earrings && item is BaseEarrings)
            {
                return true;
            }
            else if (f.Necklace && item is BaseNecklace)
            {
                return true;
            }
            else if (f.Talisman && item is BaseTalisman)
            {
                return true;
            }

            return false;
        }

        public int GetPageCount(int count)
        {
            return (count + 49) / 50;
        }

        public int GetIndexForPage(int page)
        {
            int index = 0;

            while (page-- > 0)
                index += GetCountForIndex(index);

            return index;
        }

        public int GetCountForIndex(int index)
        {
            int slots = 0;
            int count = 0;

            List<Item> list = m_List;

            for (int i = index; i >= 0 && i < list.Count; ++i)
            {
                Item recipe = list[i];

                if (CheckFilter(recipe))
                {
                    int add;

                    add = 1;

                    if ((slots + add) > 50)
                        break;

                    slots += add;
                }

                ++count;
            }

            return count;
        }

        public JewelryBoxGump(Mobile from, JewelryBox box)
            : this(from, box, 0)
        {
        }

        public JewelryBoxGump(Mobile from, JewelryBox box, int page)
            : base(100, 100)
        {
            from.CloseGump(typeof(JewelryBoxGump));

            m_From = from;
            m_Box = box;
            m_Page = page;

            m_List = new List<Item>();

            foreach (Item item in m_Box.Items)
            {
                if (!CheckFilter(item))
                    continue;

                m_List.Add(item);
            }

            int index = GetIndexForPage(page);
            int count = GetCountForIndex(index);
            int pageCount = GetPageCount(m_List.Count);
            int currentpage = pageCount > 0 ? (page + 1) : 0;

            int tableIndex = 0;

            for (int i = index; i < (index + count) && i >= 0 && i < m_List.Count; ++i)
            {
                Item item = m_List[i];

                if (!CheckFilter(item))
                    continue;

                ++tableIndex;
            }

            AddPage(0);

            AddImage(0, 0, 0x9CCA);
            AddHtmlLocalized(40, 2, 500, 20, 1114513, "#1157694", 0x7FF0, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>   

            AddHtmlLocalized(50, 30, 100, 20, 1157695, 0x7FF0, false, false); // Select Filter:

            AddHtmlLocalized(41, 350, 123, 20, 1157698, string.Format("{0}@{1}", m_List.Count, m_Box.DefaultMaxItems), 0x7FF0, false, false); // Items: ~1_NUM~ of ~2_MAX~
            AddHtmlLocalized(212, 350, 123, 20, 1153561, string.Format("{0}@{1}", currentpage, pageCount), 0x7FF0, false, false); // Page ~1_CUR~ of ~2_MAX~
            AddHtmlLocalized(416, 350, 100, 20, 1153562, 0x7FF0, false, false); // <DIV ALIGN="CENTER">PAGE</DIV>

            JewelryBoxFilter f = box.Filter;

            AddHtmlLocalized(200, 30, 90, 20, 1154607, f.Ring ? 0x421F : LabelColor, false, false); // Ring
            AddButton(160, 30, 0xFA5, 0xFA7, 101, GumpButtonType.Reply, 0);

            AddHtmlLocalized(325, 30, 90, 20, 1079905, f.Bracelet ? 0x421F : LabelColor, false, false); // Bracelet
            AddButton(285, 30, 0xFA5, 0xFA7, 102, GumpButtonType.Reply, 0);

            AddHtmlLocalized(450, 30, 90, 20, 1079903, f.Earrings ? 0x421F : LabelColor, false, false); // Earrings
            AddButton(410, 30, 0xFA5, 0xFA7, 104, GumpButtonType.Reply, 0);

            AddHtmlLocalized(200, 55, 90, 20, 1157697, f.Necklace ? 0x421F : LabelColor, false, false); // Necklace
            AddButton(160, 55, 0xFA5, 0xFA7, 108, GumpButtonType.Reply, 0);

            AddHtmlLocalized(325, 55, 90, 20, 1071023, f.Talisman ? 0x421F : LabelColor, false, false); // Talisman
            AddButton(285, 55, 0xFA5, 0xFA7, 116, GumpButtonType.Reply, 0);

            AddHtmlLocalized(450, 55, 90, 20, 1062229, f.IsDefault ? 0x421F : LabelColor, false, false); // All
            AddButton(410, 55, 0xFA5, 0xFA7, 132, GumpButtonType.Reply, 0);

            AddButton(356, 353, 0x15E3, 0x15E7, 11, GumpButtonType.Reply, 0); // First page
            AddButton(376, 350, 0xFAE, 0xFB0, 1, GumpButtonType.Reply, 0); // Previous page

            AddButton(526, 350, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0); // Next Page         
            AddButton(560, 353, 0x15E1, 0x15E5, 12, GumpButtonType.Reply, 0); // Last page

            AddHtmlLocalized(270, 385, 100, 20, 1157696, LabelColor, false, false); // ADD JEWELRY
            AddButton(225, 385, 0xFAB, 0xFAD, 3, GumpButtonType.Reply, 0);

            int x = 0;

            for (int i = index; i < (index + count) && i >= 0 && i < m_List.Count; ++i)
            {
                Item item = m_List[i];

                int xoffset = ((x / 5) * 50);
                int yoffset = ((i % 5) * 50);

                x++;

                AddECHandleInput();
                AddButton(50 + xoffset, 90 + yoffset, 0x92F, 0x92F, item.Serial, GumpButtonType.Reply, 0);
                AddItemProperty(item.Serial);
                AddItem(57 + xoffset, 108 + yoffset, item.ItemID, item.Hue);
                AddECHandleInput();
            }
        }

        private class InternalTarget : Target
        {
            private readonly JewelryBox m_Box;
            private readonly int m_Page;

            public InternalTarget(Mobile from, JewelryBox box, int page)
                : base(-1, false, TargetFlags.None)
            {
                m_Box = box;
                m_Page = page;
            }

            public void TryDrop(Mobile from, Item dropped)
            {
                if (!m_Box.CheckAccessible(from, m_Box))
                {
                    from.SendLocalizedMessage(1061637); // You are not allowed to access this.
                }
                else if (!dropped.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1157726); // You must be carrying the item to add it to the jewelry box.
                    return;
                }
                else if (m_Box.IsAccept(dropped))
                {
                    if (m_Box.IsFull)
                    {
                        from.SendLocalizedMessage(1157723); // The jewelry box is full.
                    }
                    else
                    {
                        m_Box.DropItem(dropped);
                        from.Target = new InternalTarget(from, m_Box, m_Page);
                    }
                }
                else if (dropped is Container)
                {
                    Container c = dropped as Container;

                    int count = 0;

                    for (int i = c.Items.Count - 1; i >= 0; --i)
                    {
                        if (i < c.Items.Count && m_Box.IsAccept(c.Items[i]))
                        {
                            if (m_Box.IsFull)
                            {
                                from.SendLocalizedMessage(1157723); // The jewelry box is full.
                                break;
                            }
                            else
                            {
                                m_Box.DropItem(c.Items[i]);
                                count++;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        from.CloseGump(typeof(JewelryBoxGump));
                        from.SendGump(new JewelryBoxGump(from, m_Box, m_Page));
                    }
                    else
                    {
                        from.SendLocalizedMessage(1157724); // This is not a ring, bracelet, necklace, earring, or talisman.
                        from.SendGump(new JewelryBoxGump(from, m_Box, m_Page));
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1157724); // This is not a ring, bracelet, necklace, earring, or talisman.
                    from.SendGump(new JewelryBoxGump(from, m_Box, m_Page));
                }
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Box != null && !m_Box.Deleted && targeted is Item)
                {
                    TryDrop(from, (Item)targeted);
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (m_Box != null && !m_Box.Deleted)
                {
                    from.CloseGump(typeof(JewelryBoxGump));
                    from.SendGump(new JewelryBoxGump(from, m_Box, m_Page));
                    from.SendLocalizedMessage(1157726); // You must be carrying the item to add it to the jewelry box.
                }
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (!m_Box.CheckAccessible(m_From, m_Box))
            {
                m_From.SendLocalizedMessage(1061637); // You are not allowed to access this.
                return;
            }

            JewelryBoxFilter f = m_Box.Filter;

            int index = info.ButtonID;

            switch (index)
            {
                case 0: { break; }
                case 1: // Previous page
                    {
                        if (m_Page > 0)
                        {
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box, m_Page - 1));
                        }
                        else
                        {
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box, m_Page));
                        }

                        break;
                    }
                case 2: // Next Page
                    {
                        if (GetIndexForPage(m_Page + 1) < m_List.Count)
                        {
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box, m_Page + 1));
                        }
                        else
                        {
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box, m_Page));
                        }

                        return;
                    }
                case 3: // ADD JEWELRY
                    {
                        m_From.Target = new InternalTarget(m_From, m_Box, m_Page);
                        m_From.SendLocalizedMessage(1157725); // Target rings, bracelets, necklaces, earrings, or talisman in your backpack. You may also target a sub-container to add contents to the the jewelry box. When done, press ESC.
                        m_From.SendGump(new JewelryBoxGump(m_From, m_Box));
                        break;
                    }
                case 11: // First page
                    {
                        if (m_Page > 0)
                        {
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box, 1));
                        }
                        else
                        {
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box, m_Page));
                        }

                        break;
                    }
                case 12: // Last Page
                    {
                        int pagecount = GetPageCount(m_List.Count);

                        if (m_Page != pagecount && m_Page >= 1)
                        {
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box, pagecount));
                        }
                        else
                        {
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box, m_Page));
                        }

                        break;
                    }
                case 101: // Ring
                    {
                        f.Ring = true;
                        m_From.SendGump(new JewelryBoxGump(m_From, m_Box));

                        break;
                    }
                case 102: // Bracelet
                    {
                        f.Bracelet = true;
                        m_From.SendGump(new JewelryBoxGump(m_From, m_Box));

                        break;
                    }
                case 104: // Earrings
                    {
                        f.Earrings = true;
                        m_From.SendGump(new JewelryBoxGump(m_From, m_Box));

                        break;
                    }
                case 108: // Necklace
                    {
                        f.Necklace = true;
                        m_From.SendGump(new JewelryBoxGump(m_From, m_Box));

                        break;
                    }
                case 116: // Talisman
                    {
                        f.Talisman = true;
                        m_From.SendGump(new JewelryBoxGump(m_From, m_Box));

                        break;
                    }
                case 132: // ALL
                    {
                        f.Clear();
                        m_From.SendGump(new JewelryBoxGump(m_From, m_Box));

                        break;
                    }
                default:
                    {
                        Item item = m_Box.Items.Find(x => x.Serial == index);

                        if (item != null)
                        {
                            m_From.AddToBackpack(item);
                            m_From.SendGump(new JewelryBoxGump(m_From, m_Box));
                        }

                        break;
                    }
            }
        }
    }
}
