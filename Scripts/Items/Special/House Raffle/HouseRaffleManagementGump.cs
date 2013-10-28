using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class HouseRaffleManagementGump : Gump
    {
        public const int LabelColor = 0xFFFFFF;
        public const int HighlightColor = 0x11EE11;
        private readonly HouseRaffleStone m_Stone;
        private readonly List<RaffleEntry> m_List;
        private readonly SortMethod m_Sort;
        private int m_Page;
        public HouseRaffleManagementGump(HouseRaffleStone stone)
            : this(stone, SortMethod.Default, 0)
        {
        }

        public HouseRaffleManagementGump(HouseRaffleStone stone, SortMethod sort, int page)
            : base(40, 40)
        {
            this.m_Stone = stone;
            this.m_Page = page;

            this.m_List = new List<RaffleEntry>(this.m_Stone.Entries);
            this.m_Sort = sort;

            switch ( this.m_Sort )
            {
                case SortMethod.Name:
                    {
                        this.m_List.Sort(NameComparer.Instance);

                        break;
                    }
                case SortMethod.Account:
                    {
                        this.m_List.Sort(AccountComparer.Instance);

                        break;
                    }
                case SortMethod.Address:
                    {
                        this.m_List.Sort(AddressComparer.Instance);

                        break;
                    }
            }

            this.AddPage(0);

            this.AddBackground(0, 0, 618, 354, 9270);
            this.AddAlphaRegion(10, 10, 598, 334);

            this.AddHtml(10, 10, 598, 20, this.Color(this.Center("Raffle Management"), LabelColor), false, false);

            this.AddHtml(45, 35, 100, 20, this.Color("Location:", LabelColor), false, false);
            this.AddHtml(145, 35, 250, 20, this.Color(this.m_Stone.FormatLocation(), LabelColor), false, false);

            this.AddHtml(45, 55, 100, 20, this.Color("Ticket Price:", LabelColor), false, false);
            this.AddHtml(145, 55, 250, 20, this.Color(this.m_Stone.FormatPrice(), LabelColor), false, false);

            this.AddHtml(45, 75, 100, 20, this.Color("Total Entries:", LabelColor), false, false);
            this.AddHtml(145, 75, 250, 20, this.Color(this.m_Stone.Entries.Count.ToString(), LabelColor), false, false);

            this.AddButton(440, 33, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
            this.AddHtml(474, 35, 120, 20, this.Color("Sort by name", LabelColor), false, false);

            this.AddButton(440, 53, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
            this.AddHtml(474, 55, 120, 20, this.Color("Sort by account", LabelColor), false, false);

            this.AddButton(440, 73, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0);
            this.AddHtml(474, 75, 120, 20, this.Color("Sort by address", LabelColor), false, false);

            this.AddImageTiled(13, 99, 592, 242, 9264);
            this.AddImageTiled(14, 100, 590, 240, 9274);
            this.AddAlphaRegion(14, 100, 590, 240);

            this.AddHtml(14, 100, 590, 20, this.Color(this.Center("Entries"), LabelColor), false, false);

            if (page > 0)
                this.AddButton(567, 104, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0);
            else
                this.AddImage(567, 104, 0x25EA);

            if ((page + 1) * 10 < this.m_List.Count)
                this.AddButton(584, 104, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0);
            else
                this.AddImage(584, 104, 0x25E6);

            this.AddHtml(14, 120, 30, 20, this.Color(this.Center("DEL"), LabelColor), false, false);
            this.AddHtml(47, 120, 250, 20, this.Color("Name", LabelColor), false, false);
            this.AddHtml(295, 120, 100, 20, this.Color(this.Center("Address"), LabelColor), false, false);
            this.AddHtml(395, 120, 150, 20, this.Color(this.Center("Date"), LabelColor), false, false);
            this.AddHtml(545, 120, 60, 20, this.Color(this.Center("Num"), LabelColor), false, false);

            int idx = 0;
            Mobile winner = this.m_Stone.Winner;

            for (int i = page * 10; i >= 0 && i < this.m_List.Count && i < (page + 1) * 10; ++i, ++idx)
            {
                RaffleEntry entry = this.m_List[i];

                if (entry == null)
                    continue;

                this.AddButton(13, 138 + (idx * 20), 4002, 4004, 6 + i, GumpButtonType.Reply, 0);

                int x = 45;
                int color = (winner != null && entry.From == winner) ? HighlightColor : LabelColor;

                string name = null;

                if (entry.From != null)
                {
                    Account acc = entry.From.Account as Account;

                    if (acc != null)
                        name = String.Format("{0} ({1})", entry.From.Name, acc);
                    else
                        name = entry.From.Name;
                }

                if (name != null)
                    this.AddHtml(x + 2, 140 + (idx * 20), 250, 20, this.Color(name, color), false, false);

                x += 250;

                if (entry.Address != null)
                    this.AddHtml(x, 140 + (idx * 20), 100, 20, this.Color(this.Center(entry.Address.ToString()), color), false, false);

                x += 100;

                this.AddHtml(x, 140 + (idx * 20), 150, 20, this.Color(this.Center(entry.Date.ToString()), color), false, false);
                x += 150;

                this.AddHtml(x, 140 + (idx * 20), 60, 20, this.Color(this.Center("1"), color), false, false);
                x += 60;
            }
        }

        public enum SortMethod
        {
            Default,
            Name,
            Account,
            Address
        }
        public string Right(string text)
        {
            return String.Format("<DIV ALIGN=RIGHT>{0}</DIV>", text);
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int buttonId = info.ButtonID;

            switch ( buttonId )
            {
                case 1: // Previous
                    {
                        if (this.m_Page > 0)
                            this.m_Page--;

                        from.SendGump(new HouseRaffleManagementGump(this.m_Stone, this.m_Sort, this.m_Page));

                        break;
                    }
                case 2: // Next
                    {
                        if ((this.m_Page + 1) * 10 < this.m_Stone.Entries.Count)
                            this.m_Page++;

                        from.SendGump(new HouseRaffleManagementGump(this.m_Stone, this.m_Sort, this.m_Page));

                        break;
                    }
                case 3: // Sort by name
                    {
                        from.SendGump(new HouseRaffleManagementGump(this.m_Stone, SortMethod.Name, 0));

                        break;
                    }
                case 4: // Sort by account
                    {
                        from.SendGump(new HouseRaffleManagementGump(this.m_Stone, SortMethod.Account, 0));

                        break;
                    }
                case 5: // Sort by address
                    {
                        from.SendGump(new HouseRaffleManagementGump(this.m_Stone, SortMethod.Address, 0));

                        break;
                    }
                default: // Delete
                    {
                        buttonId -= 6;

                        if (buttonId >= 0 && buttonId < this.m_List.Count)
                        {
                            this.m_Stone.Entries.Remove(this.m_List[buttonId]);

                            if (this.m_Page > 0 && this.m_Page * 10 >= this.m_List.Count - 1)
                                this.m_Page--;

                            from.SendGump(new HouseRaffleManagementGump(this.m_Stone, this.m_Sort, this.m_Page));
                        }

                        break;
                    }
            }
        }

        private class NameComparer : IComparer<RaffleEntry>
        {
            public static readonly IComparer<RaffleEntry> Instance = new NameComparer();
            public NameComparer()
            {
            }

            public int Compare(RaffleEntry x, RaffleEntry y)
            {
                bool xIsNull = (x == null || x.From == null);
                bool yIsNull = (y == null || y.From == null);

                if (xIsNull && yIsNull)
                    return 0;
                else if (xIsNull)
                    return -1;
                else if (yIsNull)
                    return 1;

                int result = Insensitive.Compare(x.From.Name, y.From.Name);

                if (result == 0)
                    return x.Date.CompareTo(y.Date);
                else
                    return result;
            }
        }

        private class AccountComparer : IComparer<RaffleEntry>
        {
            public static readonly IComparer<RaffleEntry> Instance = new AccountComparer();
            public AccountComparer()
            {
            }

            public int Compare(RaffleEntry x, RaffleEntry y)
            {
                bool xIsNull = (x == null || x.From == null);
                bool yIsNull = (y == null || y.From == null);

                if (xIsNull && yIsNull)
                    return 0;
                else if (xIsNull)
                    return -1;
                else if (yIsNull)
                    return 1;

                Account a = x.From.Account as Account;
                Account b = y.From.Account as Account;

                if (a == null && b == null)
                    return 0;
                else if (a == null)
                    return -1;
                else if (b == null)
                    return 1;

                int result = Insensitive.Compare(a.Username, b.Username);

                if (result == 0)
                    return x.Date.CompareTo(y.Date);
                else
                    return result;
            }
        }

        private class AddressComparer : IComparer<RaffleEntry>
        {
            public static readonly IComparer<RaffleEntry> Instance = new AddressComparer();
            public AddressComparer()
            {
            }

            public int Compare(RaffleEntry x, RaffleEntry y)
            {
                bool xIsNull = (x == null || x.Address == null);
                bool yIsNull = (y == null || y.Address == null);

                if (xIsNull && yIsNull)
                    return 0;
                else if (xIsNull)
                    return -1;
                else if (yIsNull)
                    return 1;

                byte[] a = x.Address.GetAddressBytes();
                byte[] b = y.Address.GetAddressBytes();

                for (int i = 0; i < a.Length && i < b.Length; i++)
                {
                    int compare = a[i].CompareTo(b[i]);

                    if (compare != 0)
                        return compare;
                }

                return x.Date.CompareTo(y.Date);
            }
        }
    }
}