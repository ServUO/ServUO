//   ___|========================|___
//   \  |  Written by Felladrin  |  /   [AddToParty Command] - Current version: 1.0 (November 21, 2013)
//    > |     November 2013      | < 
//   /__|========================|__\   Description: Allows players to add new party members from anywhere.

using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Network;
using Server.Factions;

namespace Server.Commands
{
    public class AddToPartyCommand
    {
        public static class Config
        {
            public static bool Enabled = true; // Is this command enabled?
            public static bool OnlyLeadersCanAdd = true; // New party members can only be added by the party leader?
        }

        public static void Initialize()
        {
            if (Config.Enabled)
                CommandSystem.Register("AddToParty", AccessLevel.Player, new CommandEventHandler(AddToParty_OnCommand));
        }

        [Usage("AddToParty <Name>")]
        [Description("Used to add a player to your party. Optionally you can provide a name to filter.")]
        public static void AddToParty_OnCommand(CommandEventArgs e)
        {
            e.Mobile.CloseGump(typeof(AddToPartyGump));
            e.Mobile.SendGump(new AddToPartyGump(e.Mobile, e.ArgString));
            e.Mobile.SendMessage("Who would you like to add to your party?");
        }

        public class PartyInvitationGump : Gump
        {
            private Mobile m_Target, m_From;

            public PartyInvitationGump(Mobile from, Mobile target)
                : base(0, 0)
            {
                m_From = from;
                m_Target = target;

                DeclineTimer.Start(target, from);
                Timer.DelayCall(TimeSpan.FromSeconds(30), delegate { target.CloseGump(typeof(PartyInvitationGump)); });

                this.Closable = false;
                this.Disposable = false;
                this.Dragable = true;
                this.Resizable = false;

                AddPage(0);
                AddBackground(11, 318, 225, 239, 9270);
                AddButton(136, 520, 12018, 12020, (int)Buttons.GumpRefuse, GumpButtonType.Reply, 0);
                AddButton(35, 520, 12000, 12002, (int)Buttons.GumpAccept, GumpButtonType.Reply, 0);
                AddLabel(30, 335, 95, string.Format("Invitation from {0}", m_From.Name));
                AddLabel(30, 360, 68, string.Format("Do you want to join {0} party?", ((m_From.Female) ? "her" : "his")));
                AddImage(33, 386, 11413);
            }

            public enum Buttons
            {
                GumpRefuse,
                GumpAccept,
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_From == null || m_Target == null)
                    return;

                switch (info.ButtonID)
                {
                    case (int)Buttons.GumpRefuse:
                        PartyCommands.Handler.OnDecline(m_Target, m_From);
                        break;
                    case (int)Buttons.GumpAccept:
                        PartyCommands.Handler.OnAccept(m_Target, m_From);
                        break;
                }
            }
        }

        public class AddToPartyGump : Gump
        {
            public static bool OldStyle = PropsConfig.OldStyle;

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

            private static bool PrevLabel = false, NextLabel = false;

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

            private Mobile m_Owner;
            private List<Mobile> m_Mobiles;
            private int m_Page;

            private class InternalComparer : IComparer<Mobile>
            {
                public static readonly IComparer<Mobile> Instance = new InternalComparer();

                public InternalComparer() { }

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

            public AddToPartyGump(Mobile owner, string filter)
                : this(owner, BuildList(owner, filter), 0)
            {
            }

            public AddToPartyGump(Mobile owner, List<Mobile> list, int page)
                : base(GumpOffsetX, GumpOffsetY)
            {
                owner.CloseGump(typeof(AddToPartyGump));

                m_Owner = owner;
                m_Mobiles = list;

                if (m_Mobiles.Count == 0)
                {
                    owner.SendMessage(38, "There are no players available to invite.");
                }
                else
                {
                    Initialize(page);
                }
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

                    if (m != null)
                    {
                        if (filter != null && (m.Name == null || m.Name.ToLower().IndexOf(filter) < 0))
                            continue;

                        if (m == owner || m.AccessLevel > AccessLevel.Player)
                            continue;

                        list.Add(m);
                    }
                }

                list.Sort(InternalComparer.Instance);

                return list;
            }

            public void Initialize(int page)
            {
                m_Page = page;

                int count = m_Mobiles.Count - (page * EntryCount);

                if (count < 0)
                    count = 0;
                else if (count > EntryCount)
                    count = EntryCount;

                int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

                AddPage(0);

                AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
                AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

                int x = BorderSize + OffsetSize;
                int y = BorderSize + OffsetSize;

                int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

                if (!OldStyle)
                    AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID);

