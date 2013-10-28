using System;
using System.Collections.Generic;
using Server.Accounting;
using Server.Commands;
using Server.Items;
using Server.Multis;
using Server.Targeting;

namespace Server.Gumps
{
    public class ViewHousesGump : Gump
    {
        private const int White16 = 0x7FFF;
        private const int White = 0xFFFFFF;
        private readonly Mobile m_From;
        private readonly List<BaseHouse> m_List;
        private readonly BaseHouse m_Selection;
        public ViewHousesGump(Mobile from, List<BaseHouse> list, BaseHouse sel)
            : base(50, 40)
        {
            this.m_From = from;
            this.m_List = list;
            this.m_Selection = sel;

            from.CloseGump(typeof(ViewHousesGump));

            this.AddPage(0);

            this.AddBackground(0, 0, 240, 360, 5054);
            this.AddBlackAlpha(10, 10, 220, 340);

            if (sel == null || sel.Deleted)
            {
                this.m_Selection = null;

                this.AddHtml(35, 15, 120, 20, this.Color("House Type", White), false, false);

                if (list.Count == 0)
                    this.AddHtml(35, 40, 160, 40, this.Color("There were no houses found for that player.", White), false, false);

                this.AddImage(190, 17, 0x25EA);
                this.AddImage(207, 17, 0x25E6);

                int page = 0;

                for (int i = 0; i < list.Count; ++i)
                {
                    if ((i % 15) == 0)
                    {
                        if (page > 0)
                            this.AddButton(207, 17, 0x15E1, 0x15E5, 0, GumpButtonType.Page, page + 1);

                        this.AddPage(++page);

                        if (page > 1)
                            this.AddButton(190, 17, 0x15E3, 0x15E7, 0, GumpButtonType.Page, page - 1);
                    }

                    object name = this.FindHouseName(list[i]);

                    this.AddHtml(15, 40 + ((i % 15) * 20), 20, 20, this.Color(String.Format("{0}.", i + 1), White), false, false);

                    if (name is int)
                        this.AddHtmlLocalized(35, 40 + ((i % 15) * 20), 160, 20, (int)name, White16, false, false);
                    else if (name is string)
                        this.AddHtml(35, 40 + ((i % 15) * 20), 160, 20, this.Color((string)name, White), false, false);

                    this.AddButton(198, 39 + ((i % 15) * 20), 4005, 4007, i + 1, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                string houseName, owner, location;
                Map map = sel.Map;

                houseName = (sel.Sign == null) ? "An Unnamed House" : sel.Sign.GetName();
                owner = (sel.Owner == null) ? "nobody" : sel.Owner.Name;

                int xLong = 0, yLat = 0, xMins = 0, yMins = 0;
                bool xEast = false, ySouth = false;

                bool valid = Sextant.Format(sel.Location, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth);

                if (valid)
                    location = String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                else
                    location = "unknown";

                this.AddHtml(10, 15, 220, 20, this.Color(this.Center("House Properties"), White), false, false);

                this.AddHtml(15, 40, 210, 20, this.Color("Facet:", White), false, false);
                this.AddHtml(15, 40, 210, 20, this.Color(this.Right(map == null ? "(null)" : map.Name), White), false, false);

                this.AddHtml(15, 60, 210, 20, this.Color("Location:", White), false, false);
                this.AddHtml(15, 60, 210, 20, this.Color(this.Right(sel.Location.ToString()), White), false, false);

                this.AddHtml(15, 80, 210, 20, this.Color("Sextant:", White), false, false);
                this.AddHtml(15, 80, 210, 20, this.Color(this.Right(location), White), false, false);

                this.AddHtml(15, 100, 210, 20, this.Color("Owner:", White), false, false);
                this.AddHtml(15, 100, 210, 20, this.Color(this.Right(owner), White), false, false);

                this.AddHtml(15, 120, 210, 20, this.Color("Name:", White), false, false);
                this.AddHtml(15, 120, 210, 20, this.Color(this.Right(houseName), White), false, false);

                this.AddHtml(15, 140, 210, 20, this.Color("Friends:", White), false, false);
                this.AddHtml(15, 140, 210, 20, this.Color(this.Right(sel.Friends.Count.ToString()), White), false, false);

                this.AddHtml(15, 160, 210, 20, this.Color("Co-Owners:", White), false, false);
                this.AddHtml(15, 160, 210, 20, this.Color(this.Right(sel.CoOwners.Count.ToString()), White), false, false);

                this.AddHtml(15, 180, 210, 20, this.Color("Bans:", White), false, false);
                this.AddHtml(15, 180, 210, 20, this.Color(this.Right(sel.Bans.Count.ToString()), White), false, false);

                this.AddHtml(15, 200, 210, 20, this.Color("Decays:", White), false, false);
                this.AddHtml(15, 200, 210, 20, this.Color(this.Right(sel.CanDecay ? "Yes" : "No"), White), false, false);

                this.AddHtml(15, 220, 210, 20, this.Color("Decay Level:", White), false, false);
                this.AddHtml(15, 220, 210, 20, this.Color(this.Right(sel.DecayLevel.ToString()), White), false, false);

                this.AddButton(15, 245, 4005, 4007, 1, GumpButtonType.Reply, 0);
                this.AddHtml(50, 245, 120, 20, this.Color("Go to house", White), false, false);

                this.AddButton(15, 265, 4005, 4007, 2, GumpButtonType.Reply, 0);
                this.AddHtml(50, 265, 120, 20, this.Color("Open house menu", White), false, false);

                this.AddButton(15, 285, 4005, 4007, 3, GumpButtonType.Reply, 0);
                this.AddHtml(50, 285, 120, 20, this.Color("Demolish house", White), false, false);

                this.AddButton(15, 305, 4005, 4007, 4, GumpButtonType.Reply, 0);
                this.AddHtml(50, 305, 120, 20, this.Color("Refresh house", White), false, false);
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("ViewHouses", AccessLevel.GameMaster, new CommandEventHandler(ViewHouses_OnCommand));
        }

        [Usage("ViewHouses")]
        [Description("Displays a menu listing all houses of a targeted player. The menu also contains specific house details, and options to: go to house, open house menu, and demolish house.")]
        public static void ViewHouses_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ViewHouses_OnTarget));
        }

