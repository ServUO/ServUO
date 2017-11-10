using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class WhoGump : Gump
    {
        public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
        public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;
        public static readonly int TextHue = PropsConfig.TextHue;
        public static readonly int TextOffsetX = PropsConfig.TextOffsetX;
        public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
        public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
        public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
        public static readonly int BackGumpID = PropsConfig.BackGumpID;
        public static readonly int SetGumpID = PropsConfig.SetGumpID;
        public static readonly int SetWidth = PropsConfig.SetWidth;
        public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
        public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
        public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;
        public static readonly int PrevWidth = PropsConfig.PrevWidth;
        public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
        public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
        public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;
        public static readonly int NextWidth = PropsConfig.NextWidth;
        public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
        public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;
        public static readonly int OffsetSize = PropsConfig.OffsetSize;
        public static readonly int EntryHeight = PropsConfig.EntryHeight;
        public static readonly int BorderSize = PropsConfig.BorderSize;
        public static bool OldStyle = PropsConfig.OldStyle;
        private static readonly bool PrevLabel = false;
        private static readonly bool NextLabel = false;
        private static readonly int PrevLabelOffsetX = PrevWidth + 1;
        private static readonly int PrevLabelOffsetY = 0;
        private static readonly int NextLabelOffsetX = -29;
        private static readonly int NextLabelOffsetY = 0;
        private static readonly int EntryWidth = 180;
        private static readonly int EntryCount = 15;
        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private readonly Mobile m_Owner;
        private readonly List<Mobile> m_Mobiles;
        private int m_Page;

        public bool EC { get { return m_Owner != null && m_Owner.NetState != null && m_Owner.NetState.IsEnhancedClient; } }

        public WhoGump(Mobile owner, string filter)
            : this(owner, BuildList(owner, filter), 0)
        {
        }

        public WhoGump(Mobile owner, List<Mobile> list, int page)
            : base(GumpOffsetX, GumpOffsetY)
        {
            owner.CloseGump(typeof(WhoGump));

            this.m_Owner = owner;
            this.m_Mobiles = list;

            this.Initialize(page);
        }

        public static void Initialize()
        {
            CommandSystem.Register("Who", AccessLevel.Counselor, new CommandEventHandler(WhoList_OnCommand));
            CommandSystem.Register("WhoList", AccessLevel.Counselor, new CommandEventHandler(WhoList_OnCommand));
        }

        public static List<Mobile> BuildList(Mobile owner, string filter)
        {
            if (filter != null && (filter = filter.Trim()).Length == 0)
                filter = null;
            else
                filter = filter.ToLower();

            List<Mobile> list = new List<Mobile>();
            List<NetState> states = NetState.Instances;

            for (int i = 0; i < states.Count; ++i)
            {
                Mobile m = states[i].Mobile;

                if (m != null && (m == owner || !m.Hidden || owner.AccessLevel >= m.AccessLevel || (m is PlayerMobile && ((PlayerMobile)m).VisibilityList.Contains(owner))))
                {
                    if (filter != null && (m.Name == null || m.Name.ToLower().IndexOf(filter) < 0))
                        continue;

                    list.Add(m);
                }
            }

            list.Sort(InternalComparer.Instance);

            return list;
        }

        public void Initialize(int page)
        {
            this.m_Page = page;

            int count = this.m_Mobiles.Count - (page * EntryCount);

            if (count < 0)
                count = 0;
            else if (count > EntryCount)
                count = EntryCount;

            int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
            this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize;

            int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

            if (!OldStyle)
                this.AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID);

            this.AddLabel(x + TextOffsetX, y, TextHue, String.Format("Page {0} of {1} ({2})", page + 1, (this.m_Mobiles.Count + EntryCount - 1) / EntryCount, this.m_Mobiles.Count));

            x += emptyWidth + OffsetSize;

            if (OldStyle)
                this.AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
            else
                this.AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

            if (page > 0)
            {
                this.AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0);

                if (PrevLabel)
                    this.AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
            }

            x += PrevWidth + OffsetSize;

            if (!OldStyle)
                this.AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

            if ((page + 1) * EntryCount < this.m_Mobiles.Count)
            {
                this.AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1);

                if (NextLabel)
                    this.AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
            }

            for (int i = 0, index = page * EntryCount; i < EntryCount && index < this.m_Mobiles.Count; ++i, ++index)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                Mobile m = this.m_Mobiles[index];

                this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                this.AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m), m.Deleted ? "(deleted)" : m.Name);

                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                    this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                if (m.NetState != null && !m.Deleted)
                    this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 3, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch ( info.ButtonID )
            {
                case 0: // Closed
                    {
                        return;
                    }
                case 1: // Previous
                    {
                        if (this.m_Page > 0)
                            from.SendGump(new WhoGump(from, this.m_Mobiles, this.m_Page - 1));

                        break;
                    }
                case 2: // Next
                    {
                        if ((this.m_Page + 1) * EntryCount < this.m_Mobiles.Count)
                            from.SendGump(new WhoGump(from, this.m_Mobiles, this.m_Page + 1));

                        break;
                    }
                default:
                    {
                        int index = (this.m_Page * EntryCount) + (info.ButtonID - 3);

                        if (index >= 0 && index < this.m_Mobiles.Count)
                        {
                            Mobile m = this.m_Mobiles[index];

                            if (m.Deleted)
                            {
                                from.SendMessage("That player has deleted their character.");
                                from.SendGump(new WhoGump(from, this.m_Mobiles, this.m_Page));
                            }
                            else if (m.NetState == null)
                            {
                                from.SendMessage("That player is no longer online.");
                                from.SendGump(new WhoGump(from, this.m_Mobiles, this.m_Page));
                            }
                            else if (m == from || !m.Hidden || from.AccessLevel >= m.AccessLevel || (m is PlayerMobile && ((PlayerMobile)m).VisibilityList.Contains(from))) 
                            {
                                from.SendGump(new ClientGump(from, m.NetState));
                            }
                            else
                            {
                                from.SendMessage("You cannot see them.");
                                from.SendGump(new WhoGump(from, this.m_Mobiles, this.m_Page));
                            }
                        }

                        break;
                    }
            }
        }

        [Usage("WhoList [filter]")]
        [Aliases("Who")]
        [Description("Lists all connected clients. Optionally filters results by name.")]
        private static void WhoList_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new WhoGump(e.Mobile, e.ArgString));
        }

        private int GetHueFor(Mobile m)
        {
            switch ( m.AccessLevel )
            {
                case AccessLevel.Owner:
                case AccessLevel.Developer:
                case AccessLevel.Administrator: return EC ? 0x51D : 0x516;
                case AccessLevel.Seer: return EC ? 0x142 : 0x144;
                case AccessLevel.GameMaster: return EC ? 0x11 : 0x21;
                case AccessLevel.Decorator: return 0x2;
                case AccessLevel.VIP:
                case AccessLevel.Player:
                default:
                    {
                        if (m.Kills >= 5)
                            return EC ? 0x20 : 0x21;
                        else if (m.Criminal)
                            return EC ? 0x3AE : 0x3B1;

                        return EC ? 0x5C : 0x58;
                    }
            }
        }

        private class InternalComparer : IComparer<Mobile>
        {
            public static readonly IComparer<Mobile> Instance = new InternalComparer();
            public InternalComparer()
            {
            }

            public int Compare(Mobile x, Mobile y)
            {
                if (x == null || y == null)
                    throw new ArgumentException();

                if (x.AccessLevel > y.AccessLevel)
                    return -1;
                else if (x.AccessLevel < y.AccessLevel)
                    return 1;
                else
                    return Insensitive.Compare(x.Name, y.Name);
            }
        }
    }
}