                AddLabel(x + TextOffsetX, y, TextHue, String.Format("Add who? (Page {1}/{2})", m_Mobiles.Count, page + 1, (m_Mobiles.Count + EntryCount - 1) / EntryCount));

                x += emptyWidth + OffsetSize;

                if (OldStyle)
                    AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
                else
                    AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

                if (page > 0)
                {
                    AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0);

                    if (PrevLabel)
                        AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
                }

                x += PrevWidth + OffsetSize;

                if (!OldStyle)
                    AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

                if ((page + 1) * EntryCount < m_Mobiles.Count)
                {
                    AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1);

                    if (NextLabel)
                        AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
                }

                for (int i = 0, index = page * EntryCount; i < EntryCount && index < m_Mobiles.Count; ++i, ++index)
                {
                    x = BorderSize + OffsetSize;
                    y += EntryHeight + OffsetSize;

                    Mobile m = m_Mobiles[index];

                    AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                    AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m), m.Deleted ? "(deleted)" : m.Name);

                    x += EntryWidth + OffsetSize;

                    if (SetGumpID != 0)
                        AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                    if (m.NetState != null && !m.Deleted)
                        AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 3, GumpButtonType.Reply, 0);
                }
            }

            private static int GetHueFor(Mobile m)
            {
                switch (m.AccessLevel)
                {
                    case AccessLevel.Owner:
                    case AccessLevel.Developer:
                    case AccessLevel.Administrator: return 0x516;
                    case AccessLevel.Seer: return 0x144;
                    case AccessLevel.GameMaster: return 0x21;
                    case AccessLevel.Counselor: return 0x2;
                    case AccessLevel.Player:
                    default:
                        {
                            if (m.Kills >= 5)
                                return 0x21;
                            else if (m.Criminal)
                                return 0x3B1;

                            return 0x58;
                        }
                }
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                Mobile from = state.Mobile;

                switch (info.ButtonID)
                {
                    case 0: // Closed
                        {
                            return;
                        }
                    case 1: // Previous
                        {
                            if (m_Page > 0)
                                from.SendGump(new AddToPartyGump(from, m_Mobiles, m_Page - 1));

                            break;
                        }
                    case 2: // Next
                        {
                            if ((m_Page + 1) * EntryCount < m_Mobiles.Count)
                                from.SendGump(new AddToPartyGump(from, m_Mobiles, m_Page + 1));

                            break;
                        }
                    default:
                        {
                            int index = (m_Page * EntryCount) + (info.ButtonID - 3);

                            if (index >= 0 && index < m_Mobiles.Count)
                            {
                                Mobile m = m_Mobiles[index];

                                if (m.Deleted)
                                {
                                    from.SendMessage("That player has deleted their character.");
                                    from.SendGump(new AddToPartyGump(from, m_Mobiles, m_Page));
                                }
                                else if (m.NetState == null)
                                {
                                    from.SendMessage("That player is no longer online.");
                                    from.SendGump(new AddToPartyGump(from, m_Mobiles, m_Page));
                                }
                                else
                                {
                                    Party p = Party.Get(from);
                                    Party mp = Party.Get(m);

                                    if (from == m)
                                        from.SendLocalizedMessage(1005439); // You cannot add yourself to a party.
                                    else if (Config.OnlyLeadersCanAdd && p != null && p.Leader != from)
                                        from.SendLocalizedMessage(1005453); // You may only add members to the party if you are the leader.
                                    else if (p != null && (p.Members.Count + p.Candidates.Count) >= Party.Capacity)
                                        from.SendLocalizedMessage(1008095); // You may only have 10 in your party (this includes candidates).
                                    else if (!m.Player)
                                        from.SendLocalizedMessage(1005444); // The creature ignores your offer.
                                    else if (mp != null && mp == p)
                                        from.SendLocalizedMessage(1005440); // This person is already in your party!
                                    else if (mp != null)
                                        from.SendLocalizedMessage(1005441); // This person is already in a party!
                                    else
                                    {
                                        Faction ourFaction = Faction.Find(from);
                                        Faction theirFaction = Faction.Find(m);

                                        if (ourFaction != null && theirFaction != null && ourFaction != theirFaction)
                                        {
                                            from.SendLocalizedMessage(1008088); // You cannot have players from opposing factions in the same party!
                                            m.SendLocalizedMessage(1008093); // The party cannot have members from opposing factions.
                                            break;
                                        }

                                        if (p == null)
                                            from.Party = p = new Party(from);

                                        if (!p.Candidates.Contains(m))
                                            p.Candidates.Add(m);

                                        m.SendGump(new PartyInvitationGump(from, m));

                                        m.Send(new PartyInvitation(from));

                                        m.Party = from;

                                        from.SendMessage(68, "Invitation sent to {0}.", m.Name);
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }
    }
}