        public static void ViewHouses_OnTarget(Mobile from, object targeted)
        {
            if (targeted is Mobile)
                from.SendGump(new ViewHousesGump(from, GetHouses((Mobile)targeted), null));
        }

        public static List<BaseHouse> GetHouses(Mobile owner)
        {
            List<BaseHouse> list = new List<BaseHouse>();

            Account acct = owner.Account as Account;

            if (acct == null)
            {
                list.AddRange(BaseHouse.GetHouses(owner));
            }
            else
            {
                for (int i = 0; i < acct.Length; ++i)
                {
                    Mobile mob = acct[i];

                    if (mob != null)
                        list.AddRange(BaseHouse.GetHouses(mob));
                }
            }

            list.Sort(HouseComparer.Instance);

            return list;
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (this.m_Selection == null)
            {
                int v = info.ButtonID - 1;

                if (v >= 0 && v < this.m_List.Count)
                    this.m_From.SendGump(new ViewHousesGump(this.m_From, this.m_List, this.m_List[v]));
            }
            else if (!this.m_Selection.Deleted)
            {
                switch ( info.ButtonID )
                {
                    case 0:
                        {
                            this.m_From.SendGump(new ViewHousesGump(this.m_From, this.m_List, null));
                            break;
                        }
                    case 1:
                        {
                            Map map = this.m_Selection.Map;

                            if (map != null && map != Map.Internal)
                                this.m_From.MoveToWorld(this.m_Selection.BanLocation, map);

                            this.m_From.SendGump(new ViewHousesGump(this.m_From, this.m_List, this.m_Selection));

                            break;
                        }
                    case 2:
                        {
                            this.m_From.SendGump(new ViewHousesGump(this.m_From, this.m_List, this.m_Selection));

                            HouseSign sign = this.m_Selection.Sign;

                            if (sign != null && !sign.Deleted)
                                sign.OnDoubleClick(this.m_From);

                            break;
                        }
                    case 3:
                        {
                            this.m_From.SendGump(new ViewHousesGump(this.m_From, this.m_List, this.m_Selection));
                            this.m_From.SendGump(new HouseDemolishGump(this.m_From, this.m_Selection));

                            break;
                        }
                    case 4:
                        {
                            this.m_Selection.RefreshDecay();
                            this.m_From.SendGump(new ViewHousesGump(this.m_From, this.m_List, this.m_Selection));

                            break;
                        }
                }
            }
        }

        public object FindHouseName(BaseHouse house)
        {
            int multiID = house.ItemID;
            HousePlacementEntry[] entries;

            entries = HousePlacementEntry.ClassicHouses;

            for (int i = 0; i < entries.Length; ++i)
            {
                if (entries[i].MultiID == multiID)
                    return entries[i].Description;
            }

            entries = HousePlacementEntry.TwoStoryFoundations;

            for (int i = 0; i < entries.Length; ++i)
            {
                if (entries[i].MultiID == multiID)
                    return entries[i].Description;
            }

            entries = HousePlacementEntry.ThreeStoryFoundations;

            for (int i = 0; i < entries.Length; ++i)
            {
                if (entries[i].MultiID == multiID)
                    return entries[i].Description;
            }

            return house.GetType().Name;
        }

        public string Right(string text)
        {
            return String.Format("<div align=right>{0}</div>", text);
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public void AddBlackAlpha(int x, int y, int width, int height)
        {
            this.AddImageTiled(x, y, width, height, 2624);
            this.AddAlphaRegion(x, y, width, height);
        }

        private class HouseComparer : IComparer<BaseHouse>
        {
            public static readonly IComparer<BaseHouse> Instance = new HouseComparer();
            public int Compare(BaseHouse x, BaseHouse y)
            {
                return x.BuiltOn.CompareTo(y.BuiltOn);
            }
        }
    }
}