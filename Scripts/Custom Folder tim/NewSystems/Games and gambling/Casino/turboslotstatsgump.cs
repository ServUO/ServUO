#undef XMLSPAWNER
//#define XMLSPAWNER
using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class TurboSlotsStatGump : Gump
    {
        private ArrayList m_SlotArray;

        public TurboSlotsStatGump(Mobile from, ArrayList t)
            : base(25, 25)
        {
            m_SlotArray = t;
            string text = null;
            int starty = 40;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            AddBackground(0, 0, 530, 325, 5170);
            if (from.AccessLevel >= AccessLevel.GameMaster && m_SlotArray !=null && m_SlotArray.Count > 0)
                AddButton(478, 30, 0xFAB, 0xFAD, 100, GumpButtonType.Reply, 0);
            AddLabel(165, 3, 55, "Top Ten Recent Jackpot Winners");
            m_SlotArray.Sort(new ListAmountSorter());
            AddLabel(55, 20, 1149, "Name");
            AddLabel(235, 20, 1149, "Date");
            AddLabel(390, 20, 1149, "Value/Gold");
            AddImageTiled(40, 65, 450, 45, 0xBBC);
            AddImageTiled(40, 115, 450, 170, 9204);
            for (int i = 0; i < 10; i++)
            {
                text = String.Format("{0:##}.", i + 1);
                AddLabel(20, starty + i*25, 1152, @text);
            }
            if (m_SlotArray != null && m_SlotArray.Count > 0)
            {
                int count = m_SlotArray.Count > 9 ? 10 : m_SlotArray.Count;
                for (int i = 0; i < count; i++)
                {
                    if (m_SlotArray[i] == null || ((TurboSlot)m_SlotArray[i]).LastWonBy == null) { break; }
                    else
                    {
                        TurboSlot ts = (TurboSlot)m_SlotArray[i];
                        text = String.Format("{0}", ts.LastWonBy.Name);
                        AddLabel(45, starty, ts.Hue - 1, @text);
                        text = String.Format("{0}", ts.LastWonByDate);
                        AddLabel(175, starty, ts.Hue - 1, @text);
                        text = String.Format("{0:##,###,##0}", ts.LastWonAmount);
                        AddLabel(385, starty, ts.Hue - 1, @text);
                    }
                    starty += 25;
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            if (from == null)
                return;
            if (info.ButtonID == 100 && from.AccessLevel >= AccessLevel.GameMaster)// Admin Gump
            {
                ArrayList turboslotsarray = new ArrayList();
                foreach (TurboSlot t in m_SlotArray) // Duplicate the array to avoid conflicts
                    turboslotsarray.Add(t);
                from.SendGump(new TurboSlotsStatGump(from, m_SlotArray));
                from.CloseGump(typeof(TurboSlotsAdminGump));
                from.SendGump(new TurboSlotsAdminGump(from, turboslotsarray, 0, true, 0));
            }
        }

        private class ListAmountSorter : IComparer
        {
            public ListAmountSorter()
                : base()
            {
            }

            public int Compare(object x, object y)
            {
                if (x == null || y == null || x == y) return 0;
                if (((TurboSlot)x).LastWonAmount == ((TurboSlot)y).LastWonAmount)
                {
                    return ((((TurboSlot)x).LastWonByDate > ((TurboSlot)y).LastWonByDate) ? -1 : 1);
                }
                return ((((TurboSlot)x).LastWonAmount > ((TurboSlot)y).LastWonAmount) ? -1 : 1);
            }
        }
    }
    public class TurboSlotsAdminGump : Gump
    {
        private ArrayList m_SlotArray;
        private int m_page;
        private bool m_sort;
        private int m_sorttype;

        public TurboSlotsAdminGump(Mobile from, ArrayList t, int page, bool sort, int sorttype)
            : base(192, 135)
        {
            m_SlotArray = t;
            m_page = page;
            m_sort = sort;
            m_sorttype = sorttype;
            int starty = 33;
            int slotHue = 0;
            string text = null;
            int[] buttonx = new int[] { 610, 70, 210, 290, 400, 510 };
            string[] pstring = new string[] { "L", "N", "T", "E", "C", "R" };
            decimal Collected = 0;
            decimal Won = 0;
            decimal WinningPercentage = 0;
            decimal NetProfit = 0;
            TurboSlot ts = null;
            int index = 0;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            if (m_sort)
                this.m_SlotArray.Sort(new SortArray(m_sorttype));
            AddPage(0);
            AddBackground(0, 0, 735, 410, 5120);
            AddBackground(10, 340, 683, 35, 5054);
            AddLabel(80, 11, 0x384, "Theme/Odds");
            AddLabel(220, 11, 0x384, "Spins");
            AddLabel(300, 11, 0x384, "Collected");
            AddLabel(415, 11, 0x384, "Won");
            AddLabel(520, 11, 0x384, "Net Profit");
            AddLabel(620, 11, 0x384, "Payback %");
            AddLabel(245, 380, 0x384, "Set all Active");
            AddLabel(460, 380, 0x384, "Set all Inactive");
            AddButton(210, 380, 0xFA8, 0xFAA, 1150, GumpButtonType.Reply, 0); //Active
            AddButton(425, 380, 0xFA2, 0xFA3, 1250, GumpButtonType.Reply, 0); //Set Active
            for (int i = 0; i < m_SlotArray.Count; i++)
            {
                ts = (TurboSlot)m_SlotArray[i];
                Collected += ts.TotalCollected;
                Won += ts.TotalWon;
            }

            try { WinningPercentage = Won / Collected; } // Avoid Divide by Zero
            catch { WinningPercentage = 0; }
            for (int i = 0; i < 6; i++)
            {
                if (m_sorttype == i)
                    AddButton(buttonx[i], 3, 2224, 2224, 200 + i, GumpButtonType.Reply, 0);
                else
                    AddButton(buttonx[i], 3, 2103, 2224, 200 + i, GumpButtonType.Reply, 0);
            }
            if (m_page < 9)
                AddButton(22, 5, 0x8B1 + m_page, 0x8B1 + m_page, 0, GumpButtonType.Page, 0);
            else
                AddLabel(26, 5, 1149, (m_page + 1).ToString());
            if ((m_page * 12) + 12 < m_SlotArray.Count)
                AddButton(41, 7, 0x15E1, 0x15E5, 101, GumpButtonType.Reply, 0);// Forward Arrow
            if (m_page != 0)
                AddButton(5, 7, 0x15E3, 0x15E7, 102, GumpButtonType.Reply, 0); // Backward Arrow

            for (int i = 0; i < 12; i++)
            {
                index = (m_page * 12) + i;
                if (index < m_SlotArray.Count)
                {
                    ts = (TurboSlot)m_SlotArray[index];
                    if (ts.Deleted)
                    {
                        AddImageTiled(65, starty, 630, 21, 5124);
                        slotHue = 0;
                    }
                    else
                    {
                        slotHue = ts.Hue - 1;
                        AddButton(5, starty, 0xFAB, 0xFAD, 900 + i, GumpButtonType.Reply, 0);   // Properties  
                        AddButton(30, starty, 0xFAE, 0xFAF, 1000 + i, GumpButtonType.Reply, 0); // Go to
                        if (ts.Active)
                        {
                            AddImageTiled(65, starty, 630, 21, 0xBBC);
                            AddButton(700, starty, 0xFA8, 0xFA2, 1100 + i, GumpButtonType.Reply, 0);  //Set Inactive
                        }
                        else
                        {
                            AddImageTiled(65, starty, 630, 21, 9204);
                            AddButton(700, starty, 0xFA2, 0xFA8, 1200 + i, GumpButtonType.Reply, 0); //Set Active
                        }
                    }

                    text = String.Format("{0}", ts.SlotTheme);
                    AddLabel(70, starty, slotHue, @text);
                    text = String.Format("({0})", pstring[(int)ts.CurrentPayback]);
                    if ((int)ts.SlotPaybackOdds == 5)
                        AddLabel(180, starty, 0, @text);
                    else
                        AddLabel(180, starty, slotHue, @text);
                    if (ts.InUseBy != null)
                        AddLabel(201, starty, slotHue, @"*");
                    text = String.Format("{0:#,###,##0}", ts.TotalSpins);
                    AddLabel(215, starty, slotHue, @text);
                    text = String.Format("{0:##,###,##0}", ts.TotalCollected);
                    AddLabel(295, starty, slotHue, @text);
                    text = String.Format("{0:##,###,##0}", ts.TotalWon);
                    AddLabel(410, starty, slotHue, @text);
                    NetProfit = ts.TotalCollected - ts.TotalWon;
                    text = String.Format("{0:##,###,##0}", NetProfit);
                    if (NetProfit < 0)
                        AddLabel(515, starty, 36, @text);
                    else
                        AddLabel(515, starty, slotHue, @text);
                    if (ts.ProgIsMaster)
                        AddLabel(612, starty, slotHue, "P");
                    else if (ts.ProgSlotMaster != null)
                        AddLabel(612, starty, slotHue, "p");
                    text = String.Format("{0,-10:##0.00}", ts.WinningPercentage);
                    if (ts.WinningPercentage > 99.5)
                        AddLabel(645, starty, 36, @text);
                    else
                        AddLabel(645, starty, slotHue, @text);
                }
                else
                    break;
                starty += 25;
            }
            starty = 348;
            text = String.Format("({0:##0} Slots)", m_SlotArray.Count);
            AddLabel(10, starty - 15, 1149, @text);
            NetProfit = Collected - Won;
            AddLabel(32, starty, 55, "<Totals>");
            text = String.Format("{0:###,###,##0}", Collected);
            AddLabel(95, starty, 0x384, "Collected:");
            AddLabel(155, starty, 2212, @text);
            text = String.Format("{0:###,###,##0}", Won);
            AddLabel(265, starty, 0x384, @"Won:");
            AddLabel(300, starty, 2212, @text);
            text = String.Format(" {0:###,###,##0}", NetProfit);
            AddLabel(404, starty, 0x384, @"Net Profit:");
            if (NetProfit < 0)
                AddLabel(470, starty, 36, @text);
            else
                AddLabel(470, starty, 267, @text);
            try { WinningPercentage = (Won / Collected) * 100; }
            catch { WinningPercentage = 0; }
            int PaybackHue = 267;
            if (WinningPercentage > 99.5m)
                PaybackHue = 37;
            else if (WinningPercentage > 90.0m)
                PaybackHue = 267;
            else if (WinningPercentage > 80.0m)
                PaybackHue = 87;
            else if (WinningPercentage > 60.0m)
                PaybackHue = 55;
            else
                PaybackHue = 37;
            AddLabel(571, starty, 0x384, @"PayBack %:");
            text = String.Format("{0:##0.00}", WinningPercentage);
            AddLabel(645, starty, PaybackHue, @text);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            if (from == null || info.ButtonID == 0)
                return;
            if (info.ButtonID == 101)
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page + 1, true, m_sorttype));
            else if (info.ButtonID == 102)
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page - 1, true, m_sorttype));
            else if (info.ButtonID >= 200 && info.ButtonID <= 205)
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page, true, info.ButtonID - 200)); //Sort by selection
            else if (info.ButtonID >= 900 && info.ButtonID <= 915) // Slot properties
            {
                int index = (m_page * 12) + (info.ButtonID - 900);
                if (index < 0 || index >= m_SlotArray.Count || m_SlotArray[index] == null) // Should never happen but bail out just in case
                    return;
                if (!((Item)m_SlotArray[index]).Deleted)
                {
#if XMLSPAWNER
                    from.SendGump(new XmlPropertiesGump(from, m_SlotArray[index]));
#else
                    from.SendGump(new PropertiesGump(from, m_SlotArray[index]));
#endif
                }
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page, false, m_sorttype));
            }
            else if (info.ButtonID >= 1000 && info.ButtonID <= 1015) // Goto slot location
            {
                int index = (m_page * 12) + (info.ButtonID - 1000);
                if (index < 0 || index >= m_SlotArray.Count || m_SlotArray[index] == null) // Should never happen but bail out just in case
                    return;
                if (!((Item)m_SlotArray[index]).Deleted)
                {
                    from.Location = ((Item)m_SlotArray[index]).Location;
                    from.Map = ((Item)m_SlotArray[index]).Map;
                }
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page, false, m_sorttype));
            }
            else if (info.ButtonID >= 1100 && info.ButtonID <= 1115) // Goto slot location
            {
                int index = (m_page * 12) + (info.ButtonID - 1100);
                if (index < 0 || index >= m_SlotArray.Count || m_SlotArray[index] == null) // Should never happen but bail out just in case
                    return;
                if (!((Item)m_SlotArray[index]).Deleted)
                {
                    ((TurboSlot)m_SlotArray[index]).Active = false;
                }
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page, false, m_sorttype));
            }
            else if (info.ButtonID >= 1200 && info.ButtonID <= 1215) // Goto slot location
            {
                int index = (m_page * 12) + (info.ButtonID - 1200);
                if (index < 0 || index >= m_SlotArray.Count || m_SlotArray[index] == null) // Should never happen but bail out just in case
                    return;
                if (!((Item)m_SlotArray[index]).Deleted)
                {
                    ((TurboSlot)m_SlotArray[index]).Active = true;
                }
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page, false, m_sorttype));
            }
            else if (info.ButtonID == 1150) // Set all Active
            {
                foreach (TurboSlot t in m_SlotArray)
                    if (t != null && !t.Deleted)
                        ((TurboSlot)t).Active = true;
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page, false, m_sorttype));
            }
            else if (info.ButtonID == 1250) // Set all Inactive
            {
                foreach (TurboSlot t in m_SlotArray)
                    if (t != null && !t.Deleted)
                        ((TurboSlot)t).Active = false;
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page, false, m_sorttype));
            }
            else
            {
                from.SendMessage("Invalid button({0}) detected.", info.ButtonID);
                from.SendGump(new TurboSlotsAdminGump(from, m_SlotArray, m_page, false, m_sorttype));
            }
        }

        private class SortArray : IComparer
        {
            private int isorttype;
            public SortArray(int sorttype)
                : base()
            {
                isorttype = sorttype;
            }

            public int Compare(object x, object y)
            {
                if (x == null || y == null || x == y) return 0;
                switch (isorttype)
                {
                    case 1:
                        if (((TurboSlot)x).SlotTheme == ((TurboSlot)y).SlotTheme)
                            return ((((TurboSlot)x).LastPlayed > ((TurboSlot)y).LastPlayed) ? -1 : 1);
                        return ((((TurboSlot)x).SlotTheme < ((TurboSlot)y).SlotTheme) ? -1 : 1);
                    case 2:
                        if (((TurboSlot)x).TotalSpins == ((TurboSlot)y).TotalSpins)
                            return ((((TurboSlot)x).LastPlayed > ((TurboSlot)y).LastPlayed) ? -1 : 1);
                        return ((((TurboSlot)x).TotalSpins > ((TurboSlot)y).TotalSpins) ? -1 : 1);
                    case 3:
                        if (((TurboSlot)x).TotalCollected == ((TurboSlot)y).TotalCollected)
                            return ((((TurboSlot)x).LastPlayed > ((TurboSlot)y).LastPlayed) ? -1 : 1);
                        return ((((TurboSlot)x).TotalCollected > ((TurboSlot)y).TotalCollected) ? -1 : 1);
                    case 4:
                        if (((TurboSlot)x).TotalWon == ((TurboSlot)y).TotalWon)
                            return ((((TurboSlot)x).LastPlayed > ((TurboSlot)y).LastPlayed) ? -1 : 1);
                        return ((((TurboSlot)x).TotalWon > ((TurboSlot)y).TotalWon) ? -1 : 1);
                    case 5:
                        decimal wonx = ((TurboSlot)x).TotalCollected - ((TurboSlot)x).TotalWon;
                        decimal wony = ((TurboSlot)y).TotalCollected - ((TurboSlot)y).TotalWon;
                        if (wonx == wony)
                            return ((((TurboSlot)x).LastPlayed > ((TurboSlot)y).LastPlayed) ? -1 : 1);
                        return ((wonx > wony) ? -1 : 1);
                    default :
                        if (((TurboSlot)x).WinningPercentage == ((TurboSlot)y).WinningPercentage)
                            return ((((TurboSlot)x).LastPlayed > ((TurboSlot)y).LastPlayed) ? -1 : 1);
                        return ((((TurboSlot)x).WinningPercentage > ((TurboSlot)y).WinningPercentage) ? -1 : 1);
                }  
            }
        }
    }
}